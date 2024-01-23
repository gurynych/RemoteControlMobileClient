namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
    internal partial class RequestPermissionsService
    {
        public partial async Task<bool> RequestPermission()
        {
            return (await Permissions.RequestAsync<Permissions.Photos>()) == PermissionStatus.Granted;
        }
    }
}

