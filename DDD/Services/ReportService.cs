using System;
using System.Data;
using DDD.Database;

namespace DDD.Services
{
    /// <summary>
    /// Report queries combine/aggregate data across several tables, so they
    /// don't map cleanly onto a single model class. This service keeps that
    /// aggregate SQL out of ReportsForm, which should only handle UI work.
    /// </summary>
    public class ReportService
    {
        public int GetTotalCustomers() => Convert.ToInt32(DatabaseConnection.ExecuteScalar("SELECT COUNT(*) FROM Customer"));

        public int GetTotalOrders() => Convert.ToInt32(DatabaseConnection.ExecuteScalar("SELECT COUNT(*) FROM Orders"));

        public int GetTotalRiders() => Convert.ToInt32(DatabaseConnection.ExecuteScalar("SELECT COUNT(*) FROM Rider"));

        public decimal GetTotalRevenue()
        {
            object result = DatabaseConnection.ExecuteScalar("SELECT SUM(OrderAmount) FROM Orders");
            return (result == null || result == DBNull.Value) ? 0 : Convert.ToDecimal(result);
        }

        public DataTable GetAllOrdersWithCustomerName()
        {
            return DatabaseConnection.ExecuteQuery(
                @"SELECT o.OrderNo, o.CustomerID, c.CustomerName, o.OrderDate, o.OrderTime,
                         o.OrderStatus, o.PaymentMethod, o.OrderAmount, o.DispatchedTime
                  FROM Orders o
                  LEFT JOIN Customer c ON o.CustomerID = c.CustomerID
                  ORDER BY o.OrderDate DESC, o.OrderNo");
        }

        public DataTable GetOrdersByStatus()
        {
            return DatabaseConnection.ExecuteQuery(
                @"SELECT OrderStatus, COUNT(*) AS OrderCount, SUM(OrderAmount) AS TotalAmount
                  FROM Orders GROUP BY OrderStatus ORDER BY OrderCount DESC");
        }

        public DataTable GetRevenueByPaymentMethod()
        {
            return DatabaseConnection.ExecuteQuery(
                @"SELECT PaymentMethod, COUNT(*) AS OrderCount, SUM(OrderAmount) AS TotalAmount
                  FROM Orders GROUP BY PaymentMethod ORDER BY TotalAmount DESC");
        }

        public DataTable GetFoodItemSalesSummary()
        {
            return DatabaseConnection.ExecuteQuery(
                @"SELECT f.ItemNo, f.ItemName, f.ItemCategory, f.Price,
                         COALESCE(SUM(oi.Quantity), 0) AS TotalQuantitySold,
                         COALESCE(SUM(oi.SubTotal), 0) AS TotalRevenue
                  FROM FoodItem f
                  LEFT JOIN OrderItem oi ON f.ItemNo = oi.ItemNo
                  GROUP BY f.ItemNo, f.ItemName, f.ItemCategory, f.Price
                  ORDER BY TotalRevenue DESC");
        }
    }
}
