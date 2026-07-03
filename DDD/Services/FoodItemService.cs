using System;
using System.Collections.Generic;
using DDD.Helpers;
using DDD.Models;
using DDD.Repositories;

namespace DDD.Services
{
    /// <summary>Validates FoodItem data before handing it to FoodItemRepository. Forms call this class only.</summary>
    public class FoodItemService
    {
        private readonly ICrudRepository<FoodItem> _repository = new FoodItemRepository();

        public List<FoodItem> GetAll() => _repository.GetAll();

        public FoodItem Search(string itemNo) => _repository.Search(itemNo);

        public void Add(FoodItem item)
        {
            Validate(item);
            _repository.Add(item);
        }

        public void Update(FoodItem item)
        {
            Validate(item);
            _repository.Update(item);
        }

        public void Delete(string itemNo) => _repository.Delete(itemNo);

        private void Validate(FoodItem item)
        {
            if (Validator.IsNullOrEmpty(item.ItemNo))
                throw new ArgumentException("Item No is required.");
            if (Validator.IsNullOrEmpty(item.ItemName))
                throw new ArgumentException("Item Name is required.");
            if (item.Price < 0)
                throw new ArgumentException("Price must be a non-negative number.");
        }
    }
}
