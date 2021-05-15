﻿using OrdersScreen.Models;
using OrdersScreen.ViewModels;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OrdersScreen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public OrdersViewFeeder ordersViewFeeder;
        public OrdersViewModel vm = new OrdersViewModel();

        public MainPage()
        {
            InitializeComponent();

            BindingContext = vm;

            btnRunApp.Clicked += BtnRunApp_Clicked;
            btnAddOrder.Clicked += BtnAddOrder_Clicked;
            btnUpdateOrder.Clicked += BtnUpdateOrder_Clicked;
        }

        private void BtnRunApp_Clicked(object sender, System.EventArgs e)
        {
            ordersViewFeeder.Start(vm);
        }

        private void BtnUpdateOrder_Clicked(object sender, System.EventArgs e)
        {
            vm.UpdateOrder();
        }

        private void BtnAddOrder_Clicked(object sender, System.EventArgs e)
        {
            vm.AddOrder();
        }
    }
}