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
        private static string REVIEWS_VALUE = "Reviews of ";
        private static string TIMINGS_VALUE = "Timings of ";

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
            if (value!= null && value.photo != null && value.photo.Count > 0 && value.photo[0].thumbnailUrl != null)
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
            if (isSelected) title = title + "*";
            CardAction cardButton = new CardAction()
            {
                Value = value,
                Type = action,
                Title = title
            };
            return cardButton;
        }

        public static string getQueryUrl(Dictionary<string, string> bingUrlHash, string query)
        {
            string queryUrl = "";
            string queryValue = getQueryValue(query);
            if (handleRestaurantEvents(query)) // getting current url in case of reviews or timings
            {
                bingUrlHash.TryGetValue(AppInfo.CURRENT_URL, out queryUrl);
            }
            else if (bingUrlHash != null && bingUrlHash.ContainsKey(queryValue))
            {
                bingUrlHash.TryGetValue(queryValue, out queryUrl);
            }
            else
            {
                queryUrl = BingServiceUtils.URL + query;
            }
            return queryUrl;
        }

        public static string getReviewsImBackValue(Value value)
        {
            return REVIEWS_VALUE + value.name;
        }

        public static string getTimingsImBackValue(Value value)
        {
            return TIMINGS_VALUE + value.name;
        }


        public static bool handleRestaurantEvents(string text)
        {
            MessageType messageType = parseQuery(text);
            if(messageType.Equals(MessageType.Timings)|| messageType.Equals(MessageType.Reviews))
            {
                return true;
            }
            return false;
        }
        public static MessageType parseQuery(string query)
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
            else if (query.Contains("Review"))
            {
                messageType = MessageType.Reviews;
            }
            else if (query.Contains("Timing"))
            {
                messageType = MessageType.Timings;
            }
            return messageType;
        }

        public static string getReplyTimingText(RootObject rootObject, string inputText)
        {
            string restaurantName = getRemoveSubString(inputText, TIMINGS_VALUE);
            Value timingsValue = getRestaurantValue(rootObject, restaurantName);
            string outputString = "";
            foreach(OpeningHoursSpecification openDay in timingsValue.openingHoursSpecification) {
                outputString += openDay.dayOfWeek + ": " + openDay.opens.hour.ToString("00") + ":" + openDay.opens.minute.ToString("00") +" - " +
                    openDay.closes.hour.ToString("00") + ":" + openDay.closes.minute.ToString("00") + "<br />";
            }

            return outputString;
        }

        public static List<string> getReplyReviewsText(RootObject rootObject, string inputText)
        {
            string restaurantName = getRemoveSubString(inputText, REVIEWS_VALUE);
            Value reviewsValue = getRestaurantValue(rootObject, restaurantName);
            List<string> reviews = new List<string>();
            foreach(Review review in reviewsValue.review)
            {
                string reviewText = "";
                reviewText = review.provider.Last<Provider>().name + "<br />";
                reviewText += review.reviewRating.text + "/" + review.reviewRating.bestRating.ToString() + "<br />";
                reviewText += review.comment.text;
                reviews.Add(reviewText);
            }
            return reviews;
        }

        private static Value getRestaurantValue(RootObject rootObject, string restaurantName)
        {
            Value returnValue = null;

            foreach (Value value in rootObject.value)
            {
                if (value.name.ToLower() == restaurantName.ToLower())
                {
                    returnValue = value;
                    break;
                }
            }
            return returnValue;
        }
        private static string getQueryValue(string query)
        {
            var split = query.Split(' ');
            return split[split.Length - 1].TrimEnd().TrimStart().ToLower();
         }
        private static string getRemoveSubString(string sourceString, string removeString)
        {
            int index = sourceString.IndexOf(removeString);
            string cleanPath = (index < 0)
                ? sourceString
                : sourceString.Remove(index, removeString.Length);
            return cleanPath.TrimStart().TrimEnd();
        }

    }
}