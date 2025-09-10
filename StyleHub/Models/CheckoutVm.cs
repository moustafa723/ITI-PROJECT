namespace StyleHub.Models
{
    public class CheckoutVm
    {
        public Cart Cart { get; set; } = new();
        public AddressDto? DefaultAddress { get; set; }
    }

}
