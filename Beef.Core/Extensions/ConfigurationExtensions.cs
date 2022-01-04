using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;

namespace Beef.Core.Extensions;

public static class ConfigurationExtensions
{
    public static TOptions? GetFromSection<TOptions>(this IConfiguration configuration)
    {
        if (configuration.GetSection(typeof(TOptions).Name) is { } section && section.Exists())
        {
            var options = (TOptions)FormatterServices.GetUninitializedObject(typeof(TOptions));
            section.Bind(options);
            return options;
        }

        return default;
    }
}