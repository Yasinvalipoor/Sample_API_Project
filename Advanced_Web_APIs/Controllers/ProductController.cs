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
    public async Task<ActionResult> GetAllProductsByPaging([FromQuery] PaginationQueryParameters paginationQuery)
    {
        if (paginationQuery.Page <= 0 && paginationQuery.Size <= 0) BadRequest("Page and Size must be greater than zero.");

        IQueryable<Product> products = _context.Products.AsQueryable()
            .Skip(paginationQuery.Size * (paginationQuery.Page - 1))
            .Take(paginationQuery.Size);

        return Ok(await products.ToArrayAsync());
    }

    [HttpGet("Filtering")]
    public async Task<ActionResult> GetAllProductsByFiltering([FromQuery] PriceQueryParameters priceQuery)
    {
        IQueryable<Product> products = _context.Products;
        if (priceQuery.MinPrice is not null && priceQuery.MaxPrice is not null)
        {
            products = products.Where(p => p.Price >= priceQuery.MinPrice.Value && p.Price <= priceQuery.MaxPrice.Value);
        }
        return Ok(await products.ToArrayAsync());
    }

    [HttpGet("Searching")]
    public async Task<ActionResult> GetAllProductsBySearching([FromQuery] SearchingQueryParameters searchingQuery)
    {
        IQueryable<Product> products = _context.Products;
        if (!string.IsNullOrEmpty(searchingQuery.Sku))
        {
            products = products.Where(p => p.Sku == searchingQuery.Sku);
        }
        if (!string.IsNullOrEmpty(searchingQuery.Name))
        {
            products = products
                .Where(p => p.Name.ToLower()
                .Contains(searchingQuery.Name.ToLower()));
        }
        return Ok(await products.ToArrayAsync());
    }

    [HttpGet("Sorting")]
    public async Task<IActionResult> GetAllProductsBySorting([FromQuery] SortingQueryParameters sortingQuery)
    {
        IQueryable<Product> products = _context.Products;
        if (!string.IsNullOrEmpty(sortingQuery.SortBy))
        {
            if (typeof(Product).GetProperty(sortingQuery.SortBy) is not null)
            {
                products = products.OrderByCustom(sortingQuery.SortBy, sortingQuery.SortOrder);
            }
        }
        return Ok(await products.ToArrayAsync());

        //if (!string.IsNullOrEmpty(queryParameters.SortBy))
        //{
        //    if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
        //    {
        //        products = products.OrderByCustom(
        //            queryParameters.SortBy,
        //            queryParameters.SortOrder);
        //    }
        //}
    }
}
