using System;
using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using GDriveUploader;

namespace DbBackup
{
    public class DbBackupManager : IDbBackupManager
    {
        private readonly DbSettings _dbSettings;

        private readonly SqlConnection _sqlConnection;

        private readonly IGDriveUploadManager _gDriveUploadManager;

        private string _dbBackupName;

        public DbBackupManager(IConfiguration configurationRoot, IGDriveUploadManager gDriveUploadManager)
        {
            _dbSettings = new DbSettings();

            configurationRoot.Bind(DbSettings.SectionName, _dbSettings);

            _sqlConnection = new SqlConnection(configurationRoot.GetConnectionString(DbSettings.ConnectionStringName));

            _gDriveUploadManager = gDriveUploadManager;
        }
        
        public async Task<bool> StartDbBackup()
        {
            _dbBackupName = $"{_dbSettings.ExportDir}\\{_dbSettings.DbName}.{DateTime.UtcNow.Date.ToString("yyyyMMdd")}.bak";
            
            Console.WriteLine($"Exporting db to {_dbBackupName}");
            
            await _sqlConnection.OpenAsync();

            var sqlCommand = new SqlCommand($"BACKUP DATABASE[{_dbSettings.DbName}] TO  DISK = '{_dbBackupName}'", _sqlConnection);

            var result = await sqlCommand.ExecuteNonQueryAsync();

            Console.WriteLine("Db backup is finished.");
            
            return result == 1;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Backup started");
            
            await StartDbBackup();

            Console.WriteLine("Starting upload");
            
            await _gDriveUploadManager.UploadToGoogleDrive(_dbBackupName);
            
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _sqlConnection.CloseAsync();
        }
    }
}