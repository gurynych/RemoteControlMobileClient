using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class AuthorizationViewModel : ObservableValidator, ITransient
    {
        private readonly ServerAPIProvider apiProvider;
        private readonly TcpCryptoClientCommunicator communicator;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Поле \"пароль\" обязательно для заполненеия")]
        private string password;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Поле \"почта\" обязательно для заполненеия")]
        private string email;

        [ObservableProperty]
        private string passwordError = null;

        [ObservableProperty]
        private bool passwordHasError = false;

        [ObservableProperty]
        private string emailError = null;

        [ObservableProperty]
        private bool emailHasError = false;

        public AuthorizationViewModel(ServerAPIProvider apiProvider, TcpCryptoClientCommunicator communicator)
        {
            this.apiProvider = apiProvider;
            this.communicator = communicator;
            ErrorsChanged += (sender, args) =>
            {
                var errors = GetErrors(nameof(Password));
                PasswordError = errors.FirstOrDefault()?.ErrorMessage;
                PasswordHasError = PasswordError != null;

                errors = GetErrors(nameof(Email));
                EmailError = errors.FirstOrDefault()?.ErrorMessage;
                EmailHasError = EmailError != null;
            };
        }

        [RelayCommand(CanExecute = nameof(IsValidForm))]
        private async Task Authorize()
        {            
            if (!IsValidForm()) return;

            var tokenSource = new CancellationTokenSource(200000);
            UserDTO user = new UserDTO(Email, Password); //("gurila@gurila.com", "gurila");  
            byte[] publicKey = await apiProvider.UserAuthorizationUseAPIAsync(user, tokenSource.Token);
            try
            {
                if (publicKey == null) return;
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
						await UserStorageHelper.WriteUserAsync(user);

						RequestPermissionsService requestPermissionsService = new RequestPermissionsService();
						bool accept = await requestPermissionsService.RequestPermission();
						while (!accept)
                        {
                            await Toast.Make("Для продолжения необходимо выдать разрешение", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                            accept = await requestPermissionsService.RequestPermission();
						}

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
        private Task GoToRegistration()
        {
            return Shell.Current.GoToAsync(nameof(RegistrationPage));
        }

        private bool CheckFields()
        {
            return !(PasswordHasError || EmailHasError);
        }

        private bool IsValidForm()
        {
            return !HasErrors;
        }
    }
}