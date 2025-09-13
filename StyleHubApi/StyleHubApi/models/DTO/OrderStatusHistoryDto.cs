using StyleHubApi.Models;
using System.Text.Json.Serialization;

namespace StyleHubApi.models.DTO
{
    public class OrderStatusHistoryDto
    {
        [JsonPropertyName("state")]

        public OrderStatus Status { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
