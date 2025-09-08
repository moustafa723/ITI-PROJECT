using System.ComponentModel.DataAnnotations.Schema;

namespace StyleHubApi.Models
{
    // StyleHubApi.Models
    public class Cart
    {
        public int Id { get; set; }

        // Identity user key is string
        public string? UserId { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }




}
