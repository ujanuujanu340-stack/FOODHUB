using System;
using System.Collections.Generic;
using DDD.Helpers;
using DDD.Models;
using DDD.Repositories;

namespace DDD.Services
{
    /// <summary>
    /// Validates OrderItem data before handing it to OrderItemRepository.
    /// OrderItem uses a composite key (OrderNo + ItemNo), combined into one
    /// string via <see cref="OrderItemRepository.CombineKey"/> so it still
    /// fits the shared ICrudRepository&lt;T&gt; contract.
    /// </summary>
    public class OrderItemService
    {
        private readonly OrderItemRepository _repository = new OrderItemRepository();
        private readonly FoodItemService _foodItemService = new FoodItemService();

        public List<OrderItem> GetAll() => _repository.GetAll();

        public OrderItem Search(string orderNo, string itemNo) => _repository.Search(OrderItemRepository.CombineKey(orderNo, itemNo));

        public void Add(OrderItem orderItem)
        {
            Validate(orderItem);
            _repository.Add(orderItem);
        }

        /// <summary>Updates the row originally keyed by originalOrderNo/originalItemNo with the (possibly changed) values in orderItem.</summary>
        public void Update(string originalOrderNo, string originalItemNo, OrderItem orderItem)
        {
            Validate(orderItem);
            _repository.Update(OrderItemRepository.CombineKey(originalOrderNo, originalItemNo), orderItem);
        }

        public void Delete(string orderNo, string itemNo) => _repository.Delete(OrderItemRepository.CombineKey(orderNo, itemNo));

        /// <summary>Business rule helper: SubTotal = FoodItem.Price x Quantity, looked up from FoodItemService.</summary>
        public decimal CalculateSubTotal(string itemNo, int quantity)
        {
            FoodItem item = _foodItemService.Search(itemNo);
            return item == null ? 0 : item.Price * quantity;
        }

        private void Validate(OrderItem orderItem)
        {
            if (Validator.IsNullOrEmpty(orderItem.OrderNo))
                throw new ArgumentException("Please select an Order No.");
            if (Validator.IsNullOrEmpty(orderItem.ItemNo))
                throw new ArgumentException("Please select an Item No.");
            if (orderItem.Quantity < 0)
                throw new ArgumentException("Quantity must be a non-negative whole number.");
            if (orderItem.SubTotal < 0)
                throw new ArgumentException("Sub Total must be a non-negative number.");
        }
    }
}
