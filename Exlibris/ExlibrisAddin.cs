using ExcelDna.Integration;
using Exlibris.Configuration;
using Exlibris.Core;
using Exlibris.Core.JSONs;
using Exlibris.Excel;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Exlibris;

public class ExlibrisAddin : IExcelAddIn
{
    private static readonly Atomic<ExlibrisConfiguration> configuration = new(new ExlibrisConfiguration());
    private static IServiceCollection? services;
    private static IServiceProvider? serviceProvider;

    public static IServiceCollection Services
    {
        set
        {
            services = value;
            serviceProvider = services.BuildServiceProvider();
        }
    }

    public static ExlibrisExcelFunctionSupport GetFunctionSupport(
    [CallerMemberName] string callerName = "")
    {
        var context = serviceProvider?.GetRequiredService<ExlibrisExcelFunctionSupport>()
            ?? throw new NullReferenceException();
        context.ExcelFunctionName = callerName;
        return context;
    }

    public static ExlibrisConfiguration ExlibrisConfiguration
    {
        get => configuration.Value;
        set => configuration.Value = value;
    }

    public void AutoClose()
    {
    }

    public void AutoOpen()
    {
        var services = new ServiceCollection();
        services.AddSingleton(ObjectRegistryFactory.NewConcurrentObjectRegistry());
        services.AddSingleton(JSONParserManager.GetJSONParser());
        services.AddSingleton(() => ExlibrisConfiguration);
        services.AddSingleton(new ThrownExceptions());

        services.AddTransient<ExlibrisExcelFunctionSupport>();

        Services = services;
    }
}
