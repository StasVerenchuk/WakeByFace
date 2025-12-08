using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeByFace.App.Models;

namespace WakeByFace.App.Services
{
    public class AlarmService : IAlarmService
    {
        private readonly IDatabaseService _databaseService;

        public AlarmService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<IList<Alarm>> GetAllAsync()
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Alarm>()
                .OrderBy(a => a.Hours)
                .ThenBy(a => a.Minutes)
                .ToListAsync();
        }

        public async Task<Alarm?> GetByIdAsync(int id)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.FindAsync<Alarm>(id);
        }

        public async Task<int> AddAsync(Alarm alarm)
        {
            var db = await _databaseService.GetConnectionAsync();
            alarm.CreatedAtUtc = alarm.UpdatedAtUtc = DateTime.UtcNow;
            return await db.InsertAsync(alarm);
        }

        public async Task<int> UpdateAsync(Alarm alarm)
        {
            var db = await _databaseService.GetConnectionAsync();
            alarm.UpdatedAtUtc = DateTime.UtcNow;
            return await db.UpdateAsync(alarm);
        }

        public async Task<int> DeleteAsync(Alarm alarm)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.DeleteAsync(alarm);
        }
    }
}
