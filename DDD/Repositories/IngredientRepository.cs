using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DDD.Database;
using DDD.Models;

namespace DDD.Repositories
{
    /// <summary>OOP - POLYMORPHISM: implements ICrudRepository&lt;Ingredient&gt; with the SQL specific to the Ingredient table.</summary>
    public class IngredientRepository : ICrudRepository<Ingredient>
    {
        public void Add(Ingredient entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "INSERT INTO Ingredient (IngredientID, IngredientName) VALUES (@id, @name)",
                new SQLiteParameter("@id", entity.IngredientID),
                new SQLiteParameter("@name", entity.IngredientName));
        }

        public void Update(Ingredient entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "UPDATE Ingredient SET IngredientName=@name WHERE IngredientID=@id",
                new SQLiteParameter("@name", entity.IngredientName),
                new SQLiteParameter("@id", entity.IngredientID));
        }

        public void Delete(string id)
        {
            DatabaseConnection.ExecuteNonQuery("DELETE FROM Ingredient WHERE IngredientID=@id", new SQLiteParameter("@id", id));
        }

        public Ingredient Search(string id)
        {
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Ingredient WHERE IngredientID=@id", new SQLiteParameter("@id", id));
            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        public List<Ingredient> GetAll()
        {
            var list = new List<Ingredient>();
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Ingredient ORDER BY IngredientID");
            foreach (DataRow row in table.Rows)
                list.Add(MapRow(row));
            return list;
        }

        private static Ingredient MapRow(DataRow row)
        {
            return new Ingredient
            {
                IngredientID = row["IngredientID"].ToString(),
                IngredientName = row["IngredientName"].ToString()
            };
        }
    }
}
