using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Related
    {
        public string displayName { get; set; }
        public string id { get; set; }
        public List<Relationship> relationships { get; set; }
    }
}