using Lab8.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab8.ViewModels;

public class StudentIndexViewModel
{
    public string? Search { get; set; }
    public string? Address { get; set; }
    public int? CampusID { get; set; }
    public IReadOnlyList<Student> Students { get; set; } = new List<Student>();
    public IReadOnlyList<CampusWithCount> CampusesWithCount { get; set; }
        = new List<CampusWithCount>();
    public IEnumerable<SelectListItem> CampusFilterItems =>
        CampusesWithCount.Select(campus => new SelectListItem
        {
            Value = campus.CampusID.ToString(),
            Text = $"{campus.CampusName} ({campus.StudentCount})"
        });
}

public class CampusWithCount
{
    public int CampusID { get; set; }
    public string CampusName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}
