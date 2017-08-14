using Microsoft.Bot.Connector;
using RestaurantBot.Constants;
using RestaurantBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace RestaurantBot.Utils
{
    public class DailogUtils
    {
        private static string FILTER_STRING_1 = "Narrowing restaurants to {0} {1}";
        private static string FILTER_STRING_2 = "Narrowing to {0} restaurants";
        private static string FILTER_STRING_CUISINE = "Narrowing to {0} restaurants";
        private static string FILTER_STRING_RATING = "Narrowing to {0} rating";


        private static string REVERSE_FILTER_STRING_1 = "^Narrowing restaurants to (.*?) (.*?)$";
        private static string REVERSE_FILTER_STRING_2 = "^Narrowing to (.*?) restaurants$";

        private static string GOOGLEMAPS_URL = "https://www.google.com/maps?saddr=My+Location&daddr=";
        private static string REVIEWS_VALUE = "Reviews of ";
        private static string TIMINGS_VALUE = "Timings of ";

        public static List<CardAction> createFilterCardAction(Filter filter)
        {
            if (filter == null) return null;
            List<CardAction> listCardActions = new List<CardAction>();

            foreach (FilterValue filterValue in filter.filterValues)
            {
                string filterValueName = filterValue.name;
                if (filter.name.Equals(MessageType.Prices.ToString()) && filterValue.scalarValue != null && filterValue.scalarValue.minLevel != null)
                {
                    filterValueName = filterValue.scalarValue.minLevel;
                }

                CardAction menuCard = createCardAction(getCardActionValueFormat(filter.name, filterValueName), AppInfo.IMBACK, filterValueName, filterValue.isSelected);
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
            else if (bingUrlHash != null && bingUrlHash.ContainsKey(queryValue.ToLower()))
            {
                bingUrlHash.TryGetValue(queryValue.ToLower(), out queryUrl);
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

        public static bool handleWelcomeMessage(string text)
        {
            MessageType messageType = parseQuery(text);
            if (messageType.Equals(MessageType.Welcome))
            {
                return true;
            }
            return false;
        }

        public static bool handleFilterActions(string text)
        {
            MessageType messageType = parseQuery(text);
            if (messageType.Equals(MessageType.Cuisines) || messageType.Equals(MessageType.Ratings))
            {
                return true;
            }
            return false;
        }
        public static MessageType parseQuery(string query)
        {
            MessageType messageType = MessageType.Default;
            query = query.ToLower();
            if (query.Contains("cuisine"))
            {
                messageType = MessageType.Cuisines;
            }
            else if (query.Contains("rating"))
            {
                messageType = MessageType.Ratings;
            }
            else if (query.Contains("price"))
            {
                messageType = MessageType.Prices;
            }
            else if (query.Contains("review"))
            {
                messageType = MessageType.Reviews;
            }
            else if (query.Contains("timing"))
            {
                messageType = MessageType.Timings;
            }
            else if (query.Equals("hi") || query.Equals("hello") || query.Equals("hey"))
            {
                messageType = MessageType.Welcome;
            }
            return messageType;
        }

        public static string getReplyTimingText(RootObject rootObject, string inputText)
        {
            string restaurantName = getRemoveSubString(inputText, TIMINGS_VALUE);
            Value timingsValue = getRestaurantValue(rootObject, restaurantName);
            string outputString = "";
            if (timingsValue == null) return outputString;
            foreach (OpeningHoursSpecification openDay in timingsValue.openingHoursSpecification) {
                if (outputString != "") outputString += "<br />";
                outputString += openDay.dayOfWeek + ": " + openDay.opens.hour.ToString("00") + ":" + openDay.opens.minute.ToString("00") +" - " +
                    openDay.closes.hour.ToString("00") + ":" + openDay.closes.minute.ToString("00");
            }

            return outputString;
        }

        public static List<string> getReplyReviewsText(RootObject rootObject, string inputText)
        {
            string restaurantName = getRemoveSubString(inputText, REVIEWS_VALUE);
            Value reviewsValue = getRestaurantValue(rootObject, restaurantName);
            List<string> reviews = new List<string>();
            foreach(Review review in reviewsValue.review.Take(3))
            {
                string reviewText = "";
                reviewText = review.provider.Last<Provider>().name + "<br />";
                reviewText += review.reviewRating.text + "/" + review.reviewRating.bestRating.ToString() + "<br />";
                reviewText += review.comment.text;
             //   reviewText += getHyperLink("read more", review.url);
                reviews.Add(reviewText);
            }
            return reviews;
        }

        public static string restaurantCardSubtitle(Value value)
        {
            string cardSubtitle = "";
            if (value.aggregateRating != null &&value.review != null && value.review.First<Review>() != null)
            {
                cardSubtitle = value.aggregateRating.text +"/" + value.aggregateRating.bestRating.ToString()+ " on " ;
                cardSubtitle += value.review.First<Review>().provider[0].name + " (" + value.aggregateRating.reviewCount.ToString() + ")";
            }

            return cardSubtitle;
        }

        public static string restaurantCardText(Value value)
        {
            string cardText = "";
            if (value.servesCuisine != null)
            {
                cardText = value.servesCuisine.First<string>() + ", ";
            }
            if (value.priceRange != null)
            {
                cardText += value.priceRange.ToString();
            }

            return cardText;
        }

        private static string getCardActionValueFormat(string messageType, string value)
        {
            string cardActionText = "";
            if (MessageType.Prices.ToString().Equals(messageType)) {
                cardActionText = string.Format(FILTER_STRING_2, value.ToLower());
            }
            else if (MessageType.Cuisines.ToString().Equals(messageType))
            {
                cardActionText = string.Format(FILTER_STRING_CUISINE, value.ToLower());
            }
            else if (MessageType.Ratings.ToString().Equals(messageType))
            {
                cardActionText = string.Format(FILTER_STRING_RATING, value.ToLower());
            }
            else {
                cardActionText = string.Format(FILTER_STRING_2, value.ToLower());
            }
            return cardActionText;
        }

        private static string getHyperLink(string text, string link)
        {
            return "<a href='" + link + "'>"+text+"</a>";
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

            string value = query.Replace("restaurants", "").Replace(MessageType.Cuisines.ToString(),"").Replace("rating", "").Replace(MessageType.Prices.ToString(), "").Replace("to","").Replace("Narrowing","");


            return value.TrimStart().TrimEnd();
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