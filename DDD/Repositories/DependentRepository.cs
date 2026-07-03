using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DDD.Database;
using DDD.Models;

namespace DDD.Repositories
{
    /// <summary>OOP - POLYMORPHISM: implements ICrudRepository&lt;Dependent&gt; with the SQL specific to the Dependent table.</summary>
    public class DependentRepository : ICrudRepository<Dependent>
    {
        public void Add(Dependent entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                @"INSERT INTO Dependent (DependentID, EmployeeNo, DependentName, Relationship, DateOfBirth)
                  VALUES (@id, @empNo, @name, @rel, @dob)",
                new SQLiteParameter("@id", entity.DependentID),
                new SQLiteParameter("@empNo", entity.EmployeeNo),
                new SQLiteParameter("@name", entity.DependentName),
                new SQLiteParameter("@rel", entity.Relationship),
                new SQLiteParameter("@dob", entity.DateOfBirth.ToString("yyyy-MM-dd")));
        }

        public void Update(Dependent entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "UPDATE Dependent SET EmployeeNo=@empNo, DependentName=@name, Relationship=@rel, DateOfBirth=@dob WHERE DependentID=@id",
                new SQLiteParameter("@empNo", entity.EmployeeNo),
                new SQLiteParameter("@name", entity.DependentName),
                new SQLiteParameter("@rel", entity.Relationship),
                new SQLiteParameter("@dob", entity.DateOfBirth.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@id", entity.DependentID));
        }

        public void Delete(string id)
        {
            DatabaseConnection.ExecuteNonQuery("DELETE FROM Dependent WHERE DependentID=@id", new SQLiteParameter("@id", id));
        }

        public Dependent Search(string id)
        {
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Dependent WHERE DependentID=@id", new SQLiteParameter("@id", id));
            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        public List<Dependent> GetAll()
        {
            var list = new List<Dependent>();
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Dependent ORDER BY DependentID");
            foreach (DataRow row in table.Rows)
                list.Add(MapRow(row));
            return list;
        }

        private static Dependent MapRow(DataRow row)
        {
            DateTime.TryParse(row["DateOfBirth"].ToString(), out DateTime dob);
            return new Dependent
            {
                DependentID = row["DependentID"].ToString(),
                EmployeeNo = row["EmployeeNo"].ToString(),
                DependentName = row["DependentName"].ToString(),
                Relationship = row["Relationship"].ToString(),
                DateOfBirth = dob
            };
        }
    }
}
