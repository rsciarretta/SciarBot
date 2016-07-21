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
    public class TravelGuidDialog : LuisDialog<object>
    {
       // public const string Entity_location = "Location";

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greet")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            EntityRecommendation subject;

            await context.PostAsync($"Ciao caro!!!");
            context.Wait(MessageReceived);
            }
        }

       
    }