namespace StyleHubApi.models
{
    public class Address
    {
        public int Id { get; set; }
        public string UserId { get; set; }   // FK إلى AspNetUsers

        public string Label { get; set; }    // Home / Office
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string ContactName { get; set; }
        public string Phone { get; set; }

        public bool IsDefault { get; set; }
    }
   


}
