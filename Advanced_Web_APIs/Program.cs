using Advanced_Web_APIs.Models.DbConfig;
using Advanced_Web_APIs.Models.entities;
using Advanced_Web_APIs.Models.Query;
using Microsoft.EntityFrameworkCore;

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopContext>();
    db.Database.EnsureCreatedAsync().Wait();
}

//Tip = [AsParameters] 
app.MapGet("/Product/Filtering", async (ShopContext _context, [AsParameters] PriceQueryParameters priceQuery) =>
{
    IQueryable<Product> products = _context.Products;
    if (priceQuery.MaxPrice is not null && priceQuery.MinPrice is not null)
    {
        products = products.Where(p => p.Price >= priceQuery.MinPrice && p.Price <= priceQuery.MaxPrice);
    }
    return Results.Ok(await products.ToArrayAsync());
});

app.Run();