using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
 
namespace GuestBook.Web.Services
{
   public class BlobStorageService
   {
       private readonly BlobContainerClient _container;
 
       public BlobStorageService(IConfiguration config)
       {
           var connStr = config["AzureStorage:ConnectionString"];
           var blobService = new BlobServiceClient(connStr);
           _container = blobService.GetBlobContainerClient("guestbookpics");
           _container.CreateIfNotExists(PublicAccessType.None);
       }
 
       public async Task<string> UploadPhotoAsync(IFormFile file)
       {
           string blobName = $"photo_{Guid.NewGuid()}.jpg";
           var blob = _container.GetBlobClient(blobName);
           using var stream = file.OpenReadStream();
           await blob.UploadAsync(stream, overwrite: true);
           return blob.Uri.ToString();
       }
 
       public List<string> GetAllBlobUrls()
       {
           var urls = new List<string>();
           foreach (var blob in _container.GetBlobs())
           {
               var blobClient = _container.GetBlobClient(blob.Name);
               urls.Add(blobClient.Uri.ToString());
           }
           return urls;
       }
 
       public async Task DeleteBlobAsync(string blobUrl)
       {
           var uri = new Uri(blobUrl);
           var blobName = Path.GetFileName(uri.LocalPath);
           var blobClient = _container.GetBlobClient(blobName);
           await blobClient.DeleteIfExistsAsync();
       }
   }
}