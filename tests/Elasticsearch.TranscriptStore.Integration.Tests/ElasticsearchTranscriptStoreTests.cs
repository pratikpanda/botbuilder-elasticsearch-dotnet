using Bot.Builder.Elasticsearch.TranscriptStore;
using Elasticsearch.Net;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using Nest.JsonNetSerializer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elasticsearch.TranscriptStore.Integration.Tests
{
    [TestClass]
    [TestCategory("Integration")]
    [TestCategory("Transcript Store")]
    public class ElasticsearchTranscriptStoreTests
    {
        private string elasticsearchEndpoint;
        private ITranscriptStore transcriptStore;

        public ElasticsearchTranscriptStoreTests() { }

        [TestInitialize]
        public void Initialize()
        {
            // Get elasticsearch configuration from external file.
            var config = new ConfigurationBuilder()
                .AddJsonFile("elasticsearchsettings.json")
                .Build();

            var elasticsearchTranscriptStoreOptions = new ElasticsearchTranscriptStoreOptions
            {
                ElasticsearchEndpoint = new Uri(config["Endpoint"]),
                UserName = config["UserName"],
                Password = config["Password"],
                IndexName = config["IndexName"]
            };

            elasticsearchEndpoint = config["Endpoint"];

            transcriptStore = new ElasticsearchTranscriptStore(elasticsearchTranscriptStoreOptions);
        }

        [TestCleanup]
        public async Task TestCleanUp()
        {
            if (transcriptStore != null)
            {
                // Get elasticsearch configuration from external file.
                var config = new ConfigurationBuilder()
                    .AddJsonFile("elasticsearchsettings.json")
                    .Build();

                var elasticsearchTranscriptStoreOptions = new ElasticsearchTranscriptStoreOptions
                {
                    ElasticsearchEndpoint = new Uri(config["Endpoint"]),
                    UserName = config["UserName"],
                    Password = config["Password"],
                    IndexName = config["IndexName"]
                };

                var connectionPool = new SingleNodeConnectionPool(elasticsearchTranscriptStoreOptions.ElasticsearchEndpoint);
                var connectionSettings = new ConnectionSettings(connectionPool, sourceSerializer: JsonNetSerializer.Default);

                if (!string.IsNullOrEmpty(elasticsearchTranscriptStoreOptions.UserName) && !string.IsNullOrEmpty(elasticsearchTranscriptStoreOptions.Password))
                {
                    connectionSettings = connectionSettings.BasicAuthentication(elasticsearchTranscriptStoreOptions.UserName, elasticsearchTranscriptStoreOptions.Password);
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
            Assert.ThrowsException<ArgumentNullException>(() => new ElasticsearchTranscriptStore(null));

            // No Endpoint. Should throw. 
            Assert.ThrowsException<ArgumentNullException>(() => new ElasticsearchTranscriptStore(new ElasticsearchTranscriptStoreOptions
            {
                UserName = "testUserName",
                Password = "testPassword",
                IndexName = "testIndexName"
            }));

            // No Index name. Should throw. 
            Assert.ThrowsException<ArgumentNullException>(() => new ElasticsearchTranscriptStore(new ElasticsearchTranscriptStoreOptions
            {
                ElasticsearchEndpoint = new Uri(elasticsearchEndpoint),
                UserName = "testUserName",
                Password = "testPassword"
            }));
        }

        [TestMethod]
        public async Task ElasticsearchTranscriptStore_LogSingleActivityTest()
        {
            var activity = new Activity();
            activity.ChannelId = "TestChannelId";
            activity.Conversation = new ConversationAccount();
            activity.Conversation.Id = "TestConversationId";
            activity.Timestamp = DateTimeOffset.Now;

            // Log activity.
            await transcriptStore.LogActivityAsync(activity);
        }

        [TestMethod]
        public async Task ElasticsearchTranscriptStore_LogMultipleActivitiesTest()
        {
            var activity = new Activity();
            activity.ChannelId = "TestChannelId";
            activity.Conversation = new ConversationAccount();
            activity.Conversation.Id = "TestConversationId";
            activity.Timestamp = DateTimeOffset.Now;

            for (int i = 0; i < 50; i++)
            {
                // Log activity.
                await transcriptStore.LogActivityAsync(activity);

                // Update timestamp.
                activity.Timestamp = DateTimeOffset.Now.AddSeconds(1);
            }
        }

        [TestMethod]
        public async Task ElasticsearchTranscriptStore_RetrieveTranscriptsTest()
        {
            // Arrange
            var activity = new Activity();
            activity.ChannelId = "TestChannelId";
            activity.Conversation = new ConversationAccount();
            activity.Conversation.Id = "TestConversationId";
            activity.Timestamp = DateTimeOffset.Now;

            for (int i = 0; i < 50; i++)
            {
                // Log activity.
                await transcriptStore.LogActivityAsync(activity);

                // Update timestamp.
                activity.Timestamp = DateTimeOffset.Now.AddSeconds(1);
            }

            // Act
            var result = new List<IActivity>();
            var pagedResult = new PagedResult<IActivity>();
            do
            {
                pagedResult = await transcriptStore.GetTranscriptActivitiesAsync(activity.ChannelId, activity.Conversation.Id, pagedResult.ContinuationToken);

                foreach (var item in pagedResult.Items)
                {
                    result.Add(item);
                }

                Assert.AreNotEqual(0, pagedResult.Items.Length);
            }
            while (pagedResult.ContinuationToken != null);

            // Assert
            Assert.AreNotEqual(0, result.Count);
        }

        [TestMethod]
        public async Task ElasticsearchTranscriptStore_ListTranscriptsTest()
        {
            // Arrange
            var activity = new Activity();
            activity.ChannelId = "TestChannelId";
            activity.Conversation = new ConversationAccount();
            activity.Conversation.Id = "TestConversationId";
            activity.Timestamp = DateTimeOffset.Now;

            for (int i = 0; i < 50; i++)
            {
                // Log activity.
                await transcriptStore.LogActivityAsync(activity);

                // Update timestamp.
                activity.Timestamp = DateTimeOffset.Now.AddSeconds(1);
            }

            // Act
            var result = new List<TranscriptInfo>();
            var pagedResult = new PagedResult<TranscriptInfo>();
            do
            {
                pagedResult = await transcriptStore.ListTranscriptsAsync(activity.ChannelId, pagedResult.ContinuationToken);

                foreach (var item in pagedResult.Items)
                {
                    result.Add(item);
                }

                Assert.AreNotEqual(0, pagedResult.Items.Length);
            }
            while (pagedResult.ContinuationToken != null);

            // Assert
            Assert.AreNotEqual(0, result.Count);
        }

        [TestMethod]
        public async Task ElasticsearchTranscriptStore_DeleteTranscriptsTest()
        {
            // Arrange
            var activity = new Activity();
            activity.ChannelId = "TestChannelId";
            activity.Conversation = new ConversationAccount();
            activity.Conversation.Id = "TestConversationId";
            activity.Timestamp = DateTimeOffset.Now;

            for (int i = 0; i < 50; i++)
            {
                // Log activity.
                await transcriptStore.LogActivityAsync(activity);

                // Update timestamp.
                activity.Timestamp = DateTimeOffset.Now.AddSeconds(1);
            }

            // Act
            await transcriptStore.DeleteTranscriptAsync(activity.ChannelId, activity.Conversation.Id);

            var result = new List<IActivity>();
            var pagedResult = new PagedResult<IActivity>();
            do
            {
                pagedResult = await transcriptStore.GetTranscriptActivitiesAsync(activity.ChannelId, activity.Conversation.Id, pagedResult.ContinuationToken);

                foreach (var item in pagedResult.Items)
                {
                    result.Add(item);
                }
            }
            while (pagedResult.ContinuationToken != null);

            // Assert
            Assert.AreEqual(0, result.Count);
        }
    }
}
