using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;

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

                //Tracker tracker = new Tracker("UA-80613133-1", "sciarbot.azurewebsites.net");
                //tracker.TrackPageView(HttpContext, "My API - Create", "api/create");

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
                    return null;
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