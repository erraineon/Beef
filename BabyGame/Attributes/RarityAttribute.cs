using BabyGame.Models;

namespace BabyGame.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RarityAttribute(double rarity = BabyRarities.Common) : Attribute
{
    public double Rarity { get; set; } = rarity;
}