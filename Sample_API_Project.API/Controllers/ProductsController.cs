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

        [HttpGet("IsAvailable")]
        public async Task<ActionResult> GetAvailableProducts()
        {
            return Ok(await _context.Products.Where(c => c.IsAvailable).ToArrayAsync());
        }


        //Adding An Item
        [HttpPost]
        public async Task<ActionResult> PostProduct(Product product)
        {
            //Model Validation (Product & Category)
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            //Handling Errors
            if (product is null)
            {
                return NoContent();
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            //CreatedAtAction Is Helper Method
            //Status Code HTTP 201 (Created) Response / (With Option Things Like Id)
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }


        //Updating An Item
        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id) return BadRequest();
            _context.Entry(product).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id)) return NotFound();
                else throw;
            }
            return NoContent();
        }
    }
}
