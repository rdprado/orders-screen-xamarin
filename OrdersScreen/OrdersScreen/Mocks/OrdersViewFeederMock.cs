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
        private const int MaxAdds = 10000;
        private TimeSpan orderInterval = TimeSpan.FromMilliseconds(50);
        private readonly Random random = new Random();

        public void Start(OrdersViewModel viewModel)
        {
            int maxAdds = 10000;
            int counter = 0;
            int orderId = 0;

            var t = new Task(()=> {
                while (counter++ < maxAdds)
                {
                    var chanceToUpdateOrCreate = random.Next(1, 10);
                    Thread.Sleep(orderInterval.Milliseconds);

                    if (chanceToUpdateOrCreate % 2 == 1)
                    {
                        var order = new Order { 
                            PlanName = orderId++.ToString(),
                            Charges = "as",
                            DaysInWeek = "40",
                            TotalDays = "321" 
                        };

                            Application.Current.Dispatcher.BeginInvokeOnMainThread(() => viewModel.AddOrder(order));
                    }
                    else
                    {
                        if (orderId - 1 > 0)
                        {
                            var randIndexToUpdate = random.Next(0, orderId - 1);
                            Application.Current.Dispatcher.BeginInvokeOnMainThread(
                                () => viewModel.UpdateOrder(new Order {
                                    PlanName = randIndexToUpdate.ToString(),
                                    Charges = "DESAFIO",
                                    DaysInWeek = "10",
                                    TotalDays = "DESAFIO"
                                }));
                        }
                    }
                }
            });

            t.Start();
        }
    }
}
