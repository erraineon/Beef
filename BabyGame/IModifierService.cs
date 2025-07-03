using BabyGame.Data;
using BabyGame.Modifiers;

namespace BabyGame;

public interface IModifierService
{
    bool TryUseModifier<TModifier>(Marriage marriage) where TModifier : Modifier;
    TModifier? GetModifierOrNull<TModifier>(Marriage marriage) where TModifier : Modifier;
    Task AddModifierAsync(Marriage marriage, Modifier modifier, bool log);
    IEnumerable<TModifier> GetActiveModifiers<TModifier>(Marriage marriage) where TModifier : Modifier;
    void UseModifier<TModifier>(TModifier modifier) where TModifier : Modifier;
}