using Rest_SikkerApi.data;
using Microsoft.EntityFrameworkCore;
using Rest_SikkerApi.models;

namespace Rest_SikkerApi.repos
{
    public class SikkerRepo : ISikkerRepo
    {
        private readonly AppDbContext _context;
        // måske implementer en user, så they can't get others imges
        public SikkerRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Image> SaveImageAsync(Image imageEntity)
        {
            _context.Images.Add(imageEntity);
            await _context.SaveChangesAsync();
            return imageEntity;
        }

        public async Task<List<Image>> GetAllImagesAsync()
        {
            return await _context.Images.ToListAsync();
        }

        public IEnumerable<Image> GetAmountImage(int amount = 20)
        {
            return _context.Images.OrderBy(i => i.Id).Take(amount);
        }

        public IEnumerable<Image> GetAfterIDImage(int id, int amount = 20)
        {
            return _context.Images.Where(i => i.Id > id).OrderBy(i => i.Id).Take(amount);
        }
    
            // System state - stored in memory for now
        private static bool _systemOnline = false;

        public bool GetSystemState()
        {
            return _systemOnline;
        }

        public bool SetSystemState(bool state)
        {
            _systemOnline = state;
            return _systemOnline;
        }
    }
}

