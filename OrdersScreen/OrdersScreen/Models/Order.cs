using System;

namespace OrdersScreen.Models
{
    public class Order
    {
        public string Id { get; set; } = "";
        public DateTime CreationDate { get; set; }
        public string Advisor { get; set; } = "";
        public string Account { get; set; } = "";
        public string Symbol { get; set; } = "";
        public string OrdType { get; set; } = "";
        public int OrdQty { get; set; } = 0;
        public int OrdQtyApparent { get; set; } = 0;
        public int LeavesQty { get => OrdQty - CumQty; }
        public int CancelledQty { get; set; }
        public int CumQty { get; set; } = 0;
        public decimal TotalValue { get; set; } = 0;
        public decimal AvailableValue { get; set; } = 0;
        public decimal GoalValue { get; set; } = 0;
    }
}
