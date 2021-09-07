using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DbBackup;
using GDriveUploader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AutoDbBackup
{
    class Program
    {
        async static Task Main(string[] args)
        {

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("appSettings.json", optional: false);
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostBuilderContext, configBuilder) =>
                {
                    configBuilder.AddJsonFile($"appSettings.json", false, true)
                        .AddJsonFile($"appSettings.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables();

                    if (hostBuilderContext.HostingEnvironment.IsDevelopment())
                    {
                        configBuilder.AddUserSecrets<Program>();
                    }

                })
                .ConfigureServices((hostContext, serviceCollection) =>
                {
                    serviceCollection.AddLogging((logger) => logger.AddSerilog())
                        .AddSingleton<IDbBackupManager, DbBackupManager>()
                        .AddSingleton<IGDriveUploadManager, GDriveUploadManager>()
                        .AddSingleton(typeof(RunTasks))
                        .BuildServiceProvider();

                    serviceCollection.AddHostedService<DbBackupManager>();

                })
                .Build();

            host.Run();
            


            //var serviceCollection = new ServiceCollection()
            //    .AddLogging((logger) => logger.AddSerilog())
            //    .AddSingleton<IDbBackupManager, DbBackupManager>()
            //    .AddSingleton<IGDriveUploadManager, GDriveUploadManager>()
            //    .AddSingleton(typeof(RunTasks))
            //    .BuildServiceProvider();
            ////.AddSingleton()

            //var env = AppContext.GetData("Environment");


            //var builder = new ConfigurationBuilder()
            //    .AddJsonFile($"AppSettings.json", false, true)
            //    .AddJsonFile($"AppSettings.{env}.json", true, true)
            //    .AddUserSecrets(Assembly.Load("AutoDbBackup"))
            //    .AddEnvironmentVariables();

            //var config = builder.Build();

            //Console.WriteLine("config" + config.GetValue<string>("GoogleDriveApi:ApiKeyJsonFile"));

            //ConfigurationManager
            Console.WriteLine("Hello World!");


            var result = await host.Services.GetService<IDbBackupManager>().StartDbBackup();

            //var result = await serviceCollection.GetService<IDbBackupManager>().StartDbBackup();

            // var taskService = serviceCollection.GetService(typeof(RunTasks));
        }
    }
}
