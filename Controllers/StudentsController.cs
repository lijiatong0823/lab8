using Lab8.Data;
using Lab8.Models;
using Lab8.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab8.Controllers;

public class StudentsController(Lab8Context db) : Controller
{
    public async Task<IActionResult> Index(string? search, int? campusID)
    {
        search = search?.Trim();
        var students = SearchStudents(search);
        if (campusID.HasValue)
            students = students.Where(student => student.CampusID == campusID);
        ViewBag.Search = search;
        ViewBag.CampusID = campusID;
        ViewBag.Campuses = new SelectList(
            await db.Campuses.OrderBy(campus => campus.Name).ToListAsync(),
            "ID", "Name", campusID);
        return View(await students.OrderBy(student => student.Name).ToListAsync());
    }

    public async Task<IActionResult> Advanced(StudentIndexViewModel model)
    {
        model.Search = model.Search?.Trim();
        model.Address = model.Address?.Trim();
        var students = SearchStudents(model.Search);
        if (!string.IsNullOrEmpty(model.Address))
        {
            string address = model.Address.ToLowerInvariant();
            students = students.Where(student => student.Address.ToLower().Contains(address));
        }
        var counts = await students.GroupBy(student => student.CampusID)
            .Select(group => new { CampusID = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.CampusID, item => item.Count);
        var campuses = await db.Campuses.OrderBy(campus => campus.Name).ToListAsync();
        model.CampusesWithCount = campuses.Select(campus => new CampusWithCount
        {
            CampusID = campus.ID,
            CampusName = campus.Name,
            StudentCount = counts.GetValueOrDefault(campus.ID)
        }).ToList();
        if (model.CampusID.HasValue)
            students = students.Where(student => student.CampusID == model.CampusID);
        model.Students = await students.OrderBy(student => student.Name).ToListAsync();
        return View(model);
    }

    private IQueryable<Student> SearchStudents(string? search)
    {
        var students = db.Students.AsNoTracking().Include(student => student.Campus).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim().ToLowerInvariant();
            students = students.Where(student => student.Name.ToLower().Contains(term)
                || student.Address.ToLower().Contains(term)
                || student.Campus.Name.ToLower().Contains(term));
        }
        return students;
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await db.Students.AsNoTracking().Include(item => item.Campus).SingleOrDefaultAsync(item => item.ID == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        await PrepareOptions();
        return View(new Student());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Name,Address,CampusID")] Student item)
    {
        await ValidateSelection(item);
        if (ModelState.IsValid)
        {
            db.Students.Add(item);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        await PrepareOptions(item.CampusID);
        return View(item);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await db.Students.FindAsync(id);
        if (item == null) return NotFound();
        await PrepareOptions(item.CampusID);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Address,CampusID")] Student item)
    {
        if (id != item.ID) return BadRequest();
        await ValidateSelection(item);
        if (!ModelState.IsValid)
        {
            await PrepareOptions(item.CampusID);
            return View(item);
        }
        var existing = await db.Students.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Name = item.Name;
        existing.Address = item.Address;
        existing.CampusID = item.CampusID;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Students.AsNoTracking().Include(item => item.Campus).SingleOrDefaultAsync(item => item.ID == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await db.Students.Include(item => item.Campus).SingleOrDefaultAsync(item => item.ID == id);
        if (item == null) return NotFound();

        db.Students.Remove(item);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task PrepareOptions(int? selected = null)
    {
        ViewBag.CampusOptions = new SelectList(
            await db.Campuses.AsNoTracking().OrderBy(item => item.Name).ToListAsync(),
            "ID", "Name", selected);
    }

    private async Task ValidateSelection(Student item)
    {
        if (!await db.Campuses.AnyAsync(parent => parent.ID == item.CampusID))
            ModelState.AddModelError("CampusID", "Choose an existing campus.");
    }

}
