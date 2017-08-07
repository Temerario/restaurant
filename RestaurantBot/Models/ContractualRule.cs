using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class ContractualRule
    {
        public string _type { get; set; }
        public string text { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public bool optionalForListDisplay { get; set; }
        public string targetPropertyName { get; set; }
        public bool? mustBeCloseToContent { get; set; }
    }
}