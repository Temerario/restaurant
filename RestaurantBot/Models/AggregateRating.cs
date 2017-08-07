using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class AggregateRating
    {
        public string text { get; set; }
        public double ratingValue { get; set; }
        public int bestRating { get; set; }
        public int reviewCount { get; set; }
    }
}