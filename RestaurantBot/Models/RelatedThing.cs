using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class RelatedThing
    {
        public string _type { get; set; }
        public string readLink { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public string name { get; set; }
        public Image image { get; set; }
        public EntityPresentationInfo entityPresentationInfo { get; set; }
        public string bingId { get; set; }
        public Geo geo { get; set; }
        public RoutablePoint routablePoint { get; set; }
        public bool isPermanentlyClosed { get; set; }
    }
}