using System;

namespace DDD.Models
{
    /// <summary>OOP - CLASS/OBJECT: represents one row of the Dependent table. EmployeeNo links back to the Rider who this person depends on.</summary>
    public class Dependent
    {
        public string DependentID { get; set; }
        public string EmployeeNo { get; set; }
        public string DependentName { get; set; }
        public string Relationship { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
