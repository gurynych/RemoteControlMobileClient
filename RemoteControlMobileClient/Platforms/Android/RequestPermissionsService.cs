using Android.Content;

namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
    internal partial class RequestPermissionsService
    {
        public partial Task<bool> RequestPermission()
        {
            if (!Android.OS.Environment.IsExternalStorageManager)
            {
                Android.Net.Uri uri = Android.Net.Uri.Parse("package:" + Android.App.Application.Context.ApplicationContext.PackageName); ;
                Intent intent = new Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission, uri);
                Platform.CurrentActivity.StartActivityForResult(intent, 1);
            }

            return Task.FromResult(Android.OS.Environment.IsExternalStorageManager);
        }
    }
}
