using Xamarin.Forms;

namespace OrdersScreen.WPFFrmwrk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Forms.Init();
            LoadApplication(new OrdersScreen.App());
        }
    }
}
