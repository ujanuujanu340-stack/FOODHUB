using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DDD.Database;
using DDD.Models;

namespace DDD.Repositories
{
    /// <summary>
    /// OOP - POLYMORPHISM: implements ICrudRepository&lt;Customer&gt; with the
    /// SQL specific to the Customer table. CustomerService (and forms,
    /// indirectly) only ever talk to the ICrudRepository&lt;Customer&gt;
    /// contract, never to this class's SQL directly.
    /// </summary>
    public class CustomerRepository : ICrudRepository<Customer>
    {
        public void Add(Customer entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                @"INSERT INTO Customer (CustomerID, CustomerName, NIC, DateOfBirth, ContactNumber, LocationNo, Lane, Street, City)
                  VALUES (@id, @name, @nic, @dob, @contact, @loc, @lane, @street, @city)",
                new SQLiteParameter("@id", entity.CustomerID),
                new SQLiteParameter("@name", entity.CustomerName),
                new SQLiteParameter("@nic", entity.NIC),
                new SQLiteParameter("@dob", entity.DateOfBirth.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@contact", entity.ContactNumber),
                new SQLiteParameter("@loc", entity.LocationNo),
                new SQLiteParameter("@lane", entity.Lane),
                new SQLiteParameter("@street", entity.Street),
                new SQLiteParameter("@city", entity.City));
        }

        public void Update(Customer entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                @"UPDATE Customer SET CustomerName=@name, NIC=@nic, DateOfBirth=@dob, ContactNumber=@contact,
                  LocationNo=@loc, Lane=@lane, Street=@street, City=@city WHERE CustomerID=@id",
                new SQLiteParameter("@name", entity.CustomerName),
                new SQLiteParameter("@nic", entity.NIC),
                new SQLiteParameter("@dob", entity.DateOfBirth.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@contact", entity.ContactNumber),
                new SQLiteParameter("@loc", entity.LocationNo),
                new SQLiteParameter("@lane", entity.Lane),
                new SQLiteParameter("@street", entity.Street),
                new SQLiteParameter("@city", entity.City),
                new SQLiteParameter("@id", entity.CustomerID));
        }

        public void Delete(string id)
        {
            DatabaseConnection.ExecuteNonQuery("DELETE FROM Customer WHERE CustomerID=@id", new SQLiteParameter("@id", id));
        }

        public Customer Search(string id)
        {
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Customer WHERE CustomerID=@id", new SQLiteParameter("@id", id));
            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        public List<Customer> GetAll()
        {
            var list = new List<Customer>();
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Customer ORDER BY CustomerID");
            foreach (DataRow row in table.Rows)
                list.Add(MapRow(row));
            return list;
        }

        /// <summary>Converts one database row into a Customer object.</summary>
        private static Customer MapRow(DataRow row)
        {
            DateTime.TryParse(row["DateOfBirth"].ToString(), out DateTime dob);
            return new Customer
            {
                CustomerID = row["CustomerID"].ToString(),
                CustomerName = row["CustomerName"].ToString(),
                NIC = row["NIC"].ToString(),
                DateOfBirth = dob,
                ContactNumber = row["ContactNumber"].ToString(),
                LocationNo = row["LocationNo"].ToString(),
                Lane = row["Lane"].ToString(),
                Street = row["Street"].ToString(),
                City = row["City"].ToString()
            };
        }
    }
}
