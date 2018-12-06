using Bot.Builder.Elasticsearch.TranscriptLogger;
using Elasticsearch.Net;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using Nest.JsonNetSerializer;
using System;
using System.Threading.Tasks;

namespace Elasticsearch.TranscriptLogger.Integration.Tests
{
    [TestClass]
    [TestCategory("Integration")]
    [TestCategory("Transcript Logger")]
    public class ElasticsearchTranscriptLoggerTests
    {
        private string elasticsearchEndpoint;
        private ITranscriptLogger transcriptLogger;

        public ElasticsearchTranscriptLoggerTests() { }

        [TestInitialize]
        public void Initialize()
        {
            // Get elasticsearch configuration from external file.
            var config = new ConfigurationBuilder()
                .AddJsonFile("elasticsearchsettings.json")
                .Build();

            var elasticsearchTranscriptLoggerOptions = new ElasticsearchTranscriptLoggerOptions
            {
                ElasticsearchEndpoint = new Uri(config["Endpoint"]),
                UserName = config["UserName"],
                Password = config["Password"],
                IndexName = config["IndexName"]
            };

            elasticsearchEndpoint = config["Endpoint"];

            transcriptLogger = new ElasticsearchTranscriptLogger(elasticsearchTranscriptLoggerOptions);
        }

        [TestCleanup]
        public async Task TestCleanUp()
        {
            if (transcriptLogger != null)
            {
                // Get elasticsearch configuration from external file.
                var config = new ConfigurationBuilder()
                    .AddJsonFile("elasticsearchsettings.json")
                    .Build();

                var elasticsearchTranscriptLoggerOptions = new ElasticsearchTranscriptLoggerOptions
                {
                    ElasticsearchEndpoint = new Uri(config["Endpoint"]),
                    UserName = config["UserName"],
                    Password = config["Password"],
                    IndexName = config["IndexName"]
                };

                var connectionPool = new SingleNodeConnectionPool(elasticsearchTranscriptLoggerOptions.ElasticsearchEndpoint);
                var connectionSettings = new ConnectionSettings(connectionPool, sourceSerializer: JsonNetSerializer.Default);

                if (!string.IsNullOrEmpty(elasticsearchTranscriptLoggerOptions.UserName) && !string.IsNullOrEmpty(elasticsearchTranscriptLoggerOptions.Password))
                {
                    connectionSettings = connectionSettings.BasicAuthentication(elasticsearchTranscriptLoggerOptions.UserName, elasticsearchTranscriptLoggerOptions.Password);
                }

                var client = new ElasticClient(connectionSettings);
                try
                {
                    await client.DeleteIndexAsync(Indices.Index(config["IndexName"] + "-" + DateTime.Now.ToString("MM-dd-yyyy")));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error cleaning up resources: {0}", ex.ToString());
                }
            }
        }

        [TestMethod]
        public void Constructor_Should_Throw_Exception_On_InvalidOptions()
        {
            // No Options. Should throw. 
            Assert.ThrowsException<ArgumentNullException>(() => new ElasticsearchTranscriptLogger(null));

            // No Endpoint. Should throw. 
            Assert.ThrowsException<ArgumentNullException>(() => new ElasticsearchTranscriptLogger(new ElasticsearchTranscriptLoggerOptions
            {
                UserName = "testUserName",
                Password = "testPassword",
                IndexName = "testIndexName"
            }));

            // No Index name. Should throw. 
            Assert.ThrowsException<ArgumentNullException>(() => new ElasticsearchTranscriptLogger(new ElasticsearchTranscriptLoggerOptions
            {
                ElasticsearchEndpoint = new Uri(elasticsearchEndpoint),
                UserName = "testUserName",
                Password = "testPassword"
            }));
        }

        [TestMethod]
        public async Task ElasticsearchTranscriptLogger_LogSingleActivityTest()
        {
            var activity = new Activity();
            activity.ChannelId = "TestChannelId";
            activity.Conversation = new ConversationAccount();
            activity.Conversation.Id = "TestConversationId";
            activity.Timestamp = DateTimeOffset.Now;

            // Log activity.
            await transcriptLogger.LogActivityAsync(activity);
        }

        [TestMethod]
        public async Task ElasticsearchTranscriptLogger_LogMultipleActivitiesTest()
        {
            var activity = new Activity();
            activity.ChannelId = "TestChannelId";
            activity.Conversation = new ConversationAccount();
            activity.Conversation.Id = "TestConversationId";
            activity.Timestamp = DateTimeOffset.Now;

            // Log first activity.
            await transcriptLogger.LogActivityAsync(activity);

            // Update timestamp.
            activity.Timestamp = DateTimeOffset.Now.AddMilliseconds(1);

            // Log second activity.
            await transcriptLogger.LogActivityAsync(activity);

            // Update timestamp.
            activity.Timestamp = DateTimeOffset.Now.AddMilliseconds(1);

            // Log third activity.
            await transcriptLogger.LogActivityAsync(activity);
        }
    }
}
