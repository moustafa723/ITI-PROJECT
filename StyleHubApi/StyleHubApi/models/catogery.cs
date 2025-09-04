using System.ComponentModel.DataAnnotations.Schema;

namespace StyleHubApi.models
{
    public class Catogery
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IFormFile Photo { get; set; }
        public int Count_Product { get; set; }
        public string Back_Color { get; set; }

        [ForeignKey(nameof(product))]
        public int productId { get; set; }
        public product product { get; set; }

    }
}
