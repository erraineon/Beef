using System.ComponentModel;
using System.Reflection;
using BabyGame.Data;
using BabyGame.Extensions;
using Humanizer;

namespace BabyGame.Services;

public class ModifierService(ITimeProvider timeProvider, IBabyGameLogger logger)
    : IModifierService
{
    public bool TryUseModifier<TModifier>(Marriage marriage) where TModifier : Modifier
    {
        var modifier = GetModifierOrNull<TModifier>(marriage);
        if (modifier != null) UseModifier(modifier);
        return modifier != null;
    }

    public void UseModifier<TModifier>(TModifier modifier) where TModifier : Modifier
    {
        if (modifier.ChargesLeft != null) modifier.ChargesLeft--;
        modifier.TimesUsed++;
    }

    public void AddModifier(Marriage marriage, Modifier modifier, bool log)
    {
        modifier.CreatedAt = timeProvider.Now;
        marriage.Modifiers.Add(modifier);
        if (log)
        {
            var description = modifier.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description ??
                              "A mysterious effect";
            var expiration = modifier.ChargesLeft != null
                ? $" for the next {"time".ToQuantity(modifier.ChargesLeft.Value, ShowQuantityAs.Words)}"
                : modifier.EndsAt != null
                    ? $" for {(modifier.EndsAt - modifier.CreatedAt).Value.Humanize(3)}"
                    : string.Empty;
            logger.Log($"Received {modifier.GetDisplayName()} - {description}{expiration}.");
        }
    }

    public TModifier? GetModifierOrNull<TModifier>(Marriage marriage) where TModifier : Modifier
    {
        var activeModifiers = GetActiveModifiers<TModifier>(marriage);
        var modifier = activeModifiers.FirstOrDefault();
        return modifier;
    }

    public IEnumerable<TModifier> GetActiveModifiers<TModifier>(Marriage marriage) where TModifier : Modifier
    {
        var now = timeProvider.Now;
        var activeModifiers = marriage.Modifiers
            .OfType<TModifier>()
            .OrderBy(x => x.CreatedAt)
            .Where(x => GetIsActive(x, now));
        return activeModifiers;
    }

    private bool GetIsActive<TModifier>(TModifier value, DateTimeOffset now) where TModifier : Modifier
    {
        return value.ChargesLeft is null or > 0 && (value.EndsAt == null || value.EndsAt > now);
    }
}