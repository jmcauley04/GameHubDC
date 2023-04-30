using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GameHub.Blazor.Shared;

public static class IServiceInjector
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        foreach (var type in GetTypesWithHelpAttribute(AppDomain.CurrentDomain.GetAssemblies()))
        {
            switch (type.injectionType)
            {
                case InjectableAttribute.InjectionType.Transient:
                    services.AddTransient(type.serviceType);
                    break;
                case InjectableAttribute.InjectionType.Scoped:
                    services.AddScoped(type.serviceType);
                    break;
                case InjectableAttribute.InjectionType.Singleton:
                    services.AddSingleton(type.serviceType);
                    break;
            }
            Console.WriteLine($"Injected type {type.serviceType.Name}");
        }

        return services;
    }

    static IEnumerable<(Type serviceType, InjectableAttribute.InjectionType injectionType)> GetTypesWithHelpAttribute(Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
            foreach (Type type in assembly.GetTypes())
            {
                var attributes = type.GetCustomAttributes(typeof(InjectableAttribute), true);
                if (attributes.Length > 0)
                {
                    var attribute = (InjectableAttribute)attributes[0];
                    yield return (type, attribute.Type);
                }
            }
    }
}
