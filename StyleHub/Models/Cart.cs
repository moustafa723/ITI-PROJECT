

namespace StyleHub.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public ICollection<Cart_Item> CartItems { get; set; } 

    }
}
