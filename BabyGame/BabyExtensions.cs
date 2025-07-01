using BabyGame.Data;

namespace BabyGame;

public static class BabyExtensions
{
    private static string[] Ranks = ["F", "E", "D", "C", "B", "A", "S", "SS", "SSS"];

    public static string GetRank(this Baby baby) =>
        baby.Level < 10 ? Ranks[Math.Max(0, baby.Level)] : new string('★', baby.Level - 9);
}