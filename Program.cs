using Lab8.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Lab8Context>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Lab8Context")));
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Lab8Context>();
    db.Database.Migrate();
    SeedData.Initialize(db);
}
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
