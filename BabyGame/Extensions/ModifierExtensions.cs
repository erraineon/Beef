using System.ComponentModel;
using System.Reflection;
using BabyGame.Data;
using Humanizer;

namespace BabyGame.Extensions;

public static class ModifierExtensions
{
    public static string GetDisplayName(this Modifier modifier)
    {
        var type = modifier.GetType();
        var displayName = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                          type.Name.Humanize();
        return displayName;
    }
}