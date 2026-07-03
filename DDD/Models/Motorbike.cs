using System;

namespace DDD.Models
{
    /// <summary>OOP - CLASS/OBJECT: represents one row of the Motorbike table.</summary>
    public class Motorbike
    {
        public string VehicleRegNo { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string EngineNo { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}
