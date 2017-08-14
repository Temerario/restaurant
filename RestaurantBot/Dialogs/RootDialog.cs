using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using RestaurantBot.Models;
using RestaurantBot.Constants;
using RestaurantBot.Utils;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Collections;

namespace RestaurantBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            //Dictionary<string, string> bingUrlHash = BingServiceUtils.getRootObject(context, activity, activity.Text);
            //DailogUtils.FetchQueryUrl(activity.Text);

            string currentBingUrl = DailogUtils.getQueryUrl(activity.Text);
            RootObject latestRootObject = BingServiceUtils.getRootObject(currentBingUrl);
            string queryText = DailogUtils.FetchQueryText(activity.Text);
            if (DailogUtils.handleWelcomeMessage(activity.Text))
            {
                await context.PostAsync(AppInfo.WELCOME_MESSAGE.ToString());
            }
            else if (latestRootObject.totalEstimatedMatches == 0 || latestRootObject.searchAction == null || latestRootObject.searchAction.location == null) // handling strings for which bing gave zero results
            {
                await context.PostAsync("Sorry, we didn't find any restaurants matching your criteria. Try searching for \"cheap eats in Chandni Chowk\", \"best pizza in Mumbai\", etc.");
            }
            else if (DailogUtils.handleRestaurantEvents(queryText)) // handling reviews and timings
            {
                await handleRestaurantEvents(context, latestRootObject, queryText);
                await showDefaultDialogs(context, latestRootObject, currentBingUrl);
            }
            else {
                await showDefaultDialogs(context, latestRootObject, currentBingUrl);
            }
            context.Wait(MessageReceivedAsync);
        }

        private async Task showDefaultDialogs(IDialogContext context, RootObject rootObject, string currentBingUrl)
        {
            await CreateSummaryDailog(context, rootObject, currentBingUrl);
            await CreateRestaurantDailog(context, rootObject, currentBingUrl);
            await CreateFiltersDailog(context, rootObject, currentBingUrl);
        }

        private async Task handleRestaurantEvents(IDialogContext context, RootObject latestRootObject, string inputText)
        {
            MessageType messageType = DailogUtils.parseQuery(inputText);
            if(messageType == MessageType.Timings)
            {
                string text = DailogUtils.getReplyTimingText(latestRootObject, inputText);
                await context.PostAsync(text);
            }
            else if(messageType ==MessageType.Reviews)
            {
                List<string> reviews = DailogUtils.getReplyReviewsText(latestRootObject, inputText);
                foreach(string review in reviews)
                {
                    await context.PostAsync(review);
                }
            }
           
        }

        private async Task CreateSummaryDailog(IDialogContext context, RootObject rootObject, string currentBingUrl)
        {
            if (rootObject.filters == null || rootObject.filters.Count == 0) return;
            string replyText = "", cuisine = "", prices = "", rating = "";
            string location = " "+rootObject.searchAction.location.Last<Location>().name;
            foreach(Filter filter in rootObject.filters)
            {
                if (filter.isSelected)
                {
                    foreach(FilterValue filterValue in filter.filterValues)
                    {
                        if (filterValue.isSelected)
                        {
                            if(filter.name.Equals(MessageType.Cuisines.ToString())) cuisine = " " + filterValue.name;
                            else if(filter.name.Equals(MessageType.Ratings.ToString())) rating = " with " + filterValue.name + " rating";
                            else if (filter.name.Equals(MessageType.Prices.ToString()))
                            {
                                if (filterValue.scalarValue != null && filterValue.scalarValue.minLevel != null) prices = " " +filterValue.scalarValue.minLevel;
                                else prices = " " +filterValue.name;
                            }
                        }
                    }
                }

            }
            
            replyText = "Showing"+ prices.ToLower() + cuisine.ToLower() + " restaurants in" + location + rating.ToLower();
            await context.PostAsync(replyText);

        }

        private async Task CreateRestaurantDailog(IDialogContext context, RootObject rootObject, string currentBingUrl)
        {
            var replyToConversation = context.MakeMessage();
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            foreach (Value value in rootObject.value)
            {
                List<CardAction> cardActions = createRestaurantCardActions(value, currentBingUrl);
                List<CardImage> cardImages = DailogUtils.createRestaurantImageActions(value);


                var heroCard = new HeroCard()
                {
                    Title = FormatRestaurantName(value.name),
                    Subtitle = DailogUtils.restaurantCardSubtitle(value),
                    //Text = DailogUtils.restaurantCardText(value),
                    Buttons = cardActions,
                    Images = cardImages,
                    Tap = DailogUtils.createCardAction(value.url, AppInfo.OPEN_URL, "tap", null)

            };

                replyToConversation.Attachments.Add(heroCard.ToAttachment());
            }
            await context.PostAsync(replyToConversation);
        }

        private static string FormatRestaurantName(string text)
        {
            return text.Replace("&", "n");
        }
        private List<CardAction> createRestaurantCardActions(Value value, string currentBingUrl)
        {
            List<CardAction> listCardActions = new List<CardAction>();

            CardAction menuCard = DailogUtils.createCardAction(value.menuUrl, AppInfo.OPEN_URL, AppInfo.MENU, currentBingUrl, false);
            if(menuCard !=null ) listCardActions.Add(menuCard);

            CardAction reviewsCard = DailogUtils.createCardAction(DailogUtils.getReviewsImBackValue(value), AppInfo.IMBACK, AppInfo.REVIEWS, currentBingUrl, true); //TODO
            if (reviewsCard != null) listCardActions.Add(reviewsCard);

            CardAction timingsCard = DailogUtils.createCardAction(DailogUtils.getTimingsImBackValue(value), AppInfo.IMBACK, AppInfo.TIMINGS, currentBingUrl, true); //TODO
            if (timingsCard != null) listCardActions.Add(timingsCard);

           // CardAction bookTableCard = DailogUtils.createCardAction(value.reservationUrl, AppInfo.OPEN_URL, AppInfo.BOOK_TABLE);
           // if (bookTableCard != null) listCardActions.Add(bookTableCard);

            CardAction locationCard = DailogUtils.createCardAction(DailogUtils.getGoogleMapsLink(value), AppInfo.OPEN_URL, AppInfo.LOCATION, currentBingUrl, false);
            if (locationCard != null) listCardActions.Add(locationCard);

            return listCardActions;
        }

        private async Task CreateFiltersDailog(IDialogContext context, RootObject rootObject, string currentBingUrl)
        {
            if (rootObject.filters == null || rootObject.filters.Count == 0) return;
            var replyToConversation = context.MakeMessage();
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            List<Filter> reversedFilter = rootObject.filters;
            reversedFilter.Reverse();

            foreach (Filter filter in reversedFilter)
            {
                string title = filter.name;
                string[] supportedFilters = { MessageType.Ratings.ToString(), MessageType.Prices.ToString(), MessageType.Cuisines.ToString() };
                if (!supportedFilters.Contains(title)) continue;

                if (filter.isSelected)title = title + "*";
                List<CardAction> cardActions = DailogUtils.createFilterCardAction(filter);
                for(int i = 0; i< cardActions.Count; i += 5)
                {
                    List<CardAction> buttons = new List<CardAction>();
                    for(int j =i; j<=i + 5 && j< cardActions.Count; j++)
                    {
                        buttons.Add(cardActions[j]);
                    }
                    var heroCard = new HeroCard()
                    {
                        Title = title,
                        Buttons = buttons
                    };

                    replyToConversation.Attachments.Add(heroCard.ToAttachment());
                }
                
            }
            await context.PostAsync(replyToConversation);
        }
    }
}