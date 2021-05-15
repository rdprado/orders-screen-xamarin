using OrdersScreen.Models;
using System.Collections.Generic;

namespace OrdersScreen.ViewModels
{
    public class OrdersViewModel
    {
        public OrdersViewModel()
        {
            Orders = new List<Order>();
            Orders.Add(new Order { PlanName = "AAA", Charges = "2", TotalDays = "0", DaysInWeek = "40" });
            Orders.Add(new Order { PlanName = "BBB", Charges = "2", TotalDays = "0", DaysInWeek = "40" });
            Orders.Add(new Order { PlanName = "CCC", Charges = "2", TotalDays = "0", DaysInWeek = "40" });
            Orders.Add(new Order { PlanName = "DDD", Charges = "2", TotalDays = "0", DaysInWeek = "40" });
        }

        public IList<Order> Orders { get; set; }
    }
}
