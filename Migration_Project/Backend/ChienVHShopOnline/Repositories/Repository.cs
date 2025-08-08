using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Data;

namespace ChienVHShopOnline.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly AppDbContext _dbContext;

        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Cannot add null entity");

            _dbContext.Add(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item == null)
                throw new KeyNotFoundException($"No entity found with key: {key}");

            _dbContext.Remove(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }

        public abstract Task<T> Get(K key);

        public abstract Task<IEnumerable<T>> GetAll();

        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem == null)
                throw new KeyNotFoundException($"No entity found with key: {key}");

            _dbContext.Entry(myItem).CurrentValues.SetValues(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }
    }
}