using Microsoft.EntityFrameworkCore;
using StyleHubApi.models;

namespace StyleHubApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal TotalAmount { get; internal set; }
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
    }

}
