
﻿namespace StyleHub.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Totalamount { get; set; }
        public string Status { get; set; }
        public string AddressId { get; set; }
        public int phone { get; set; }
        public bool payment { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int CartId { get; set; }
        public string UserId { get; set; }
        public ICollection<AddressDto> Addresses { get; set; }
        public Cart Cart { get; set; }
        public List<OrderItem> OrderItems { get; set; }

    }
}
