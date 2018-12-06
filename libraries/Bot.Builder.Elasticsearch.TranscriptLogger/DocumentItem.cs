using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Builder.Elasticsearch.TranscriptLogger
{
    public class DocumentItem
    {
        /// <summary>
        /// Gets or sets the channel id.
        /// </summary>
        /// <value>
        /// The channel id.
        /// </value>
        public string ChannelId { get; set; }

        /// <summary>
        /// Gets or sets the conversation id.
        /// </summary>
        /// <value>
        /// The conversation id.
        /// </value>
        public string ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the persisted activity.
        /// </summary>
        /// <value>
        /// The persisted activity.
        /// </value>
        public IActivity Activity { get; set; }

        /// <summary>
        /// Gets or sets the current timestamp.
        /// </summary>
        /// <value>
        /// The current timestamp.
        /// </value>
        public DateTimeOffset? Timestamp { get; set; }
    }
}
