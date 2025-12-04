using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeByFace.App.Models;

namespace WakeByFace.App.Services
{
    public class DatabaseService : IDatabaseService
    {
        private const string DatabaseFileName = "wakebyface.db3";
        private SQLiteAsyncConnection? _connection;

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_connection != null)
                return _connection;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

            _connection = new SQLiteAsyncConnection(dbPath,
                SQLiteOpenFlags.ReadWrite |
                SQLiteOpenFlags.Create |
                SQLiteOpenFlags.SharedCache);

            await _connection.CreateTableAsync<Alarm>();

            return _connection;
        }
    }
}
