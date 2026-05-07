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

        public async Task<User> SaveUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<Image>> GetAllImagesAsync()
        {
            return await _context.Images.ToListAsync();
        }

        public async Task<Image?> GetImageByIdAsync(int id)
        {
            return await _context.Images.FindAsync(id);
        }

        public async Task<User?> GetUserByFirebaseIdAsync(string ownerUid)
        {
            return await _context.Users.FindAsync(ownerUid);
        }

        public async Task<bool> UpdateUserChatIdAsync(string ownerUid, string telegramChatId, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(new object?[] { ownerUid }, ct);
            if (user == null)
            {
               return false;

                // _context.Users.Add(new User { OwnerUid = ownerUid, TelegramChatId = telegramChatId });
            }
            user.TelegramChatId = telegramChatId;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<User?> GetUserByChatIdAsync(string telegramChatId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.TelegramChatId == telegramChatId);
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
        public async Task<List<Image>> GetImagesByOwnerUidAsync(string ownerUid)
        {
            return await _context.Images
                .Where(i => i.OwnerUid == ownerUid)
                .OrderByDescending(i => i.Id)
                .ToListAsync();
        }
    }
}

