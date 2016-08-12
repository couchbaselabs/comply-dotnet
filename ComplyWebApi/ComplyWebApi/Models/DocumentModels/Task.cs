using System.Collections.Generic;

namespace ComplyWebApi.Models.DocumentModels
{
    public class Task
    {
        public string _type { get; set; }
        public string _id { get; set; }
        public List<string> Users { get; set; }
        public string Owner { get; set; }
        public string CreatedON { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string Permalink { get; set; }
        public List<History> History { get; set; }
    }

    public class History
    {
        public string Log { get; set; }
        public string User { get; set; }
        public string CreatedAt { get; set; }
    }
}