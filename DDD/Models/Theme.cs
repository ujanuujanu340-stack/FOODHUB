namespace DDD.Models
{
    /// <summary>OOP - CLASS/OBJECT: represents one row of the Theme (colour scheme) table. VehicleRegNo links back to a Motorbike.</summary>
    public class Theme
    {
        public string ThemeID { get; set; }
        public string VehicleRegNo { get; set; }
        public string ColourName { get; set; }
    }
}
