using WakeByFace.App.Services;
using WakeByFace.App.ViewModels;

namespace WakeByFace.App.Views;

public partial class CreateAlarmPage : ContentPage
{
    public CreateAlarmViewModel ViewModel => (CreateAlarmViewModel)BindingContext;

    public CreateAlarmPage()
    {
        InitializeComponent();

        var dbService = new DatabaseService();
        var alarmService = new AlarmService(dbService);
        var vm = new CreateAlarmViewModel(alarmService);

        BindingContext = vm;
        vm.RequestClose += async () => await Navigation.PopAsync();
    }

    public CreateAlarmPage(CreateAlarmViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        viewModel.RequestClose += async () => await Navigation.PopAsync();
	}

    private void OnDayChecked(object sender, CheckedChangedEventArgs e)
    {
        /*
        var cb = (CheckBox)sender;

        if (e.Value) // увімкнено
        {
            cb.BackgroundColor = (Color)Resources["OrangeColor"];
        }
        else // вимкнено
        {
            cb.BackgroundColor = (Color)Resources["SectionsColor"];
        }
        */
    }

    private async void ConfirmButtonClicked(object sender, EventArgs e)
    {
        if (HoursCarousel.CurrentItem is string hourStr &&
            int.TryParse(hourStr , out var hour))
        {
            ViewModel.Hour = hour;
        }

        if (MinutesCarousel.CurrentItem is string minuteStr &&
            int.TryParse(minuteStr, out var minute))
        {
            ViewModel.Minute = minute;
        }

        ViewModel.Name = entryName.Text ?? string.Empty;

        ViewModel.RepeatMon = Mon.IsChecked;
        ViewModel.RepeatTue = Tue.IsChecked;
        ViewModel.RepeatWed = Wed.IsChecked;
        ViewModel.RepeatThu = Thu.IsChecked;
        ViewModel.RepeatFri = Fri.IsChecked;
        ViewModel.RepeatSat = Sat.IsChecked;
        ViewModel.RepeatSun = Sun.IsChecked;

        await ViewModel.SaveAsync();
    }

    private void DismissButtonClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}