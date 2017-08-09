using Microsoft.Azure.Documents;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using RestaurantBot.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RestaurantBot
{
    public class AzureStorageProvider
    {
        private CloudTableClient _tableClient;
        private CloudTable conversationsTable;

        public AzureStorageProvider(CloudStorageAccount storageAccount)
        {
            _tableClient = storageAccount.CreateCloudTableClient();
            conversationsTable = _tableClient.GetTableReference("RestaurantBotConversations");
            conversationsTable.CreateIfNotExists();
        }

        public async Task InsertConversation(ConversationEntity conversationEntity)
        {
            TableOperation upsertOperation = TableOperation.InsertOrReplace(conversationEntity);
            await conversationsTable.ExecuteAsync(upsertOperation);
        }

        public IEnumerable<ConversationEntity> GetConversationsData(string conversationId, string channelId)
        {
            TableQuery<ConversationEntity> filterQuery = new TableQuery<ConversationEntity>().Where(GetConversationFilters(conversationId, channelId));
            return conversationsTable.ExecuteQuery(filterQuery);
        }
        private string GetConversationFilters(string conversationId, string channelId)
        {
            var filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, conversationId);
            var filter1 = TableQuery.GenerateFilterCondition("ChannelId", QueryComparisons.Equal, channelId);
            return TableQuery.CombineFilters(filter, TableOperators.And, filter1);
        }
    }
}
