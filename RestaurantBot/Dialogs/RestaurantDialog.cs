using Chronic.Handlers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using RestaurantBot.Constants;
using RestaurantBot.Models;
using RestaurantBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RestaurantBot.Dialogs
{
    public class RestaurantDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            RootObject rootObject = BingServiceUtils.getRootObject("https://www.bingapis.com/api/v7/Places/search?q=thai+restaurant+hyderabad");
            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");
            await CreateRestaurantDailog(context, rootObject);
            await CreateFiltersDailog(context, rootObject);
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

            CardAction menuCard = DailogUtils.createCardAction(value.menuUrl, AppInfo.OPEN_URL, AppInfo.MENU, null);
            listCardActions.Add(menuCard);

            CardAction reviewsCard = DailogUtils.createCardAction(value.menuUrl, AppInfo.OPEN_URL, AppInfo.REVIEWS, null); //TODO
            listCardActions.Add(reviewsCard);

            CardAction timingsCard = DailogUtils.createCardAction(value.menuUrl, AppInfo.OPEN_URL, AppInfo.MENU, null); //TODO
            listCardActions.Add(timingsCard);

            CardAction bookTableCard = DailogUtils.createCardAction(value.reservationUrl, AppInfo.OPEN_URL, AppInfo.BOOK_TABLE, null);
            listCardActions.Add(bookTableCard);

            CardAction locationCard = DailogUtils.createCardAction(DailogUtils.getGoogleMapsLink(value), AppInfo.OPEN_URL, AppInfo.LOCATION, null);
            listCardActions.Add(locationCard);

            return listCardActions;
        }

        private async Task CreateFiltersDailog(IDialogContext context, RootObject rootObject)
        {
            var replyToConversation = context.MakeMessage();
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            foreach (Filter filter in rootObject.filters)
            {
                var heroCard = new HeroCard()
                {
                    Title = filter.name,
                    Buttons = DailogUtils.createFilterCardAction(filter)
                };

                replyToConversation.Attachments.Add(heroCard.ToAttachment());
            }
            await context.PostAsync(replyToConversation);
        }
    }
}