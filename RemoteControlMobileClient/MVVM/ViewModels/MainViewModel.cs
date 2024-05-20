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
using NetworkMessage.Exceptions;
using System;
using CommunityToolkit.Maui.Alerts;

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
        private long receiveProcessProgress;

        [ObservableProperty]
        private long sendProcessProgress;

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
            CancellationToken token = tokenSource.Token;
            return Task.Run(async () =>
            {
                Progress<long> receiveProgress = new Progress<long>((i) => ReceiveProcessProgress = i);
                Progress<long> sendProgress = new Progress<long>((i) => SendProcessProgress = i);

				while (!token.IsCancellationRequested)
				{
					try
					{
						BaseIntent intent = await communicator.ReceiveAsync(token: token).ConfigureAwait(false);
						token.ThrowIfCancellationRequested();
						if (intent == null)
						{
							continue;
						}
						
						BaseNetworkCommandResult result = await intent
							.CreateCommand(factory)
							.ExecuteAsync()
						.ConfigureAwait(false);

						await communicator.SendObjectAsync(result, token: token).ConfigureAwait(false);
					}
					catch (DeviceNotConnectedException notConnEx)
					{
                        await Snackbar.Make("Устройство отключено").Show();
						Debug.WriteLine(notConnEx.Message);
						return;
					}
					catch (OperationCanceledException operationCanncelEx)
					{
						await Snackbar.Make("Устройство отключено").Show();
						Debug.WriteLine(operationCanncelEx.Message);
						return;
					}
					catch (Exception ex)
					{
						await Snackbar.Make("Устройство отключено").Show();
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
