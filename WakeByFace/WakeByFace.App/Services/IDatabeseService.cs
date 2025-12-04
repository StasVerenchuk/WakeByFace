using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeByFace.App.Services
{
    public interface IDatabaseService
    {
        Task<SQLiteAsyncConnection> GetConnectionAsync();
    }
}
