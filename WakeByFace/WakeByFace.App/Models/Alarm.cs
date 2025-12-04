using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace WakeByFace.App.Models
{
    [AddINotifyPropertyChangedInterface]
    [SQLite.Table("Alarms")]
    public class Alarm
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Time
        public int Hours { get; set; }
        public int Minutes { get; set; }

        // Repeat alarm by days
        public bool RepeatMon { get; set; }
        public bool RepeatTue { get; set; }
        public bool RepeatWed { get; set; }
        public bool RepeatThu { get; set; }
        public bool RepeatFri { get; set; }
        public bool RepeatSat { get; set; }
        public bool RepeatSun { get; set; }

        // Name
        public string Name { get; set; } = string.Empty;

        // Configuration
        public string SoundName { get; set; } = "Default";
        public bool IsVibrationEnabled { get; set; } = true;
        public bool IsEnabled { get; set; } = true;

        // Service data
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        [Ignore]
        public string TimeFormatted => $"{Hours:00}:{Minutes:00}";


        [Ignore]
        public string RepeatDaysDisplay
        {
            get
            {
                var days = new List<string>();
                if (RepeatMon) days.Add("Пн");
                if (RepeatTue) days.Add("Вт");
                if (RepeatWed) days.Add("Ср");
                if (RepeatThu) days.Add("Чт");
                if (RepeatFri) days.Add("Пт");
                if (RepeatSat) days.Add("Сб");
                if (RepeatSun) days.Add("Нд");

                return days.Count == 0
                    ? "Одноразово"
                    : string.Join("  ", days);
            }
        }
    }
}
