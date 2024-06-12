using Android.App;
using Android.Content;
using Android.Content.PM;

namespace RemoteControlMobileClient
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnActivityResult(int requesCode, Result resultCode, Intent? data)
        {
            if (resultCode == Result.Ok)
            {

            }

            base.OnActivityResult(requesCode, resultCode, data);
        }
	}
}
