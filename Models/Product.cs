using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lab8.Models;

public class Product
{
    public int ID { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [Range(0, 1000000)]
    public decimal Price { get; set; }
    [StringLength(300)]
    public string Description { get; set; } = string.Empty;
    [Range(1, int.MaxValue), Display(Name = "Category")]
    public int CategoryID { get; set; }
    [ValidateNever]
    public Category Category { get; set; } = null!;
}
