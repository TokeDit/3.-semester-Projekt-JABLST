using Rest_SikkerApi.data;
using Microsoft.EntityFrameworkCore;

namespace Rest_SikkerApi.repos
{
    public class SikkerRepo
    {
        private readonly AppDbContext _context;
        public SikkerRepo(AppDbContext context) 
        { 
            _context = context;
        }


    }
}
