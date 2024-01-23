using CommunityToolkit.Mvvm.Input;
using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.Pages;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
    internal partial class AppShellViewModel : ITransient
    {
        public AppShellViewModel()
        {
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));          
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(ConnectedDevicesPage), typeof(ConnectedDevicesPage));
            Routing.RegisterRoute(nameof(NestedFilesInDirectoryPage), typeof(NestedFilesInDirectoryPage));
        }

        [RelayCommand]
        public Task SetStartUpPage()
        {
            /*RequestPermissionsService requestPermissionsService = new RequestPermissionsService();
            bool accept = false;
            while (!accept)
            {
                accept = requestPermissionsService.RequestPermission();
            }*/

            //TODO: добавить проверку токена
            if (false)
            {
                return Shell.Current.GoToAsync(nameof(MainPage));
            }

            return Task.CompletedTask;
        }
    }
}
