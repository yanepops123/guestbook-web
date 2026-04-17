using Azure.Storage.Queues;

namespace GuestBook.Web.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queue;

        public QueueStorageService(IConfiguration config)
        {
            var connStr = config["AzureStorage:ConnectionString"];
            _queue = new QueueClient(connStr, "guestthumbs");
            _queue.CreateIfNotExists();
        }

        public async Task SendMessageAsync(string message)
        {
            await _queue.SendMessageAsync(message);
        }
    }
}