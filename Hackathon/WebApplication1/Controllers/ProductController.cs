using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductContext _context;

        public ProductController(ProductContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
          if (_context.Product == null)
          {
              return NotFound();
          }
            return await _context.Product.ToListAsync();
        }

        public int calculate(List<Product> products, string str)
        {
            Dictionary<char, int> strCount = new Dictionary<char, int>();
            foreach (char ch in str)
            {
                if (strCount.ContainsKey(ch)) strCount[ch]++;
                else strCount[ch] = 1;
            }
            Dictionary<char, int> prices = new Dictionary<char, int>();
            Dictionary<char, (int frequencyTerm, int discountedPrice)> offers = new Dictionary<char, (int, int)>();
            foreach (Product product in products)
            {
                if (!prices.ContainsKey(product.Item)) prices[product.Item] = product.Price;
                else continue;

                if (product.Offer != null)
                {
                    int quantity = Convert.ToInt32(product.Offer.Split(" ")[0]);
                    int price = Convert.ToInt32(product.Offer.Split(" ")[2]);

                    if (!offers.ContainsKey(product.Item)) offers[product.Item] = (quantity, price);
                    else continue;
                }
            }

            int total = 0;
            int offerTotal = 0;
            int normalTotal = 0;


            foreach (KeyValuePair<char, int> pair in strCount)
            {

                Console.WriteLine(pair.Key + "------" + pair.Value);
                if (offers.ContainsKey(pair.Key))
                {
                    //frenquency Multiplier Calculation
                    var tuple = offers[pair.Key];
                    int frequencyTerm = tuple.frequencyTerm;
                    int discountedPrice = tuple.discountedPrice;
                    
                    int nosOffer = pair.Value / frequencyTerm;
                    int remainderOffer = pair.Value % frequencyTerm;

                    offerTotal = nosOffer * discountedPrice;
                    
                    if(remainderOffer > 0) 
                    {
                        offerTotal += remainderOffer * prices[pair.Key];
                    }
                    
                }
                else
                {
                    //Normal Calculation
                    normalTotal = pair.Value * prices[pair.Key];
                }

                Console.WriteLine(total);
                total += normalTotal + offerTotal;

            }


            return total;
        }

        //GET: api/Product/checkout?str="AAAB"
        [HttpGet("checkout")]
        public async Task<int> CheckoutProduct(string str)
        {
            var products = await _context.Product.ToListAsync();

            int result = calculate(products, str);
            return result;
            //Console.Out.WriteLine(products);
            //return Ok("dev this!!!!");
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(char id)
        {
          if (_context.Product == null)
          {
              return NotFound();
          }
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(char id, Product product)
        {
            if (id != product.Item)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
          if (_context.Product == null)
          {
              return Problem("Entity set 'ProductContext.Product'  is null.");
          }
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Item }, product);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(char id)
        {
            if (_context.Product == null)
            {
                return NotFound();
            }
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(char id)
        {
            return (_context.Product?.Any(e => e.Item == id)).GetValueOrDefault();
        }
    }
}
