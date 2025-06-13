using PropFinderApi.Interfaces;
using PropFinderApi.Contexts;
using PropFinderApi.Exceptions;

namespace PropFinderApi.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly PropFinderDbContext _propFinderDbContext;

        public Repository(PropFinderDbContext propFinderDbContext)
        {
            _propFinderDbContext = propFinderDbContext;
        }

        public async Task<T> Add(T item)
        {
            if (item == null)
                throw new BadRequestException("Cannot add null entity");

            _propFinderDbContext.Add(item);
            await _propFinderDbContext.SaveChangesAsync();
            return item;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            _propFinderDbContext.Remove(item);
            await _propFinderDbContext.SaveChangesAsync();
            return item;
        }

        public abstract Task<T> Get(K key);

        public abstract Task<IEnumerable<T>> GetAll();

        public async Task<T> Update(K key, T item)
        {
            var existing = await Get(key);
            _propFinderDbContext.Entry(existing).CurrentValues.SetValues(item);
            await _propFinderDbContext.SaveChangesAsync();
            return item;
        }
    }
}
