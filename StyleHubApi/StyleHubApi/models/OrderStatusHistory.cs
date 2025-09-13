using StyleHubApi.Models;

namespace StyleHubApi.models
{
 
        public class OrderStatusHistory
        {
            public int Id { get; set; }

            public int OrderId { get; set; }
            public Order Order { get; set; } = null!;

            public OrderStatus Status { get; set; }
            public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        }
    

}
