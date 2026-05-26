namespace Rest_SikkerApi;

using Rest_SikkerApi.models;
using System.IO;

public class FileHandlingService
{
	private readonly string _imageFolderPath;

	public FileHandlingService()
	{
		_imageFolderPath = Path.Combine("/home/stefan/projects/SchoolShit/EksamenSys", "images");
		Directory.CreateDirectory(_imageFolderPath);
	}

	public async Task<Image?> GetImageAsync(int id)
	{
		string filePath = Path.Combine(_imageFolderPath, id.ToString());
		if (!File.Exists(filePath))
		{
			return null;
		}

		byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
		return new Image
		{
			Id = id,
			ImageData = Convert.ToBase64String(imageBytes),
			ImagePath = Path.Combine("images", id.ToString())
		};
	}

	public async Task UploadImageAsync(int id, string image)
	{
		byte[] imageByteData = Convert.FromBase64String(image);
		string filePath = Path.Combine(_imageFolderPath, id.ToString());
		await File.WriteAllBytesAsync(filePath, imageByteData);
	}
}
