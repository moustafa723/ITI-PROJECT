namespace StyleHub.Models
{
    public class Cart_Item
    {
       public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }

        public Cart carts { get; set; }
        public Product Product { get; set; }

    }
}
