namespace Rest_SikkerApi;

using Rest_SikkerApi.models;

public class FileHandlingService
{
    private readonly string _imageFolderPath;

    public FileHandlingService(string imageFolderPath)
    {
        _imageFolderPath = imageFolderPath;
        Directory.CreateDirectory(_imageFolderPath);
    }

    public async Task<Image?> GetImageAsync(int id)
    {
        string path = Path.Combine(_imageFolderPath, id.ToString());
        if (!File.Exists(path)) return null;
        byte[] bytes = await File.ReadAllBytesAsync(path);
        return new Image { Id = id, ImageData = Convert.ToBase64String(bytes) };
    }

    public async Task UploadImageAsync(int id, string image)
    {
        byte[] bytes = Convert.FromBase64String(image);
        await File.WriteAllBytesAsync(Path.Combine(_imageFolderPath, id.ToString()), bytes);
    }
}
