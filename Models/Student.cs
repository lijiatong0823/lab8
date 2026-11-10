using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lab8.Models;

public class Student
{
    public int ID { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(200)]
    public string Address { get; set; } = string.Empty;
    [Range(1, int.MaxValue), Display(Name = "Campus")]
    public int CampusID { get; set; }
    [ValidateNever]
    public UniversityCampus Campus { get; set; } = null!;
}
