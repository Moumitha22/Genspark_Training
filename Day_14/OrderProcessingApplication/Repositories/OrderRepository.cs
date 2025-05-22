using System;
using System.Collections.Generic;
using OrderProcessingApplication.Exceptions;
using OrderProcessingApplication.Exceptions.cs;
using OrderProcessingApplication.Interfaces;
using OrderProcessingApplication.Models;

namespace OrderProcessingApplication.Repositories
{
    public class OrderRepository : Repository<int, Order>
    {
        public override ICollection<Order> GetAll()
        {
            if (_items.Count == 0)
            {
                throw new CollectionEmptyException("No orders found");
            }
            return _items;
        }

        public override Order GetById(int id)
        {
            if (_items.Count == 0)
            {
                throw new CollectionEmptyException("No orders found");
            }
            var item = _items.FirstOrDefault(order => order.Id == id);
            if (item == null)
            {
                throw new KeyNotFoundException($"Order with id {id} not found");
            }
            return item;
        }

        public override int GenerateID()
        {
            if (_items.Count == 0)
            {
                return 101;
            }
            return _items.Max(i => i.Id) + 1;
        }


    }
}
