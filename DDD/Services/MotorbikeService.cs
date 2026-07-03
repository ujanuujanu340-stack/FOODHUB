using System;
using System.Collections.Generic;
using DDD.Helpers;
using DDD.Models;
using DDD.Repositories;

namespace DDD.Services
{
    /// <summary>Validates Motorbike data before handing it to MotorbikeRepository. Forms call this class only.</summary>
    public class MotorbikeService
    {
        private readonly ICrudRepository<Motorbike> _repository = new MotorbikeRepository();

        public List<Motorbike> GetAll() => _repository.GetAll();

        public Motorbike Search(string vehicleRegNo) => _repository.Search(vehicleRegNo);

        public void Add(Motorbike motorbike)
        {
            Validate(motorbike);
            _repository.Add(motorbike);
        }

        public void Update(Motorbike motorbike)
        {
            Validate(motorbike);
            _repository.Update(motorbike);
        }

        public void Delete(string vehicleRegNo) => _repository.Delete(vehicleRegNo);

        private void Validate(Motorbike motorbike)
        {
            if (Validator.IsNullOrEmpty(motorbike.VehicleRegNo))
                throw new ArgumentException("Vehicle Reg No is required.");
            if (Validator.IsNullOrEmpty(motorbike.Brand))
                throw new ArgumentException("Brand is required.");
        }
    }
}
