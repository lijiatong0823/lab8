using System.ComponentModel.DataAnnotations;

namespace Lab8.Models;

public class UniversityCampus
{
    public int ID { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
