using System.Collections.Generic;

namespace DingdongCall.Models
{
    public class Application_info
    {
        public string application_id { get; set; }
        public string application_name { get; set; }
    }

    public class Attributes
    {
        public string bizname { get; set; }
        public string type { get; set; }
    }

    public class Session
    {
        public string is_new { get; set; }
        public string session_id { get; set; }
        public Dictionary<string, string> attributes { get; set; }
    }


    public class User
    {
        public string user_id { get; set; }
        public Dictionary<string, string> attributes { get; set; }
    }

    public class Slots
    {
        public string bizname { get; set; }
        public string type { get; set; }
    }

    public class Extend
    {
    }

    public class DingDongRequest
    {
        public string versionid { get; set; }
        public string status { get; set; }
        public string sequence { get; set; }
        public string timestamp { get; set; }
        public Application_info application_info { get; set; }
        public Session session { get; set; }
        public User user { get; set; }
        public string input_text { get; set; }
        public Slots slots { get; set; }
        public Extend extend { get; set; }
    }
}