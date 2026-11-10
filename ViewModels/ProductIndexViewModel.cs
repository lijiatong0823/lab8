using Lab8.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab8.ViewModels;

public class ProductIndexViewModel
{
    public string? Search { get; set; }
    public int? CategoryID { get; set; }
    public IReadOnlyList<Product> Products { get; set; } = new List<Product>();
    public IReadOnlyList<SelectListItem> CategoryFilterItems { get; set; }
        = new List<SelectListItem>();
}
