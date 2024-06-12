using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RemoteControlMobileClient.BusinessLogic.DTO;
using RemoteControlMobileClient.BusinessLogic.Services;
using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.Pages;
using DeviceDTO = RemoteControlMobileClient.BusinessLogic.DTO.DeviceDTO;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
    [QueryProperty(nameof(User), "User")]
    //[QueryProperty(nameof(ConnectedDevices), "ConnectedDevices")]
    public partial class ConnectedDeviceViewModel : ObservableObject, ITransient
    {
        private readonly ServerAPIProvider apiProvider;

        [ObservableProperty]
        private UserDTO user;

        [ObservableProperty]
        private List<DeviceDTO> connectedDevices;

        [ObservableProperty]
        private bool isBusy;

        public ConnectedDeviceViewModel(ServerAPIProvider apiProvider)
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
        private Task OpenDeviceInfo(DeviceDTO device)
        {
			return Shell.Current.GoToAsync(nameof(DeviceInfoPage), new Dictionary<string, object>()
            { 
                { "User", User },
                { "Device", device }
            });
        }
    }
}
