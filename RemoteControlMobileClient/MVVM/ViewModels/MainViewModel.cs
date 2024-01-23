using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetworkMessage.CommandsResults;
using NetworkMessage.Communicator;
using NetworkMessage.Intents;
using RemoteControlMobileClient.MVVM.LifeCycles;
using System.Diagnostics;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;
using NetworkMessage.CommandFactory;
using RemoteControlMobileClient.Pages;
using RemoteControlMobileClient.BusinessLogic.Models;
using RemoteControlMobileClient.BusinessLogic.Services;
using DevExpress.Data.Mask.Internal;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
    [QueryProperty(nameof(User), "User")]
    internal partial class MainViewModel : ObservableObject, ITransient
    {
        private readonly ICommandFactory factory;
        private readonly TcpCryptoClientCommunicator communicator;
        private readonly ServerAPIProviderService apiProvider;
        private CancellationTokenSource tokenSource;

        [ObservableProperty]
        private bool isConneted;

        [ObservableProperty]
        private string actualAction;

        [ObservableProperty]
        private int receiveProcessProgress;

        [ObservableProperty]
        private int sendProcessProgress;

        [ObservableProperty]
        private User user;

        public MainViewModel(TcpCryptoClientCommunicator communicator, CommandFactoryService commandFactoryService, ServerAPIProviderService apiProvider)
        {
            this.communicator = communicator;
            this.apiProvider = apiProvider;
            this.factory = commandFactoryService.CreateCommandFactory();
            IsConneted = communicator.IsConnected;
        }

        [RelayCommand]
        private async Task Reconnect()
        {
            IsConneted = false;
            var tokenSource = new CancellationTokenSource(30000);
            IsConneted =
                await communicator.ReconnectWithHandshakeAsync(ServerAPIProviderService.ServerAddress, 11000, token: tokenSource.Token);
        }

        [RelayCommand]
        private Task StartListen()
        {
            tokenSource = new CancellationTokenSource();
            return Task.Run(async () =>
            {
                Progress<int> receiveProgress = new Progress<int>((i) => ReceiveProcessProgress = i);
                Progress<int> sendProgress = new Progress<int>((i) => SendProcessProgress = i);

                while (!tokenSource.IsCancellationRequested)
                {
                    try
                    {
                        ActualAction = "Получение намерения\n";
                        ReceiveProcessProgress = 0;
                        BaseIntent intent = await communicator.ReceiveIntentAsync(receiveProgress, tokenSource.Token).ConfigureAwait(false);
                        if (intent == null)
                        {
                            continue;
                        }

                        SendProcessProgress = 0;
                        ActualAction += $"Полученное намерение: {intent.IntentType}\n";
                        ActualAction += $"Выполнение команды\n";
                        BaseNetworkCommandResult result = await intent.CreateCommand(factory).ExecuteAsync().ConfigureAwait(false);
                        ActualAction += "Отправка результа команды\n";
                        await communicator.SendMessageAsync(new NetworkMessage.NetworkMessage(result), sendProgress, tokenSource.Token).ConfigureAwait(false);
                        ActualAction += "Результат отправлен";
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            });
        }

        [RelayCommand]
        private Task StopListenAsync()
        {
            return tokenSource?.CancelAsync();
        }

        [RelayCommand]
        private async Task GoToConnectedDevicesPage()
        {
            try
            {
                //var tokenSource = new CancellationTokenSource(5000);
                //var connectedDevices = await apiProvider.GetConnectedDeviceAsync(User, tokenSource.Token);
                await Shell.Current.GoToAsync(nameof(ConnectedDevicesPage), new Dictionary<string, object>()
                {
                    { nameof(User), User }
                    //{ "ConnectedDevices", connectedDevices }
                }).ContinueWith(_ => tokenSource?.CancelAsync()?.ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                await Task.CompletedTask;
            }
        }        
    }
}
