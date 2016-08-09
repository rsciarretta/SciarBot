using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            public string activationCode { get; set; }
            public bool isActivated { get; set; }

            private IMongoDatabase _db = null;
            private void connect()
            {
                if (_db == null)
                {
                    var _client = new MongoClient(ConfigurationManager.AppSettings["Mongo:ConnectionString"]);
                    _db = _client.GetDatabase("bots");
                }
            }

            public void saveData()
            {
                connect();
                var _collection = _db.GetCollection<BsonDocument>("Users");
                var _filter = Builders<BsonDocument>.Filter;
                _collection.UpdateOneAsync(_filter.Eq("email", email), getUpdateDefinition(), new UpdateOptions { IsUpsert = true });
            }


            public bool getData()
            {
                connect();
                var _collection = _db.GetCollection<BsonDocument>("Users");
                var _filter = Builders<BsonDocument>.Filter;
                BsonDocument doc = _collection.Find(_filter.Eq("email", email)).FirstOrDefault();
                if (doc == null)
                {
                    return false;
                }
                name = doc.GetElement("name").Value.ToString();
                email = doc.GetElement("email").Value.ToString();
                age = doc.GetElement("age").Value.ToString();
                activationCode = doc.GetElement("activationcode").Value.ToString();
                isActivated = Convert.ToBoolean(doc.GetElement("isactivated").Value.ToString());
                return true;
            }

            private UpdateDefinition<BsonDocument> getUpdateDefinition()
            {
                var _update = Builders<BsonDocument>.Update;
                return _update.Set("name", name).Set("email", email).Set("age", age).Set("activationcode", activationCode).Set("isactivated", isActivated);
            }
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
            else
            {
                if (!string.IsNullOrEmpty(user.email))
                {
                    if (!string.IsNullOrEmpty(user.activationCode) && !user.isActivated)
                    {
                        message += "Il tuo account non è attivo.";
                    }
                }
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
            //object o = result.GetAwaiter();
        }
    }
}