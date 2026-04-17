using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GuestBook.Web.Models;
using GuestBook.Web.Services;

namespace GuestBook.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TableStorageService _table;
        private readonly BlobStorageService _blob;
        private readonly QueueStorageService _queue;

        [BindProperty]
        public string GuestName { get; set; } = string.Empty;

        [BindProperty]
        public string Message { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? Photo { get; set; }

        public IEnumerable<GuestBookEntry> Entries { get; set; }
            = new List<GuestBookEntry>();

        public IndexModel(TableStorageService table,
            BlobStorageService blob, QueueStorageService queue)
        {
            _table = table; 
            _blob = blob; 
            _queue = queue;
        }

        public void OnGet()
        {
            Entries = _table.GetTodayEntries();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string photoUrl = "/images/default.png";

            if (Photo != null && Photo.Length > 0)
            {
                photoUrl = await _blob.UploadPhotoAsync(Photo);
            }

            var entry = new GuestBookEntry
            {
                GuestName = GuestName,
                Message = Message,
                PhotoUrl = photoUrl,
                ThumbnailUrl = photoUrl
            };

            await _table.AddEntryAsync(entry);

            await _queue.SendMessageAsync(
                $"{entry.PartitionKey}|{entry.RowKey}");

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(
            string partitionKey, string rowKey)
        {
            await _table.DeleteEntryAsync(partitionKey, rowKey);
            return RedirectToPage();
        }
    }
}