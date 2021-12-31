using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;

namespace Beef.Core.Extensions;

public static class ConfigurationExtensions
{
    public static TOptions GetFromSection<TOptions>(this IConfiguration configuration)
    {
        var options = (TOptions)FormatterServices.GetUninitializedObject(typeof(TOptions));
        configuration.GetSection(typeof(TOptions).Name).Bind(options);
        return options;
    }
}