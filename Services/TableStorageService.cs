using Azure.Data.Tables; 
using GuestBook.Web.Models;

namespace GuestBook.Web.Services
{
    public class TableStorageService
    {
        private readonly TableClient _tableClient;

        public TableStorageService(IConfiguration config)
        {
            var connStr = config["AzureStorage:ConnectionString"];
            var serviceClient = new TableServiceClient(connStr);
            serviceClient.CreateTableIfNotExists("GuestBookEntries");
            _tableClient = serviceClient.GetTableClient("GuestBookEntries");
        }

        public async Task AddEntryAsync(GuestBookEntry entry)
        {
            await _tableClient.AddEntityAsync(entry);
        }

        public IEnumerable<GuestBookEntry> GetTodayEntries()
        {
            string pk = DateTime.UtcNow.ToString("yyyyMMdd");
            return _tableClient.Query<GuestBookEntry>
                (e => e.PartitionKey == pk);
        }

        public async Task DeleteEntryAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public IEnumerable<GuestBookEntry> GetAllEntries()
        {
            return _tableClient.Query<GuestBookEntry>();
        }
    }
}