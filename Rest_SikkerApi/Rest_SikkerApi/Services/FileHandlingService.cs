namespace Rest_SikkerApi;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Rest_SikkerApi.models;
using Xunit.Sdk;

public class FileHandlingService
{
	private readonly BlobServiceClient _blobServiceClient;
	private readonly BlobContainerClient _blobContainerClient;

	public FileHandlingService(BlobServiceClient blobServiceClient)
	{
		_blobServiceClient = blobServiceClient;
		_blobContainerClient = _blobServiceClient.GetBlobContainerClient("images");
	}

	public async Task<Image?> GetImageAsync(int id)
	{
		BlobClient blobClient = _blobContainerClient.GetBlobClient(id.ToString());
            if (await blobClient.ExistsAsync())
            {                
                Azure.Response<BlobDownloadResult> download = await blobClient.DownloadContentAsync();
                var image = new Image
                {
                    Id = id,
                    ImageData = Convert.ToBase64String(download.Value.Content.ToArray())
                };
                return image;
            }
            return null;
	}

	public async Task<BlobContentInfo> UploadImageAsync(int id, string image)
	{
		byte[] imageByteData = Convert.FromBase64String(image);
		BinaryData imageData = new BinaryData(imageByteData);
		return await _blobContainerClient.UploadBlobAsync(id.ToString(), imageData);
	}

}
