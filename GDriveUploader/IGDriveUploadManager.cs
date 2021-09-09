using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GDriveUploader
{
    public interface IGDriveUploadManager
    {
        Task<bool> UploadToGoogleDrive(string uploadFilepath);
    }
}