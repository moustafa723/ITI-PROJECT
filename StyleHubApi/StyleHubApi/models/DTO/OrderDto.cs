using StyleHubApi.Models;

namespace StyleHubApi.models.DTO
{
    public class OrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AddressId { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new();

        // ✅ الحالة الحالية
        public OrderStatus Status { get; set; }

        // ✅ سجل الحالات
        public List<OrderStatusHistory> StatusHistory { get; set; } = new();
    }
}
