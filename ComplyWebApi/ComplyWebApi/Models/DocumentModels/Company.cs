namespace ComplyWebApi.Models.DocumentModels
{
    public class Company
    {
        public string _id { get; set; }
        public string _type { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
    }
}