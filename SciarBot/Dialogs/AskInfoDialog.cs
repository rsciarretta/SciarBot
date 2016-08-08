using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SciarBot.Dialogs
{
    public class WeatherInfo
    {
        public WeatherCondition[] weather { get; set; }
        public Cloudiness clouds { get; set; }
        public Temperature main { get; set; }
    }

    public class Temperature
    {
        public float temp { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
    }

    public class WeatherCondition
    {
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Cloudiness
    {
        public int all { get; set; }
    }

    [LuisModel("5156e36a-157b-46a1-81c3-7d09bf6f4a7c", "5667affcf08744dc835d70c31ec6f2a9")]
    [Serializable]
    public class AskInfoDialog : LuisDialog<object>
    {
        [LuisIntent("askinfo.weather")]
        public async Task AskInfo_Weather(IDialogContext context, LuisResult result)
        {
            EntityRecommendation _city = new EntityRecommendation();
            if (result.TryFindEntity("City", out _city))
            {
                using (HttpClient http = new HttpClient())
                {
                    var weather = await http.GetAsync($"http://api.openweathermap.org/data/2.5/weather?q={_city.Entity}&appid=57ac6660e74e730300992117b1f698dd&lang=it&units=metric");
                    WeatherInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfo>(weather.Content.ReadAsStringAsync().Result);
                    var reply = context.MakeMessage();
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Title = $"{info.weather[0].description}",
                        Text = $"Temperatura: {info.main.temp}°C (min {info.main.temp_min} / max {info.main.temp_max}",
                        Subtitle = $"nuvole {info.clouds.all}%",
                        Images = new List<CardImage>() { new CardImage(url: $"http://openweathermap.org/img/w/{info.weather[0].icon}.png") }
                    };
                    reply.Attachments = new List<Attachment>();
                    reply.Attachments.Add(plCard.ToAttachment());
                    await context.PostAsync(reply);
                    context.Wait(MessageReceived);
                }
                return;
            }

            await context.PostAsync("askinfo.weather");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.person")]
        public async Task AskInfo_Person(IDialogContext context, LuisResult result)
        {
            EntityRecommendation _name = new EntityRecommendation();
            if (result.TryFindEntity("Name", out _name))
            {
                await context.PostAsync($"non conosco nessun {_name.Entity}. mi spiace!");
                context.Wait(MessageReceived);
                return;
            }
            await context.PostAsync("askinfo.person");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askinfo.aboutme")]
        public async Task AskInfo_AboutMe(IDialogContext context, LuisResult result)
        {
            string message = $"Io sono la versione avanzata dello SciarBot!";
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

    }
}