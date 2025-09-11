using Microsoft.EntityFrameworkCore;
using StyleHubApi.models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleHubApi.Models
{

    [Table("Orders")]
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }


        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; }
        public string AddressId { get; set; }
        public string Phone { get; set; }
        public bool Payment { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string UserId { get; set; }
        public ICollection<Address> Addresses { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

}
