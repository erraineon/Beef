using System.ComponentModel;
using System.Reflection;
using BabyGame.Data;
using Humanizer;

namespace BabyGame.Extensions;

public static class BabyExtensions
{
    private static readonly string[] Ranks = ["F", "E", "D", "C", "B", "A", "S", "SS", "SSS"];

    public static string GetRank(this Baby baby) =>
        !baby.IsStarRank() ? Ranks[Math.Max(1, baby.Level) - 1] : new string('★', baby.Level - 9);

    public static bool IsStarRank(this Baby baby)
    {
        return baby.Level >= 10;
    }
    public static bool IsMaxRank(this Baby baby)
    {
        return baby.Level >= 15;
    }

    public static string GetTypeName(this Baby baby)
    {
        return baby.GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
               baby.GetType().Name.Humanize();
    }
}