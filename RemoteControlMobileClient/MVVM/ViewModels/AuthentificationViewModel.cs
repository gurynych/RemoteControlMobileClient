using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RemoteControlMobileClient.MVVM.LifeCycles;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
    internal partial class AuthentificationViewModel : ObservableValidator, ITransient
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Поле \"пароль\" обязательно для заполненеия")]
        private string authPassword;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Поле \"почта\" обязательно для заполненеия")]
        private string authEmail;

        [ObservableProperty]
        private string authPasswordError = null;

        [ObservableProperty]
        private bool authPasswordHasError = false;

        [ObservableProperty]
        private string authEmailError = null;

        [ObservableProperty]
        private bool authEmailHasError = false;

        public AuthentificationViewModel()
        {
            ErrorsChanged += (sender, args) =>
            {
                var errors = GetErrors(nameof(AuthPassword));
                AuthPasswordError = errors.FirstOrDefault()?.ErrorMessage;
                AuthPasswordHasError = AuthPasswordError != null;

                errors = GetErrors(nameof(AuthEmail));
                AuthEmailError = errors.FirstOrDefault()?.ErrorMessage;
                AuthEmailHasError = AuthEmailError != null;
            };
        }
        
        [RelayCommand(CanExecute = nameof(CheckFields))]    
        public async Task Authorize()
        {            
            
            await Task.Delay(500);
        }

        private bool CheckFields()
        {
            return !HasErrors;
        }
    }
}