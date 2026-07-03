namespace DDD.Models
{
    /// <summary>OOP - CLASS/OBJECT: link row for the many-to-many relationship between FoodItem and Ingredient. ItemNo + IngredientID together form the primary key.</summary>
    public class ItemIngredient
    {
        public string ItemNo { get; set; }
        public string IngredientID { get; set; }
        public decimal Quantity { get; set; }
    }
}
