using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Image
    {
        public List<Provider> provider { get; set; }
        public string contentUrl { get; set; }
        public string contentUrlPingSuffix { get; set; }
        public string hostPageUrl { get; set; }
        public string hostPageUrlPingSuffix { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}