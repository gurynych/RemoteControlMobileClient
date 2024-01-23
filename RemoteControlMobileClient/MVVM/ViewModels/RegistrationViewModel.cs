using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetworkMessage.Communicator;
using RemoteControlMobileClient.BusinessLogic.Models;
using RemoteControlMobileClient.BusinessLogic.Services;
using RemoteControlMobileClient.BusinessLogic.Services.Partial;
using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.Pages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
    internal partial class RegistrationViewModel : ObservableValidator, ITransient
    {
        private readonly ServerAPIProviderService apiProvider;
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

        public RegistrationViewModel(ServerAPIProviderService apiProvider, TcpCryptoClientCommunicator communicator)
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
            RequestPermissionsService requestPermissionsService = new RequestPermissionsService();
            bool accept = await requestPermissionsService.RequestPermission();
            if (accept)
            {
                var tokenSource = new CancellationTokenSource(20000);
                User user = new User(Login, Email, Password); //("gurila@gurila.com", "gurila");  
                byte[] publicKey = await apiProvider.UserRegistrationUseAPIAsync(user, tokenSource.Token);
                try
                {
                    if (publicKey == null) return;
                    bool connected =
                        await communicator.ConnectAsync(ServerAPIProviderService.ServerAddress, 11000, tokenSource.Token);
                    if (!connected) return;

                    communicator.SetExternalPublicKey(publicKey);
                    int repeat = 0;
                    while (repeat < 10)
                    {
                        bool success = await communicator.HandshakeAsync(token: tokenSource.Token);
                        if (success)
                        {
                            user.AuthToken = publicKey;
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
