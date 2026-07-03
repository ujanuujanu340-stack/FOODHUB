using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DDD.Database;
using DDD.Models;

namespace DDD.Repositories
{
    /// <summary>OOP - POLYMORPHISM: implements ICrudRepository&lt;Rider&gt; with the SQL specific to the Rider table.</summary>
    public class RiderRepository : ICrudRepository<Rider>
    {
        public void Add(Rider entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                @"INSERT INTO Rider (EmployeeNo, FirstName, MiddleName, LastName, NIC, DateOfBirth, ContactNumber, LicenseNumber, Address, Age)
                  VALUES (@id, @first, @middle, @last, @nic, @dob, @contact, @license, @address, @age)",
                new SQLiteParameter("@id", entity.EmployeeNo),
                new SQLiteParameter("@first", entity.FirstName),
                new SQLiteParameter("@middle", entity.MiddleName),
                new SQLiteParameter("@last", entity.LastName),
                new SQLiteParameter("@nic", entity.NIC),
                new SQLiteParameter("@dob", entity.DateOfBirth.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@contact", entity.ContactNumber),
                new SQLiteParameter("@license", entity.LicenseNumber),
                new SQLiteParameter("@address", entity.Address),
                new SQLiteParameter("@age", (object)entity.Age ?? DBNull.Value));
        }

        public void Update(Rider entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                @"UPDATE Rider SET FirstName=@first, MiddleName=@middle, LastName=@last, NIC=@nic, DateOfBirth=@dob,
                  ContactNumber=@contact, LicenseNumber=@license, Address=@address, Age=@age WHERE EmployeeNo=@id",
                new SQLiteParameter("@first", entity.FirstName),
                new SQLiteParameter("@middle", entity.MiddleName),
                new SQLiteParameter("@last", entity.LastName),
                new SQLiteParameter("@nic", entity.NIC),
                new SQLiteParameter("@dob", entity.DateOfBirth.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@contact", entity.ContactNumber),
                new SQLiteParameter("@license", entity.LicenseNumber),
                new SQLiteParameter("@address", entity.Address),
                new SQLiteParameter("@age", (object)entity.Age ?? DBNull.Value),
                new SQLiteParameter("@id", entity.EmployeeNo));
        }

        public void Delete(string id)
        {
            DatabaseConnection.ExecuteNonQuery("DELETE FROM Rider WHERE EmployeeNo=@id", new SQLiteParameter("@id", id));
        }

        public Rider Search(string id)
        {
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Rider WHERE EmployeeNo=@id", new SQLiteParameter("@id", id));
            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        public List<Rider> GetAll()
        {
            var list = new List<Rider>();
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Rider ORDER BY EmployeeNo");
            foreach (DataRow row in table.Rows)
                list.Add(MapRow(row));
            return list;
        }

        private static Rider MapRow(DataRow row)
        {
            DateTime.TryParse(row["DateOfBirth"].ToString(), out DateTime dob);
            int? age = row["Age"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["Age"]);
            return new Rider
            {
                EmployeeNo = row["EmployeeNo"].ToString(),
                FirstName = row["FirstName"].ToString(),
                MiddleName = row["MiddleName"].ToString(),
                LastName = row["LastName"].ToString(),
                NIC = row["NIC"].ToString(),
                DateOfBirth = dob,
                ContactNumber = row["ContactNumber"].ToString(),
                LicenseNumber = row["LicenseNumber"].ToString(),
                Address = row["Address"].ToString(),
                Age = age
            };
        }
    }
}
