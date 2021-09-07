using DbBackup;

namespace AutoDbBackup
{
    public class RunTasks
    {
        private readonly IDbBackupManager _dbBackupManager;

        public RunTasks(IDbBackupManager dbBackupManager)
        {
            _dbBackupManager = dbBackupManager;
        }

        public void StartDbBackup()
        {

        }
    }
}