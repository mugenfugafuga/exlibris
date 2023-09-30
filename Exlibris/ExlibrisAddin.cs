using ExcelDna.Integration;
using ExcelDna.IntelliSense;
using Exlibris.Configuration;
using Exlibris.Core;
using Exlibris.Core.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Exlibris
{
    public class ExlibrisAddin : IExcelAddIn
    {
        private static readonly ExlibrisLock exlibrisLock = new ExlibrisLock();
        private static IServiceProvider serviceProvider;

        private static void SetUpAddin(ExlibrisConfiguration configuration)
        {
            var srv = new ServiceCollection()
                .AddSingleton(configuration)
                .Apply(configuration.DIConfiguration);

            using (var _ = exlibrisLock.GetWriteLock())
            {
                serviceProvider = srv.BuildServiceProvider();
            }
        }

        public static ExlibrisExcelFunctionSupport GetFunctionSupport(
            [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerName = "")
        {
            using (var _ = exlibrisLock.GetReadLock())
            {
                var context = serviceProvider?.GetRequiredService<ExlibrisExcelFunctionSupport>()
                    ?? throw new NullReferenceException();
                context.ExcelFunctionName = $"{callerFilePath}.{callerName}";
                return context;
            }
        }

        public void AutoClose()
        {
            IntelliSenseServer.Uninstall();
        }

        public void AutoOpen()
        {
            IntelliSenseServer.Install();

            SetUpAddin(LoadConfiguration());
        }

        private static ExlibrisConfiguration LoadConfiguration()
        {
            var baseDir = ExcelDnaUtil.XllPathInfo.Directory?.FullName;

            try
            {
                if (baseDir != null)
                {
                    var config = new ConfigurationBuilder()
                        .SetBasePath(baseDir)
                        .AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables()
                        .Build();

                    var exlibrisConfiguration = config.Get<ExlibrisConfiguration>();
                    if (exlibrisConfiguration != null)
                    {
                        return exlibrisConfiguration;
                    }
                }
            }
            catch
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    Converters = new JsonConverter[] { new StringEnumConverter()},
                };

                var js= JsonSerializer.Create(jsonSerializerSettings);
                
                using (var stream = new FileStream($"{baseDir}\\appsettings.json", FileMode.Open, FileAccess.Read))
                using (var sr = new StreamReader(stream))
                using (var reader = new JsonTextReader(sr))
                {
                    return js.Deserialize<ExlibrisConfiguration>(reader);
                }
            }

            throw new InvalidOperationException("failed to load appsettings.json");
        }
    }
}