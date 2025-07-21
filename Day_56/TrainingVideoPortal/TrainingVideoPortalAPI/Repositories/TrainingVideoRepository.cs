using Microsoft.EntityFrameworkCore;
using TrainingVideoPortalAPI.Contexts;
using TrainingVideoPortalAPI.Interfaces;
using TrainingVideoPortalAPI.Models;

namespace TrainingVideoPortalAPI.Repositories
{
    public class TrainingVideoRepository : Repository<Guid, TrainingVideo>
    {
        public TrainingVideoRepository(TrainingVideoDbContext context) : base(context) { }

        public override async Task<TrainingVideo> Get(Guid key)
        {
            return await _context.TrainingVideos.FindAsync(key)
                   ?? throw new KeyNotFoundException("Video not found");
        }

        public override async Task<IEnumerable<TrainingVideo>> GetAll()
        {
            return await _context.TrainingVideos
                .OrderByDescending(v => v.UploadDate)
                .ToListAsync();
        }
    }
}
