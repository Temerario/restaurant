using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Comment
    {
        public string dateCreated { get; set; }
        public Creator creator { get; set; }
        public string text { get; set; }
    }
}