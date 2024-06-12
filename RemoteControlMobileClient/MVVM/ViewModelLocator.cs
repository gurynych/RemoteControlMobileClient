using RemoteControlMobileClient.MVVM.ViewModels;

namespace RemoteControlMobileClient.MVVM
{
    public class ViewModelLocator
    {
        public AuthorizationViewModel AuthrizationViewModel => MauiProgram.GetRequiredService<AuthorizationViewModel>();

        public RegistrationViewModel RegistrationViewModel => MauiProgram.GetRequiredService<RegistrationViewModel>();

        public MainViewModel MainViewModel => MauiProgram.GetRequiredService<MainViewModel>();

        public ConnectedDeviceViewModel ConnectedDeviceViewModel => MauiProgram.GetRequiredService<ConnectedDeviceViewModel>();

        public DeviceFolderViewModel DeviceFolderViewModel => MauiProgram.GetRequiredService<DeviceFolderViewModel>();

        public AppShellViewModel AppShellViewModel => MauiProgram.GetRequiredService<AppShellViewModel>();

        public StartupViewModel StartupViewModel => MauiProgram.GetRequiredService<StartupViewModel>();

        public DeviceInfoViewModel DeviceInfoViewModel => MauiProgram.GetRequiredService<DeviceInfoViewModel>();

        public RunningProgramsViewModel RunningProgramsViewModel => MauiProgram.GetRequiredService<RunningProgramsViewModel>();
	}
}
