using System.ComponentModel.DataAnnotations.Schema;

namespace StyleHub.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public ICollection<Cart_Item> CartItems { get; set; } 

    }
}
