using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OrdersScreen.Utils;

namespace OrdersScreen.ViewModels
{
    public class OrdersViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<OrderViewModel> orders = new ObservableCollection<OrderViewModel>();
        public event PropertyChangedEventHandler PropertyChanged;

        public OrdersViewModel()
        {
            Orders.Add(new OrderViewModel { Id = "AAA", CreationDate = "25/05/2020 19:40:56", Account = "333333", Symbol = "HGLG45", OrdType="C"  , OrdQty = "1", LeavesQty = "1", CumQty ="0", AvailableValue="7,00", TotalValue = "7,00", GoalValue="9,00" });
            Orders.Add(new OrderViewModel { Id = "BBB", CreationDate = "25/05/2020 19:41:56", Account = "333333", Symbol = "HGLG45", OrdType = "C", OrdQty = "1", LeavesQty = "1",   CumQty = "0", AvailableValue = "7,00", TotalValue = "7,00", GoalValue = "9,00" });
            Orders.Add(new OrderViewModel { Id = "CCC", CreationDate = "25/05/2020 19:42:56", Account = "333333", Symbol = "HGLG45", OrdType = "C", OrdQty = "1", LeavesQty = "1",   CumQty = "0", AvailableValue = "7,00", TotalValue = "7,00", GoalValue = "9,00" });
            Orders.Add(new OrderViewModel { Id = "DDD", CreationDate = "25/05/2020 19:43:56", Account = "333333", Symbol = "HGLG45", OrdType = "C", OrdQty = "1", LeavesQty = "1",   CumQty = "0", AvailableValue = "7,00", TotalValue="7,00", GoalValue = "9,00" });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null ){
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        public void AddOrder(OrderViewModel order)
        {
            Orders.Add(order);
            this.OnPropertyChanged("Count");
        }

        public void AddOrder()
        {
            Orders.Add(new OrderViewModel { Id = "AAA", CreationDate = "2", Account = "0", Symbol = "40" });
            this.OnPropertyChanged("Count");
        }

        public void UpdateOrder()
        {
            int index = orders.FindIndex(o => o.Id == "AAA");
            if (index > -1)
            {
                var item = orders[index];
                item.CreationDate = "Smith";
                orders[index] = item;
            }
        }

        //public void UpdateOrder(OrderViewModel order)
        //{
        //    order.Update();
        //}

        internal OrderViewModel GetOrderVMByIndex(int index)
        {
            return orders[index];

            //return new OrderViewModel
            //{
            //    Id              = vm.Id,
            //    CreationDate    = vm.CreationDate,
            //    Advisor         = vm.Advisor,
            //    Account         = vm.Account,
            //    Symbol          = vm.Symbol,
            //    OrdType         = vm.OrdType,
            //    OrdQty          = vm.OrdQty,
            //    OrdQtyApparent  = vm.OrdQtyApparent,
            //    LeavesQty       = vm.LeavesQty,
            //    CancelledQty    = vm.CancelledQty,
            //    CumQty          = vm.CumQty,
            //    TotalValue      = vm.TotalValue,
            //    AvailableValue  = vm.AvailableValue,
            //    GoalValue       = vm.GoalValue,
            //};
        }

        public IList<OrderViewModel> Orders { get => orders; }

        public string Count { get => orders.Count.ToString(); }
    }
}
