using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.ServiceBus.Messaging;

namespace SciarBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                Message reply = message.CreateReplyMessage();

                if (message.Text.ToLower() == "ping")
                    reply.Text = $"Pong";

                if (message.Text.ToLower().StartsWith("ciao"))
                    reply.Text = $"Ciao **{message.From.Name}**!!!";

                if (message.Text.ToLower() == "debug")
                {
                    reply.Text = $"Channel Id: {message.From.ChannelId}";
                    reply.Text += $"Address: {message.From.Address}\n";
                    reply.Text += $"Id: {message.From.Id}\n";
                    reply.Text += $"Name: {message.From.Name}\n";
                    reply.Text += $"Channel Coversation Id: {message.ChannelConversationId}\n";
                    reply.Text += $"Conversation Id: {message.ConversationId}\n";
                }


                if (reply.Text != "")
                    return reply;
                else
                {
                    var connectionString = "Endpoint=sb://chatbot.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=FmoM2di+b7oFix1xzWA7tWgIZQuowplGjm3W47cNbuk=";
                    var queueName = "messages";

                    var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
                    var msg = new BrokeredMessage($"{message.Text}");
                    client.Send(msg);

                    return null;
                }
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}