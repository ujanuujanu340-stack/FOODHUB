using System;

namespace DDD.Models
{
    /// <summary>
    /// OOP - INHERITANCE base class. Any "person" in the FoodHub system
    /// (a Customer or a Rider) shares these four properties, so they live
    /// here once instead of being duplicated in every class that needs them.
    /// The class is "abstract" because a bare Person is never created on its
    /// own - only concrete subclasses (Customer, Rider) are.
    /// </summary>
    public abstract class Person
    {
        // OOP - ENCAPSULATION: fields are exposed only through public
        // properties, never as raw public fields.
        public string NIC { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ContactNumber { get; set; }

        // "virtual" allows a subclass to override this property with its
        // own logic (see Customer.Address for an example of POLYMORPHISM).
        public virtual string Address { get; set; }
    }
}
