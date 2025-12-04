using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeByFace.App.Models;
using WakeByFace.App.Services;

namespace WakeByFace.App.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly IAlarmService _alarmService;

        public ObservableCollection<Alarm> Alarms { get; } = new();

        public MainPageViewModel(IAlarmService alarmService)
        {
            _alarmService = alarmService;
        }

        public async Task LoadAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var items = await _alarmService.GetAllAsync();
                Alarms.Clear();

                foreach (var alarm in items)
                {
                    Alarms.Add(alarm);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public Task UpdateAlarmAsync(Alarm alarm)
        {
            return _alarmService.UpdateAsync(alarm);
        }
    }
}
