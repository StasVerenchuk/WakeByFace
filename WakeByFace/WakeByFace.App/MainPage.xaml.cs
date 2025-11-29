using WakeByFace.App.Views;

namespace WakeByFace.App
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void CreateAlarmButtonClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CreateAlarmPage());
        }
    }

}
