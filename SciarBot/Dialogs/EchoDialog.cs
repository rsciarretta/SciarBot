using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace SciarBot.Dialogs
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.None);
            }
            else
            {
                string reply = "";
                if (message.Text.ToLower() == "ping")
                    reply = $"Pong";

                if (message.Text.ToLower().StartsWith("ciao"))
                    reply = $"Ciao **{message.From.Name}**!";

                if (message.Text.ToLower() == "debug")
                {
                    reply += $"Id: {message.From.Id}\n";
                    reply += $"Name: {message.From.Name}\n";
                    reply += $"Coversation Id: {message.Conversation.Id}\n";
                }

                if (reply == "")
                {
                    var connectionString = "Endpoint=sb://chatbot.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=FmoM2di+b7oFix1xzWA7tWgIZQuowplGjm3W47cNbuk=";
                    var queueName = "messages";

                    var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
                    var messageBus = new BrokeredMessage(message.Text);
                    client.Send(messageBus);

                    reply = "...ci devo pensare su...";
                }

                await context.PostAsync(string.Format("{0}: {1}", this.count++, reply));
                context.Wait(MessageReceivedAsync);
            }
        }


        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                //context.Reset();
                await context.PostAsync("done");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}