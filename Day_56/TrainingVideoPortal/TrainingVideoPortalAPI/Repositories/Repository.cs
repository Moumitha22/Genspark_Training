using TrainingVideoPortalAPI.Contexts;
using TrainingVideoPortalAPI.Interfaces;

namespace TrainingVideoPortalAPI.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly TrainingVideoDbContext _context;

        public Repository(TrainingVideoDbContext context)
        {
            _context = context;
        }

        public async Task<T> Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Cannot add null entity.");

            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item == null)
                throw new KeyNotFoundException("Entity not found for deletion.");

            _context.Remove(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<T> Update(K key, T item)
        {
            var existing = await Get(key);
            if (existing == null)
                throw new KeyNotFoundException("Entity not found for update.");

            _context.Entry(existing).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public abstract Task<T> Get(K key);
        public abstract Task<IEnumerable<T>> GetAll();
    }
}
