using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace StyleHubApi.Models.DTO
{
    public class AddressDto
    {
        public string Label { get; set; }    // Home / Office
        public string Line1 { get; set; }
        [BindNever]           // يمنع الـ ModelBinder من توقعها
        [JsonIgnore]          // يمنع JSON من طلبها
        public string? UserId { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
    }
}