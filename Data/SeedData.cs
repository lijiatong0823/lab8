using Lab8.Models;

namespace Lab8.Data;

public static class SeedData
{
    public static void Initialize(Lab8Context db)
    {
        if (!db.Campuses.Any())
        {
            db.Campuses.AddRange(
                new UniversityCampus
                {
                    Name = "Auckland",
                    Students = new List<Student>
                    {
                        new() { Name = "Alex Chen", Address = "12 Sample Road, Auckland" },
                        new() { Name = "Mia Lee", Address = "35 Example Lane, Auckland" }
                    }
                },
                new UniversityCampus
                {
                    Name = "Manawatu",
                    Students = new List<Student>
                    {
                        new() { Name = "Sam Wilson", Address = "8 Demo Street, Palmerston North" },
                        new() { Name = "Alex Smith", Address = "22 Sample Road, Palmerston North" }
                    }
                },
                new UniversityCampus
                {
                    Name = "Wellington",
                    Students = new List<Student>
                    {
                        new() { Name = "Jordan Taylor", Address = "24 Demo Avenue, Wellington" },
                        new() { Name = "Mia Brown", Address = "10 Example Lane, Wellington" }
                    }
                });
        }
        if (!db.Categories.Any())
        {
            db.Categories.Add(new Category
            {
                Name = "Stationery",
                Products = new List<Product>
                {
                    new() { Name = "Notebook", Price = 4.50m, Description = "A5 ruled notebook" },
                    new() { Name = "Pen", Price = 2.00m, Description = "Blue ink pen" }
                }
            });
        }
        db.SaveChanges();
    }
}
