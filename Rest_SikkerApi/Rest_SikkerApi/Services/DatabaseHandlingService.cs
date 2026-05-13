using Rest_SikkerApi.data;
using Rest_SikkerApi.models;
using Rest_SikkerApi.repos;

namespace Rest_SikkerApi;

public class DatabaseHandlingService
{
	private readonly AppDbContext _context;

	public DatabaseHandlingService(AppDbContext context)
	{
		_context = context;
	}

	public async Task<Image> SaveImageAsync(Image imageEntity)
	{
		imageEntity.ImageData = "image";
		_context.Images.Add(imageEntity);
		await _context.SaveChangesAsync();
		return imageEntity;
	}
}
