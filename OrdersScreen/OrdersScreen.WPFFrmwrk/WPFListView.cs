using OrdersScreen.WPFFrmwrk;
using System.Windows.Controls;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(Xamarin.Forms.ListView), typeof(WPFListView))]
namespace OrdersScreen.WPFFrmwrk
{
    class WPFListView : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            foreach (var item in Control.Children)
            {
                if (item is ListView)
                {
                    var list = item as ListView;
                    list.ItemContainerStyle = App.Current.Resources["LvItemStyle"] as System.Windows.Style;
                }
            }
        }
    }
}
