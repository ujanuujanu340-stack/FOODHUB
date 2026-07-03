namespace DDD.Models
{
    /// <summary>
    /// OOP - INHERITANCE: Customer "is a" Person, so it reuses NIC,
    /// DateOfBirth, ContactNumber and Address from the base class and only
    /// adds the properties that are specific to a customer.
    /// </summary>
    public class Customer : Person
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string LocationNo { get; set; }
        public string Lane { get; set; }
        public string Street { get; set; }
        public string City { get; set; }

        // OOP - POLYMORPHISM: the FoodHub Customer table stores the address
        // as four separate columns instead of one, so this override changes
        // *how* Address behaves for a Customer while keeping the same
        // property name/type declared on Person. Callers that only know
        // about "Person.Address" still get a sensible value.
        public override string Address
        {
            get => $"{LocationNo}, {Lane}, {Street}, {City}";
            set { /* Address is derived from the parts above; direct assignment is ignored. */ }
        }
    }
}
