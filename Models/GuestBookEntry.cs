using Azure;
using Azure.Data.Tables;

namespace GuestBook.Web.Models
{
    public class GuestBookEntry : ITableEntity
    {
        public string PartitionKey { get; set; } = DateTime.UtcNow.ToString("yyyyMMdd");
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string GuestName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
    }
}