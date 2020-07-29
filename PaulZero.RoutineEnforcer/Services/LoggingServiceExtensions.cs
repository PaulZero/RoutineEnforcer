using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using System.IO;

namespace PaulZero.RoutineEnforcer.Services
{
    public static class LoggingServiceExtensions
    {
        public static IServiceCollection AddRoutineEnforcerLogging(this IServiceCollection services)
        {
            services.AddLogging(o =>
            {
                var appDataDirectory = PathUtilities.GetProgramDataDirectory();

                o.ClearProviders();
                o.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

                var config = new LoggingConfiguration();
                var fileTarget = new FileTarget
                {
                    Name = "file",
                    FileName = Path.Combine(appDataDirectory, "RoutineEnforcer.log"),
                    FileNameKind = FilePathKind.Absolute,
                    ArchiveOldFileOnStartup = true,
                    AutoFlush = true,
                    ArchiveFileName = Path.Combine(appDataDirectory, "RoutineEnforcer.{#}.log"),
                    ArchiveFileKind = FilePathKind.Absolute,
                    ArchiveNumbering = ArchiveNumberingMode.Date,
                    ArchiveDateFormat = "yyyy-MM-dd",
                    ArchiveEvery = FileArchivePeriod.Day,
                    MaxArchiveFiles = 7
                };

                config.AddTarget(fileTarget);
                config.AddRuleForAllLevels(fileTarget);

                LogManager.Configuration = config;

                o.AddNLog(LogManager.Configuration);
            });

            return services;
        }
    }
}
