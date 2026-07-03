using System;
using System.Collections.Generic;
using DDD.Helpers;
using DDD.Models;
using DDD.Repositories;

namespace DDD.Services
{
    /// <summary>Validates Rider data before handing it to RiderRepository. Forms call this class only.</summary>
    public class RiderService
    {
        private readonly ICrudRepository<Rider> _repository = new RiderRepository();

        public List<Rider> GetAll() => _repository.GetAll();

        public Rider Search(string employeeNo) => _repository.Search(employeeNo);

        public void Add(Rider rider)
        {
            Validate(rider);
            _repository.Add(rider);
        }

        public void Update(Rider rider)
        {
            Validate(rider);
            _repository.Update(rider);
        }

        public void Delete(string employeeNo) => _repository.Delete(employeeNo);

        private void Validate(Rider rider)
        {
            if (Validator.IsNullOrEmpty(rider.EmployeeNo))
                throw new ArgumentException("Employee No is required.");
            if (Validator.IsNullOrEmpty(rider.FirstName) || Validator.IsNullOrEmpty(rider.LastName))
                throw new ArgumentException("First Name and Last Name are required.");
            if (!Validator.IsNullOrEmpty(rider.NIC) && !Validator.IsValidNIC(rider.NIC))
                throw new ArgumentException("NIC format is invalid (expected 9 digits + V/X or 12 digits).");
            if (!Validator.IsNullOrEmpty(rider.ContactNumber) && !Validator.IsValidPhone(rider.ContactNumber))
                throw new ArgumentException("Contact Number format is invalid.");
            if (rider.Age.HasValue && rider.Age.Value < 0)
                throw new ArgumentException("Age cannot be negative.");
        }
    }
}
