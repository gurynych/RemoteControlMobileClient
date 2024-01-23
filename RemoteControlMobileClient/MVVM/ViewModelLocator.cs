using RemoteControlMobileClient.MVVM.ViewModels;

namespace RemoteControlMobileClient.MVVM
{
    internal class ViewModelLocator
    {
        public AuthorizationViewModel AuthrizationViewModel => MauiProgram.GetRequiredService<AuthorizationViewModel>();

        public RegistrationViewModel RegistrationViewModel => MauiProgram.GetRequiredService<RegistrationViewModel>();

        public MainViewModel MainViewModel => MauiProgram.GetRequiredService<MainViewModel>();

        public ConnectedDeviceViewModel ConnectedDeviceViewModel => MauiProgram.GetRequiredService<ConnectedDeviceViewModel>();

        public NestedFilesInDirectoryViewModel NestedFilesInDirectoryViewModel => MauiProgram.GetRequiredService<NestedFilesInDirectoryViewModel>();

        public AppShellViewModel AppShellViewModel => MauiProgram.GetRequiredService<AppShellViewModel>();
    }
}
