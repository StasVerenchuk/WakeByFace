using WakeByFace.App.Models;
using WakeByFace.App.Services;
using WakeByFace.App.ViewModels;
using WakeByFace.App.Views;

namespace WakeByFace.App
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel ViewModel => (MainPageViewModel)BindingContext;

        private readonly TestEmotionPage _testEmotionPage;

        public MainPage(TestEmotionPage testEmotionPage)
        {
            InitializeComponent();

            _testEmotionPage = testEmotionPage;

            // Простий варіант: створюємо сервіси вручну
            var dbService = new DatabaseService();
            var alarmService = new AlarmService(dbService);
            BindingContext = new MainPageViewModel(alarmService);
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

            await Navigation.PushAsync(new AlarmRingingPage(testAlarm));
        }

        private async void TestEmotionRecognitionButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_testEmotionPage);
        }
    }
}
