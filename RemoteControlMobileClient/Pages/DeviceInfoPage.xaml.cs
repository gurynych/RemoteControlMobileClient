using DevExpress.Maui.Controls;

namespace RemoteControlMobileClient.Pages;

public partial class DeviceInfoPage : ContentPage
{
	public DeviceInfoPage()
	{
		InitializeComponent();
	}

	private async void TabView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (sender is not TabView) return;

		TabView tabView = sender as TabView;
		if (tabView.SelectedItem is TabViewItem item && item.BindingContext is Element element)
		{
			await ScrollView.ScrollToAsync(element, ScrollToPosition.Start, true);
		}
	}
}