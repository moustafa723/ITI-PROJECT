
﻿namespace StyleHub.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? AddressId { get; set; }
        public AddressDto Address { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
