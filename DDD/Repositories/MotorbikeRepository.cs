using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DDD.Database;
using DDD.Models;

namespace DDD.Repositories
{
    /// <summary>OOP - POLYMORPHISM: implements ICrudRepository&lt;Motorbike&gt; with the SQL specific to the Motorbike table.</summary>
    public class MotorbikeRepository : ICrudRepository<Motorbike>
    {
        public void Add(Motorbike entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                @"INSERT INTO Motorbike (VehicleRegNo, Brand, Model, EngineNo, RegisteredDate)
                  VALUES (@id, @brand, @model, @engine, @regDate)",
                new SQLiteParameter("@id", entity.VehicleRegNo),
                new SQLiteParameter("@brand", entity.Brand),
                new SQLiteParameter("@model", entity.Model),
                new SQLiteParameter("@engine", entity.EngineNo),
                new SQLiteParameter("@regDate", entity.RegisteredDate.ToString("yyyy-MM-dd")));
        }

        public void Update(Motorbike entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "UPDATE Motorbike SET Brand=@brand, Model=@model, EngineNo=@engine, RegisteredDate=@regDate WHERE VehicleRegNo=@id",
                new SQLiteParameter("@brand", entity.Brand),
                new SQLiteParameter("@model", entity.Model),
                new SQLiteParameter("@engine", entity.EngineNo),
                new SQLiteParameter("@regDate", entity.RegisteredDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@id", entity.VehicleRegNo));
        }

        public void Delete(string id)
        {
            DatabaseConnection.ExecuteNonQuery("DELETE FROM Motorbike WHERE VehicleRegNo=@id", new SQLiteParameter("@id", id));
        }

        public Motorbike Search(string id)
        {
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Motorbike WHERE VehicleRegNo=@id", new SQLiteParameter("@id", id));
            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        public List<Motorbike> GetAll()
        {
            var list = new List<Motorbike>();
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Motorbike ORDER BY VehicleRegNo");
            foreach (DataRow row in table.Rows)
                list.Add(MapRow(row));
            return list;
        }

        private static Motorbike MapRow(DataRow row)
        {
            DateTime.TryParse(row["RegisteredDate"].ToString(), out DateTime regDate);
            return new Motorbike
            {
                VehicleRegNo = row["VehicleRegNo"].ToString(),
                Brand = row["Brand"].ToString(),
                Model = row["Model"].ToString(),
                EngineNo = row["EngineNo"].ToString(),
                RegisteredDate = regDate
            };
        }
    }
}
