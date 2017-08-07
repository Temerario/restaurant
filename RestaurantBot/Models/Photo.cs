using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Photo
    {
        public string thumbnailUrl { get; set; }
        public List<Provider> provider { get; set; }
        public string contentUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}