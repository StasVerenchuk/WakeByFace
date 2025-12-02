namespace WakeByFace.App.Views;

public partial class CreateAlarmPage : ContentPage
{

	public CreateAlarmPage()
	{
		InitializeComponent();
	}

    private void OnDayTapped(object sender, TappedEventArgs e)
    {

    }

    private void OnDayChecked(object sender, CheckedChangedEventArgs e)
    {
        var cb = (CheckBox)sender;

        if (e.Value) // увімкнено
        {
            cb.BackgroundColor = (Color)Resources["OrangeColor"];
        }
        else // вимкнено
        {
            cb.BackgroundColor = (Color)Resources["SectionsColor"];
        }
    }

    private void ConfirmButtonClicked(object sender, EventArgs e)
    {

    }

    private void DismissButtonClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}