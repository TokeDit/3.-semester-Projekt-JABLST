using Rest_SikkerApi.models;

namespace Rest_SikkerApi.repos
{
    public interface ISikkerRepo
    {
        Task<Image> SaveImageAsync(Image imageEntity);
        Task<User> SaveUserAsync(User user);
        Task<List<Image>> GetAllImagesAsync();
        Task<Image?> GetImageByIdAsync(int id, string uid);
        Task<User?> GetUserByFirebaseIdAsync(string ownerUid);
        Task<bool> UpdateUserChatIdAsync(string ownerUid, string telegramChatId, CancellationToken ct = default);
        Task<User?> GetUserByChatIdAsync(string telegramChatId);
        Task<List<Image>> GetImagesByOwnerUidAsync(string ownerUid);
        Task<List<User>> GetUsersWithReportsEnabledAsync();
        Task<List<Image>> GetImagesByOwnerUidSinceAsync(string ownerUid, uint reportFrequency);

        //ADDITIONAL METHODS FOR USER MANAGEMENT
        Task<List<Image>> GetImagesByOwnerUidAndMonthAsync(string ownerUid, int year, int month);
        Task<User?> UpdateUserAsync(string ownerUid, string? telegramChatId, int reportFrequency, bool reportEnabled);
        Task<IEnumerable<Image>> GetAmountImageAsync(string uid, int amount = 20);
        IEnumerable<Image> GetAfterIDImage(int id, int amount = 20);
        Task<IEnumerable<Image>> GetBeforeIDImageAsync(string uid, int id, int amount = 20);
        Task<bool> CheckIfUserExist(string uid);

        bool GetSystemState();
        bool SetSystemState(bool state);
    }
}
