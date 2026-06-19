using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitTrack.Data;
using FitTrack.Models;

namespace FitTrack.Controllers;

// KONTROLER treningów — obsługuje żądania HTTP, komunikuje się z modelem
// (przez kontekst EF Core) i przekazuje dane do widoków.
public class WorkoutsController : Controller
{
    private readonly FitTrackContext _context;

    public WorkoutsController(FitTrackContext context)
    {
        _context = context;
    }

    // GET: /  lub /Workouts
    // Lista treningów z wyszukiwaniem, filtrowaniem i sortowaniem.
    public async Task<IActionResult> Index(string? searchString, int? categoryId, Intensity? intensity, string? sort)
    {
        IQueryable<Workout> query = _context.Workout.Include(w => w.Category);

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(w => w.Name.Contains(searchString)
                                     || (w.Notes != null && w.Notes.Contains(searchString)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(w => w.CategoryId == categoryId.Value);
        }

        if (intensity.HasValue)
        {
            query = query.Where(w => w.Intensity == intensity.Value);
        }

        query = sort switch
        {
            "name" => query.OrderBy(w => w.Name),
            "name_desc" => query.OrderByDescending(w => w.Name),
            "date" => query.OrderBy(w => w.ScheduledDate),
            "intensity" => query.OrderBy(w => w.Intensity),
            _ => query.OrderByDescending(w => w.ScheduledDate),
        };

        var viewModel = new WorkoutFilterViewModel
        {
            Workouts = await query.ToListAsync(),
            Categories = new SelectList(await _context.Category.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", categoryId),
            SearchString = searchString,
            CategoryId = categoryId,
            Intensity = intensity,
            Sort = sort,
        };

        return View(viewModel);
    }

    // GET: /Workouts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var workout = await _context.Workout
            .Include(w => w.Category)
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workout == null)
        {
            return NotFound();
        }

        return View(workout);
    }

    // GET: /Workouts/Create
    [Authorize]
    public IActionResult Create()
    {
        PopulateCategories();
        return View();
    }

    // POST: /Workouts/Create
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,CategoryId,Intensity,DurationMinutes,ScheduledDate,Notes")] Workout workout)
    {
        if (ModelState.IsValid)
        {
            _context.Add(workout);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Trening został dodany.";
            return RedirectToAction(nameof(Index));
        }
        PopulateCategories(workout.CategoryId);
        return View(workout);
    }

    // GET: /Workouts/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var workout = await _context.Workout.FindAsync(id);
        if (workout == null)
        {
            return NotFound();
        }
        PopulateCategories(workout.CategoryId);
        return View(workout);
    }

    // POST: /Workouts/Edit/5
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CategoryId,Intensity,DurationMinutes,ScheduledDate,Notes")] Workout workout)
    {
        if (id != workout.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(workout);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Trening został zaktualizowany.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkoutExists(workout.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        PopulateCategories(workout.CategoryId);
        return View(workout);
    }

    // GET: /Workouts/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var workout = await _context.Workout
            .Include(w => w.Category)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workout == null)
        {
            return NotFound();
        }

        return View(workout);
    }

    // POST: /Workouts/Delete/5
    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var workout = await _context.Workout.FindAsync(id);
        if (workout != null)
        {
            _context.Workout.Remove(workout);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Trening został usunięty.";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool WorkoutExists(int id) => _context.Workout.Any(e => e.Id == id);

    private void PopulateCategories(int? selected = null)
    {
        ViewBag.CategoryId = new SelectList(_context.Category.OrderBy(c => c.Name), "Id", "Name", selected);
    }
}
