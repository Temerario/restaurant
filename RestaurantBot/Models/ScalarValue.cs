using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class ScalarValue
    {
        public int maxRating { get; set; }
        public int minRating { get; set; }
        public string minLevel { get; set; }
        public string maxLevel { get; set; }
        public string symbol { get; set; }
    }
}