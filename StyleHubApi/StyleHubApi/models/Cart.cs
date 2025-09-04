using System.ComponentModel.DataAnnotations.Schema;

namespace StyleHubApi.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ICollection<User> Users { get; set; }

    }
}
