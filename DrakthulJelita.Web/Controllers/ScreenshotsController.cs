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

    // GET: Screenshots/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var screenshot = await context.Screenshots.FindAsync(id);
        if (screenshot == null) return NotFound();
        return View(screenshot);
    }

    // POST: Screenshots/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Path,MimeType,Size,WowName,CreatedAt,WowClassId,Width,Height")]
        Screenshot screenshot)
    {
        if (id != screenshot.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(screenshot);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScreenshotExists(screenshot.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(screenshot);
    }

    // GET: Screenshots/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var screenshot = await context.Screenshots
            .FirstOrDefaultAsync(m => m.Id == id);
        if (screenshot == null) return NotFound();

        return View(screenshot);
    }

    // POST: Screenshots/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var screenshot = await context.Screenshots.FindAsync(id);
        if (screenshot != null) context.Screenshots.Remove(screenshot);

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ScreenshotExists(int id)
    {
        return context.Screenshots.Any(e => e.Id == id);
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