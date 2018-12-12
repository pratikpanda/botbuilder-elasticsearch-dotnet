using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.Elasticsearch.TranscriptStore.Manager
{
    public class ElasticsearchTranscriptStoreManager
    {
        private ElasticsearchTranscriptStore transcriptStore;
        private string outputDirectory;

        public ElasticsearchTranscriptStoreManager()
        {
            // Get elasticsearch configuration from external file.
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var elasticsearchTranscriptStoreOptions = new ElasticsearchTranscriptStoreOptions
            {
                ElasticsearchEndpoint = new Uri(config["ElasticsearchEndpoint"]),
                UserName = config["ElasticsearchUserName"],
                Password = config["ElasticsearchPassword"],
                IndexName = config["ElasticsearchIndexName"]
            };

            this.transcriptStore = new ElasticsearchTranscriptStore(elasticsearchTranscriptStoreOptions);
            this.outputDirectory = config["OutputDirectory"];
        }
        public async Task<RetrieveTranscriptResult> RetrieveTranscriptAsync(string channelId, string conversationId)
        {
            var retrieveTranscriptResult = new RetrieveTranscriptResult();

            retrieveTranscriptResult.Transcript = new List<Activity>();

            var pagedResult = new PagedResult<IActivity>();
            do
            {
                pagedResult = await transcriptStore.GetTranscriptActivitiesAsync(channelId, conversationId, pagedResult.ContinuationToken);

                foreach (var item in pagedResult.Items)
                {
                    retrieveTranscriptResult.Transcript.Add(item as Activity);
                }
            }
            while (pagedResult.ContinuationToken != null);

            if (retrieveTranscriptResult.Transcript.Count == 0)
            {
                retrieveTranscriptResult.Message = "Could not find any activities in the store for the corresponding channel id and conversation id.";
                retrieveTranscriptResult.Retrieved = false;
                return retrieveTranscriptResult;
            }
            else
            {
                retrieveTranscriptResult.Message = "Successfully retrieved the transcript from store.";
                retrieveTranscriptResult.Retrieved = true;
                return retrieveTranscriptResult;
            }
        }

        public async Task<GenerateTranscriptFileResult> GenerateTranscriptFileAsync(IList<Activity> transcript)
        {
            if (transcript.Count == 0)
            {
                throw new InvalidOperationException("Could not find any activity to generate the transcript file.");
            }

            string channelId = transcript[0].ChannelId;
            string conversationId = transcript[0].Conversation.Id;

            var generateTranscriptFileResult = new GenerateTranscriptFileResult();
            var filePath = outputDirectory + channelId + "\\";
            if (!Directory.Exists(filePath))
            {
                var directoryInfo = Directory.CreateDirectory(filePath);
            }

            var fileName = conversationId + ".transcript";
            fileName = fileName.Replace("|", "-");
            var fileWriter = File.CreateText(filePath + fileName);
            await fileWriter.WriteAsync(JsonConvert.SerializeObject(transcript, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            fileWriter.Close();

            generateTranscriptFileResult.Message = $"Successfully generated the transcript file {fileName} at {filePath}.";
            generateTranscriptFileResult.Generated = true;

            return generateTranscriptFileResult;
        }

        public async Task<DeleteTranscriptResult> DeleteTranscriptAsync(string channelId, string conversationId)
        {
            var deleteTranscriptResult = new DeleteTranscriptResult();

            await transcriptStore.DeleteTranscriptAsync(channelId, conversationId);
            deleteTranscriptResult.Deleted = true;
            deleteTranscriptResult.Message = "Successfully deleted the transcript file from store.";

            return deleteTranscriptResult;
        }
    }
}
