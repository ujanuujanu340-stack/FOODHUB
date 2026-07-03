using System;
using System.Collections.Generic;
using DDD.Helpers;
using DDD.Models;
using DDD.Repositories;

namespace DDD.Services
{
    /// <summary>Validates Theme/Colour data before handing it to ThemeRepository. Forms call this class only.</summary>
    public class ThemeService
    {
        private readonly ICrudRepository<Theme> _repository = new ThemeRepository();

        public List<Theme> GetAll() => _repository.GetAll();

        public Theme Search(string themeId) => _repository.Search(themeId);

        public void Add(Theme theme)
        {
            Validate(theme);
            _repository.Add(theme);
        }

        public void Update(Theme theme)
        {
            Validate(theme);
            _repository.Update(theme);
        }

        public void Delete(string themeId) => _repository.Delete(themeId);

        private void Validate(Theme theme)
        {
            if (Validator.IsNullOrEmpty(theme.ThemeID))
                throw new ArgumentException("Theme ID is required.");
            if (Validator.IsNullOrEmpty(theme.VehicleRegNo))
                throw new ArgumentException("Please select a Vehicle Reg No.");
            if (Validator.IsNullOrEmpty(theme.ColourName))
                throw new ArgumentException("Colour Name is required.");
        }
    }
}
