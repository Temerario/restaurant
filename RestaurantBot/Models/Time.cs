using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Time
    {
        public int hour { get; set; }
        public int minute { get; set; }
        public int second { get; set; }
        public int milliSecond { get; set; }
    }
}