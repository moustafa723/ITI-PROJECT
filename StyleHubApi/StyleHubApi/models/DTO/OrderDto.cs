namespace StyleHubApi.models.DTO
{
    public class OrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AddressId { get; set; }              // ✅ عنوان الأوردر

        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
