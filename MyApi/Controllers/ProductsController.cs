using System.Diagnostics;
using System.Security.Claims;

namespace MyApi.Controllers;

[ApiController]
[Route("products")]
//[Authorize]
public class ProductsController : ControllerBase {
    private readonly MyContext _context;

    public ProductsController(MyContext context) {
        _context = context;
    }

    [HttpGet("/test")]
    public ActionResult<Account> Test() {
        var identity = User.Identity;

        if(!identity.IsAuthenticated)
            return null;

        var c = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        var account = _context.Accounts.Find(int.Parse(c.Value));
        return account;
    }


    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts() => await _context.Products.ToListAsync();

    // GET: api/Products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id) {
        var product = await _context.Products.FindAsync(id);

        if(product == null) {
            return NotFound();
        }

        return product;
    }

    // PUT: api/Products/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutProduct(int id, Product product) {
        if(id != product.Id) {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        } catch(DbUpdateConcurrencyException) {
            if(!ProductExists(id)) {
                return NotFound();
            } else {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Products
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product) {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    // DELETE: api/Products/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id) {
        var product = await _context.Products.FindAsync(id);
        if(product == null) {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id) => _context.Products.Any(e => e.Id == id);
}
