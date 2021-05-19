using OrdersScreen.Models;
using OrdersScreen.ViewModels;
using OrdersScreen.Views;
using System;
using System.Collections.Generic;
using System.Timers;
using Xamarin.Forms;

namespace OrdersScreen.Mocks
{
    class OrdersViewFeederMock : OrdersViewFeeder
    {
        #region settings

        bool useIncreasedLoad = true;
        private const int maxAdds = 1000;
        private const float increaseLoadFactor = 0.3f;
        private TimeSpan orderInterval = TimeSpan.FromMilliseconds(50);
        private TimeSpan increasedLoadInterval = TimeSpan.FromSeconds(10);

        #endregion

        #region fakedata

        public List<string> fakeSymbols 
            = new List<string> { "PETR54", "ABSD11", "HGTD40", "UBDF4", "UTTO2", "OMGR4", "HTT13" };

        public List<string> fakeOrdTypes
            = new List<string> { "1", "2", "3", "C"};

        #endregion
        
        private readonly Random random = new Random();
        private int currentOrderId = 0;
        private static Timer timer;

        public void Start(OrdersViewModel ordersVM)
        {
            int addsCount = 0;
            double increaseLoadTimeBank = 0;

            timer = new System.Timers.Timer(orderInterval.TotalMilliseconds);
            timer.Elapsed += (object source, ElapsedEventArgs args) =>
            {
                if (useIncreasedLoad)
                {
                    increaseLoadTimeBank += orderInterval.TotalMilliseconds;
                    if (increaseLoadTimeBank > increasedLoadInterval.TotalMilliseconds)
                    {
                        // reset so the increased load only happens again after n milliseconds
                        increaseLoadTimeBank = 0; 

                        TryToIncreaseLoad(ordersVM);
                    }
                }

                if (addsCount < maxAdds) // limit adds to avoid memory consumption with ListView cells
                {
                    SimulateAddAndOrUpdateOrder(ordersVM, ref addsCount);
                }
                else
                {
                    SimulateUpdateOrder(ordersVM);
                }
            };
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void TryToIncreaseLoad(OrdersViewModel ordersVM)
        {
            var ordersCount = int.Parse(ordersVM.OrdersCount);
            if (ordersCount > 100)
            {
                var numUpdates = ordersCount * increaseLoadFactor;
                for (int i = 0; i < numUpdates; i++)
                {
                    SimulateUpdateOrder(ordersVM);
                }
            }
        }

        private int SimulateAddAndOrUpdateOrder(OrdersViewModel ordersVM, ref int addsCount)
        {
            // Add and/or update

            var chanceToAdd = random.Next(1, 10);
            if (chanceToAdd % 2 == 0)
            {
                // add
                SimulateAddOrder(ordersVM);
                addsCount++;

                if (chanceToAdd <= 5)
                {
                    // and update
                    SimulateUpdateOrder(ordersVM);
                }
            }
            else
            {
                // or just update
                SimulateUpdateOrder(ordersVM);
            }

            return addsCount;
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
                CreationDate = order.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),
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
            
            // Choose a random order to update...
            var randIndexToUpdate = random.Next(0, currentOrderId - 1);
            var orderVM = ordersViewModel.GetOrderVMByIndex(randIndexToUpdate); // O(1)

            // but simulate as if we had to find the order using the ID
            orderVM = ordersViewModel.GetOrderVMByID(orderVM.Id);

            var cumQtyToUpdate = int.Parse(orderVM.CumQty);
            cumQtyToUpdate = int.Parse(orderVM.LeavesQty) > 0 ? cumQtyToUpdate + 1 : cumQtyToUpdate;
            orderVM.CumQty = cumQtyToUpdate.ToString();
            orderVM.LeavesQty = (int.Parse(orderVM.OrdQty) - cumQtyToUpdate).ToString();
            
            // do some random chance to update the following fields
            var chanceToUpdateValues = random.Next(1, 10);
            if (chanceToUpdateValues % 2 == 0)
            {
                orderVM.TotalValue = random.Next(0, 1000).ToString();
            }
            if (chanceToUpdateValues % 3 == 0)
            {
                orderVM.AvailableValue = random.Next(0, 1000).ToString();
            }
            orderVM.GoalValue = random.Next(0, 1000).ToString();
            Application.Current.Dispatcher.BeginInvokeOnMainThread(() => orderVM.Update());
        }


    }
}
