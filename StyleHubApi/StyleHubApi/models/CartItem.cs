using Microsoft.EntityFrameworkCore;

namespace StyleHubApi.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
