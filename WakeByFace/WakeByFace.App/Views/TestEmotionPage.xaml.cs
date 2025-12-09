using WakeByFace.App.Services.ML;

namespace WakeByFace.App.Views;

public partial class TestEmotionPage : ContentPage
{
    private readonly IEmotionClassifier _emotionClassifier;
    private readonly string[] _emotions = { "happy", "neutral", "sad", "surprise" };
    private readonly Random rand = new Random();

    private string _currentTarget = string.Empty;

	public TestEmotionPage(IEmotionClassifier emotionClassifier)
	{
		InitializeComponent();
        _emotionClassifier = emotionClassifier;

        PickNewTarget();
	}

    private void PickNewTarget()
    {
        _currentTarget = _emotions[rand.Next(_emotions.Length)];
        targetEmotionLabel.Text = $"Спробуйте зобразити емоцію: {_currentTarget}";
        statusLabel.Text = "Натисніть \'Зробити фото\' і покажіть цю емоцію";
        resultLabel.Text = string.Empty;
        btnBack.IsEnabled = false;
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
                btnBack.IsEnabled = true;
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
        PickNewTarget();
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}