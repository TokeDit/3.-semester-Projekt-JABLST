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

        public async Task<List<Image>> GetAllImagesAsync()
        {
            return await _context.Images.ToListAsync();

        }

    }
}

