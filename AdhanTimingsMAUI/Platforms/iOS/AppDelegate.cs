using Foundation;
using UIKit;
using UserNotifications;

namespace AdhanTimingsMAUI
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Request notification permissions
            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge,
                (granted, error) =>
                {
                    if (granted)
                    {
                        Console.WriteLine("Notification permission granted.");
                    }
                    else
                    {
                        Console.WriteLine("Notification permission denied.");
                    }
                });

            return base.FinishedLaunching(application, launchOptions);
        }
    }
}
