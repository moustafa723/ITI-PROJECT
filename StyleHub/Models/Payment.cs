namespace ITI_FinalProject.Models
{
    public class Payment
    {
        public int ID { get; set; }
        public double amount { get; set; }
        public DateOnly date { get; set; }
        public ICollection<Order> orders { get; set; }
    }
}
