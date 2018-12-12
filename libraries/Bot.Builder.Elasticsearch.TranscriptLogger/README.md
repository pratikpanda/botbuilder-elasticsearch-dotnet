# Bot.Builder.Elasticsearch.TranscriptLogger
Elasticsearch based transcript logger extension for bots created using Microsoft Bot Framework.

## Usage
The extension uses NEST as the native client for connecting and working with Elasticsearch. Therefore the configuration options have been created following NEST standards and guidelines.

To instantiate the transcript logger, please refer to the code snippet below:

```csharp
var elasticsearchTranscriptLoggerOptions = new ElasticsearchTranscriptLoggerOptions();
elasticsearchTranscriptLoggerOptions.ElasticsearchEndpoint = new Uri("http://localhost:9200");
elasticsearchTranscriptLoggerOptions.UserName = "xxxxx";
elasticsearchTranscriptLoggerOptions.Password = "yyyyy";
elasticsearchTranscriptLoggerOptions.IndexName = "transcript-log-data";

ITranscriptLogger transcriptLogger = new ElasticsearchTranscriptLogger(elasticsearchTranscriptLoggerOptions);
```
To log an activity using the transcript logger:

```csharp
// Log an activity using the transcript logger.
await transcriptLogger.LogActivityAsync(activity);
```


## Behaviour
The component automatically creates and maintains rolling indexes for storing data. This means if you are specifying the `IndexName` as transcript-log-data, on any given day the index would be named as transcript-log-data-current-date for e.g. transcript-log-data-09-27-2018. This would help to expire/delete old indexes by running a periodic job.

As soon as the component does not find the index specific to the current day, it creates a new rolling index and aliases it with `IndexName` for e.g. transcript-log-data. The component writes and maintains each activity log as a single document within the index. Thus, for a single conversation there can be multiple documents spanning through a single or multiple indexes. While writing the log it uses the index name.
