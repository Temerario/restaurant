using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Models
{
    public class ConversationEntity : TableEntity
    {
        public ConversationEntity(String id, String serviceUrl, String conversationId, String channelId, string rootObject)
        {
            this.PartitionKey = id;
            this.RowKey = conversationId;
            this.ServiceUrl = serviceUrl;
            this.ChannelId = channelId;
            this.ConversationData = rootObject;
            this.Timestamp = DateTime.Now;
        }

        public ConversationEntity() { }

        public DateTime LastupdatedTime = DateTime.Now;

        public string ConversationData { get; set; }
        public string ServiceUrl { get; set; }
        public string ChannelId { get; set; }
    }
}
