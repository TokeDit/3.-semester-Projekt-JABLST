using Rest_SikkerApi.models;

namespace Rest_SikkerApi.repos
{
    public interface ISikkerRepo
    {
        Task<Image> SaveImageAsync(Image imageEntity);
        Task<List<Image>> GetAllImagesAsync();
        Task<Image?> GetImageByIdAsync(string id);
        Task<User?> GetUserByFirebaseIdAsync(string firebaseId);
        Task UpdateUserChatIdAsync(int userId, string chatId);
        Task<User?> GetUserByChatIdAsync(string chatId);
    }
}