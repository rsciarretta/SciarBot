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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SciarBot.Dialogs
{
    [LuisModel("bcbf7623-f866-4d8d-957c-ca4cc3dcb3b8", "5667affcf08744dc835d70c31ec6f2a9")]
    [Serializable]
    public class ForwarderDialog : LuisDialog<object>
    {
        private const string wcmApiEndpointPattern = "http://if-demo-distribution.azurewebsites.net/v1/content/it-IT/bot_answers?tags.slug={0}";

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


        public class BotAnswer
        {
            public List<AnswerItem> items;
        }

        public class AnswerItem
        {
            public string title;
            public AnswerFields fields;
        }

        public class AnswerFields
        {
            public string Answer;
            public string Link;
            public bool IsQuestion;
        }


        public static string LastIntent;
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

        [LuisIntent("offend")]
        public async Task Offend(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Non è carino quello che mi dici :(");
            context.Wait(MessageReceived);
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

        public static async Task<BotAnswer> GetWcmBotAnswerByTag(string tag)
        {
            using (HttpClient http = new HttpClient())
            {
                var wcmApi = await http.GetAsync(string.Format(wcmApiEndpointPattern, tag));
                BotAnswer answer = Newtonsoft.Json.JsonConvert.DeserializeObject<BotAnswer>(wcmApi.Content.ReadAsStringAsync().Result);
                return answer;
            }
        }

        public static async Task<string> GetMessage(LuisResult result, string entityName)
        {
            var intent = result.Intents[0];
            var query = (LastIntent != null ? LastIntent : intent.Intent).Replace('.', '-');
            string defaultTag = string.Concat(query, ",default");
            BotAnswer answer;
            EntityRecommendation _entity = new EntityRecommendation();
            if (result.TryFindEntity(entityName, out _entity))
            {
                string tag = string.Concat(query, ",", _entity.Entity.Replace(' ', '-').ToLower());

                answer = await GetWcmBotAnswerByTag(tag);
                if (answer.items.Count.Equals(0))
                {
                    answer = await GetWcmBotAnswerByTag(defaultTag);
                }
                var item = answer.items.First();
                if (item.fields.IsQuestion)
                {
                    LastIntent = intent.Intent;
                }
                string message = item.fields.Answer;
                if (!string.IsNullOrEmpty(item.fields.Link))
                {
                    message = string.Concat(message, "\r\n", item.fields.Link);
                }
                return message;
            }
            answer = await GetWcmBotAnswerByTag(defaultTag);
            return answer.items.First().fields.Answer;
        }
    }
}