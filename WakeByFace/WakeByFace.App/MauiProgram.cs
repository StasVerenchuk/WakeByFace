using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using WakeByFace.App.Services;
using WakeByFace.App.Services.ML;
using WakeByFace.App.ViewModels;
using WakeByFace.App.Views;

namespace WakeByFace.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //SQLite
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddSingleton<IAlarmService, AlarmService>();

            builder.Services.AddSingleton<IForegroundAlarmScheduler, ForegroundAlarmScheduler>();

            // ML
#if ANDROID

            builder.Services.AddSingleton<IEmotionClassifier, EmotionClassifier>();
#endif

            // ViewModels
            builder.Services.AddTransient<CreateAlarmViewModel>();

            // Pages
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<CreateAlarmPage>();
            builder.Services.AddTransient<AlarmRingingPage>();

            // Audio
            builder.Services.AddSingleton(AudioManager.Current);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
