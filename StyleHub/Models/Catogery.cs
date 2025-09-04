using System.ComponentModel.DataAnnotations.Schema;

namespace StyleHub.Models
{
    public class Catogery
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(product))]
        public int productId { get; set; }
        public product product { get; set; }

    }
}
