namespace StyleHub.Models
{
    public class AccountViewModel
    {
        public List<AddressDto> Addresses { get; set; } = new();
        public List<Order> MyOrders { get; set; } = new();
    }
}
