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

        public async Task<Image?> GetImageByIdAsync(string id)
        {
            return await _context.Images.FindAsync(id);
        }

        public virtual async Task<User?> GetUserByFirebaseIdAsync(string firebaseId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.FirebaseId == firebaseId);
        }

        public virtual async Task UpdateUserChatIdAsync(int userId, string chatId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.ChatId = chatId;
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<User?> GetUserByChatIdAsync(string chatId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
        }
    }
}

