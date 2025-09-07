
﻿namespace StyleHub.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public decimal Totalamount { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public int phone { get; set; }
        public bool payment { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public List<OrderItem> OrderItems { get; set; }

    }
}
