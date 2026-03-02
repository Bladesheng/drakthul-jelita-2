using Amazon.S3;
using Amazon.S3.Model;
using DrakthulJelita.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrakthulJelita.Web.Data;
using DrakthulJelita.Web.Models;
using DrakthulJelita.Web.ViewModels;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DrakthulJelita.Web.Controllers;

public class ScreenshotsController(
    AppDbContext context,
    IAmazonS3 s3,
    IOptions<S3Options> s3Options
) : Controller
{
    public async Task<IActionResult> Index()
    {
        var screenshots = await context.Screenshots
            .AsNoTracking()
            .Include(s => s.WowClass)
            .OrderBy(s => s.WowName)
            .Select(s => new ScreenshotIndexVm.ScreenshotVm
            {
                Id = s.Id,
                Path = s.Path,
                WowName = s.WowName,
                WowClassId = s.WowClassId,
                Width = s.Width,
                Height = s.Height,
                WowClassName = s.WowClass.Name,
                WowClassColor = s.WowClass.Color
            })
            .ToListAsync();

        var screenshotsByWowClassId = screenshots
            .GroupBy(s => s.WowClassId)
            .ToDictionary(
                g => g.Key,
                IReadOnlyList<ScreenshotIndexVm.ScreenshotVm> (g) => g.ToList()
            );

        var wowClasses = await GetWowClassesAsync();

        return View(new ScreenshotIndexVm
        {
            Screenshots = screenshotsByWowClassId,
            WowClasses = wowClasses
        });
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string wowName,
        [FromQuery] int wowClassId
    )
    {
        var screenshots = await context.Screenshots
            .Where(s => s.WowName == wowName && s.WowClassId == wowClassId)
            .ToListAsync();

        return Ok(screenshots);
    }

    public async Task<IActionResult> Create()
    {
        var wowClasses = await GetWowClassesAsync();

        return View(new ScreenshotCreateVm
        {
            WowClasses = wowClasses
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(
        ScreenshotCreateVm vm
    )
    {
        if (!ModelState.IsValid)
        {
            vm.WowClasses = await GetWowClassesAsync();
            return View(vm);
        }

        var input = vm.Input;

        var isValidClass =
            await context.WowClasses.AnyAsync(wowClass => wowClass.Id == input.WowClassId);
        if (!isValidClass)
            return await Fail(nameof(vm.Input.WowClassId), "Invalid class value.");


        if (input.FileUpload.Length > 1 * 1024 * 1024)
            return await Fail(nameof(vm.Input.FileUpload), "File must be 1MB or smaller.");


        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/avif" };
        if (!allowedTypes.Contains(input.FileUpload.ContentType))
            return await Fail(nameof(vm.Input.FileUpload), "File must be an image.");


        var exists = await context.Screenshots.AnyAsync(screenshot =>
            screenshot.WowName == input.WowName && screenshot.WowClassId == input.WowClassId
        );
        if (exists)
            return await Fail(nameof(vm.Input.WowName),
                "Screenshot with that name and class already exists."
            );


        using var magickImage = new MagickImage();
        await magickImage.ReadAsync(input.FileUpload.OpenReadStream());
        magickImage.Format = MagickFormat.Avif;
        /*
         * AVIF 90 is as close as it gets to the original quality.
         * Webp 100 has similar size, and sometimes similar quality too, but other times AVIF just
         * looks better.
         */
        magickImage.Quality = 90;

        using var memoryStream = new MemoryStream();
        await magickImage.WriteAsync(memoryStream);
        memoryStream.Position = 0;

        var size = (int)memoryStream.Length;
        var width = (int)magickImage.Width;
        var height = (int)magickImage.Height;

        var key = $"{Guid.NewGuid()}.avif";

        await s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = s3Options.Value.Bucket,
            Key = key,
            InputStream = memoryStream,
            ContentType = "image/avif",
            // R2 doesn't support chunked streaming that AWS SDK uses by default.
            DisablePayloadSigning = true
        });

        var screenshot = new Screenshot
        {
            Path = key,
            MimeType = "image/avif",
            Size = size,
            WowName = input.WowName,
            WowClassId = input.WowClassId,
            Width = width,
            Height = height
        };
        context.Add(screenshot);
        await context.SaveChangesAsync();

        TempData["Status"] = "screenshot-created";
        return RedirectToAction(nameof(Create));

        async Task<IActionResult> Fail(string key, string message)
        {
            ModelState.AddModelError($"Input.{key}", message);
            vm.WowClasses = await GetWowClassesAsync();
            return View(vm);
        }
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var screenshot = await context.Screenshots.FindAsync(id);
        if (screenshot == null) return NotFound();

        return View(new ScreenshotEditVm
        {
            WowClasses = await GetWowClassesAsync(),
            Input = new ScreenshotEditVm.InputVm
            {
                WowName = screenshot.WowName,
                WowClassId = screenshot.WowClassId
            },
            Display = new ScreenshotEditVm.DisplayVm
            {
                Id = screenshot.Id,
                Path = screenshot.Path,
                Width = screenshot.Width,
                Height = screenshot.Height
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ScreenshotEditVm vm
    )
    {
        var screenshot = await context.Screenshots.FindAsync(id);
        if (screenshot == null) return NotFound();

        if (!ModelState.IsValid)
        {
            vm.WowClasses = await GetWowClassesAsync();
            vm.Display = new ScreenshotEditVm.DisplayVm
            {
                Id = screenshot.Id,
                Path = screenshot.Path,
                Width = screenshot.Width,
                Height = screenshot.Height
            };
            return View(vm);
        }

        screenshot.WowName = vm.Input.WowName;
        screenshot.WowClassId = vm.Input.WowClassId;
        await context.SaveChangesAsync();

        TempData["Status"] = "screenshot-updated";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var screenshot = await context.Screenshots.FindAsync(id);
        if (screenshot == null) return NotFound();

        context.Screenshots.Remove(screenshot);
        await context.SaveChangesAsync();

        TempData["Status"] = "screenshot-deleted";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<WowClassVm>> GetWowClassesAsync()
    {
        return await context.WowClasses
            .AsNoTracking()
            .Select(c => new WowClassVm
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color
            })
            .ToListAsync();
    }
}