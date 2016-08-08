using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Threading.Tasks;

namespace SciarBot.Dialogs
{
    [LuisModel("f66c63d6-b30c-4d1b-ae07-ddd5240c9de5", "5667affcf08744dc835d70c31ec6f2a9")]
    [Serializable]
    public class SetInfoDialog : LuisDialog<object>
    {
        public SetInfoDialog() { }

        [LuisIntent("setinfo.user.name")]
        [LuisIntent("setinfo.user.email")]
        [LuisIntent("setinfo.user.age")]
        [LuisIntent("setinfo.user.activationcode")]
        public async Task SetInfo_User(IDialogContext context, LuisResult result)
        {
            if (ForwarderDialog.user == null)
            {
                ForwarderDialog.user = new ForwarderDialog.UserData();

            }
            EntityRecommendation _name = new EntityRecommendation();
            if (result.TryFindEntity("Name", out _name))
            {
                await context.PostAsync($"Ciao {_name.Entity}");
                context.Wait(MessageReceived);
                return;
            }
            await context.PostAsync("setinfo.user.X");
            context.Wait(MessageReceived);
        }



    }
}