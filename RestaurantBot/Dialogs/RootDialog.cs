using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using RestaurantBot.Models;
using RestaurantBot.Constants;
using RestaurantBot.Utils;
using System.Collections.Generic;

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
            RootObject rootObject = BingServiceUtils.getRootObject(context, activity.Text);
            string queryUrl = DailogUtils.getQueryUrl(rootObject, activity.Text);
            RootObject latestRootObject = BingServiceUtils.getRootObject(queryUrl);
            if(latestRootObject == null)
            {
                await context.PostAsync("Please enter your city name");
            }
            else { 
                await CreateRestaurantDailog(context, latestRootObject);
                await CreateFiltersDailog(context, latestRootObject);
                context.UserData.SetValue("rootObject", latestRootObject);
                try
                {

                    RootObject k = context.UserData.GetValue<RootObject>("rootObject");
                }
                catch (KeyNotFoundException KeyNotFoundException)
                {
                    var a = "1";

                }
            }
            context.Wait(MessageReceivedAsync);


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

            CardAction reviewsCard = DailogUtils.createCardAction(value.menuUrl, AppInfo.OPEN_URL, AppInfo.REVIEWS); //TODO
            if (reviewsCard != null) listCardActions.Add(reviewsCard);

            CardAction timingsCard = DailogUtils.createCardAction(value.menuUrl, AppInfo.OPEN_URL, AppInfo.TIMINGS); //TODO
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
                if (filter.isSelected)title =  "*" + title; 

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