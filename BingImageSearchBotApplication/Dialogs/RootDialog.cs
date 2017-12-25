using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using RestSharp;
using System.Collections.Generic;

namespace BingImageSearchBotApplication.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private string key = "your key";

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var client = new RestClient("https://api.cognitive.microsoft.com/bing/v7.0/");
            var request = new RestRequest("images/search", Method.GET);
            request.AddHeader("Ocp-Apim-Subscription-Key", key);
            request.AddParameter("q", activity.Text);

            var response = await client.ExecuteTaskAsync<RootObject>(request);
            var replyMessage = context.MakeMessage();
            List<Attachment> list = new List<Attachment>();

            foreach (var item in response.Data.value)
            {
                Attachment attachment = new Attachment();
                attachment = GetInternetAttachment(item.name, item.encodingFormat, item.contentUrl);
                list.Add(attachment);
            }
            replyMessage.Attachments = list;
            await context.PostAsync(replyMessage);
            context.Wait(MessageReceivedAsync);
        }

        private static Attachment GetInternetAttachment(string name, string contentType, string url)
        {
            return new Attachment
            {
                Name = name,
                ContentType = "image/"+contentType,
                ContentUrl = url
            };
        }
    }


    public class Instrumentation
    {
    }

    public class Thumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class InsightsMetadata
    {
        public int pagesIncludingCount { get; set; }
        public int availableSizesCount { get; set; }
    }

    public class Value
    {
        public string webSearchUrl { get; set; }
        public string name { get; set; }
        public string thumbnailUrl { get; set; }
        public DateTime datePublished { get; set; }
        public string contentUrl { get; set; }
        public string hostPageUrl { get; set; }
        public string contentSize { get; set; }
        public string encodingFormat { get; set; }
        public string hostPageDisplayUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Thumbnail thumbnail { get; set; }
        public string imageInsightsToken { get; set; }
        public InsightsMetadata insightsMetadata { get; set; }
        public string imageId { get; set; }
        public string accentColor { get; set; }
    }

    public class RootObject
    {
        public string _type { get; set; }
        public Instrumentation instrumentation { get; set; }
        public string webSearchUrl { get; set; }
        public int totalEstimatedMatches { get; set; }
        public int nextOffset { get; set; }
        public List<Value> value { get; set; }
    }
}