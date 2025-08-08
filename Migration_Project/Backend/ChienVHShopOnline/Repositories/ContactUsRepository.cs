using ChienVHShopOnline.Data;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class ContactUsRepository : Repository<int, ContactUs>
    {
        public ContactUsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<ContactUs> Get(int id)
        {
            var contact = await _dbContext.ContactUs.FindAsync(id);
            return contact ?? 
                throw new KeyNotFoundException($"Contact with ID {id} not found.");
        }

        public override async Task<IEnumerable<ContactUs>> GetAll()
        {
            return await _dbContext.ContactUs
            .OrderBy(c => c.Name)
            .ToListAsync();
        }

    }
}
