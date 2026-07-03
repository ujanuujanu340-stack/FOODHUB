using System;
using System.Collections.Generic;
using DDD.Helpers;
using DDD.Models;
using DDD.Repositories;

namespace DDD.Services
{
    /// <summary>Validates Order data before handing it to OrderRepository. Forms call this class only.</summary>
    public class OrderService
    {
        private readonly ICrudRepository<Order> _repository = new OrderRepository();

        public List<Order> GetAll() => _repository.GetAll();

        public Order Search(string orderNo) => _repository.Search(orderNo);

        public void Add(Order order)
        {
            Validate(order);
            _repository.Add(order);
        }

        public void Update(Order order)
        {
            Validate(order);
            _repository.Update(order);
        }

        public void Delete(string orderNo) => _repository.Delete(orderNo);

        private void Validate(Order order)
        {
            if (Validator.IsNullOrEmpty(order.OrderNo))
                throw new ArgumentException("Order No is required.");
            if (Validator.IsNullOrEmpty(order.CustomerID))
                throw new ArgumentException("Please select a Customer ID.");
            if (Validator.IsNullOrEmpty(order.OrderStatus))
                throw new ArgumentException("Please select an Order Status.");
            if (order.OrderAmount < 0)
                throw new ArgumentException("Order Amount must be a non-negative number.");
        }
    }
}
