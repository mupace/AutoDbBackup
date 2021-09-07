using System.Threading.Tasks;

namespace GDriveUploader
{
    public interface IGDriveUploadManager
    {
        Task<bool> UploadToGoogleDrive();
    }
}