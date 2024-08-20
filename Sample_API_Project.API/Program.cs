using Microsoft.EntityFrameworkCore;
using Sample_API_Project.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopContext>(option =>
{
    option.UseInMemoryDatabase("Shop");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

//It Must Be Used
using (var Scope = app.Services.CreateScope())
{
    var db = Scope.ServiceProvider.GetRequiredService<ShopContext>();
    db.Database.EnsureCreatedAsync().Wait();
    // Or await db.Database.EnsureCreatedAsync();
}
//Minimal APIs
app.MapGet("/Product", async (ShopContext _context) =>
{
    return _context.Products.ToList();
});
app.MapGet("/Product/{id}", async (int id, ShopContext _context) =>
{
    var product = _context.Products.Find(id);
    if (product is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(product);
});
app.MapGet("/Product/IsAvailable", async (ShopContext _context) =>
{
    return _context.Products.Where(c => c.IsAvailable).ToArrayAsync();
});

app.Run();
