using Plugin.Maui.Audio;
using WakeByFace.App.Models;

namespace WakeByFace.App.Views;

public partial class AlarmRingingPage : ContentPage
{
    private readonly Alarm _alarm;
    private IAudioPlayer? _player;

    public AlarmRingingPage(Alarm alarm)
    {
        InitializeComponent();
        _alarm = alarm;
        BindingContext = alarm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await StartRingtoneAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopRingtone();
    }

    private async Task StartRingtoneAsync()
    {
        try
        {
            if (_player != null)
                return;

            var stream = await FileSystem.OpenAppPackageFileAsync("rain_drops.mp3");

            var audioManager = AudioManager.Current;
            _player = audioManager.CreatePlayer(stream);
            _player.Loop = true;
            _player.Play();

            System.Diagnostics.Debug.WriteLine("Success. Start playing ringtone.");
        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Audio error: {ex.Message}");
        }
    }

    private void StopRingtone()
    {
        if (_player != null)
        {
            _player.Stop();
            _player.Dispose();
            _player = null;
        }
    }

    private async void StopButtonClicked(object sender, EventArgs e)
    {
        StopRingtone();
        await Navigation.PopAsync();
    }

    private async void TestButtonClicked(object sender, EventArgs e)
    {
        StopRingtone();
        await StartRingtoneAsync();
    }
}