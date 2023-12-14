using DevExpress.Maui.Editors;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Android.Content;
using Android.App;
using RemoteControlMobileClient.MVVM.ViewModels;

namespace RemoteControlMobileClient
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnOpenWebButtonClicked(System.Object sender, System.EventArgs e)
        {

            ActivityManager activityManager = Android.App.Application.Context.GetSystemService(Context.ActivityService) as ActivityManager;
            var memoryInfo = new ActivityManager.MemoryInfo();
            activityManager.GetMemoryInfo(memoryInfo);
            long total = memoryInfo.TotalMem / 1024 / 1024;
            long t = memoryInfo.AvailMem / 1024 / 1024;

            if (!Android.OS.Environment.IsExternalStorageManager)
            {
                Android.Net.Uri uri = Android.Net.Uri.Parse("package:" + Android.App.Application.Context.ApplicationContext.PackageName);
                Intent intent = new Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission, uri);
                Platform.CurrentActivity.StartActivityForResult(intent, 1);

            }
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            var dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
            var files = Directory.GetFiles(dir.AbsolutePath, "*");
            //Permissions.RequestAsync<>
            byte percent = (byte)(Battery.Default.ChargeLevel * 100);
            
            
                        
            
            //Navigation.PushAsync()

            //await Browser.OpenAsync("https://www.devexpress.com/maui/");
        }
    }
}