using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeByFace.App.Models;
using WakeByFace.App.Services.ML;
using WakeByFace.App.Views;

namespace WakeByFace.App.Services
{
    public class ForegroundAlarmScheduler : IForegroundAlarmScheduler
    {
        private readonly IAlarmService _alarmService;
        private readonly IEmotionClassifier _emotionClassifier;
        private readonly IDispatcher _dispatcher;

        private CancellationTokenSource? _cts;
        private bool _isRunning;

        private readonly HashSet<int> _triggeredAlarmIds = new();

        public ForegroundAlarmScheduler(IAlarmService alarmService, IEmotionClassifier emotionClassifier)
        {
            _alarmService = alarmService;
            _emotionClassifier = emotionClassifier;

            _dispatcher = Application.Current?.Dispatcher
                          ?? Dispatcher.GetForCurrentThread();
        }

        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _cts = new CancellationTokenSource();

            _ = Task.Run(() => LoopAsync(_cts.Token));
        }

        public void Stop()
        {
            _isRunning = false;
            _cts?.Cancel();
            _cts = null;
        }

        private async Task LoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    var alarms = await _alarmService.GetAllAsync();

                    foreach (var alarm in alarms.Where(a => a.IsEnabled))
                    {
                        if (_triggeredAlarmIds.Contains(alarm.Id))
                            continue;

                        if (ShouldFireNow(alarm, now))
                        {
                            _triggeredAlarmIds.Add(alarm.Id);

                            _dispatcher.Dispatch(async () =>
                            {
                                try
                                {
                                    var mainPage = Application.Current?.MainPage;
                                    var ringingPage = new AlarmRingingPage(alarm, _emotionClassifier, _alarmService);

                                    if (mainPage is NavigationPage nav)
                                    {
                                        await nav.PushAsync(ringingPage);
                                    }
                                    else if (mainPage != null)
                                    {
                                        await mainPage.Navigation.PushAsync(ringingPage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"[AlarmScheduler] Navigation error: {ex}");
                                }
                            });
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    // нормальне завершення
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[AlarmScheduler] error: {ex}");
                }

                // Перевірка раз на 20 секунд
                await Task.Delay(TimeSpan.FromSeconds(20), token);
            }
        }

        private bool ShouldFireNow(Alarm alarm, DateTime nowLocal)
        {
            // Використовуємо логіку з моделі
            if (!alarm.MatchesToday(nowLocal))
                return false;

            if (!alarm.IsTimeToRing(nowLocal))
                return false;

            return true;
        }
    }
}
