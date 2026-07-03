using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DDD.Database;
using DDD.Models;

namespace DDD.Repositories
{
    /// <summary>OOP - POLYMORPHISM: implements ICrudRepository&lt;Order&gt; with the SQL specific to the Orders table.</summary>
    public class OrderRepository : ICrudRepository<Order>
    {
        public void Add(Order entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                @"INSERT INTO Orders (OrderNo, CustomerID, OrderDate, OrderTime, OrderStatus, PaymentMethod, OrderAmount, DispatchedTime)
                  VALUES (@id, @customer, @orderDate, @orderTime, @status, @payment, @amount, @dispatched)",
                new SQLiteParameter("@id", entity.OrderNo),
                new SQLiteParameter("@customer", entity.CustomerID),
                new SQLiteParameter("@orderDate", entity.OrderDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@orderTime", entity.OrderTime.ToString(@"hh\:mm\:ss")),
                new SQLiteParameter("@status", entity.OrderStatus),
                new SQLiteParameter("@payment", entity.PaymentMethod),
                new SQLiteParameter("@amount", entity.OrderAmount),
                new SQLiteParameter("@dispatched", entity.DispatchedTime.HasValue ? (object)entity.DispatchedTime.Value.ToString(@"hh\:mm\:ss") : DBNull.Value));
        }

        public void Update(Order entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                @"UPDATE Orders SET CustomerID=@customer, OrderDate=@orderDate, OrderTime=@orderTime, OrderStatus=@status,
                  PaymentMethod=@payment, OrderAmount=@amount, DispatchedTime=@dispatched WHERE OrderNo=@id",
                new SQLiteParameter("@customer", entity.CustomerID),
                new SQLiteParameter("@orderDate", entity.OrderDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@orderTime", entity.OrderTime.ToString(@"hh\:mm\:ss")),
                new SQLiteParameter("@status", entity.OrderStatus),
                new SQLiteParameter("@payment", entity.PaymentMethod),
                new SQLiteParameter("@amount", entity.OrderAmount),
                new SQLiteParameter("@dispatched", entity.DispatchedTime.HasValue ? (object)entity.DispatchedTime.Value.ToString(@"hh\:mm\:ss") : DBNull.Value),
                new SQLiteParameter("@id", entity.OrderNo));
        }

        public void Delete(string id)
        {
            DatabaseConnection.ExecuteNonQuery("DELETE FROM Orders WHERE OrderNo=@id", new SQLiteParameter("@id", id));
        }

        public Order Search(string id)
        {
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Orders WHERE OrderNo=@id", new SQLiteParameter("@id", id));
            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        public List<Order> GetAll()
        {
            var list = new List<Order>();
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM Orders ORDER BY OrderNo");
            foreach (DataRow row in table.Rows)
                list.Add(MapRow(row));
            return list;
        }

        private static Order MapRow(DataRow row)
        {
            DateTime.TryParse(row["OrderDate"].ToString(), out DateTime orderDate);
            TimeSpan.TryParse(row["OrderTime"].ToString(), out TimeSpan orderTime);
            TimeSpan? dispatchedTime = TimeSpan.TryParse(row["DispatchedTime"].ToString(), out TimeSpan dt) ? dt : (TimeSpan?)null;

            return new Order
            {
                OrderNo = row["OrderNo"].ToString(),
                CustomerID = row["CustomerID"].ToString(),
                OrderDate = orderDate,
                OrderTime = orderTime,
                OrderStatus = row["OrderStatus"].ToString(),
                PaymentMethod = row["PaymentMethod"].ToString(),
                OrderAmount = row["OrderAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(row["OrderAmount"]),
                DispatchedTime = dispatchedTime
            };
        }
    }
}
