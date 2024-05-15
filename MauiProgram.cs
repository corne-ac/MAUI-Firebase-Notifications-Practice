using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase;
using Plugin.Firebase.Auth;
using Firebase;
using Plugin.Firebase.Bundled.Shared;
using Plugin.Firebase.Crashlytics;

#if IOS
using Plugin.Firebase.Bundled.Platforms.iOS;
#else
using Plugin.Firebase.Bundled.Platforms.Android;
#endif

namespace PushMauiSample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .RegisterFirebaseServices() // Add this part to register the service for firebase
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }


        // This prepares the application and device to receive notifications

        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) => {
                CrossFirebase.Initialize(CreateCrossFirebaseSettings());
                CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);
                return false;
            }));
#else
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                    CrossFirebase.Initialize(activity, CreateCrossFirebaseSettings())));
                CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);
#endif
            });

            builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
            return builder;
        }

        private static CrossFirebaseSettings CreateCrossFirebaseSettings()
        {
            return new CrossFirebaseSettings(isAuthEnabled: true, isCloudMessagingEnabled: true, isAnalyticsEnabled: true);
        }
    }

}

