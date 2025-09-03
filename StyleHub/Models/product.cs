using System.Runtime.InteropServices;

namespace StyleHub.Models
{
    public class product
    {
        public int Id  { get; set; }  
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string color { get; set; }
        public string image { get; set; }
       public string in_stock { get; set; }
        public ICollection<Catogery> catogeries { get; set; }
    }
}
