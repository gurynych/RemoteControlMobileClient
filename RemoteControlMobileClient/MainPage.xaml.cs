using CommunityToolkit.Maui.Alerts;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;

namespace RemoteControlMobileClient
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

		private async void ContentPage_Loaded(object sender, EventArgs e)
		{
			RequestPermissionsService requestPermissionsService = new RequestPermissionsService();
			bool accept = await requestPermissionsService.RequestPermission();
			/*if (!accept)
			{
				await Toast.Make("Для продолжения необходимо выдать разрешение. Приложение будет закрыто", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
				await Task.Delay(1500);
				Environment.Exit(0);
*//*
				accept = await requestPermissionsService.RequestPermission();6

				if (!accept)
				{
					await Toast.Make("Необходимо выдать разрешение. Приложение будет закрыто", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
					Environment.Exit(0);
				}*//*
			}*/
		}
    }
}