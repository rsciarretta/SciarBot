using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SciarBot.Dialogs
{
    [LuisModel("bcbf7623-f866-4d8d-957c-ca4cc3dcb3b8", "5667affcf08744dc835d70c31ec6f2a9")]
    [Serializable]
    public class ForwarderDialog : LuisDialog<object>
    {
        public class UserData
        {
            public string name { get; set; }
            public string email { get; set; }
            public string age { get; set; }
        }

        public static UserData user = null;

        public ForwarderDialog()
        {
        }

        [LuisIntent("askinfo")]
        public async Task AskInfo(IDialogContext context, LuisResult result)
        {
            await context.Forward(new AskInfoDialog(), ResumeReceived, new Activity { Text = result.Query }, CancellationToken.None);
        }

        [LuisIntent("setinfo")]
        public async Task SetInfo(IDialogContext context, LuisResult result)
        {
            await context.Forward(new SetInfoDialog(), ResumeReceived, new Activity { Text = result.Query }, CancellationToken.None);
        }

        [LuisIntent("greet")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            string message = "Heilaaaaaa!!!\r\n";
            if (user == null)
            {
                message += "Ho un vuoto di memoria. Potresti ricordarmi come ti chiami?";
            }
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry. I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        public async Task ResumeReceived(IDialogContext context, IAwaitable<object> result)
        {
            object o = result.GetAwaiter();
        }
    }
}