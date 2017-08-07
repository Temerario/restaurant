using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class RootObject
    {
        public string _type { get; set; }
        public Instrumentation instrumentation { get; set; }
        public QueryContext queryContext { get; set; }
        public int totalEstimatedMatches { get; set; }
        public List<Filter> filters { get; set; }
        public List<Sort> sort { get; set; }
        public List<Value> value { get; set; }
        public SearchAction searchAction { get; set; }
    }
}