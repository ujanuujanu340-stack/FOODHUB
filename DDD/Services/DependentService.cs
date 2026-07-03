using System;
using System.Collections.Generic;
using DDD.Helpers;
using DDD.Models;
using DDD.Repositories;

namespace DDD.Services
{
    /// <summary>Validates Dependent data before handing it to DependentRepository. Forms call this class only.</summary>
    public class DependentService
    {
        private readonly ICrudRepository<Dependent> _repository = new DependentRepository();

        public List<Dependent> GetAll() => _repository.GetAll();

        public Dependent Search(string dependentId) => _repository.Search(dependentId);

        public void Add(Dependent dependent)
        {
            Validate(dependent);
            _repository.Add(dependent);
        }

        public void Update(Dependent dependent)
        {
            Validate(dependent);
            _repository.Update(dependent);
        }

        public void Delete(string dependentId) => _repository.Delete(dependentId);

        private void Validate(Dependent dependent)
        {
            if (Validator.IsNullOrEmpty(dependent.DependentID))
                throw new ArgumentException("Dependent ID is required.");
            if (Validator.IsNullOrEmpty(dependent.EmployeeNo))
                throw new ArgumentException("Please select an Employee No.");
            if (Validator.IsNullOrEmpty(dependent.DependentName))
                throw new ArgumentException("Dependent Name is required.");
        }
    }
}
