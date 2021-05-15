using OrdersScreen.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OrdersScreen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrdersHistoryView : ContentView
    {
        public OrdersViewModel vm  = new OrdersViewModel();

        public OrdersHistoryView()
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}