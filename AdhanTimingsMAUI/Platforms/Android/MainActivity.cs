using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;

namespace AdhanTimingsMAUI
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Request notification permission for Android 13+
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.PostNotifications }, 101);
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 101)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    Console.WriteLine("Notification permission granted.");
                }
                else
                {
                    Console.WriteLine("Notification permission denied.");
                }
            }
        }
    }
}
