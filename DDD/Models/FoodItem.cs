namespace DDD.Models
{
    /// <summary>OOP - CLASS/OBJECT: represents one row of the FoodItem table.</summary>
    public class FoodItem
    {
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public decimal Price { get; set; }
    }
}
