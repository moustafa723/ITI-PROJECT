namespace StyleHubApi.models.Vm
{
    public class CategoryVm
    {
      
            public string Name { get; set; } = string.Empty;

            // هنا هنستقبل الصورة من الفورم
            public IFormFile? Photo { get; set; }


            public string Back_Color { get; set; } = "#FFFFFF";
       
    }
}
