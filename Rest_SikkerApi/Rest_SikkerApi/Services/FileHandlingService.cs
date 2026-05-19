namespace Rest_SikkerApi;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class FileHandlingService
{
	private readonly BlobServiceClient _blobServiceClient;
	private readonly BlobContainerClient _blobContainerClient;

	public FileHandlingService(BlobServiceClient blobServiceClient)
	{
		_blobServiceClient = blobServiceClient;
		_blobContainerClient = _blobServiceClient.GetBlobContainerClient("images");
	}

	public async Task<BlobContentInfo> UploadImageAsync(int id, string image)
	{
		byte[] imageByteData = Convert.FromBase64String(image);
		BinaryData imageData = new BinaryData(imageByteData);
		return await _blobContainerClient.UploadBlobAsync(id.ToString(), imageData);
	}

}
