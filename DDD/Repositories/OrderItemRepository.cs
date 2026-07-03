using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DDD.Database;
using DDD.Models;

namespace DDD.Repositories
{
    /// <summary>
    /// OOP - POLYMORPHISM: implements ICrudRepository&lt;OrderItem&gt;.
    /// OrderItem has a composite primary key (OrderNo + ItemNo), so Search
    /// and Delete accept a single "id" string in the form "OrderNo||ItemNo"
    /// (see <see cref="CombineKey"/>) to stay compatible with the shared
    /// ICrudRepository&lt;T&gt; contract.
    /// </summary>
    public class OrderItemRepository : ICrudRepository<OrderItem>
    {
        private const string KeySeparator = "||";

        public static string CombineKey(string orderNo, string itemNo) => $"{orderNo}{KeySeparator}{itemNo}";

        public void Add(OrderItem entity)
        {
            DatabaseConnection.ExecuteNonQuery(
                "INSERT INTO OrderItem (OrderNo, ItemNo, Quantity, SubTotal) VALUES (@order, @item, @qty, @subTotal)",
                new SQLiteParameter("@order", entity.OrderNo),
                new SQLiteParameter("@item", entity.ItemNo),
                new SQLiteParameter("@qty", entity.Quantity),
                new SQLiteParameter("@subTotal", entity.SubTotal));
        }

        public void Update(OrderItem entity)
        {
            // Update targets the row currently keyed by the entity's own OrderNo/ItemNo values.
            DatabaseConnection.ExecuteNonQuery(
                "UPDATE OrderItem SET Quantity=@qty, SubTotal=@subTotal WHERE OrderNo=@order AND ItemNo=@item",
                new SQLiteParameter("@qty", entity.Quantity),
                new SQLiteParameter("@subTotal", entity.SubTotal),
                new SQLiteParameter("@order", entity.OrderNo),
                new SQLiteParameter("@item", entity.ItemNo));
        }

        /// <summary>Updates a row identified by its original composite key, allowing the key values themselves to change.</summary>
        public void Update(string originalId, OrderItem entity)
        {
            var (originalOrderNo, originalItemNo) = SplitKey(originalId);
            DatabaseConnection.ExecuteNonQuery(
                @"UPDATE OrderItem SET OrderNo=@newOrder, ItemNo=@newItem, Quantity=@qty, SubTotal=@subTotal
                  WHERE OrderNo=@oldOrder AND ItemNo=@oldItem",
                new SQLiteParameter("@newOrder", entity.OrderNo),
                new SQLiteParameter("@newItem", entity.ItemNo),
                new SQLiteParameter("@qty", entity.Quantity),
                new SQLiteParameter("@subTotal", entity.SubTotal),
                new SQLiteParameter("@oldOrder", originalOrderNo),
                new SQLiteParameter("@oldItem", originalItemNo));
        }

        public void Delete(string id)
        {
            var (orderNo, itemNo) = SplitKey(id);
            DatabaseConnection.ExecuteNonQuery(
                "DELETE FROM OrderItem WHERE OrderNo=@order AND ItemNo=@item",
                new SQLiteParameter("@order", orderNo),
                new SQLiteParameter("@item", itemNo));
        }

        public OrderItem Search(string id)
        {
            var (orderNo, itemNo) = SplitKey(id);
            DataTable table = DatabaseConnection.ExecuteQuery(
                "SELECT * FROM OrderItem WHERE OrderNo=@order AND ItemNo=@item",
                new SQLiteParameter("@order", orderNo),
                new SQLiteParameter("@item", itemNo));
            return table.Rows.Count == 0 ? null : MapRow(table.Rows[0]);
        }

        public List<OrderItem> GetAll()
        {
            var list = new List<OrderItem>();
            DataTable table = DatabaseConnection.ExecuteQuery("SELECT * FROM OrderItem ORDER BY OrderNo, ItemNo");
            foreach (DataRow row in table.Rows)
                list.Add(MapRow(row));
            return list;
        }

        private static (string, string) SplitKey(string id)
        {
            string[] parts = (id ?? string.Empty).Split(new[] { KeySeparator }, StringSplitOptions.None);
            return (parts.Length > 0 ? parts[0] : string.Empty, parts.Length > 1 ? parts[1] : string.Empty);
        }

        private static OrderItem MapRow(DataRow row)
        {
            return new OrderItem
            {
                OrderNo = row["OrderNo"].ToString(),
                ItemNo = row["ItemNo"].ToString(),
                Quantity = row["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["Quantity"]),
                SubTotal = row["SubTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SubTotal"])
            };
        }
    }
}
