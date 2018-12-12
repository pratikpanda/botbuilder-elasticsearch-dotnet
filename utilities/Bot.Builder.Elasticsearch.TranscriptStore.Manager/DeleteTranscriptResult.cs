using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Builder.Elasticsearch.TranscriptStore.Manager
{
    public class DeleteTranscriptResult
    {
        public bool Deleted { get; set; }
        public string Message { get; set; }
    }
}
