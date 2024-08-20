using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample_API_Project.API.Models;

namespace Sample_API_Project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();

        }
        //1* Not Bad But Not Good
        //[HttpGet]
        //public IEnumerable<Product> GetAllPeoducts()
        //{
        //    return _context.Products.ToArray();
        //}

        //2* Good - Flexible
        //ActionResult<IEnumerable<Product>> Or We Can Use ActionResult
        //[HttpGet]
        //public ActionResult GetAllPeoducts()
        //{
        //    return Ok(_context.Products.ToArray());
        //}

        //3* Best - Flexible - Fast
        //Using async Task - await
        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            return Ok(await _context.Products.ToArrayAsync());
        }

        //[HttpGet]
        //[Route("{id}")]
        //Or We Can Use Like Below
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            //Handling Errors
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
