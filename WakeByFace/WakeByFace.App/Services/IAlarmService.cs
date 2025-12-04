using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeByFace.App.Models;

namespace WakeByFace.App.Services
{
    public interface IAlarmService
    {
        Task<IList<Alarm>> GetAllAsync();
        Task<Alarm?> GetByIdAsync(int id);
        Task<int> AddAsync(Alarm alarm);
        Task<int> UpdateAsync(Alarm alarm);
        Task<int> DeleteAsync(Alarm alarm);
    }
}
