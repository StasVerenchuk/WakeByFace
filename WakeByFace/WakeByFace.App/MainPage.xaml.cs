using WakeByFace.App.Models;
using WakeByFace.App.Services;
using WakeByFace.App.Services.ML;
using WakeByFace.App.ViewModels;
using WakeByFace.App.Views;

namespace WakeByFace.App
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel ViewModel => (MainPageViewModel)BindingContext;

        private readonly TestEmotionPage _testEmotionPage;
        private readonly IEmotionClassifier _emotionClassifier;
        private readonly IAlarmService _alarmService;

        public MainPage(TestEmotionPage testEmotionPage, IEmotionClassifier emotionClassifier, IAlarmService alarmService)
        {
            InitializeComponent();

            /*_testEmotionPage = testEmotionPage;
            _emotionClassifier = emotionClassifier;

            // Простий варіант: створюємо сервіси вручну
            var dbService = new DatabaseService();
            var alarmService = new AlarmService(dbService);
            BindingContext = new MainPageViewModel(alarmService);*/

            _testEmotionPage = testEmotionPage;
            _emotionClassifier = emotionClassifier;
            _alarmService = alarmService;

            BindingContext = new MainPageViewModel(_alarmService);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.LoadAsync();
        }

        private async void AlarmEnabledToggled(object sender, ToggledEventArgs e)
        {
            if (sender is Switch sw && sw.BindingContext is Alarm alarm)
            {
                // IsEnabled вже змінений через Binding
                await ViewModel.UpdateAlarmAsync(alarm);
            }
        }

        private async void CreateAlarmButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateAlarmPage());
        }

        private async void StartTestAlarmButtonClicked(object sender, EventArgs e)
        {
            var now = DateTime.UtcNow;

            var testAlarm = new Alarm
            {
                Name = "Тест будильника",
                Hours = now.Hour,
                Minutes = now.Minute
            };

            await Navigation.PushAsync(new AlarmRingingPage(testAlarm, _emotionClassifier, _alarmService));
        }

        private async void TestEmotionRecognitionButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_testEmotionPage);
        }
    }
}
