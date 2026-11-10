using System.ComponentModel.DataAnnotations;

namespace Lab8.Models;

public class Category
{
    public int ID { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
