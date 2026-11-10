using Lab8.Data;
using Lab8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab8.Controllers;

public class CampusesController(Lab8Context db) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await db.Campuses.AsNoTracking().Include(item => item.Students).OrderBy(item => item.Name).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var item = await db.Campuses.AsNoTracking().Include(item => item.Students).SingleOrDefaultAsync(item => item.ID == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        
        return View(new UniversityCampus());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Name")] UniversityCampus item)
    {
        
        if (ModelState.IsValid)
        {
            db.Campuses.Add(item);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        return View(item);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await db.Campuses.FindAsync(id);
        if (item == null) return NotFound();
        
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] UniversityCampus item)
    {
        if (id != item.ID) return BadRequest();
        
        if (!ModelState.IsValid)
        {
            
            return View(item);
        }
        var existing = await db.Campuses.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Name = item.Name;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Campuses.AsNoTracking().Include(item => item.Students).SingleOrDefaultAsync(item => item.ID == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await db.Campuses.Include(item => item.Students).SingleOrDefaultAsync(item => item.ID == id);
        if (item == null) return NotFound();

        if (await db.Students.AnyAsync(child => child.CampusID == id))
        {
            ModelState.AddModelError("", "Remove or reassign the related students first.");
            return View(item);
        }
        db.Campuses.Remove(item);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}
