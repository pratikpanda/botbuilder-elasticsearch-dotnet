﻿# Bot.Builder.Elasticsearch.TranscriptStore
Elasticsearch based transcript store extension for bots created using Microsoft Bot Framework.

## Usage
The extension uses NEST as the native client for connecting and working with Elasticsearch. Therefore the configuration options have been created following NEST standards and guidelines.

To instantiate the transcript store, please refer the code snippet provided below:

```csharp
// Instantiate the Elasticsearch transcript store.
var elasticsearchTranscriptStoreOptions = new ElasticsearchTranscriptStoreOptions();
elasticsearchTranscriptStoreOptions.ElasticsearchEndpoint = new Uri("http://localhost:9200");
elasticsearchTranscriptStoreOptions.UserName = "xxxxx";
elasticsearchTranscriptStoreOptions.Password = "yyyyy";
elasticsearchTranscriptStoreOptions.IndexName = "transcript-store-data";

ITranscriptStore transcriptStore = new ElasticsearchTranscriptStore(elasticsearchTranscriptStoreOptions);
```

To log an activity to the transcript store:

```csharp
// Log an activity to the transcript store.
await transcriptStore.LogActivityAsync(activity);
```

To retrieve transcripts from the transcript store:

```csharp
// Retrieve transcripts from the transcript store.
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
```

To list transcripts from the transcript store:

```csharp
// List transcripts from the transcript store.
var result = new List<TranscriptInfo>();
var pagedResult = new PagedResult<TranscriptInfo>();
do
{
    pagedResult = await transcriptStore.ListTranscriptsAsync(activity.ChannelId, pagedResult.ContinuationToken);

    foreach (var item in pagedResult.Items)
    {
        result.Add(item);
    }
}
while (pagedResult.ContinuationToken != null);
```

## Behaviour
The component automatically creates and maintains rolling indexes for storing data. This means if you are specifying the `IndexName` as transcript-store-data, on any given day the index would be named as transcript-store-data-current-date for e.g. transcript-store-data-09-27-2018. This would help to expire/delete old indexes by running a periodic job.

As soon as the component does not find the index specific to the current day, it creates a new rolling index and aliases it with `IndexName` for e.g. transcript-store-data. The component writes and maintains each activity log as a single document within the index. Thus, for a single conversation there can be multiple documents spanning through a single or multiple indexes. While writing the log it uses the index name.
