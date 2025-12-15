using Plugin.Maui.Audio;
using System.Diagnostics;
using WakeByFace.App.Models;
using WakeByFace.App.Services;
using WakeByFace.App.Services.ML;

namespace WakeByFace.App.Views;

public partial class AlarmRingingPage : ContentPage
{
    private readonly Alarm _alarm;
    private readonly IEmotionClassifier _emotionClassifier;
    private readonly IAlarmService _alarmService;

    private IAudioPlayer? _player;

    private readonly string[] _emotions = { "happy", "neutral", "sad", "surprise" };
    private readonly Random _rand = new();
    private string _currentTarget = string.Empty;

    public AlarmRingingPage(Alarm alarm, IEmotionClassifier emotionClassifier, IAlarmService alarmService)
    {
        InitializeComponent();

        _alarm = alarm;
        _emotionClassifier = emotionClassifier;
        _alarmService = alarmService;

        BindingContext = alarm;
        PickNewTargetEmotion();
    }

    private void PickNewTargetEmotion()
    {
        _currentTarget = _emotions[_rand.Next(_emotions.Length)];
        targetEmotionLabel.Text = $"Зобразіть емоцію: {_currentTarget}";
        statusLabel.Text = "Натисніть «Зробити фото»";
        resultLabel.Text = string.Empty;
        btnStop.IsEnabled = false;
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
        Debug.WriteLine("[AlarmRingingPage] StopButtonClicked fired");

        StopRingtone();

        _alarm.IsEnabled = false;
        await _alarmService.UpdateAsync(_alarm);

        await Navigation.PopAsync();
    }



    private async void CaptureButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Зобразіть потрібну емоцію"
            });

            if (photo == null)
                return;

            using var stream = await photo.OpenReadAsync();

            var result = await _emotionClassifier.PredictAsync(stream);

            resultLabel.Text = $"Розпізнано: {result.Label}\n" +
                    $"Ймовірності: {string.Join(", ", result.Probabilities.Select(p => p.ToString("0.00")))}";

            if (string.Equals(result.Label, _currentTarget, StringComparison.OrdinalIgnoreCase))
            {
                statusLabel.Text = "Вітаю! Емоцію розпізнано правильно";
                btnStop.IsEnabled = true;
            }
            else
            {
                statusLabel.Text = "Емоція не співпала. Спробуйте ще раз.";
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Помилка", "Камера не підтримується на цьому пристрої", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Помилка доступу", "Немає доступу до камери. Перевірте дозволи у налаштуваннях", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Щось пішло не так: {ex.Message}", "OK");
        }
    }

    private void ShuffleButtonClicked(object sender, EventArgs e)
    {
        PickNewTargetEmotion();
    }
}