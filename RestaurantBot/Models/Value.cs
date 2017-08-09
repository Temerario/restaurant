using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class Value
    {
        public string _type { get; set; }
        public string id { get; set; }
      //  public string readLink { get; set; }
      //  public List<ContractualRule> contractualRules { get; set; }
       // public string webSearchUrl { get; set; }
     //   public string webSearchUrlPingSuffix { get; set; }
        public string name { get; set; }
        public string url { get; set; }
      //  public string urlPingSuffix { get; set; }
        //public Image image { get; set; }
      //  public SocialMediaInfo socialMediaInfo { get; set; }
      //  public EntityPresentationInfo entityPresentationInfo { get; set; }
      //  public string bingId { get; set; }
        public Geo geo { get; set; }
     //   public RoutablePoint routablePoint { get; set; }
     //   public Address address { get; set; }
     //   public string telephone { get; set; }
     //   public AggregateRating aggregateRating { get; set; }
      //  public List<OpeningHoursSpecification> openingHoursSpecification { get; set; }
      //  public List<Review> review { get; set; }
        public List<Photo> photo { get; set; }
     //   public string timeZone { get; set; }
     //   public string priceRange { get; set; }
     //   public List<string> paymentAccepted { get; set; }
    //    public bool isPermanentlyClosed { get; set; }
     //   public List<string> features { get; set; }
     //   public bool acceptsReservations { get; set; }
        public string reservationUrl { get; set; }
     //   public string reservationUrlPingSuffix { get; set; }
     //   public List<string> servesCuisine { get; set; }
        public string menuUrl { get; set; }
     //   public string menuUrlPingSuffix { get; set; }
    //    public List<Dish> dishes { get; set; }
        public string description { get; set; }
    }
}