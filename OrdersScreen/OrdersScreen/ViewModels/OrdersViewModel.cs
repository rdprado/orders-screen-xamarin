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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        public void AddOrder(OrderViewModel order)
        {
            Orders.Add(order);
            this.OnPropertyChanged("OrdersCount");
        }

        internal OrderViewModel GetOrderVMByIndex(int index)
        {
            return orders[index];
        }

        public OrderViewModel GetOrderVMByID(string id)
        {
            int index = orders.FindIndex(o => o.Id == id);
            if (index > -1)
            {
                return orders[index];
            }
            else
            {
                return default;
            }
        }


        public IList<OrderViewModel> Orders { get => orders; }

        public string OrdersCount { get => orders.Count.ToString(); }
    }
}