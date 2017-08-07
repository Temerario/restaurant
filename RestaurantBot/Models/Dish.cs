using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Dish
    {
        public string name { get; set; }
        public AggregateRating aggregateRating { get; set; }
        public List<Review> review { get; set; }
        public int totalReviewCount { get; set; }
        public int positiveReviewCount { get; set; }
        public int negativeReviewCount { get; set; }
    }
}