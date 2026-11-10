using Lab8.Data;
using Lab8.Models;
using Lab8.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab8.Controllers;

public class ProductsController(Lab8Context db) : Controller
{
    public async Task<IActionResult> Index(ProductIndexViewModel model)
    {
        model.Search = model.Search?.Trim();
        var products = db.Products.AsNoTracking().Include(product => product.Category).AsQueryable();
        if (!string.IsNullOrEmpty(model.Search))
        {
            string term = model.Search.ToLowerInvariant();
            products = products.Where(product => product.Name.ToLower().Contains(term)
                || product.Description.ToLower().Contains(term)
                || product.Category.Name.ToLower().Contains(term));
        }
        var counts = await products.GroupBy(product => product.CategoryID)
            .Select(group => new { CategoryID = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.CategoryID, item => item.Count);
        var categories = await db.Categories.OrderBy(category => category.Name).ToListAsync();
        model.CategoryFilterItems = categories.Select(category => new SelectListItem
        {
            Value = category.ID.ToString(),
            Text = $"{category.Name} ({counts.GetValueOrDefault(category.ID)})"
        }).ToList();
        if (model.CategoryID.HasValue)
            products = products.Where(product => product.CategoryID == model.CategoryID);
        model.Products = await products.OrderBy(product => product.Name).ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await db.Products.AsNoTracking().Include(item => item.Category).SingleOrDefaultAsync(item => item.ID == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        await PrepareOptions();
        return View(new Product());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Name,Price,Description,CategoryID")] Product item)
    {
        await ValidateSelection(item);
        if (ModelState.IsValid)
        {
            db.Products.Add(item);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        await PrepareOptions(item.CategoryID);
        return View(item);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await db.Products.FindAsync(id);
        if (item == null) return NotFound();
        await PrepareOptions(item.CategoryID);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Price,Description,CategoryID")] Product item)
    {
        if (id != item.ID) return BadRequest();
        await ValidateSelection(item);
        if (!ModelState.IsValid)
        {
            await PrepareOptions(item.CategoryID);
            return View(item);
        }
        var existing = await db.Products.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Name = item.Name;
        existing.Price = item.Price;
        existing.Description = item.Description;
        existing.CategoryID = item.CategoryID;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Products.AsNoTracking().Include(item => item.Category).SingleOrDefaultAsync(item => item.ID == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await db.Products.Include(item => item.Category).SingleOrDefaultAsync(item => item.ID == id);
        if (item == null) return NotFound();

        db.Products.Remove(item);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task PrepareOptions(int? selected = null)
    {
        ViewBag.CategoryOptions = new SelectList(
            await db.Categories.AsNoTracking().OrderBy(item => item.Name).ToListAsync(),
            "ID", "Name", selected);
    }

    private async Task ValidateSelection(Product item)
    {
        if (!await db.Categories.AnyAsync(parent => parent.ID == item.CategoryID))
            ModelState.AddModelError("CategoryID", "Choose an existing category.");
    }

}
