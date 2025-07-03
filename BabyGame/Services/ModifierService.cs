using System.ComponentModel;
using System.Reflection;
using BabyGame.Data;
using Humanizer;

namespace BabyGame.Services;

public class ModifierService(ITimeProvider timeProvider, IBabyGameLogger logger, IBabyGameRepository babyGameRepository)
    : IModifierService
{
    public async Task<bool> TryUseModifierAsync<TModifier>(Marriage marriage) where TModifier : Modifier
    {
        var modifier = GetModifierOrNull<TModifier>(marriage);
        if (modifier != null) await UseModifierAsync(modifier);
        return modifier != null;
    }

    public async Task UseModifierAsync<TModifier>(TModifier modifier) where TModifier : Modifier
    {
        if (modifier.ChargesLeft != null) modifier.ChargesLeft--;
        modifier.TimesUsed++;
        await babyGameRepository.SaveChangesAsync();
    }

    public async Task AddModifierAsync(Marriage marriage, Modifier modifier, bool log)
    {
        modifier.CreatedAt = timeProvider.Now;
        marriage.Modifiers.Add(modifier);
        await babyGameRepository.SaveChangesAsync();
        if (log)
        {
            var modifierType = modifier.GetType();
            var displayName = modifierType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                              modifierType.Name.Humanize();
            var description = modifierType.GetCustomAttribute<DescriptionAttribute>()?.Description ??
                              "A strange effect";
            var expiration = modifier.ChargesLeft != null
                ? $" for the next ${"time".ToQuantity(modifier.ChargesLeft.Value, ShowQuantityAs.Words)}"
                : modifier.EndsAt != null
                    ? $" for {(modifier.EndsAt - modifier.CreatedAt).Value.Humanize(3)}"
                    : string.Empty;
            logger.Log($"Received {displayName} - {description}{expiration}.");
        }
    }

    public TModifier? GetModifierOrNull<TModifier>(Marriage marriage) where TModifier : Modifier
    {
        var now = timeProvider.Now;
        var modifier = marriage.Modifiers
            .OfType<TModifier>()
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefault(x => x.ChargesLeft is null or > 0 && (x.EndsAt == null || x.EndsAt > now));
        return modifier;
    }
}