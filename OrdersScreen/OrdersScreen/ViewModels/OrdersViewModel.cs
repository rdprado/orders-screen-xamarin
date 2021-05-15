using OrdersScreen.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OrdersScreen.Utils;

namespace OrdersScreen.ViewModels
{
    public class OrdersViewModel
    {
        public ObservableCollection<Order> orders = new ObservableCollection<Order>();

        public OrdersViewModel()
        {
            Orders.Add(new Order { PlanName = "AAA", Charges = "2", TotalDays = "0", DaysInWeek = "40" });
            Orders.Add(new Order { PlanName = "BBB", Charges = "2", TotalDays = "0", DaysInWeek = "40" });
            Orders.Add(new Order { PlanName = "CCC", Charges = "2", TotalDays = "0", DaysInWeek = "40" });
            Orders.Add(new Order { PlanName = "DDD", Charges = "2", TotalDays = "0", DaysInWeek = "40" });
        }

        public void AddOrder(Order order)
        {
            Orders.Add(order);
        }

        public void AddOrder()
        {
            Orders.Add(new Order { PlanName = "AAA", Charges = "2", TotalDays = "0", DaysInWeek = "40" });
        }

        public void UpdateOrder()
        {
            int index = orders.FindIndex(o => o.PlanName == "AAA");
            if (index > -1)
            {
                var item = orders[index];
                item.Charges = "Smith";
                orders[index] = item;
            }
        }

        public IList<Order> Orders { get => orders; }
    }
}
