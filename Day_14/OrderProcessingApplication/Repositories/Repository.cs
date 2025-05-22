using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderProcessingApplication.Interfaces;

namespace OrderProcessingApplication.Repositories
{

    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly List<T> _items = new();

        public abstract K GenerateID();

        public abstract T GetById(K key);

        public abstract ICollection<T> GetAll();

        public T Add(T item)
        {
            var id = GenerateID();
            var property = typeof(T).GetProperty("Id");
            if (property != null)
            {
                property.SetValue(item, id);
            }
            _items.Add(item);  
            return item;       
        }
    }
}
