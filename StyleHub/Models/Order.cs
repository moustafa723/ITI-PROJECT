namespace StyleHub.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public float Totalamount { get; set; }
        public string Status { get; set; }
        public string address { get; set; }
        public int phone { get; set; }
        public bool payment { get; set; }

    }
}
