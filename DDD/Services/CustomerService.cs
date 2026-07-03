using System;
using System.Collections.Generic;
using DDD.Helpers;
using DDD.Models;
using DDD.Repositories;

namespace DDD.Services
{
    /// <summary>
    /// Sits between the UI and the repository. Forms call this class only -
    /// never CustomerRepository or DatabaseConnection directly - so all
    /// Customer validation rules live in exactly one place.
    /// </summary>
    public class CustomerService
    {
        private readonly ICrudRepository<Customer> _repository = new CustomerRepository();

        public List<Customer> GetAll() => _repository.GetAll();

        public Customer Search(string customerId) => _repository.Search(customerId);

        public void Add(Customer customer)
        {
            Validate(customer);
            _repository.Add(customer);
        }

        public void Update(Customer customer)
        {
            Validate(customer);
            _repository.Update(customer);
        }

        public void Delete(string customerId) => _repository.Delete(customerId);

        /// <summary>Throws ArgumentException with a friendly message when the customer data breaks a business rule.</summary>
        private void Validate(Customer customer)
        {
            if (Validator.IsNullOrEmpty(customer.CustomerID))
                throw new ArgumentException("Customer ID is required.");
            if (Validator.IsNullOrEmpty(customer.CustomerName))
                throw new ArgumentException("Customer Name is required.");
            if (!Validator.IsNullOrEmpty(customer.NIC) && !Validator.IsValidNIC(customer.NIC))
                throw new ArgumentException("NIC format is invalid (expected 9 digits + V/X or 12 digits).");
            if (!Validator.IsNullOrEmpty(customer.ContactNumber) && !Validator.IsValidPhone(customer.ContactNumber))
                throw new ArgumentException("Contact Number format is invalid.");
        }
    }
}
