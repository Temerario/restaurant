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
                CardAction menuCard = createCardAction(FILTER_STRING + "  " + filter.name + " - " + filterValue.name, AppInfo.IMBACK, filterValue.name, filterValue.isSelected);
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
        public static CardAction createCardAction(string value, string action, string title, bool isSelected = false)
        {
            if (value == null || action == null || title == null || value == "") return null;
            if (isSelected) title = "*" + title;
            CardAction cardButton = new CardAction()
            {
                Value = value,
                Type = action,
                Title = title
            };
            return cardButton;
        }

        public static string getQueryUrl(RootObject rootObject, string query)
        {
            string queryUrl = "";
            var queryString = parseQuery(query);
            string queryValue = getQueryValue(query);
            foreach(Filter filter in rootObject.filters)
            {
                if (filter.name.Equals(queryString.ToString()) && queryUrl.Equals(""))
                {
                    foreach(FilterValue filterValue in filter.filterValues)
                    {
                        if (filterValue.name.Contains(queryValue))
                        {
                            queryUrl = filterValue.url;
                            filterValue.isSelected = true;
                            break;
                        }
                    }
                }
                continue;
            }
            if (queryUrl == "") queryUrl = BingServiceUtils.URL + query;
            return queryUrl;
        }

        private static MessageType parseQuery(string query)
        {
            MessageType messageType = MessageType.Default;
            if (query.Contains("Cuisine"))
            {
                messageType = MessageType.Cuisines;
            }
            else if (query.Contains("Rating"))
            {
                messageType = MessageType.Ratings;
            }
            else if (query.Contains("Price"))
            {
                messageType = MessageType.Prices;
            }
            return messageType;
        }

        private static string getQueryValue(string query)
        {
            var split = query.Split('-');
            return split[split.Length - 1].TrimEnd().TrimStart();
         }

    }
}