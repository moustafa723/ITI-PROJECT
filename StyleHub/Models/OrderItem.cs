namespace ITI_FinalProject.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quentity { get; set; }
        public double unitPrice { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<product> products { get; set; }

    }
}
