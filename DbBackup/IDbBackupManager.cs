using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace DbBackup
{
    public interface IDbBackupManager : IHostedService
    {
        Task<bool> StartDbBackup();
    }
}