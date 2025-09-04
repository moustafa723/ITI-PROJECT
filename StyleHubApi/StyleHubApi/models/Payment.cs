using Microsoft.EntityFrameworkCore;

namespace StyleHubApi.Models
{
    public class Payment
    {
        public int Id { get; set; }
        [Precision(18, 2)]

        public decimal amount { get; set; }
        public DateTime date { get; set; }
        public ICollection<Order> orders { get; set; }
    }
}
