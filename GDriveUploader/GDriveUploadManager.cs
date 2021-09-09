using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GDriveUploader
{
    public class GDriveUploadManager : IGDriveUploadManager
    {
        private readonly GoogleDriveSettings _driveSettings;

        private readonly string[] Scopes = { DriveService.Scope.DriveFile };

        public GDriveUploadManager(IConfiguration configurationRoot)
        {
            _driveSettings = new GoogleDriveSettings();

            configurationRoot.Bind(GoogleDriveSettings.SectionName, _driveSettings);
        }

        public async Task<bool> UploadToGoogleDrive(string uploadFilepath)
        {
            Console.WriteLine("Checking google credentials");
            UserCredential credential;

            using (var stream = new FileStream(_driveSettings.ApiKeyJsonFile, FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            Console.WriteLine("Checking application api");
            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _driveSettings.ApplicationName,
            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();

            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }

            Console.WriteLine("Uploading files");
            
            var fileName = uploadFilepath.Split('\\', StringSplitOptions.TrimEntries).Last();

            using (var stream = new FileStream(uploadFilepath, FileMode.Open,
                FileAccess.Read))
            {
                var file = new Google.Apis.Drive.v3.Data.File();
                file.Name = fileName;
                
                var createFileRequest = service.Files.Create(file, stream,
                    "application/octet-stream");
                
                var uploadResult = await createFileRequest.UploadAsync();
                
            }

            return true;
        }
    }
}