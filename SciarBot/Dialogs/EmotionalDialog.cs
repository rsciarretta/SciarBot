using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SciarBot.Dialogs
{
    [LuisModel("dd731aec-e570-48cd-99a0-1d27f01f8766", "d82b0af2e59f4f49a63ad52b282aaf60")]
    [Serializable]
    public class EmotionalDialog : LuisDialog<object>
    {
       // public const string Entity_location = "Location";

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("greet")]
        public async Task Greet_Start(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Ciao caro!!!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("greet.start")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Hey ciao!!! Come posso aiutarti?");
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
            await context.PostAsync($"Tu sei Leggenda! ...ora trovo il modo per capire il tuo nome.");
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
    }   
}