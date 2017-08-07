using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class SearchAction
    {
        public List<Location> location { get; set; }
        public string query { get; set; }
    }
}