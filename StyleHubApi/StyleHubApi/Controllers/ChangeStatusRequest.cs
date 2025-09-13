using StyleHubApi.Models;

namespace StyleHubApi.Controllers
{
    public class ChangeStatusRequest
    {
        public OrderStatus NewStatus { get; set; }

    }
}