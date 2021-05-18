# orders-screen-xamarin
Xamarin project to test the creation of a cross-platform app for WPF and UWP using a datagrid view with stock market orders. View entries can be added or updated in this example.

## 1- Layout ##
The first phase was building a screen with orders in a datagrid style view. Unlike WPF or Windows forms, Xamarin does not have a native datagrid control so the options were to use an existent non native solution versus building one from scratch. SyncFusion claims to have a high performance datagrid view but it has a commercial license. There is also a free project called akgulebubekir/Xamarin.Forms.DataGrid which is not supported by WPF. So it was decided to build a simple one using the ListView control.

![main view](https://user-images.githubusercontent.com/5822726/118692225-31bd9f00-b7e0-11eb-9681-71e4a193a4b9.PNG)

In the end, the visual was as expected. A fix was needed in the WPF project App.xaml to remove the undesired left margin for the listViewItems. Also the listview horizontal size was fixed because a bug was identified in UWP when manually resizing the window while adding and updating the listview. It could be fixed just for UWP. The challenge here with a listview was to make the header columns the same size as the listview item columns. To simplify the width of the cells were set to a fixed width.

## 2- Mock to load the view with entries ##
For this phase it was created an interface called OrdersScreenFeeder and a mock implementation to feed the view. This way it was possible for the Xamarin's App.xaml to inject the mock in the view and in the future it would be possible to inject real services to fetch or receive data. Also the mock could be in different projects. 

The MVVM pattern was used as recommended in https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/data-and-databinding with the listview itemsource binded to an ObservableCollection to automatically update the view when rows were added or removed. The ordersviewmodel collection has order viewmodels in this example. It could have the order model directly, but it was just a project decision to keep the model layer separate from view.

The created mock uses a timer and every 50ms adds an order in the observable collection or updates one order or both. And every 10 seconds simualtes a higher load of updates -- 30% of the existing orders are updated.

## 3- Performance and memory consumption ##
The first impressions were that the UWP application was performing better than the WPF one while rows were being added and/or updated. When dragging or scrolling the screen the UWP app was working smoothly and the WPF app freezing a bit. Also examining the memory profiler it was possible to notice a big increase of memory while the rows were being added.
It was also noticed that with a limit for the order creation, after all orders were created, the WPF app performance would be good again what was an indication of the order creation being a bottleneck.

There is one thread for the mock timer -- adding and/or updating orders -- and the UI thread. Since updating didn't seem to overload de UI thread, the BeginInvoke calls to execute the add or update operations in the UI thread didn't look to be a problem, as sometimes it can be. Below are the results and analysis.

Orders: 1000
Updates: infinite
Space for order rows : 32


