using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Provider
    {
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
    }
}