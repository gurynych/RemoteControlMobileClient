using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetworkMessage.Communicator;
using RemoteControlMobileClient.BusinessLogic.DTO;
using RemoteControlMobileClient.BusinessLogic.Services;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;
using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.Pages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
	public partial class RegistrationViewModel : ObservableValidator, ITransient
	{
		private readonly ServerAPIProvider apiProvider;
		private readonly TcpCryptoClientCommunicator communicator;

		[ObservableProperty]
		[NotifyDataErrorInfo]
		[Required(ErrorMessage = "Поле \"пароль\" обязательно для заполненеия")]
		[MinLength(6, ErrorMessage = "Минимум 6 символов")]
		private string password;

		[ObservableProperty]
		[NotifyDataErrorInfo]
		[Required(ErrorMessage = "Поле \"почта\" обязательно для заполненеия")]
		[RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Почта не соответствует формату")]
		private string email;

		[ObservableProperty]
		[NotifyDataErrorInfo]
		[Required(ErrorMessage = "Поле \"логин\" обязательно для заполненеия")]
		[RegularExpression(@"[a-zA-Z0-9]+$", ErrorMessage = "Логин должен содержать только латиские буквы и цифры")]
		[MinLength(3, ErrorMessage = "Логин должен быть от 3 до 20 символов")]
		[MaxLength(20, ErrorMessage = "Логин должен быть от 3 до 20 символов")]
		private string login;

		[ObservableProperty]
		private string passwordError = null;

		[ObservableProperty]
		private bool passwordHasError = false;

		[ObservableProperty]
		private string emailError = null;

		[ObservableProperty]
		private bool emailHasError = false;

		[ObservableProperty]
		private string loginError = null;

		[ObservableProperty]
		private bool loginHasError = false;

		public RegistrationViewModel(ServerAPIProvider apiProvider, TcpCryptoClientCommunicator communicator)
		{
			ErrorsChanged += (sender, args) =>
			{
				var errors = GetErrors(nameof(Password));
				PasswordError = errors.FirstOrDefault()?.ErrorMessage;
				PasswordHasError = PasswordError != null;

				errors = GetErrors(nameof(Email));
				EmailError = errors.FirstOrDefault()?.ErrorMessage;
				EmailHasError = EmailError != null;

				errors = GetErrors(nameof(Login));
				LoginError = errors.FirstOrDefault()?.ErrorMessage;
				LoginHasError = LoginError != null;
			};
			this.apiProvider = apiProvider;
			this.communicator = communicator;
		}

		[RelayCommand]
		private async Task Register()
		{

			var tokenSource = new CancellationTokenSource(20000);
			UserDTO user = new UserDTO(Login, Email, Password); //("gurila@gurila.com", "gurila");  
			Response response = await apiProvider.UserRegistrationUseAPIAsync(user, tokenSource.Token);
			byte[] publicKey = response.PublicKey;
			try
			{
				if (publicKey == default)
				{
					await Toast.Make(response.Message, CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
					return;
					//throw new NullReferenceException();
				}
				bool connected =
					await communicator.ConnectAsync(ServerAPIProvider.ServerAddress, 11000, tokenSource.Token);
				if (!connected) return;

				user.AuthToken = publicKey;
				publicKey = publicKey[..^Encoding.UTF8.GetBytes(Email).Length];
				communicator.SetExternalPublicKey(publicKey);
				int repeat = 0;
				while (repeat < 10)
				{
					bool success = await communicator.HandshakeAsync(token: tokenSource.Token);
					if (success)
					{
						tokenSource.Dispose();
						await Shell.Current.GoToAsync(nameof(MainPage), new Dictionary<string, object>()
							{
								{ "User", user }
							});

						return;
					}

					tokenSource.TryReset();
					repeat++;
				}

				Debug.WriteLine("Прошло 10 подключений", "Error");

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message, "Error");
			}
		}

		[RelayCommand]
		private Task GoToAutorization()
		{
			return Shell.Current.GoToAsync("..");
		}

		private bool CheckFields()
		{
			return !(PasswordHasError || EmailHasError);
		}
	}
}
