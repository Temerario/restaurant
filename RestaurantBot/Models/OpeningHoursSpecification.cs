using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class OpeningHoursSpecification
    {
        public string dayOfWeek { get; set; }
        public Time opens { get; set; }
        public Time closes { get; set; }
    }
}