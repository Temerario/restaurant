using Microsoft.Bot.Connector;
using RestaurantBot.Constants;
using RestaurantBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Utils
{
    public class DailogUtils
    {
        private static string FILTER_STRING = "filter applied on ";
        private static string GOOGLEMAPS_URL = "https://www.google.com/maps?saddr=My+Location&daddr=";

        public static List<CardAction> createFilterCardAction(Filter filter)
        {
            if (filter == null) return null;
            List<CardAction> listCardActions = new List<CardAction>();

            foreach (FilterValue filterValue in filter.filterValues)
            {
                CardAction menuCard = createCardAction(FILTER_STRING + filterValue.name, AppInfo.IMBACK, filterValue.name);
                listCardActions.Add(menuCard);
            }
            return listCardActions;
        }
        public static List<CardImage> createRestaurantImageActions(Value value)
        {
            List<CardImage> cardImages = new List<CardImage>();
            if (value.photo.Count > 0 && value.photo[0].thumbnailUrl != null)
            {
                cardImages.Add(new CardImage(url: value.photo[0].thumbnailUrl));

                return cardImages;
            }
            return null;
        }
        public static string getGoogleMapsLink(Value value)
        {
            if (value.geo == null) return null;
            return GOOGLEMAPS_URL + value.geo.latitude.ToString() + "," + value.geo.longitude.ToString();
        }
        public static CardAction createCardAction(string value, string action, string title)
        {
            if (value == null || action == null || title == null) return null;
            CardAction cardButton = new CardAction()
            {
                Value = value,
                Type = action,
                Title = title
            };
            return cardButton;
        }

    }
}