# orders-screen-xamarin
Xamarin project to test the creation of a cross-platform app for WPF and UWP using a datagrid view with stock market orders. View entries can be add or updates in this example.

1- Layout
The first phase was building a screen with orders in a datagrid style view. Unlike WPF or Windows forms, Xamarin does not have a native datagrid control so the options were to use an existent non native solution versus building one from scratch. SyncFusion claims to have a high performance datagrid view but it has a commercial license. There is also a free project called akgulebubekir/Xamarin.Forms.DataGrid which is not supported by WPF. So it was decided to build a simple one using the ListView control.

![main view](https://user-images.githubusercontent.com/5822726/118692225-31bd9f00-b7e0-11eb-9681-71e4a193a4b9.PNG)
