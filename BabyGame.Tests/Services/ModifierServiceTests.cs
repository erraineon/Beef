using BabyGame.Modifiers;
using BabyGame.Services;
using NSubstitute;

namespace BabyGame.Tests.Services;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
[TestClass]
public class ModifierServiceTests
{
    private ITimeProvider _timeProvider;
    private IBabyGameLogger _babyGameLogger;
    private IBabyGameRepository _babyGameRepository;
    private ModifierService _modifierService;

    [TestInitialize]
    public void Initialize()
    {
        _timeProvider = Substitute.For<ITimeProvider>();
        _timeProvider.Now.Returns(TimeUtils.AfterMarriage.AddDays(2));

        _babyGameLogger = new TestBabyGameLogger();
        _babyGameRepository = Substitute.For<IBabyGameRepository>();

        _modifierService = new ModifierService(_timeProvider, _babyGameLogger);
    }

    [TestMethod]
    public void GetModifierOrNull_Works()
    {
        var marriage = MarriageUtils.GetMarriage();
        var skipLoveCostModifier = new SkipLoveCostModifier();
        marriage.Modifiers.Add(skipLoveCostModifier);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostModifier>(marriage);
        Assert.AreEqual(skipLoveCostModifier, foundModifier);
        var notFoundModifier = _modifierService.GetModifierOrNull<SkipKissCooldownModifier>(marriage);
        Assert.IsNull(notFoundModifier);
    }

    [TestMethod]
    public void GetModifierOrNull_RemainingCharges()
    {
        var marriage = MarriageUtils.GetMarriage();
        var skipLoveCostModifier = new SkipLoveCostModifier { ChargesLeft = 1 };
        marriage.Modifiers.Add(skipLoveCostModifier);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostModifier>(marriage);
        Assert.AreEqual(skipLoveCostModifier, foundModifier);
    }

    [TestMethod]
    public void GetModifierOrNull_NoRemainingCharges()
    {
        var marriage = MarriageUtils.GetMarriage();
        var skipLoveCostModifier = new SkipLoveCostModifier { ChargesLeft = 0 };
        marriage.Modifiers.Add(skipLoveCostModifier);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostModifier>(marriage);
        Assert.IsNull(foundModifier);
    }

    [TestMethod]
    public void GetModifierOrNull_RemainingTime()
    {
        var marriage = MarriageUtils.GetMarriage();
        var skipLoveCostModifier = new SkipLoveCostModifier
        {
            CreatedAt = TimeUtils.AfterMarriage, 
            EndsAt = TimeUtils.AfterMarriage.AddDays(3)
        };
        marriage.Modifiers.Add(skipLoveCostModifier);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostModifier>(marriage);
        Assert.AreEqual(skipLoveCostModifier, foundModifier);
    }

    [TestMethod]
    public void GetModifierOrNull_NoRemainingTime()
    {
        var marriage = MarriageUtils.GetMarriage();
        var skipLoveCostModifier = new SkipLoveCostModifier
        {
            CreatedAt = TimeUtils.AfterMarriage,
            EndsAt = TimeUtils.AfterMarriage.AddDays(1)
        };
        marriage.Modifiers.Add(skipLoveCostModifier);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostModifier>(marriage);
        Assert.IsNull(foundModifier);
    }

    [TestMethod]
    public void GetModifierOrNull_PrioritizesOlder()
    {
        var marriage = MarriageUtils.GetMarriage();
        var m1 = new SkipLoveCostModifier
        {
            CreatedAt = TimeUtils.AfterMarriage,
            ChargesLeft = 1,
        };
        var m2 = new SkipLoveCostModifier
        {
            CreatedAt = TimeUtils.AfterMarriage.AddDays(-1),
            ChargesLeft = 1
        };
        var m3 = new SkipLoveCostModifier
        {
            CreatedAt = TimeUtils.AfterMarriage.AddDays(-2),
            ChargesLeft = 0
        };
        marriage.Modifiers.AddRange([m1, m2, m3]);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostModifier>(marriage);
        Assert.AreEqual(m2, foundModifier);
    }

    [TestMethod]
    public void AddModifier_Works()
    {
        var marriage = MarriageUtils.GetMarriage();
        var modifier = new SkipLoveCostModifier { ChargesLeft = 3 };
        _modifierService.AddModifier(
            marriage,
            modifier,
            true
        );
        CollectionAssert.Contains(marriage.Modifiers, modifier);

        _modifierService.AddModifier(
            marriage,
            new SkipKissCooldownModifier { EndsAt = _timeProvider.Now.AddDays(1.5)},
            true
        );
    }

    [TestMethod]
    public void UseModifier_DepletesCharges()
    {
        var marriage = MarriageUtils.GetMarriage();
        var modifier = new SkipLoveCostModifier { ChargesLeft = 3 };
        _modifierService.UseModifier(modifier);
        Assert.AreEqual(2, modifier.ChargesLeft);
        Assert.AreEqual(1, modifier.TimesUsed);
    }
}