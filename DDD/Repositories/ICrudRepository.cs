using System.Collections.Generic;

namespace DDD.Repositories
{
    /// <summary>
    /// OOP - ABSTRACTION: this interface describes WHAT every repository can
    /// do (Add, Update, Delete, Search, GetAll) without saying HOW. Each
    /// concrete repository (CustomerRepository, OrderRepository, ...)
    /// provides its own implementation - that is OOP - POLYMORPHISM: the
    /// same method names behave differently depending on which class is
    /// actually running.
    /// </summary>
    /// <typeparam name="T">The model type this repository manages (e.g. Customer).</typeparam>
    public interface ICrudRepository<T>
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(string id);
        T Search(string id);
        List<T> GetAll();
    }
}
