using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingdongCall.Models
{
    public class DingDongOpenRequest
    {
        public string operation { get; set; }
        public string userid { get; set; }
        public string timestamp { get; set; }
        public string appid { get; set; }
        public string isowner { get; set; }
    }
}