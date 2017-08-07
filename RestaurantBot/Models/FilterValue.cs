using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class FilterValue
    {
        public string name { get; set; }
        public bool isSelected { get; set; }
        public string url { get; set; }
        public ScalarValue scalarValue { get; set; }
    }
}