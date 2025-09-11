
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace StyleHubApi.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }
        public Product Product { get; set; }
        [JsonIgnore]
        public  Order Order { get; set; }
    }
}
