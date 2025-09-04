using Microsoft.EntityFrameworkCore;

namespace StyleHubApi.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }

}
