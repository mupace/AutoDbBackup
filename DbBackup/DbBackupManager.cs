using System;
using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DbBackup
{
    public class DbBackupManager : IDbBackupManager
    {
        private readonly DbSettings _dbSettings;

        private readonly SqlConnection _sqlConnection;

        public DbBackupManager(IConfiguration configurationRoot)
        {
            _dbSettings = new DbSettings();

            configurationRoot.Bind(DbSettings.SectionName, _dbSettings);

            _sqlConnection = new SqlConnection(configurationRoot.GetConnectionString(DbSettings.ConnectionStringName));
        }
        
        public async Task<bool> StartDbBackup()
        {
            await _sqlConnection.OpenAsync();

            var sqlCommand = new SqlCommand($"BACKUP DATABASE[{_dbSettings.DbName}] TO  DISK = '{_dbSettings.ExportDir}\\{_dbSettings.DbName}.{DateTime.UtcNow.Date.ToString("yyyyMMdd")}.bak'", _sqlConnection);

            var result = await sqlCommand.ExecuteNonQueryAsync();

            return result == 1;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var result = await StartDbBackup();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _sqlConnection.CloseAsync();
        }
    }
}