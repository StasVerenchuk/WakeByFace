using WakeByFace.App.Services;

namespace WakeByFace.App
{
    public partial class App : Application
    {
        private readonly IForegroundAlarmScheduler _scheduler;
        public App(MainPage mainPage, IForegroundAlarmScheduler scheduler)
        {
            InitializeComponent();
            _scheduler = scheduler;
            MainPage = new NavigationPage(mainPage);
            _scheduler.Start();
        }
    }
}
