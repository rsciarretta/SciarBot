using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Bot.Connector;
using System.Threading;

namespace SciarBot.Dialogs
{
    [LuisModel("f66c63d6-b30c-4d1b-ae07-ddd5240c9de5", "5667affcf08744dc835d70c31ec6f2a9")]
    [Serializable]
    public class SetInfoDialog : LuisDialog<object>
    {

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
                ForwarderDialog.user.name = _name.Entity;
                ForwarderDialog.user.activationCode = null;
                ForwarderDialog.user.email = null;
                ForwarderDialog.user.isActivated = false;
                await context.PostAsync($"Ciao {_name.Entity}. Potresti darmi il tuo indirizzo email così che possa registrati nel mio sistema?");
                context.Wait(MessageReceived);
                return;
            }
            EntityRecommendation _email = new EntityRecommendation();
            if (result.TryFindEntity("Email", out _email))
            {
                ForwarderDialog.user.email = _email.Entity.Replace(" ", string.Empty);
                if (!ForwarderDialog.user.email.EndsWith("@deltatre.com"))
                {
                    await context.PostAsync("L'indirizzo email da te specificato non è valido.");
                    context.Wait(MessageReceived);
                    return;
                }
                if (ForwarderDialog.user.getData())
                {
                    await context.PostAsync($"Hey ciao {ForwarderDialog.user.name}. Quasi mi dimenticavo di te :) Cosa posso fare per te?");
                    context.Wait(MessageReceived);
                    return;
                }
                await sendActivationCode(context);
                return;
            }
            EntityRecommendation _activationCode = new EntityRecommendation();
            if (result.TryFindEntity("ActivationCode", out _activationCode))
            {
                if (_activationCode.Entity.Replace(" - ", "-").Equals(ForwarderDialog.user.activationCode))
                {
                    ForwarderDialog.user.isActivated = true;
                    ForwarderDialog.user.saveData();
                    await context.PostAsync($"Complimenti, ti sei appena attivato! Grazie mille.");
                    context.Wait(MessageReceived);
                    return;
                }
                else
                {
                    await context.PostAsync($"Il codice di attivazione che mi hai inviato non è corretto :(");
                    context.Wait(MessageReceived);
                    return;
                }
            }
            await context.PostAsync("setinfo.user.X");
            context.Wait(MessageReceived);
        }

        [LuisIntent("setinfo.company.name")]
        public async Task SetInfo_Company(IDialogContext context, LuisResult result)
        {
            string _lastIntent = ForwarderDialog.LastIntent;
            if (!string.IsNullOrEmpty(_lastIntent))
            {
                await context.PostAsync(await ForwarderDialog.GetMessage(result, "Company"));
                context.Wait(MessageReceived);
                if (ForwarderDialog.LastIntent.Equals(_lastIntent))
                {
                    ForwarderDialog.LastIntent = null;
                }
                return;
            }
            await context.PostAsync("Non mi è ben chiaro il tuo messaggio. Potresti essere più specifico?");
            context.Wait(MessageReceived);
        }

        private async Task sendActivationCode(IDialogContext context)
        {
            if (ForwarderDialog.user == null)
            {
                await context.PostAsync($"Vorrei rimandarti il codice, ma non riesco ad identificarti. Potresti dirmi come ti chiami?");
                context.Wait(MessageReceived);
                return;
            }
            string activationCode = Guid.NewGuid().ToString();
            ForwarderDialog.user.activationCode = activationCode;
            ForwarderDialog.user.saveData();

            sendMail(activationCode);
            await context.PostAsync($"Ho appena inviato una email all'indirizzo da te specificato {ForwarderDialog.user.email} con un codice di attivazione.\r\n Potrebbero volerci dei minuti, scrivimelo qui in chat così che possa attivarti.");
            context.Wait(MessageReceived);
        }

        private async Task sendMail(string activationCode)
        {
            dynamic sg = new SendGridAPIClient("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");
            Email from = new Email("internationalfootball.online@deltatre.com");
            string subject = "IFBot - Attiva il tuo account";
            Email to = new Email(ForwarderDialog.user.email);
            Content content = new Content("text/plain", $"Ciao {ForwarderDialog.user.name}\r\n Il tuo codice di attivazione è:\r\n {activationCode}");
            Mail mail = new Mail(from, subject, to, content);
            dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());
        }

        public async Task ResumeReceived(IDialogContext context, IAwaitable<object> result)
        {
        }
    }
}