using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetworkMessage.CommandFactory;
using NetworkMessage.CommandsResults;
using NetworkMessage.Communicator;
using NetworkMessage.Intents;
using NetworkMessage.Models;
using RemoteControlMobileClient.BusinessLogic.Models;
using RemoteControlMobileClient.BusinessLogic.Services;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;
using RemoteControlMobileClient.MVVM.LifeCycles;
using System.Diagnostics;
using Device = RemoteControlMobileClient.BusinessLogic.Models.Device;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
    [QueryProperty(nameof(User), "User")]
    [QueryProperty(nameof(Device), "Device")]
    internal partial class NestedFilesInDirectoryViewModel : ObservableObject, ITransient
    {
        private string path = "root";
        private readonly ServerAPIProviderService apiProvider;
        private readonly TcpCryptoClientCommunicator communicator;
        private readonly ICommandFactory factory;

        [ObservableProperty]
        private User user;

        [ObservableProperty]
        private Device device;

        [ObservableProperty]
        private NestedFilesInDirectory nestedFilesInDirectory;

        [ObservableProperty]
        private IEnumerable<object> allData;

        [ObservableProperty]
        private bool isBusy;

        public NestedFilesInDirectoryViewModel(ServerAPIProviderService apiProvider,
            TcpCryptoClientCommunicator communicator,
            CommandFactoryService commandFactoryService)
        {
            this.apiProvider = apiProvider;
            this.communicator = communicator;
            this.factory = commandFactoryService.CreateCommandFactory();
        }

        [RelayCommand]
        private Task GoToConnectedDevicesPage()
        {
            return Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task LoadNestedFilesInRootDirectory()
        {
            this.path = "root";
            IsBusy = true;
            CancellationTokenSource tokenSource = new CancellationTokenSource(50000);

            NestedFilesInDirectory =
                await apiProvider.GetNestedFilesInfoInDirectoryAsync(User, Device.Id, path, tokenSource.Token);
            AllData = NestedFilesInDirectory?.NestedDirectoriesInfo.Cast<object>().Concat(NestedFilesInDirectory.NestedFilesInfo.Cast<object>());
            IsBusy = false;
        }

        [RelayCommand]
        private async Task LoadNestedFilesInDirectory(object item)
        {
            if (item is MyDirectoryInfo dir)
            {
                AllData = null;
                this.path += "/" + dir.Name;
                IsBusy = true;
                CancellationTokenSource tokenSource = new CancellationTokenSource(50000);
                NestedFilesInDirectory =
                    await apiProvider.GetNestedFilesInfoInDirectoryAsync(User, Device.Id, path, tokenSource.Token);
                if (NestedFilesInDirectory != null)
                {
                    AllData = NestedFilesInDirectory.NestedDirectoriesInfo.Cast<object>().Concat(NestedFilesInDirectory.NestedFilesInfo.Cast<object>());
                }

                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DownloadFile(string name)
        {
            IsBusy = true;
            string pathForDowload = this.path + "/" + name;
            CancellationTokenSource tokenSource = new CancellationTokenSource(1000000);
            byte[] fileBytes = await apiProvider.DownloadFileAsync(User, Device.Id, pathForDowload, tokenSource.Token);
            if (fileBytes != null)
            {
                FilesService fileService = new FilesService();
                bool isSuccess = await fileService.SaveFileAsync(fileBytes, name);
                if (isSuccess)
                {
                    await Toast.Make("Файл сохранен в папке загрузки", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
                else
                {
                    await Toast.Make("Ошибка сохранения файла", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
            }

            IsBusy = false;
        }
    }
}
