namespace DDD.Models
{
    /// <summary>OOP - CLASS/OBJECT: link row for the many-to-many relationship between Orders and FoodItem. OrderNo + ItemNo together form the primary key.</summary>
    public class OrderItem
    {
        public string OrderNo { get; set; }
        public string ItemNo { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
