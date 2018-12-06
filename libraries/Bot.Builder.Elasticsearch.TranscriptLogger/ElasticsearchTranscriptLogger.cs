using Elasticsearch.Net;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Builder.Elasticsearch.TranscriptLogger
{
    public class ElasticsearchTranscriptLogger : ITranscriptLogger
    {
        // Constants
        public const string RollingIndexDateFormat = "MM-dd-yyyy";

        private static readonly JsonSerializer jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        });

        // Prevent issues in case multiple requests arrive to create index concurrently.
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1);

        // Name of the index.
        private readonly string indexName;

        // Options for the elasticsearch transcript logger component.
        private readonly ElasticsearchTranscriptLoggerOptions elasticsearchTranscriptLoggerOptions;

        private ElasticClient elasticClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticsearchTranscriptLogger"/> class.
        /// </summary>
        /// <param name="elasticsearchTranscriptLoggerOptions"><see cref="ElasticsearchTranscriptLoggerOptions"/>.</param>
        public ElasticsearchTranscriptLogger(ElasticsearchTranscriptLoggerOptions elasticsearchTranscriptLoggerOptions)
        {
            if (elasticsearchTranscriptLoggerOptions == null)
            {
                throw new ArgumentNullException(nameof(elasticsearchTranscriptLoggerOptions), "Elasticsearch transcript logger options is required.");
            }

            if (elasticsearchTranscriptLoggerOptions.ElasticsearchEndpoint == null)
            {
                throw new ArgumentNullException(nameof(elasticsearchTranscriptLoggerOptions.ElasticsearchEndpoint), "Service endpoint for Elasticsearch is required.");
            }

            this.indexName = elasticsearchTranscriptLoggerOptions.IndexName ?? throw new ArgumentNullException("Index name for Elasticsearch is required.", nameof(elasticsearchTranscriptLoggerOptions.IndexName));

            this.elasticsearchTranscriptLoggerOptions = elasticsearchTranscriptLoggerOptions;

            InitializeSingleNodeConnectionPoolClient();
        }

        public async Task LogActivityAsync(IActivity activity)
        {
            if (activity == null)
            {
                return;
            }

            // Ensure Initialization has been run
            await InitializeAsync().ConfigureAwait(false);

            var documentItem = new DocumentItem
            {
                ChannelId = activity.ChannelId,
                ConversationId = activity.Conversation.Id,
                Timestamp = activity.Timestamp,
                Activity = activity
            };

            var indexResponse = await elasticClient.IndexAsync(documentItem, i => i
                .Index(indexName + "-" + DateTime.Now.ToString(RollingIndexDateFormat)).Refresh(Refresh.True)).ConfigureAwait(false);
        }

        /// <summary>
        /// Initializes the Elasticsearch single node connection pool client.
        /// </summary>
        private void InitializeSingleNodeConnectionPoolClient()
        {
            var connectionPool = new SingleNodeConnectionPool(elasticsearchTranscriptLoggerOptions.ElasticsearchEndpoint);
            CreateClient(connectionPool);
        }

        /// <summary>
        /// Creates the Elasticsearch client.
        /// </summary>
        /// <param name="connectionPool">Elasticsearch connection pool.</param>
        private void CreateClient(SingleNodeConnectionPool connectionPool)
        {
            // Instantiate connection settings from the connection pool.
            // Set JsonNetSerializer as the source serializer to use Newtonsoft.JSON serialization.
            var connectionSettings = new ConnectionSettings(connectionPool, sourceSerializer: JsonNetSerializer.Default);

            if (!string.IsNullOrEmpty(elasticsearchTranscriptLoggerOptions.UserName) && !string.IsNullOrEmpty(elasticsearchTranscriptLoggerOptions.Password))
            {
                connectionSettings = connectionSettings.BasicAuthentication(elasticsearchTranscriptLoggerOptions.UserName, elasticsearchTranscriptLoggerOptions.Password);
            }

            elasticClient = new ElasticClient(connectionSettings);
        }

        private async Task InitializeAsync()
        {
            // Check whether the index exists or not.
            var indexExistsResponse = await elasticClient.IndexExistsAsync(indexName + "-" + DateTime.Now.ToString(RollingIndexDateFormat));

            if (!indexExistsResponse.Exists)
            {
                // We don't (probably) have created the index yet. Enter the lock,
                // then check again (aka: Double-Check Lock pattern).
                await semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (!indexExistsResponse.Exists)
                    {
                        // If the index does not exist, create a new one with the current date and alias it.
                        var createIndexResponse = await elasticClient.CreateIndexAsync(indexName + "-" + DateTime.Now.ToString(RollingIndexDateFormat), c => c
                        .Mappings(ms => ms.Map<DocumentItem>(m => m.AutoMap()))).ConfigureAwait(false);

                        await elasticClient.AliasAsync(ac => ac.Add(a => a.Index(indexName + "-" + DateTime.Now.ToString(RollingIndexDateFormat)).Alias(indexName))).ConfigureAwait(false);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }
    }
}
