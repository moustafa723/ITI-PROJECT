using Microsoft.EntityFrameworkCore;

namespace StyleHubApi.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }
        //public List<string> ImageUrl { get; set; }

        public Cart? Cart { get; set; }
        public Product? Product { get; set; }
    }

}
