using System;

namespace DDD.Models
{
    /// <summary>OOP - CLASS/OBJECT: represents one row of the Orders table. CustomerID links back to a Customer.</summary>
    public class Order
    {
        public string OrderNo { get; set; }
        public string CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public TimeSpan OrderTime { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentMethod { get; set; }
        public decimal OrderAmount { get; set; }
        public TimeSpan? DispatchedTime { get; set; }
    }
}
