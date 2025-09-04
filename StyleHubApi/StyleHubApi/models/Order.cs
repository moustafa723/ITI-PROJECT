using Microsoft.EntityFrameworkCore;

namespace StyleHubApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string address { get; set; }
        public int phone { get; set; }
        public bool payment { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
