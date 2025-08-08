using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Interfaces;

namespace OnlineAuctionAPI.Repositories;

public class Repository<K, T> : IRepository<K, T>
    where T : class
{
    protected readonly AuctionContext _auctionContext;

    public Repository(AuctionContext auctionContext)
    {
        _auctionContext = auctionContext;
    }

    public async Task<T> Add(T item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            _auctionContext.Add(item);
            await _auctionContext.SaveChangesAsync();
            return item;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Adding the data", ex);
        }
    }

    public async Task<T> Get(K key)
    {
        try
        {
            var item = await _auctionContext.FindAsync<T>(key);
            if (item == null)
            {
                throw new NotFoundException($"Item with key {key} not found");
            }
            return item;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Get data from database", ex);
        }
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        try
        {
            var item = await _auctionContext.Set<T>().ToListAsync();
            if (item == null)
            {
                throw new NotFoundException("Auction context cannot be null");
            }
            return await _auctionContext.Set<T>().ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Getting all the data from database.", ex);
        }
    }

    public async Task<T> Update(K key, T item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            var existingItem = await Get(key);
            _auctionContext.Entry(existingItem).CurrentValues.SetValues(item);
            await _auctionContext.SaveChangesAsync();
            return existingItem;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Updating the data.", ex);
        }
    }

    public async Task<T> Delete(K key)
    {
        try
        {
            var item = await Get(key);
            _auctionContext.Remove(item);
            await _auctionContext.SaveChangesAsync();
            return item;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Deleting the data", ex);
        }
    }
}
