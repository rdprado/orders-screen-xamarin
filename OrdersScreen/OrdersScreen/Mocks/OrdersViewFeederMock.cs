using OrdersScreen.Models;
using OrdersScreen.ViewModels;
using OrdersScreen.Views;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Timers;

namespace OrdersScreen.Mocks
{
    class OrdersViewFeederMock : OrdersViewFeeder
    {
        bool useIncreasedLoad = true;

        public List<string> fakeSymbols 
            = new List<string> { "PETR54", "ABSD11", "HGTD40", "UBDF4", "UTTO2", "OMGR4", "HTT13" };

        public List<string> fakeOrdTypes
            = new List<string> { "1", "2", "3", "C"};

        private const int maxIterations = 10000000;
        private const int maxAdds = 1000;
        private TimeSpan orderInterval = TimeSpan.FromMilliseconds(50);

        private readonly Random random = new Random();
        int currentOrderId = 0;

        private static System.Timers.Timer increaseLoadTimer;

        public void SetupIncreasedLoadSimulation(OrdersViewModel ordersVM)
        {
            //increaseLoadTimer = new System.Timers.Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);
            //// Hook up the Elapsed event for the timer. 
            //increaseLoadTimer.Elapsed += (object source, ElapsedEventArgs args) => {
            //    if (int.Parse(ordersVM.Count) > 100)
            //    {
            //        for (int i = 0; i < 90; i++)
            //        {
            //            SimulateUpdateOrder(ordersVM);
            //        }
            //    }
            //};
            //increaseLoadTimer.AutoReset = true;
            //increaseLoadTimer.Enabled = true;
        }

        public void Start(OrdersViewModel ordersVM)
        {
            int iterationsCount = 0;
            int addsCount = 0;

            var t = new Task(()=> {
                while (iterationsCount++ < maxIterations)
                {
                    Thread.Sleep(orderInterval.Milliseconds);

                    if (addsCount < maxAdds) // there is a limit to add to avoid full memory consumption
                    {
                        // Add and/or update

                        var chanceToCreate = random.Next(1, 10);
                        if (chanceToCreate % 5 == 1)
                        {
                            // add
                            SimulateAddOrder(ordersVM);

                            if (chanceToCreate <= 5)
                            {
                                // and update
                                SimulateUpdateOrder(ordersVM);
                            }

                            addsCount++;
                        }
                        else
                        {
                            // or update
                            SimulateUpdateOrder(ordersVM);
                        }
                    }
                    else
                    {
                        // can only update

                        SimulateUpdateOrder(ordersVM);
                    }
                }
            });

            t.Start();

            if (useIncreasedLoad)
            {
                SetupIncreasedLoadSimulation(ordersVM);
            }
        }

        private int SimulateAddOrder(OrdersViewModel viewModel)
        {
            var totalValue = random.Next(1, 10);

            var order = new Order
            {
                Id = currentOrderId++.ToString(),
                CreationDate = DateTime.Now,
                OrdType = fakeOrdTypes[random.Next(0, fakeOrdTypes.Count - 1)],
                Symbol = fakeSymbols[random.Next(0, fakeSymbols.Count - 1)],
                Account = random.Next(0, 40000).ToString(),
                OrdQty = random.Next(1, 10000),
                TotalValue = totalValue,
                AvailableValue = totalValue,
                GoalValue = totalValue + random.Next(1, 10),
            };

            var vmOrder = PresentModelToVM(order);

            Application.Current.Dispatcher.BeginInvokeOnMainThread(() => viewModel.AddOrder(vmOrder));
            return currentOrderId;
        }

        private OrderViewModel PresentModelToVM(Order order)
        {
            var vm = new OrderViewModel
            {
                Id = order.Id,
                CreationDate = order.CreationDate.ToString(),
                Account = order.Account,
                Symbol = order.Symbol,
                OrdType = order.OrdType,
                OrdQty = order.OrdQty.ToString(),
                CumQty = order.CumQty.ToString(),
                LeavesQty = order.LeavesQty.ToString(),
                TotalValue = order.TotalValue.ToString(),
                AvailableValue = order.AvailableValue.ToString(),
                GoalValue = order.GoalValue.ToString()
            };

            return vm;
        }

        private void SimulateUpdateOrder(OrdersViewModel ordersViewModel)
        {
            if (currentOrderId <= 0 )
            {
                // at least one order must be created for the update
                return;
            }

            // ---------------------------------------------------
            // In a non test mode we would receive the data of the 
            // updated order from the logic/model layer with the 
            // values updated with the correct business logic.
            // For testing purposes get the order state directly
            // from the view model list and update the values with 
            // a dummy logic.
            // ---------------------------------------------------
            var randIndexToUpdate = random.Next(0, currentOrderId - 1);

            var orderVM = ordersViewModel.GetOrderVMByIndex(randIndexToUpdate);
            var cumQtyToUpdate = int.Parse(orderVM.CumQty);
            cumQtyToUpdate = int.Parse(orderVM.LeavesQty) > 0 ? cumQtyToUpdate + 1 : cumQtyToUpdate;
            orderVM.CumQty = cumQtyToUpdate.ToString();
            orderVM.LeavesQty = (int.Parse(orderVM.OrdQty) - cumQtyToUpdate).ToString();
            orderVM.TotalValue = random.Next(0, 1000).ToString();
            orderVM.AvailableValue = random.Next(0, 1000).ToString();
            orderVM.GoalValue = random.Next(0, 1000).ToString();
            Application.Current.Dispatcher.BeginInvokeOnMainThread(() => orderVM.Update());
        }


    }
}
