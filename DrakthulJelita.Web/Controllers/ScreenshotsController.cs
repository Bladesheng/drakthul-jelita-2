using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrakthulJelita.Web.Data;
using DrakthulJelita.Web.Models;

namespace DrakthulJelita.Web.Controllers;

public class ScreenshotsController : Controller
{
    private readonly AppDbContext _context;

    public ScreenshotsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Screenshots
    public async Task<IActionResult> Index()
    {
        return View(await _context.Screenshots.ToListAsync());
    }

    // GET: Screenshots/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var screenshot = await _context.Screenshots
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
    public async Task<IActionResult> Create(
        [Bind("Id,Path,MimeType,Size,WowName,CreatedAt,WowClassId,Width,Height")]
        Screenshot screenshot)
    {
        if (ModelState.IsValid)
        {
            _context.Add(screenshot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(screenshot);
    }

    // GET: Screenshots/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var screenshot = await _context.Screenshots.FindAsync(id);
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
                _context.Update(screenshot);
                await _context.SaveChangesAsync();
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

        var screenshot = await _context.Screenshots
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
        var screenshot = await _context.Screenshots.FindAsync(id);
        if (screenshot != null) _context.Screenshots.Remove(screenshot);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ScreenshotExists(int id)
    {
        return _context.Screenshots.Any(e => e.Id == id);
    }
}