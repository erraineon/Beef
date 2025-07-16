using BabyGame.Events;
using BabyGame.Services;

namespace BabyGame.Modifiers;

public class SkipLoveCostModifierService(ModifierService modifierService) : IEventHandler<IMarriageComplete, int>
{
    public IEnumerable<int> Handle(BabyEventArgs<IMarriageComplete, int> eventArgs)
    {
        modifierService.AddModifier(eventArgs.Marriage, new SkipLoveCostModifier { ChargesLeft = 1 }, false);
        yield return 1;
    }
}