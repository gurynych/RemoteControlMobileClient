using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Maui.Core.Internal;
using NetworkMessage.Communicator;
using RemoteControlMobileClient.BusinessLogic.Helpers;
using RemoteControlMobileClient.BusinessLogic.DTO;
using RemoteControlMobileClient.BusinessLogic.Services;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;
using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.Pages;
using System.Text;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
	public partial class StartupViewModel : ObservableObject, ITransient
	{
		private readonly ServerAPIProvider apiProvider;
		private readonly TcpCryptoClientCommunicator communicator;

		[ObservableProperty] private string progressMessage;

		public StartupViewModel(ServerAPIProvider apiProvider, TcpCryptoClientCommunicator communicator)
        {
			this.apiProvider = apiProvider ?? throw new ArgumentNullException(nameof(apiProvider));
			this.communicator = communicator ?? throw new ArgumentNullException(nameof(communicator));
		}

		[RelayCommand]
		public async Task Startup()
		{			
			ProgressMessage = "Запрос разрешений";
			RequestPermissionsService requestPermissionsService = new RequestPermissionsService();
			bool accept = await requestPermissionsService.RequestPermission();
			if (!accept)
			{
				await Toast.Make("Для продолжения необходимо выдать разрешение", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
				accept = await requestPermissionsService.RequestPermission();
				
				if (!accept)
				{
					await Toast.Make("Необходимо выдать разрешение. Приложение будет закрыто", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
					Environment.Exit(0);
				}
			}

			ProgressMessage = "Получение сохраненных данных пользователя";
			UserDTO user = await UserStorageHelper.ReadUserAsync();
			byte[] userToken = user?.AuthToken;
			if (userToken == null)
			{
				await Shell.Current.GoToAsync(nameof(AuthorizationPage));
				return;
			}

			CancellationTokenSource tokenSource = new CancellationTokenSource(15000);
			try
			{
				ProgressMessage = "Получение ключа для обмена инфомарцией от сервера";
				byte[] serverPublicKey = await apiProvider.UserAuthorizationWithTokenUseAPIAsync(userToken, tokenSource.Token);
				if (serverPublicKey == default || serverPublicKey.IsEmptyOrSingle())
				{
					await Toast.Make("Не удалось получить ключ от сервера", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
					await Shell.Current.GoToAsync(nameof(AuthorizationPage));
					return;
				}

				ProgressMessage = "Подключение к серверу";
				bool connected =
					await communicator.ConnectAsync(ServerAPIProvider.ServerAddress, 11000, tokenSource.Token);
				if (!connected)
				{
					await Toast.Make("Не удалось подключиться к серверу", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
					await Shell.Current.GoToAsync(nameof(AuthorizationPage));
					return;
				}

				ProgressMessage = "Обмен данными с сервером";
				userToken = userToken[..^Encoding.UTF8.GetBytes(user.Email).Length];
				communicator.SetExternalPublicKey(serverPublicKey);
				int repeat = 0;
				while (repeat < 10)
				{
					bool success = await communicator.HandshakeAsync(token: tokenSource.Token);
					if (success)
					{
						user = await apiProvider.GetUserByToken(userToken, tokenSource.Token);
						await UserStorageHelper.WriteUserAsync(user);
						await Shell.Current.GoToAsync(nameof(MainPage), new Dictionary<string, object>()
							{
								{ "User", user }
							});
						return;
					}

					tokenSource.TryReset();
					repeat++;
				}
			}
			catch
			{
				await Toast.Make("Ошибка автоматического входа", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
				await Shell.Current.GoToAsync(nameof(AuthorizationPage));
				return;
			}
			finally
			{
				await tokenSource.CancelAsync();
				tokenSource?.Dispose();
			}
		}
	}
}
