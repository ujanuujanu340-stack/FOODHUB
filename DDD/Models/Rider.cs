namespace DDD.Models
{
    /// <summary>
    /// OOP - INHERITANCE: Rider "is a" Person too, and inherits the same
    /// four base properties as Customer while keeping its own Address
    /// behaviour (the Rider table has one plain Address column, so no
    /// override is needed here).
    /// </summary>
    public class Rider : Person
    {
        public string EmployeeNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string LicenseNumber { get; set; }
        public int? Age { get; set; }

        // A small read-only convenience property (OOP - ENCAPSULATION: the
        // name-joining logic lives with the data instead of being repeated
        // in every form that needs to display a rider's full name).
        public string FullName => string.IsNullOrWhiteSpace(MiddleName)
            ? $"{FirstName} {LastName}"
            : $"{FirstName} {MiddleName} {LastName}";
    }
}
