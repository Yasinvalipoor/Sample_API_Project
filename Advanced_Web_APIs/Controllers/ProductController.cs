using Advanced_Web_APIs.Models.DbConfig;
using Advanced_Web_APIs.Models.entities;
using Advanced_Web_APIs.Models.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Advanced_Web_APIs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ShopContext _context;

    public ProductController(ShopContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

    [HttpGet("Paging")]
    public async Task<ActionResult> GetAllProductsByPaging([FromQuery] PaginationQueryParameters queryParameters)
    {
        if (queryParameters.Page <= 0 && queryParameters.Size <= 0) BadRequest("Page and Size must be greater than zero.");

        IQueryable<Product> products = _context.Products.AsQueryable()
            .Skip(queryParameters.Size * (queryParameters.Page - 1))
            .Take(queryParameters.Size);

        return Ok(await products.ToArrayAsync());
    }

    [HttpGet("Filtering")]
    public async Task<ActionResult> GetAllProductsByFiltering([FromQuery] PriceQueryParameters queryParameters)
    {
        IQueryable<Product> products = _context.Products;
        if (queryParameters.MinPrice is not null && queryParameters.MaxPrice is not null)
        {
            products = products.Where(p => p.Price >= queryParameters.MinPrice.Value && p.Price <= queryParameters.MaxPrice.Value);
        }
        return Ok(await products.ToArrayAsync());
    }
}
