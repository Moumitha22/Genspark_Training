
using DocShareApi.Data;
using DocShareApi.Interfaces;
using DocShareApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DocShareApi.Repositories
{

    public class DocumentRepository : IRepository<Guid, Document>
    {
        private readonly DocShareDbContext _context;
        public DocumentRepository(DocShareDbContext context)
        {
            _context = context;
        }

        public async Task<Document> Add(Document entity)
        {
            _context.Documents.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Document> Delete(Guid id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null) return null;

            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<Document> Get(Guid id)
        {
            return await _context.Documents.Include(d => d.Uploader).FirstOrDefaultAsync(d => d.DocumentId == id);
        }

        public async Task<IEnumerable<Document>> GetAll()
        {
            return await _context.Documents.Include(d => d.Uploader).ToListAsync();
        }

        public async Task<Document> Update(Guid id, Document entity)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null) return null;

            doc.FileName = entity.FileName;
            doc.Description = entity.Description;
            doc.Status = entity.Status;

            await _context.SaveChangesAsync();
            return doc;
        }
    }
}
