using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Security.Options;

public static class OptionsFromConfigurationExtensions {
    public static void AddOptionsFromConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : OptionsFromConfiguration {
        T optionsInstance = (T)Activator.CreateInstance(typeof(T));
        string position = optionsInstance?.Position;
        if (position is null) {
            throw new ArgumentException($"""Configuration "{typeof(T).Name}" has no position configured!""");
        }
        
        services.Configure((Action<T>)(options => {
            IConfigurationSection section = configuration.GetSection(position);
            section.Bind(options);
        }));
    }
}