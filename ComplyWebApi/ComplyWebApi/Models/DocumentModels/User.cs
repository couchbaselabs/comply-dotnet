namespace ComplyWebApi.Models.DocumentModels
{
    public class User
    {
        public string _id { get; set; } 
        public string _type { get; set; } 
        public Name Name { get; set; } 
        public Address Address { get; set; } 
        public string Company { get; set; } 
        public string Username { get; set; } 
        public string Phone { get; set; } 
        public string Password { get; set; } 
    }

    public class Name
    {
        public string First { get; set; }
        public string Last { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
}