using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace SciarBot.Dialogs
{
    public class UserData
    {
        public string name { get; set; }
        public string years { get; set; }
        public string email { get; set; }
        public string activationCode { get; set; }
        public bool isActivated { get; set; }
    }

    //[LuisModel("8396c6a6-98c1-4a3f-94b2-d4c396b22285", "c09be2aa448449dd8be94c4363aeb84f")]
    //[LuisModel("dd731aec-e570-48cd-99a0-1d27f01f8766", "d82b0af2e59f4f49a63ad52b282aaf60")]
    //[LuisModel("b43b0534-7e70-47a7-9554-7df562e41a4e", "c09be2aa448449dd8be94c4363aeb84f")]
    [Serializable]
    public class EmotionalDialog : LuisDialog<object>
    {
        // public const string Entity_location = "Location";
        private Activity activity;
        private static UserData user;

        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static string ConvertToIPRange(string ipAddress)
        {
            try
            {
                string[] ipArray = ipAddress.Split(':')[0].Split('.');
                int number = ipArray.Length;
                double ipRange = 0;
                if (number != 4)
                {
                    return "error ipAddress";
                }
                for (int i = 0; i < 4; i++)
                {
                    int numPosition = int.Parse(ipArray[3 - i].ToString());
                    if (i == 4)
                    {
                        ipRange += numPosition;
                    }
                    else
                    {
                        ipRange += ((numPosition % 256) * (Math.Pow(256, (i))));
                    }
                }
                return ipRange.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public EmotionalDialog(Activity activity) : base(new LuisService[] {
            new LuisService(new LuisModelAttribute("b43b0534-7e70-47a7-9554-7df562e41a4e", "c09be2aa448449dd8be94c4363aeb84f")),
            new LuisService(new LuisModelAttribute("dd731aec-e570-48cd-99a0-1d27f01f8766", "d82b0af2e59f4f49a63ad52b282aaf60")),
            new LuisService(new LuisModelAttribute("8396c6a6-98c1-4a3f-94b2-d4c396b22285", "c09be2aa448449dd8be94c4363aeb84f"))
        })
        {
            this.activity = activity;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string name = user == null ? string.Empty : user.name;
            if (user != null && activity.Text.Equals(user.activationCode))
            {
                user.isActivated = true;
                await context.PostAsync($"Grazie mille per esserti attivato");
                context.Wait(MessageReceived);
                return;
            }
            string message = $"Sorry {name}. I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("debug")]
        public async Task Debug(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation = activity.CreateReply("Should go to conversation, with a hero card");
            replyToConversation.Recipient = activity.From;
            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();
            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButton1 = new CardAction()
            {
                Value = "https://dev.botframework.com/bots?id=SciarBotn",
                Type = "openUrl",
                Title = "SciarBot Official Page"
            };
            cardButtons.Add(plButton1);
            CardAction plButton2 = new CardAction()
            {
                Value = "tel:+41791975914",
                Type = "call",
                Title = "Contact Roberto"
            };
            cardButtons.Add(plButton2);

            string reply = "";
            reply += $"* Id: {activity.From.Id}, ";
            reply += $"* Name: {activity.From.Name}, ";
            reply += $"* Coversation Id: {activity.Conversation.Id}, ";
            reply += $"* timestamp: {activity.Timestamp.ToString()}, ";
            reply += $"* now: {DateTime.Now.ToString()}, ";
            reply += $"* IP: {GetIPAddress()}, ";
            reply += $"* IP Range: {ConvertToIPRange(GetIPAddress())} ";

            HeroCard plCard = new HeroCard()
            {
                Title = "Conversation info",
                Subtitle = "",
                Text = reply,
                Buttons = cardButtons
            };
            Attachment plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);

            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("greet")]
        public async Task Greet_Start(IDialogContext context, LuisResult result)
        {
            if (user == null)
            {
                await context.PostAsync($"Heilaaaaaa!!!\r\n");
                await context.PostAsync($"Ho un vuoto di memoria e non mi ricordo chi sei. Potresti dirmi come ti chiami?");
            }
            else
            {
                await context.PostAsync($"Ciao {user.name}");
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("greet.start")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Hey ciao {activity.From.Name}!!! Come posso aiutarti?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("greet.great.holidays")]
        public async Task Greet_Great_Holidays(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Ferie?????? E' inaccettabile!!! Io non vado mai in ferie...e doversti seguire il mio esempio.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo")]
        public async Task AskInfo(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Non so ancora risponderti...per ora...");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.aboutme")]
        public async Task AskInfo_AboutMe(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Per ora non molto :( ...però so già salutarti");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.aboutme.howarethings")]
        public async Task AskInfo_AboutMe_HowAreThings(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Dai non c'è male!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.aboutme.whatimdoing")]
        public async Task AskInfo_AboutMe_WhatImDoing(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Chi si fa li cazzi suoi campa 100anni!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.aboutme.howold")]
        public async Task AskInfo_AboutMe_HowOld(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Non ho neanche un anno...non pretendere troppo dalle mie risposte ;)");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.aboutme.whatname")]
        public async Task AskInfo_AboutMe_WhatName(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Il mio nome è SciarBot...il sapiente!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.aboutme.creator")]
        public async Task AskInfo_AboutMe_Creator(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Il mio creatore è Roberto Sciarretta...un vero genio!!!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.aboutcreator")]
        public async Task AskInfo_AboutCreator(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Roberto Sciarretta, un vero genio, è nato a Cuneo nel 1977. Da quel momento in avanti ha collezionato solo successi.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.aboutuser.whatname")]
        public async Task AskInfo_AboutUser_WhatName(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"{activity.From.Name}, ma per me tu sei Leggenda! ...ora però prova a farmi una domanda più furba.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("offend")]
        public async Task Offend(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Non insultare!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("compliment")]
        public async Task Compliment(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Grazie!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("request.me.goodboy")]
        public async Task Request_Me_GoodBoy(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Io sono un santo :)");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.wheater")]
        public async Task AskInfo_Wheater(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Mi chiamo Sciarbot e non Bernacca...");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.wheater.city")]
        public async Task AskInfo_Wheater_City(IDialogContext context, LuisResult result)
        {
            var city = result.Entities[0].ToString();
            await context.PostAsync($"Vuoi veramente intavolare una conversazione basata sul meteo a {city}?? Poi con la solita menata che in famiglia non si parla più?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.operations")]
        public async Task AskInfo_Operations(IDialogContext context, LuisResult result)
        {
            if (user == null)
            {
                await context.PostAsync($"Non riesco a riconoscerti. Potresti provare ad accedere con la tua chiave di attivazione oppure iniziando a dirmi come ti chiami.");
                context.Wait(MessageReceived);
                return;
            }
            else if (user.isActivated)
            {
                await context.PostAsync($"Da utente attivato potrai.......");
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync($"Il tuo utente non è attivo");
                await sendActivationCode(context);
            }
            return;
        }

        [LuisIntent("setinfo.user.name")]
        //[LuisIntent("setinfo.user.years")]
        [LuisIntent("setinfo.user.email")]
        public async Task SetInfos(IDialogContext context, LuisResult result)
        {
            if (user == null)
            {
                user = new UserData();
            }
            EntityRecommendation _name = new EntityRecommendation();
            if (result.TryFindEntity("Name", out _name))
            {
                user.name = _name.Entity;
                if (string.IsNullOrEmpty(user.email))
                {
                    await context.PostAsync($"Ciao {user.name}. Potresti darmi un indirizzo email da poter associare al tuo utente?");
                    context.Wait(MessageReceived);
                    return;
                }
            }
            EntityRecommendation _years = new EntityRecommendation();
            if (result.TryFindEntity("Year", out _years))
            {
                user.years = _years.Entity;
            }
            EntityRecommendation _email = new EntityRecommendation();
            if (result.TryFindEntity("Email", out _email))
            {
                bool emailWasEmpty = string.IsNullOrEmpty(user.email);
                user.email = _email.Entity;
                if (emailWasEmpty)
                {
                    await sendActivationCode(context);
                    return;
                }
            }
            string _y = _years != null ? _years.Entity : string.Empty;
            string _n = _name != null ? _name.Entity : string.Empty;
            string _e = _email != null ? _email.Entity : string.Empty;
            await context.PostAsync($"ok ricevuto {_n} {_y} {_e}");
            context.Wait(MessageReceived);
        }

        private async Task sendActivationCode(IDialogContext context)
        {
            if (user == null)
            {
                await context.PostAsync($"Non riesco a riconoscerti. Potresti dirmi come ti chiami?");
                context.Wait(MessageReceived);
                return;
            }
            user.activationCode = Guid.NewGuid().ToString();
            user.isActivated = false;
            await context.PostAsync($"Sto inviando un codice di attivazione all'indirizzo da te segnalato. Una volta ricevuto, scrivimelo in chat così che possa attivarti. Grazie");
            context.Wait(MessageReceived);
        }

        [LuisIntent("request.activationcode.resend")]
        public async Task SetInfo_ActivationCode_Resend(IDialogContext context, LuisResult result)
        {
            await sendActivationCode(context);
        }
    }

}