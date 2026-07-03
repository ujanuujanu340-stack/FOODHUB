using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DDD.Database;
using DDD.Models;

namespace DDD.Repositories
{
    /// <summary>
    /// OOP - POLYMORPHISM: implements ICrudRepository&lt;ItemIngredient&gt;.
    /// ItemIngredient has a composite primary key (ItemNo + IngredientID),
    /// so Search and Delete accept a single "id" string in the form
    /// "ItemNo||IngredientID" (see <see cref="CombineKey"/>).
    /// </summary>
    public class ItemIngredientRepository : ICrudRepository<ItemIngredient>
    {
        private const string KeySeparator = "||";

        public static string CombineKey(string itemNo, string ingredientId) => $"{itemNo}{KeySeparator}{ingredientId}";

        public void Add(ItemIngredient entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "INSERT INTO ItemIngredient (ItemNo, IngredientID, Quantity) VALUES (@item, @ingredient, @qty)",
                new SQLiteParameter("@item", entity.ItemNo),
                new SQLiteParameter("@ingredient", entity.IngredientID),
                new SQLiteParameter("@qty", entity.Quantity));
        }

        public void Update(ItemIngredient entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "UPDATE ItemIngredient SET Quantity=@qty WHERE ItemNo=@item AND IngredientID=@ingredient",
                new SQLiteParameter("@qty", entity.Quantity),
                new SQLiteParameter("@item", entity.ItemNo),
                new SQLiteParameter("@ingredient", entity.IngredientID));
        }

        /// <summary>Updates a row identified by its original composite key, allowing the key values themselves to change.</summary>
        public void Update(string originalId, ItemIngredient entity)
        {
            var (originalItemNo, originalIngredientId) = SplitKey(originalId);
            DatabaseConnection.ExecuteNonQuery(
                @"UPDATE ItemIngredient SET ItemNo=@newItem, IngredientID=@newIngredient, Quantity=@qty
                  WHERE ItemNo=@oldItem AND IngredientID=@oldIngredient",
                new SQLiteParameter("@newItem", entity.ItemNo),
                new SQLiteParameter("@newIngredient", entity.IngredientID),
                new SQLiteParameter("@qty", entity.Quantity),
                new SQLiteParameter("@oldItem", originalItemNo),
                new SQLiteParameter("@oldIngredient", originalIngredientId));
        }

        public void Delete(string id)
        {
            var (itemNo, ingredientId) = SplitKey(id);
            DatabaseConnection.ExecuteNonQuery(
                "DELETE FROM ItemIngredient WHERE ItemNo=@item AND IngredientID=@ingredient",
                new SQLiteParameter("@item", itemNo),
                new SQLiteParameter("@ingredient", ingredientId));
        }

        public ItemIngredient Search(string id)
        {
            var (itemNo, ingredientId) = SplitKey(id);
            DataTable table = DatabaseConnection.ExecuteQuery(
                "SELECT * FROM ItemIngredient WHERE ItemNo=@item AND IngredientID=@ingredient",
                new SQLiteParameter("@item", itemNo),
                new SQLiteParameter("@ingredient", ingredientId));
            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        public List<ItemIngredient> GetAll()
        {
            var list = new List<ItemIngredient>();
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM ItemIngredient ORDER BY ItemNo, IngredientID");
            foreach (DataRow row in table.Rows)
                list.Add(MapRow(row));
            return list;
        }

        private static (string, string) SplitKey(string id)
        {
            string[] parts = (id ?? string.Empty).Split(new[] { KeySeparator }, StringSplitOptions.None);
            return (parts.Length > 0 ? parts[0] : string.Empty, parts.Length > 1 ? parts[1] : string.Empty);
        }

        private static ItemIngredient MapRow(DataRow row)
        {
            return new ItemIngredient
            {
                ItemNo = row["ItemNo"].ToString(),
                IngredientID = row["IngredientID"].ToString(),
                Quantity = row["Quantity"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Quantity"])
            };
        }
    }
}
