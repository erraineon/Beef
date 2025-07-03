using BabyGame.Data;
using BabyGame.Modifiers;

namespace BabyGame;

public interface IModifierService
{
    Task<bool> TryUseModifierAsync<TModifier>(Marriage marriage) where TModifier : Modifier;
    TModifier? GetModifierOrNull<TModifier>(Marriage marriage) where TModifier : Modifier;
    Task UseModifierAsync<TModifier>(TModifier modifier) where TModifier : Modifier;
    Task AddModifierAsync(Marriage marriage, Modifier modifier, bool log);
}