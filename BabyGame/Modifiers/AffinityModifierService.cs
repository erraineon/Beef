using BabyGame.Events;
using Humanizer;
using Microsoft.Extensions.Options;

namespace BabyGame.Modifiers;

public class AffinityModifierService(
    IOptionsSnapshot<IBabyGameConfiguration> configuration,
    ITimeProvider timeProvider,
    IModifierService modifierService,
    IBabyGameLogger babyGameLogger,
    IRandomProvider randomProvider)
    : IEventHandler<AffinityModifier, IKissComplete, int>,
        IEventHandler<AffinityModifier, IKissCooldownMultiplierOnKiss, double>,
        IEventHandler<IMarriageComplete, int>
{
    public IEnumerable<int> Handle(AffinityModifier proxy, BabyEventArgs<IKissComplete, int> eventArgs)
    {
        var now = timeProvider.Now;
        var lastKissedAt = proxy.Marriage.LastKissedAt;
        var firstKiss = lastKissedAt == null;
        if (firstKiss || now - lastKissedAt >= TimeSpan.FromDays(1)) proxy.Effectiveness++;
        yield return 1;
    }

    public IEnumerable<double> Handle(AffinityModifier proxy,
        BabyEventArgs<IKissCooldownMultiplierOnKiss, double> eventArgs)
    {
        // f(x)=1-a+\frac{a}{1+b\cdot\frac{x}{1000}}
        // At affinity 30, 90, 360, 100: 0.92, 0.8, 0.6, 0.4 
        var a = configuration.Value.MaxAffinityKissCooldownMultiplier;
        var b = configuration.Value.AffinityKissCooldownMultiplierRate;
        var x = proxy.Effectiveness;
        yield return 1 - a + a / (1 + b * (x / 1000.0));
    }

    public IEnumerable<int> Handle(BabyEventArgs<IMarriageComplete, int> eventArgs)
    {
        var marriage = eventArgs.Marriage;
        var affinity = randomProvider.NextInt(marriage, 1, configuration.Value.MaxInitialAffinity);
        marriage.Modifiers.Add(new AffinityModifier { Effectiveness = affinity });

        // TODO: stretch between 1 and 1000
        var compatibility = affinity switch
        {
            <= 1 => "Awful!",
            <= 5 => "Bad...",
            <= 10 => "Good!",
            <= 15 => "Fantastic!!",
            _ => "Unexpected."
        };
        babyGameLogger.Log(
            $"{marriage.Spouse1.DisplayName} and {marriage.Spouse2.DisplayName} are now married. " +
            $"Their compatibility is... {compatibility} It's as if they've known " +
            $"each other for {"day".ToQuantity(affinity, ShowQuantityAs.Words)}."
        );

        yield return 1;
    }
}