using System;

namespace Bot.Builder.Elasticsearch.TranscriptStore.Manager
{
    public class GenerateTranscriptFileResult
    {
        public bool Generated { get; set; }
        public string Message { get; set; }
    }
}
