using System;
using System.Collections.Generic;
using DDD.Helpers;
using DDD.Models;
using DDD.Repositories;

namespace DDD.Services
{
    /// <summary>Validates Ingredient data before handing it to IngredientRepository. Forms call this class only.</summary>
    public class IngredientService
    {
        private readonly ICrudRepository<Ingredient> _repository = new IngredientRepository();

        public List<Ingredient> GetAll() => _repository.GetAll();

        public Ingredient Search(string ingredientId) => _repository.Search(ingredientId);

        public void Add(Ingredient ingredient)
        {
            Validate(ingredient);
            _repository.Add(ingredient);
        }

        public void Update(Ingredient ingredient)
        {
            Validate(ingredient);
            _repository.Update(ingredient);
        }

        public void Delete(string ingredientId) => _repository.Delete(ingredientId);

        private void Validate(Ingredient ingredient)
        {
            if (Validator.IsNullOrEmpty(ingredient.IngredientID))
                throw new ArgumentException("Ingredient ID is required.");
            if (Validator.IsNullOrEmpty(ingredient.IngredientName))
                throw new ArgumentException("Ingredient Name is required.");
        }
    }
}
