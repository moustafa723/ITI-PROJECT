
﻿namespace StyleHub.Models
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
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? AddressId { get; set; }
        public OrderStatus Status { get; set; }
        public AddressDto Address { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();

    }
}
