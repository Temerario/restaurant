using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Filter
    {
        public string name { get; set; }
        public bool isSelected { get; set; }
        public string id { get; set; }
        public List<FilterValue> filterValues { get; set; }
        public ClearFilterValue clearFilterValue { get; set; }
    }
}