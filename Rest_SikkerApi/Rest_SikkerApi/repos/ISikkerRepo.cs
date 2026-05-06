using Rest_SikkerApi.models;

namespace Rest_SikkerApi.repos
{
    public interface ISikkerRepo
    {
        Task<Image> SaveImageAsync(Image imageEntity);
        Task<List<Image>> GetAllImagesAsync();
        Task<Image?> GetImageByIdAsync(int id);
        Task<User?> GetUserByFirebaseIdAsync(string ownerUid);
        Task<bool> UpdateUserChatIdAsync(string ownerUid, string telegramChatId, CancellationToken ct = default);
        Task<User?> GetUserByChatIdAsync(string telegramChatId);
    }
}
