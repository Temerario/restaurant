using Microsoft.Bot.Connector;
using RestaurantBot.Constants;
using RestaurantBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace RestaurantBot.Utils
{
    public class DailogUtils
    {
        private static string FILTER_STRING_1 = "Narrowing restaurants to {0} {1}";
        private static string FILTER_STRING_2 = "Narrowing to {0} restaurants";

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

                CardAction menuCard = createCardAction(getCardActionValueFormat(filter.name, filterValueName), AppInfo.IMBACK, filterValueName, filterValue.url, true, filterValue.isSelected);
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
        public static CardAction createCardAction(string value, string action, string title, string bingUrl, bool context = false, bool isSelected = false)
        {
            if (value == null || action == null || title == null || value == "") return null;
            if (isSelected) title = title + "*";
            if (context) value += AddContextToValue(value, bingUrl);
            CardAction cardButton = new CardAction()
            {
                Value = value,
                Type = action,
                Title = title
            };
            return cardButton;
        }

        public static string getQueryUrl(string query)
        {
            string queryUrl = FetchQueryUrl(query);
            string queryValue = getQueryValue(query);
            if(queryUrl == null)
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

        public static string FetchQueryUrl(string originalMessage)
        {
            string context = null;
            // Parse the message content xml and extract the context.
            var content = XElement.Parse($"<root>{originalMessage.Replace("&", "&amp;").Replace("'", "&apos;")}</root>");

            var contextNode = content.Element("context");
            if (contextNode != null)
            {
                var feedbackIdAttribute = contextNode.Attribute("feedbackId");
                if (feedbackIdAttribute != null)
                {
                    // Get context.
                    context = GetContext(feedbackIdAttribute.Value);
                }
            }
            return context;
        }
        public static string FetchQueryText(string originalMessage)
        {
            var content = XElement.Parse($"<root>{originalMessage.Replace("&", "&amp;").Replace("'", "&apos;")}</root>");
            // Extract the textual portion of the message content, excluding the <context /> XML element.
            StringBuilder messageBuilder = new StringBuilder();
            foreach (XNode node in content.Nodes())
            {
                if (node is XText)
                {
                    messageBuilder.Append(node.ToString());
                }
            }
            return messageBuilder.Replace("&amp;", "&").Replace("&apos;", "'").ToString();
        }

        private static readonly string ContextFormat = "{0}";

        private static string GetContext(string feedbackId)
        {
            return String.Format(ContextFormat, feedbackId);
        }


        private static string AddContextToValue(string value, string bingUrl)
        {
            return String.Format("<context feedbackId=\"{0}\"></context>", bingUrl);
        }


        private static string getCardActionValueFormat(string messageType, string value)
        {
            string cardActionText = "";
            if (MessageType.Prices.ToString().Equals(messageType)) {
                cardActionText = string.Format(FILTER_STRING_2, value);
            }
            else {
                cardActionText = string.Format(FILTER_STRING_1, value, messageType.ToString());
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
            string value = "";
            if (handleFilterActions(query))
            {
                value = query.Replace("Narrowing restaurants to ", "").Replace(MessageType.Cuisines.ToString(),"").Replace(MessageType.Ratings.ToString(), "");
            }
            else
            {
                //"Narrowing to {0} restaurants"
                value = query.Replace("Narrowing to ", "").Replace(" restaurants", "");
            }
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