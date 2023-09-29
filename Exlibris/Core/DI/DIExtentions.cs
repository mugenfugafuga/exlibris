using Exlibris.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Exlibris.Core.DI
{
    public static class DIExtentions
    {
        public static IServiceCollection Apply(this IServiceCollection services, DIConfiguration configuration)
        {
            foreach (var singleton in configuration.Singletons)
            {
                if (singleton.ImplementType != null)
                {
                    if (singleton.ServiceType != null)
                    {
                        services.AddSingleton(ReflectionUtil.GetType(singleton.ServiceType), ReflectionUtil.GetType(singleton.ImplementType));
                    }
                    else
                    {
                        services.AddSingleton(ReflectionUtil.GetType(singleton.ImplementType));
                    }
                }
            }

            foreach (var transient in configuration.Transients)
            {
                if (transient.ImplementType != null)
                {
                    if (transient.ServiceType != null)
                    {
                        services.AddTransient(ReflectionUtil.GetType(transient.ServiceType), ReflectionUtil.GetType(transient.ImplementType));
                    }
                    else
                    {
                        services.AddTransient(ReflectionUtil.GetType(transient.ImplementType));
                    }
                }
            }

            return services;
        }
    }
}