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
        public OrdersViewFeeder ordersViewFeeder;
        public OrdersViewModel vm = new OrdersViewModel();

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = vm;

            btnRunApp.Clicked += (object sender, System.EventArgs e) => ordersViewFeeder.Start(vm);
        }
    }
}