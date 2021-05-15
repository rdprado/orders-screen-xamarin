using OrdersScreen.Models;
using OrdersScreen.ViewModels;
using OrdersScreen.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OrdersScreen.Mocks
{
    class OrdersViewFeederMock : OrdersViewFeeder
    {
        const int MaxAdds = 10000;
        TimeSpan orderInterval = TimeSpan.FromMilliseconds(50);

        public void Start(OrdersViewModel viewModel)
        {
            int maxAdds = 10000;
            int counter = 0;

            var t = new Task(()=> {
                while (counter++ < maxAdds)
                {
                    Thread.Sleep(orderInterval.Milliseconds);
                    var order = new Order { PlanName = "asfda", Charges = "as", DaysInWeek="40", TotalDays="321" };
                    Application.Current.Dispatcher.BeginInvokeOnMainThread(()=> viewModel.AddOrder(order));
                    
                }
            });

            t.Start();
        }
    }
}
