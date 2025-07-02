using BabyGame.Data;

namespace BabyGame;

public static class BabyExtensions
{
    private static readonly string[] Ranks = ["F", "E", "D", "C", "B", "A", "S", "SS", "SSS"];

    public static string GetRank(this Baby baby) =>
        !baby.IsStarRank() ? Ranks[Math.Max(0, baby.Level)] : new string('★', baby.Level - 9);

    public static bool IsStarRank(this Baby baby)
    {
        return baby.Level >= 10;
    }
    public static bool IsMaxRank(this Baby baby)
    {
        return baby.Level >= 15;
    }
}