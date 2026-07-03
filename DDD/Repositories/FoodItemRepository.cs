using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DDD.Database;
using DDD.Models;

namespace DDD.Repositories
{
    /// <summary>OOP - POLYMORPHISM: implements ICrudRepository&lt;FoodItem&gt; with the SQL specific to the FoodItem table.</summary>
    public class FoodItemRepository : ICrudRepository<FoodItem>
    {
        public void Add(FoodItem entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "INSERT INTO FoodItem (ItemNo, ItemName, ItemCategory, Price) VALUES (@id, @name, @category, @price)",
                new SQLiteParameter("@id", entity.ItemNo),
                new SQLiteParameter("@name", entity.ItemName),
                new SQLiteParameter("@category", entity.ItemCategory),
                new SQLiteParameter("@price", entity.Price));
        }

        public void Update(FoodItem entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "UPDATE FoodItem SET ItemName=@name, ItemCategory=@category, Price=@price WHERE ItemNo=@id",
                new SQLiteParameter("@name", entity.ItemName),
                new SQLiteParameter("@category", entity.ItemCategory),
                new SQLiteParameter("@price", entity.Price),
                new SQLiteParameter("@id", entity.ItemNo));
        }

        public void Delete(string id)
        {
            DatabaseConnection.ExecuteNonQuery("DELETE FROM FoodItem WHERE ItemNo=@id", new SQLiteParameter("@id", id));
        }

        public FoodItem Search(string id)
        {
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM FoodItem WHERE ItemNo=@id", new SQLiteParameter("@id", id));
            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        public List<FoodItem> GetAll()
        {
            var list = new List<FoodItem>();
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM FoodItem ORDER BY ItemNo");
            foreach (DataRow row in table.Rows)
                list.Add(MapRow(row));
            return list;
        }

        private static FoodItem MapRow(DataRow row)
        {
            return new FoodItem
            {
                ItemNo = row["ItemNo"].ToString(),
                ItemName = row["ItemName"].ToString(),
                ItemCategory = row["ItemCategory"].ToString(),
                Price = row["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Price"])
            };
        }
    }
}
