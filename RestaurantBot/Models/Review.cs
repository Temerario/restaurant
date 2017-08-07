using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Review
    {
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public List<Provider> provider { get; set; }
        public ReviewRating reviewRating { get; set; }
        public Comment comment { get; set; }
    }
}