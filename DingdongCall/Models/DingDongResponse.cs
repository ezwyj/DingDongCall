using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingdongCall.Models
{
    public class Directive_items
    {
        public string content { get; set; }
        public string type { get; set; }
    }

    public class Directive
    {
        public List<Directive_items> directive_items { get; set; }
    }

    public class Rich_contents
    {
    }

    public class Push_to_app
    {
        public List<Rich_contents> rich_contents { get; set; }
        public string text { get; set; }
        public string title { get; set; }
        public string type { get; set; }
    }

    public class DingDongResponse
    {
        public Directive directive { get; set; }
        public string is_end { get; set; }
        public Push_to_app push_to_app { get; set; }
        public string sequence { get; set; }
        public string timestamp { get; set; }
        public string versionid { get; set; }
    }
}