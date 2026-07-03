using System;
using System.Collections.Generic;
using DDD.Helpers;
using DDD.Models;
using DDD.Repositories;

namespace DDD.Services
{
    /// <summary>
    /// Validates ItemIngredient data before handing it to
    /// ItemIngredientRepository. ItemIngredient uses a composite key
    /// (ItemNo + IngredientID), combined into one string via
    /// <see cref="ItemIngredientRepository.CombineKey"/> so it still fits
    /// the shared ICrudRepository&lt;T&gt; contract.
    /// </summary>
    public class ItemIngredientService
    {
        private readonly ItemIngredientRepository _repository = new ItemIngredientRepository();

        public List<ItemIngredient> GetAll() => _repository.GetAll();

        public ItemIngredient Search(string itemNo, string ingredientId) => _repository.Search(ItemIngredientRepository.CombineKey(itemNo, ingredientId));

        public void Add(ItemIngredient itemIngredient)
        {
            Validate(itemIngredient);
            _repository.Add(itemIngredient);
        }

        /// <summary>Updates the row originally keyed by originalItemNo/originalIngredientId with the (possibly changed) values in itemIngredient.</summary>
        public void Update(string originalItemNo, string originalIngredientId, ItemIngredient itemIngredient)
        {
            Validate(itemIngredient);
            _repository.Update(ItemIngredientRepository.CombineKey(originalItemNo, originalIngredientId), itemIngredient);
        }

        public void Delete(string itemNo, string ingredientId) => _repository.Delete(ItemIngredientRepository.CombineKey(itemNo, ingredientId));

        private void Validate(ItemIngredient itemIngredient)
        {
            if (Validator.IsNullOrEmpty(itemIngredient.ItemNo))
                throw new ArgumentException("Please select an Item No.");
            if (Validator.IsNullOrEmpty(itemIngredient.IngredientID))
                throw new ArgumentException("Please select an Ingredient ID.");
            if (itemIngredient.Quantity < 0)
                throw new ArgumentException("Quantity must be a non-negative number.");
        }
    }
}
