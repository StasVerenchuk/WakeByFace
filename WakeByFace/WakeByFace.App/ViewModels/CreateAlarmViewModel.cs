using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WakeByFace.App.Models;
using WakeByFace.App.Services;

namespace WakeByFace.App.ViewModels
{
    public class CreateAlarmViewModel : BaseViewModel
    {
        private readonly IAlarmService _alarmService;

        public CreateAlarmViewModel(IAlarmService alarmService)
        {
            _alarmService = alarmService;

            Hour = 6;
            Minute = 0;
            RepeatMon = true;
            RepeatTue = true;
            RepeatWed = true;
            RepeatThu = true;
            RepeatFri = true;
            RepeatSat = true;
            RepeatSun = true;

            SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
            CancelCommand = new Command(async () => await CancelAsync());
        }

        int _hour;
        public int Hour
        {
            get => _hour;
            set => SetProperty(ref _hour, value);
        }

        int _minute;
        public int Minute
        {
            get => _minute;
            set => SetProperty(ref _minute, value);
        }

        string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool RepeatMon { get; set; }
        public bool RepeatTue { get; set; }
        public bool RepeatWed { get; set; }
        public bool RepeatThu { get; set; }
        public bool RepeatFri { get; set; }
        public bool RepeatSat { get; set; }
        public bool RepeatSun { get; set; }

        bool _isVibrationEnabled = true;
        public bool IsVibrationEnabled
        {
            get => _isVibrationEnabled;
            set => SetProperty(ref _isVibrationEnabled, value);
        }

        string _soundName = "Default";
        public string SoundName
        {
            get => _soundName;
            set => SetProperty(ref _soundName, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Func<Task>? RequestClose;

        public async Task SaveAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var alarm = new Alarm
                {
                    Hours = Hour,
                    Minutes = Minute,
                    Name = Name,
                    RepeatMon = RepeatMon,
                    RepeatTue = RepeatTue,
                    RepeatWed = RepeatWed,
                    RepeatThu = RepeatThu,
                    RepeatFri = RepeatFri,
                    RepeatSat = RepeatSat,
                    RepeatSun = RepeatSun,
                    IsVibrationEnabled = IsVibrationEnabled,
                    SoundName = SoundName,
                    IsEnabled = true
                };

                await _alarmService.AddAsync(alarm);

                if (RequestClose != null)
                    await RequestClose.Invoke();
            }
            finally
            {
                IsBusy = false;
                (SaveCommand as Command)?.ChangeCanExecute();
            }
        }

        private Task CancelAsync()
        {
            return RequestClose?.Invoke() ?? Task.CompletedTask;
        }
    }
}
