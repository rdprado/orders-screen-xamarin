using OrdersScreen.Mocks;
using OrdersScreen.Views;
using Xamarin.Forms;

namespace OrdersScreen
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            var mainPage = new MainPage();
            MainPage = mainPage;
            mainPage.ordersViewFeeder = new OrdersViewFeederMock();
        }
    }
}
