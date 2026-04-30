using Rest_SikkerApi.data;
using Microsoft.EntityFrameworkCore;
using Rest_SikkerApi.models;

namespace Rest_SikkerApi.repos
{
    public class SikkerRepo
    {
        private readonly AppDbContext _context;
        // måske implementer en user, så they can't get others imges
        public SikkerRepo(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously saves a new image entity to the data store.
        /// </summary>
        /// <param name="imageEntity">The image entity to be saved. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the saved image entity,
        /// including any updated properties such as generated identifiers.</returns>
        public async Task<Image> SaveImageAsync(Image imageEntity)
        {
            _context.Images.Add(imageEntity);
            await _context.SaveChangesAsync();
            return imageEntity;
        }

        /// <summary>
        /// Asynchronously retrieves all images from the data store.
        /// </summary>
        /// <remarks>This method executes a database query to retrieve all image records. The returned
        /// list reflects the current state of the data store at the time of the query.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all images. If no
        /// images are found, the list will be empty.</returns>
        public async Task<List<Image>> GetAllImagesAsync()
        {
            return await _context.Images.ToListAsync();

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

