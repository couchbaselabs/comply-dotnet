using System.Collections.Generic;

namespace ComplyWebApi.Models.DocumentModels
{
    public class Project
    {
        public string _id { get; set; }
        public string CreatedOn { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Permalink { get; set; }

        public string Owner { get; set; }
        public List<string> Users { get; set; } 
        public List<string> Tasks { get; set; } 
    }
}