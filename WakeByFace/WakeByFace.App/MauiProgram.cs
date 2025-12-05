using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using WakeByFace.App.Services;
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

            // ViewModels
            builder.Services.AddTransient<CreateAlarmViewModel>();

            builder.Services.AddTransient<CreateAlarmPage>();
            builder.Services.AddSingleton(AudioManager.Current);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
