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
        }

        [LuisIntent("askinfo.person")]
        public async Task AskInfo_Person(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(await ForwarderDialog.GetMessage(result, "Person"));
            context.Wait(MessageReceived);
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