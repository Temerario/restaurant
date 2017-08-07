using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Profile
    {
        public string profileId { get; set; }
        public string profileUrl { get; set; }
        public string profileUrlPingSuffix { get; set; }
        public string socialNetwork { get; set; }
    }
}