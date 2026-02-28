using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrakthulJelita.Web.Data;
using DrakthulJelita.Web.Models;
using DrakthulJelita.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace DrakthulJelita.Web.Controllers;

public class ScreenshotsController(AppDbContext context) : Controller
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

        var wowClasses = await context.WowClasses
            .AsNoTracking()
            .Select(c => new ScreenshotIndexVm.WowClassVm
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color
            })
            .ToListAsync();


        return View(new ScreenshotIndexVm
        {
            Screenshots = screenshotsByWowClassId,
            WowClasses = wowClasses
        });
    }

    // GET: Screenshots/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var screenshot = await context.Screenshots
            .FirstOrDefaultAsync(m => m.Id == id);
        if (screenshot == null) return NotFound();

        return View(screenshot);
    }

    // GET: Screenshots/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Screenshots/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(
        [Bind("Id,Path,MimeType,Size,WowName,CreatedAt,WowClassId,Width,Height")]
        Screenshot screenshot)
    {
        if (ModelState.IsValid)
        {
            context.Add(screenshot);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(screenshot);
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
}