using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RemoteControlMobileClient.BusinessLogic.Models;
using RemoteControlMobileClient.BusinessLogic.Services;
using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.Pages;
using Device = RemoteControlMobileClient.BusinessLogic.Models.Device;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
    [QueryProperty(nameof(User), "User")]
    //[QueryProperty(nameof(ConnectedDevices), "ConnectedDevices")]
    internal partial class ConnectedDeviceViewModel : ObservableObject, ITransient
    {
        private readonly ServerAPIProviderService apiProvider;

        [ObservableProperty]
        private User user;

        [ObservableProperty]
        private List<Device> connectedDevices;

        [ObservableProperty]
        private bool isBusy;

        public ConnectedDeviceViewModel(ServerAPIProviderService apiProvider)
        {
            this.apiProvider = apiProvider;
        }

        [RelayCommand]
        private async Task LoadConnectedDevices()
        {
            IsBusy = true;
            var tokenSource = new CancellationTokenSource(10000);
            ConnectedDevices = await apiProvider.GetConnectedDeviceAsync(User, tokenSource.Token);
            if (ConnectedDevices == null)
            {
            }

            IsBusy = false;

            //return Task.CompletedTask;
        }

        [RelayCommand]
        private Task GoToMainPage()
        {
            return Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task GetNestedFilesInDirectory(Device device)
        {
            return Shell.Current.GoToAsync(nameof(NestedFilesInDirectoryPage), new Dictionary<string, object>()
            { 
                { "User", User },
                { "Device", device }
            });
        }
    }
}
