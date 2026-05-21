using Rest_SikkerApi.data;
using Microsoft.EntityFrameworkCore;
using Rest_SikkerApi.models;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.Security.Policy;

namespace Rest_SikkerApi.repos
{
    public class SikkerRepo : ISikkerRepo
    {
        private readonly AppDbContext _context;
        
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly FileHandlingService _fileHandlerService;
        private readonly DatabaseHandlingService _databaseHandlingService;
        // måske implementer en user, så they can't get others imges
        public SikkerRepo(AppDbContext context, BlobServiceClient blobServiceClient, FileHandlingService fileHandlingService, DatabaseHandlingService databaseHandlingService)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient("images");
            _fileHandlerService = fileHandlingService;
            _databaseHandlingService = databaseHandlingService;

        }

        /// <summary>
        /// Updates an existing user's notification settings, or creates a new user record if none exists (Upsert).
        /// </summary>
        /// <param name="ownerUid">Firebase UID of the user (primary key)</param>
        /// <param name="telegramChatId">Telegram chat ID for notifications (can be null)</param>
        /// <param name="reportFrequency">Number of days between reports (1, 7, or 30)</param>
        /// <param name="reportEnabled">Whether reports are enabled for this user</param>
        /// <returns>The updated or newly created User entity</returns>
        public async Task<User?> UpdateUserAsync(
            string ownerUid,
            string? telegramChatId,
            int reportFrequency,
            bool reportEnabled)
        {
            // Try to find existing user by its primary key (OwnerUid)
            var user = await _context.Users.FindAsync(ownerUid);

            if (user == null)
            {
                // No user record exists yet – create one (UPSERT behaviour)
                // This allows local testing where the database starts empty,
                // and the first Save operation will succeed instead of returning 404.
                user = new User { OwnerUid = ownerUid };
                _context.Users.Add(user);
            }

            // Update the settings regardless of whether the user was new or existing
            user.TelegramChatId = telegramChatId;
            user.ReportFrequency = reportFrequency;
            user.ReportEnabled = reportEnabled;

            // Persist changes to the database
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<Image> SaveImageAsync(Image imageEntity)
        {
            await _databaseHandlingService.SaveImageAsync(imageEntity);
            await _fileHandlerService.UploadImageAsync(imageEntity.Id, imageEntity.ImageData);
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
            return await _context.Images.ToListAsync() ?? new List<Image>();
        }

        // Failure occurs Azure.RequestFailedException. Multiple failures occur, an AggregateException will be thrown
        // Exceptions thrown (Azure.RequestFailedException, AggregateException)
        public async Task<Image?> GetImageByIdAsync(int id, string uid)
        {
            if (! await _databaseHandlingService.CheckIdUidMatch(id, uid))
            {
                throw new InvalidDataException("id or uid does not match the image");
            }
            return await _fileHandlerService.GetImageAsync(id);
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

        public IEnumerable<Image> GetAmountImage(string uid, int amount = 20)
        {
            return _context.Images.Where(i => i.OwnerUid == uid).OrderByDescending(i => i.Id).Take(amount);
        }

        public IEnumerable<Image> GetAfterIDImage(int id, int amount = 20)
        {
            return _context.Images.Where(i => i.Id > id).OrderByDescending(i => i.Id).Take(amount);
        }

        public async Task<IEnumerable<Image>> GetBeforeIDImageAsync(string uid, int id, int amount = 20)
        {
            IEnumerable<Image> images = _databaseHandlingService.GetBeforeIDImage(uid, id, amount);
            foreach (Image image in images)
            {
                try
                {
                    Image? imageData = await _fileHandlerService.GetImageAsync(image.Id);

                    if (imageData != null)
                    {
                        image.ImageData = imageData.ImageData;
                    }
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine("Image is not base64 convertable");
                }
            }
            return images;
        }

        // System state - stored in memory for now
        private static bool _systemOnline = false;

        public bool GetSystemState()
        {
            return _systemOnline;
        }

        public async Task<Image?> GetResentImage(string uid)
        {
            DateTime dateTime = DateTime.UtcNow;
            Image? image = await _context.Images.Where(x => x.OwnerUid == uid)
            .OrderBy(x => Math.Abs(
                EF.Functions.DateDiffSecond(x.TimeStamp, dateTime)))
            .FirstOrDefaultAsync();

            if (image != null)
            {
                Image? imageData = await _fileHandlerService.GetImageAsync(image.Id);
                image.ImageData = (imageData != null) ? imageData.ImageData : "";
            }

            return image;
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
        // Get all users with Telegram chat ID and reports enabled
        public async Task<List<User>> GetUsersWithReportsEnabledAsync()
        {
            return await _context.Users
                .Where(u => u.ReportEnabled && u.TelegramChatId != null)
                .ToListAsync();
        }
        //  Get images for a user within a time range
        public async Task<List<Image>> GetImagesByOwnerUidSinceAsync(string ownerUid, uint reportFrequency)
        {
            DateTime dt = DateTime.UtcNow.AddDays(-(double)reportFrequency);
            List<Image> result = await _context.Images.Where(i => i.OwnerUid == ownerUid && dt <= i.TimeStamp).ToListAsync();
            return result; 
        }

        //  Get images for a user filtered by month and year
        public async Task<List<Image>> GetImagesByOwnerUidAndMonthAsync(string ownerUid, int year, int month)
        {
            return await _context.Images
                .Where(i => i.OwnerUid == ownerUid &&
                       i.TimeStamp.Year == year &&
                       i.TimeStamp.Month == month)
                .OrderByDescending(i => i.Id)
                .ToListAsync();
        }
    }
}

