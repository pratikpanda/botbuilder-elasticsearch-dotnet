using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Builder.Elasticsearch.TranscriptStore.Manager
{
    public class RetrieveTranscriptResult
    {
        public List<Activity> Transcript { get; set; }
        public bool Retrieved { get; set; }
        public string Message { get; set; }
    }
}
