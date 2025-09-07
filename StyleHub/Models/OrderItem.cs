namespace StyleHub.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal unitPrice { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public Order Orders { get; set; }
        public ICollection<Product> products { get; set; }

    }
}
