using Rest_SikkerApi.models;

namespace Rest_SikkerApi.repos
{
    public interface ISikkerRepo
    {
        Task<Image> SaveImageAsync(Image imageEntity);
        Task<User> SaveUserAsync(User user);
        Task<List<Image>> GetAllImagesAsync();
        Task<Image?> GetImageByIdAsync(int id);
        Task<User?> GetUserByFirebaseIdAsync(string ownerUid);
        Task<bool> UpdateUserChatIdAsync(string ownerUid, string telegramChatId, CancellationToken ct = default);
        Task<User?> GetUserByChatIdAsync(string telegramChatId);
        Task<List<Image>> GetImagesByOwnerUidAsync(string ownerUid);
                Task<List<User>> GetUsersWithReportsEnabledAsync();
        Task<List<Image>> GetImagesByOwnerUidSinceAsync(string ownerUid, uint reportFrequency);
    }
}
