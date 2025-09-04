namespace StyleHubApi.models.Vm
{
    public class CategoryVm
    {
      
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;

            // هنا هنستقبل الصورة من الفورم
            public IFormFile? Photo { get; set; }

            public int Count_Product { get; set; }

            public string Back_Color { get; set; } = "#FFFFFF";
       
    }
}
