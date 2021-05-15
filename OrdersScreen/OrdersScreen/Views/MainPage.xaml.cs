using OrdersScreen.Models;
using OrdersScreen.ViewModels;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OrdersScreen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            btnAddOrder.Clicked += BtnAddOrder_Clicked;
            btnUpdateOrder.Clicked += BtnUpdateOrder_Clicked;
        }

        private void BtnUpdateOrder_Clicked(object sender, System.EventArgs e)
        {
            ordersHistoryView.vm.UpdateOrder();
        }

        private void BtnAddOrder_Clicked(object sender, System.EventArgs e)
        {
            ordersHistoryView.vm.AddOrder();
        }
    }
}