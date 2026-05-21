using System.Data.Entity;
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

	public async Task SaveImageAsync(Image imageEntity)
	{
		imageEntity.ImagePath = "images";
		_context.Images.Add(imageEntity);
		await _context.SaveChangesAsync();
	}

	public async Task<bool> CheckIdUidMatch(int id, string uid)
	{
		Image? image = await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
		if (image == null || image.OwnerUid != uid)
		{
			return false;
		}
		return true;
	}

	public IEnumerable<Image> GetBeforeIDImage(string uid, int id, int amount = 20)
	{
		return _context.Images.Where(i => i.Id < id && i.OwnerUid == uid).OrderByDescending(i => i.Id).Take(amount);
	}
}
