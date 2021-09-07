using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GDriveUploader
{
    public interface IGDriveUploadManager : IHostedService
    {
        Task<bool> UploadToGoogleDrive();
    }
}