using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class EntityPresentationInfo
    {
        public string entityScenario { get; set; }
        public List<string> entityTypeHints { get; set; }
        public List<Related> related { get; set; }
    }
}