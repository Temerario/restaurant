using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class ReviewRating
    {
        public string text { get; set; }
        public int ratingValue { get; set; }
        public int bestRating { get; set; }
    }

}