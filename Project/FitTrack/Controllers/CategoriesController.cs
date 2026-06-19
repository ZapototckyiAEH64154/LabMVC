using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitTrack.Data;
using FitTrack.Models;

namespace FitTrack.Controllers;

// KONTROLER kategorii (model dodatkowy) — pełny CRUD.
public class CategoriesController : Controller
{
    private readonly FitTrackContext _context;

    public CategoriesController(FitTrackContext context)
    {
        _context = context;
    }

    // GET: /Categories
    public async Task<IActionResult> Index()
    {
        return View(await _context.Category
            .Include(c => c.Workouts)
            .OrderBy(c => c.Name)
            .ToListAsync());
    }

    // GET: /Categories/Create
    [Authorize]
    public IActionResult Create() => View();

    // POST: /Categories/Create
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Kategoria została dodana.";
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: /Categories/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var category = await _context.Category.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    // POST: /Categories/Edit/5
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            _context.Update(category);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Kategoria została zaktualizowana.";
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: /Categories/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var category = await _context.Category
            .Include(c => c.Workouts)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    // POST: /Categories/Delete/5
    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Category
            .Include(c => c.Workouts)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (category.Workouts.Any())
        {
            // Nie można usunąć kategorii powiązanej z treningami.
            TempData["Error"] = "Nie można usunąć kategorii, która ma przypisane treningi.";
            return RedirectToAction(nameof(Index));
        }

        _context.Category.Remove(category);
        await _context.SaveChangesAsync();
        TempData["Message"] = "Kategoria została usunięta.";
        return RedirectToAction(nameof(Index));
    }
}
