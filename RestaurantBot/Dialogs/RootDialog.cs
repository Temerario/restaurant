using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using RestaurantBot.Models;
using RestaurantBot.Constants;
using RestaurantBot.Utils;
using System.Collections.Generic;
using Microsoft.Practices.Unity;


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
            Dictionary<string, string> bingUrlHash = BingServiceUtils.getRootObject(context, activity, activity.Text);
            string queryUrl = DailogUtils.getQueryUrl(bingUrlHash, activity.Text);
            RootObject latestRootObject = BingServiceUtils.getRootObject(queryUrl);

            if (DailogUtils.handleRestaurantEvents(activity.Text)) // handling reviews and timings
            {
                await handleRestaurantEvents(context, latestRootObject, activity.Text);
            }
            else if(latestRootObject.totalEstimatedMatches == 0) // handling strings for which bing gave zero results
            {
                await context.PostAsync("Sorry, we didn't find any restaurants matching your criteria. Try searching for 'cheap eats in location', 'best pizza in < location >, etc.");
            }
            else { 
                await CreateRestaurantDailog(context, latestRootObject);
                await CreateFiltersDailog(context, latestRootObject);
                try
                {
                    var azureStorageProvider = InjectionContainer.Instance.Container.Resolve<AzureStorageProvider>();
                    await azureStorageProvider.InsertConversation(new ConversationEntity(activity.From.Id, activity.ServiceUrl, activity.Conversation.Id, activity.ChannelId, BingServiceUtils.Serialize(latestRootObject)));
                }catch(Exception ex)
                {
                    await context.PostAsync(ex.Message.ToString());
                }
            }
            context.Wait(MessageReceivedAsync);
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

        private async Task CreateRestaurantDailog(IDialogContext context, RootObject rootObject)
        {
            var replyToConversation = context.MakeMessage();
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            foreach (Value value in rootObject.value)
            {
                List<CardAction> cardActions = createRestaurantCardActions(value);
                List<CardImage> cardImages = DailogUtils.createRestaurantImageActions(value);


                var heroCard = new HeroCard()
                {
                    Title = value.name,
                    Buttons = cardActions,
                    Images = cardImages
                };

                replyToConversation.Attachments.Add(heroCard.ToAttachment());
            }
            await context.PostAsync(replyToConversation);
        }

        private List<CardAction> createRestaurantCardActions(Value value)
        {
            List<CardAction> listCardActions = new List<CardAction>();

            CardAction menuCard = DailogUtils.createCardAction(value.menuUrl, AppInfo.OPEN_URL, AppInfo.MENU);
            if(menuCard !=null ) listCardActions.Add(menuCard);

            CardAction reviewsCard = DailogUtils.createCardAction(DailogUtils.getReviewsImBackValue(value), AppInfo.IMBACK, AppInfo.REVIEWS); //TODO
            if (reviewsCard != null) listCardActions.Add(reviewsCard);

            CardAction timingsCard = DailogUtils.createCardAction(DailogUtils.getTimingsImBackValue(value), AppInfo.IMBACK, AppInfo.TIMINGS); //TODO
            if (timingsCard != null) listCardActions.Add(timingsCard);

            CardAction bookTableCard = DailogUtils.createCardAction(value.reservationUrl, AppInfo.OPEN_URL, AppInfo.BOOK_TABLE);
            if (bookTableCard != null) listCardActions.Add(bookTableCard);

            CardAction locationCard = DailogUtils.createCardAction(DailogUtils.getGoogleMapsLink(value), AppInfo.OPEN_URL, AppInfo.LOCATION);
            if (locationCard != null) listCardActions.Add(locationCard);

            return listCardActions;
        }

        private async Task CreateFiltersDailog(IDialogContext context, RootObject rootObject)
        {
            var replyToConversation = context.MakeMessage();
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            foreach (Filter filter in rootObject.filters)
            {
                string title = filter.name;
                if (filter.isSelected)title = title + "*"; 

                var heroCard = new HeroCard()
                {
                    Title = title,
                    Buttons = DailogUtils.createFilterCardAction(filter)
                };

                replyToConversation.Attachments.Add(heroCard.ToAttachment());
            }
            await context.PostAsync(replyToConversation);
        }
    }
}