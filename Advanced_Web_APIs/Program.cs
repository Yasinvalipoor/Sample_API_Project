using Advanced_Web_APIs.Models.DbConfig;
using Advanced_Web_APIs.Models.entities;
using Advanced_Web_APIs.Models.Query;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("v"),//QueryString = Searching?sku=AWMPS&v=1.0
        new HeaderApiVersionReader("X-API-Version"),//Http Header For Accept */*
        new MediaTypeApiVersionReader("ver"));//Http Header For Accept application/json;ver=2.0
})
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

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
        products = products.Where(p => p.Price >= priceQuery.MinPrice.Value && p.Price <= priceQuery.MaxPrice.Value);
    }
    return Results.Ok(await products.ToArrayAsync());
});

app.Run();