using WakeByFace.App.Models;
using WakeByFace.App.Services;
using WakeByFace.App.ViewModels;
using WakeByFace.App.Views;

namespace WakeByFace.App
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel ViewModel => (MainPageViewModel)BindingContext;

        public MainPage()
        {
            InitializeComponent();

            // ❗ Простий варіант: створюємо сервіси вручну
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

    }
}
