namespace ITI_FinalProject.Models
{
    public class Cart_Item
    {
       public int Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public Cart carts { get; set; }
        public ICollection<product> products { get; set; }

    }
}
