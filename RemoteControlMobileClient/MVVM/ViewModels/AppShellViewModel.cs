using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.Pages;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
	public partial class AppShellViewModel : ITransient
	{		
		public AppShellViewModel()
		{
			Routing.RegisterRoute(nameof(AuthorizationPage), typeof(AuthorizationPage));
			Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
			Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
			Routing.RegisterRoute(nameof(ConnectedDevicesPage), typeof(ConnectedDevicesPage));
			Routing.RegisterRoute(nameof(DeviceInfoPage), typeof(DeviceInfoPage));
			Routing.RegisterRoute(nameof(RunningProgramsPage), typeof(RunningProgramsPage));
			Routing.RegisterRoute(nameof(DeviceFolderPage), typeof(DeviceFolderPage));
		}
	}
}
