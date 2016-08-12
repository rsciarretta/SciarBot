using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SciarBot.Dialogs
{

    [LuisModel("5156e36a-157b-46a1-81c3-7d09bf6f4a7c", "5667affcf08744dc835d70c31ec6f2a9")]
    [Serializable]
    public class AskInfoDialog : LuisDialog<object>
    {
        [LuisIntent("askinfo.weather")]
        public async Task AskInfo_Weather(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(await ForwarderDialog.GetMessage(result, "City"));
            context.Wait(MessageReceived);
            //var intent = result.Intents[0];
            //var query = intent.Intent.Replace('.', '-');
            //string defaultTag = string.Concat(query, "-default");
            //ForwarderDialog.BotAnswer answer;
            //EntityRecommendation _city = new EntityRecommendation();
            //if (result.TryFindEntity("City", out _city))
            //{
            //    string tag = string.Concat(query, "-", _city.Entity.Replace(' ', '-').ToLower());

            //    answer = await ForwarderDialog.GetWcmBotAnswerByTag(tag);
            //    if (answer.items.Count.Equals(0))
            //    {
            //        answer = await ForwarderDialog.GetWcmBotAnswerByTag(defaultTag);
            //    }
            //    await context.PostAsync(answer.items.First().fields.Answer);
            //    context.Wait(MessageReceived);
            //    return;
            //}
            //answer = await ForwarderDialog.GetWcmBotAnswerByTag(defaultTag);
            //await context.PostAsync(answer.items.First().fields.Answer);
            //context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.person")]
        public async Task AskInfo_Person(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(await ForwarderDialog.GetMessage(result, "Person"));
            context.Wait(MessageReceived);
            //var intent = result.Intents[0];
            //var query = intent.Intent.Replace('.', '-');
            //string defaultTag = string.Concat(query, "-default");
            //ForwarderDialog.BotAnswer answer;
            //EntityRecommendation _person = new EntityRecommendation();
            //if (result.TryFindEntity("Person", out _person))
            //{
            //    string tag = string.Concat(query, "-", _person.Entity.Replace(' ', '-').ToLower());
            //    answer = await ForwarderDialog.GetWcmBotAnswerByTag(tag);
            //    if (answer.items.Count.Equals(0))
            //    {
            //        answer = await ForwarderDialog.GetWcmBotAnswerByTag(defaultTag);
            //    }
            //    await context.PostAsync(answer.items.First().fields.Answer);
            //    if (!string.IsNullOrEmpty(answer.items.First().fields.Link))
            //    {
            //        await context.PostAsync(string.Concat("Guarda qui: ", answer.items.First().fields.Link));
            //    }
            //    context.Wait(MessageReceived);
            //    return;
            //}
            //answer = await ForwarderDialog.GetWcmBotAnswerByTag(defaultTag);
            //await context.PostAsync(answer.items.First().fields.Answer);
            //context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.aboutme")]
        public async Task AskInfo_AboutMe(IDialogContext context, LuisResult result)
        {
            string message = $"Io sono la versione più amorevole dello SciarBot!";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.documentation")]
        public async Task AskInfo_Documentation(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(await ForwarderDialog.GetMessage(result, "Wiki"));
            context.Wait(MessageReceived);

            //var intent = result.Intents[0];
            //var query = intent.Intent.Replace('.', '-');
            //string defaultTag = string.Concat(ForwarderDialog.LastIntent != null ? ForwarderDialog.LastIntent + "-" : "", query, "-default");
            //ForwarderDialog.BotAnswer answer;
            //EntityRecommendation _wiki = new EntityRecommendation();
            //if (result.TryFindEntity("Wiki", out _wiki))
            //{
            //    string tag = string.Concat(query, "-", _wiki.Entity.Replace(' ', '-').ToLower());

            //    answer = await ForwarderDialog.GetWcmBotAnswerByTag(tag);
            //    if (answer.items.Count.Equals(0))
            //    {
            //        answer = await ForwarderDialog.GetWcmBotAnswerByTag(defaultTag);
            //    }
            //    var item = answer.items.First();
            //    await context.PostAsync(String.Concat(item.fields.Answer, "\r\n", item.fields.Link));
            //    context.Wait(MessageReceived);
            //    if (item.fields.IsQuestion)
            //    {
            //        ForwarderDialog.LastIntent = intent.Intent;
            //    }
            //    return;
            //}
            //answer = await ForwarderDialog.GetWcmBotAnswerByTag(defaultTag);
            //await context.PostAsync(answer.items.First().fields.Answer);
            //context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            if (result.Intents[0].Intent.Equals("None"))
            {
                await context.Forward(new ForwarderDialog(), ResumeReceived, new Activity { Text = result.Query }, CancellationToken.None);
                return;
            }
            string message = $"Sorry. I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        public async Task ResumeReceived(IDialogContext context, IAwaitable<object> result)
        {
        }
    }
}