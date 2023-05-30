using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Product
    {
        [Key]
        public char Item { get; set; }
        public int Price { get; set; }
        public string? Offer { get; set; }
    }
}
