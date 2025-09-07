using Microsoft.EntityFrameworkCore;

namespace StyleHubApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }


        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool Payment { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

}
