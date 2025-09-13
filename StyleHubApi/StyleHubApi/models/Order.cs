using Microsoft.EntityFrameworkCore;
using StyleHubApi.models;

namespace StyleHubApi.Models
{
    public enum OrderStatus
    {
        Order_Confirmed = 0,
        Processing = 1,
        Packaging = 2,
        In_Transit = 3,
        Delivered = 4,
        Cancelled = 5
    }
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Order_Confirmed;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal TotalAmount { get; internal set; }
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();

    }

}
