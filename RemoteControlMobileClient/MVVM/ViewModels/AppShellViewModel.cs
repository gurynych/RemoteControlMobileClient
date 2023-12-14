using CommunityToolkit.Mvvm.Input;
using RemoteControlMobileClient.MVVM.LifeCycles;
using RemoteControlMobileClient.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlMobileClient.MVVM.ViewModels
{
    internal partial class AppShellViewModel : ITransient
    {
        public AppShellViewModel()
        {
            Routing.RegisterRoute(nameof(AuthorizationPage), typeof(AuthorizationPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        }

        [RelayCommand]
        public Task SetStartUpPage()
        {
            //TODO: добавить проверку токена
            if (true)
            {
                return Shell.Current.GoToAsync(nameof(AuthorizationPage));
            }
        }
    }
}
