public class Cart
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public User? User { get; set; }   // one-to-one
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
