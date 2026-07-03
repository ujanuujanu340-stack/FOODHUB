using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DDD.Database;
using DDD.Models;

namespace DDD.Repositories
{
    /// <summary>OOP - POLYMORPHISM: implements ICrudRepository&lt;Theme&gt; with the SQL specific to the Theme table.</summary>
    public class ThemeRepository : ICrudRepository<Theme>
    {
        public void Add(Theme entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "INSERT INTO Theme (ThemeID, VehicleRegNo, ColourName) VALUES (@id, @vehicle, @colour)",
                new SQLiteParameter("@id", entity.ThemeID),
                new SQLiteParameter("@vehicle", entity.VehicleRegNo),
                new SQLiteParameter("@colour", entity.ColourName));
        }

        public void Update(Theme entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "UPDATE Theme SET VehicleRegNo=@vehicle, ColourName=@colour WHERE ThemeID=@id",
                new SQLiteParameter("@vehicle", entity.VehicleRegNo),
                new SQLiteParameter("@colour", entity.ColourName),
                new SQLiteParameter("@id", entity.ThemeID));
        }

        public void Delete(string id)
        {
            DatabaseConnection.ExecuteNonQuery("DELETE FROM Theme WHERE ThemeID=@id", new SQLiteParameter("@id", id));
        }

        public Theme Search(string id)
        {
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Theme WHERE ThemeID=@id", new SQLiteParameter("@id", id));
            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        public List<Theme> GetAll()
        {
            var list = new List<Theme>();
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Theme ORDER BY ThemeID");
            foreach (DataRow row in table.Rows)
                list.Add(MapRow(row));
            return list;
        }

        private static Theme MapRow(DataRow row)
        {
            return new Theme
            {
                ThemeID = row["ThemeID"].ToString(),
                VehicleRegNo = row["VehicleRegNo"].ToString(),
                ColourName = row["ColourName"].ToString()
            };
        }
    }
}
