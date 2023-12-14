using RemoteControlMobileClient.MVVM.ViewModels;

namespace RemoteControlMobileClient.MVVM
{
    internal class ViewModelLocator
    {
        public AuthentificationViewModel AuthentificationViewModel => MauiProgram.GetRequiredService<AuthentificationViewModel>();

        public AppShellViewModel AppShellViewModel => MauiProgram.GetRequiredService<AppShellViewModel>();
    }
}
