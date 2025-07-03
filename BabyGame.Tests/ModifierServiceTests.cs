using BabyGame.Data;
using BabyGame.Modifiers;
using BabyGame.Services;
using NSubstitute;

namespace BabyGame.Tests;

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

        _modifierService = new ModifierService(_timeProvider, _babyGameLogger, _babyGameRepository);
    }

    [TestMethod]
    public void GetModifierOrNull_Works()
    {
        var marriage = MarriageUtils.GetMarriage();
        var skipLoveCostModifier = new SkipLoveCostBuff();
        marriage.Modifiers.Add(skipLoveCostModifier);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostBuff>(marriage);
        Assert.AreEqual(skipLoveCostModifier, foundModifier);
        var notFoundModifier = _modifierService.GetModifierOrNull<SkipKissCooldownBuff>(marriage);
        Assert.IsNull(notFoundModifier);
    }

    [TestMethod]
    public void GetModifierOrNull_RemainingCharges()
    {
        var marriage = MarriageUtils.GetMarriage();
        var skipLoveCostModifier = new SkipLoveCostBuff { ChargesLeft = 1 };
        marriage.Modifiers.Add(skipLoveCostModifier);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostBuff>(marriage);
        Assert.AreEqual(skipLoveCostModifier, foundModifier);
    }

    [TestMethod]
    public void GetModifierOrNull_NoRemainingCharges()
    {
        var marriage = MarriageUtils.GetMarriage();
        var skipLoveCostModifier = new SkipLoveCostBuff { ChargesLeft = 0 };
        marriage.Modifiers.Add(skipLoveCostModifier);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostBuff>(marriage);
        Assert.IsNull(foundModifier);
    }

    [TestMethod]
    public void GetModifierOrNull_RemainingTime()
    {
        var marriage = MarriageUtils.GetMarriage();
        var skipLoveCostModifier = new SkipLoveCostBuff
        {
            CreatedAt = TimeUtils.AfterMarriage, 
            EndsAt = TimeUtils.AfterMarriage.AddDays(3)
        };
        marriage.Modifiers.Add(skipLoveCostModifier);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostBuff>(marriage);
        Assert.AreEqual(skipLoveCostModifier, foundModifier);
    }

    [TestMethod]
    public void GetModifierOrNull_NoRemainingTime()
    {
        var marriage = MarriageUtils.GetMarriage();
        var skipLoveCostModifier = new SkipLoveCostBuff
        {
            CreatedAt = TimeUtils.AfterMarriage,
            EndsAt = TimeUtils.AfterMarriage.AddDays(1)
        };
        marriage.Modifiers.Add(skipLoveCostModifier);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostBuff>(marriage);
        Assert.IsNull(foundModifier);
    }

    [TestMethod]
    public void GetModifierOrNull_PrioritizesOlder()
    {
        var marriage = MarriageUtils.GetMarriage();
        var m1 = new SkipLoveCostBuff
        {
            CreatedAt = TimeUtils.AfterMarriage,
            ChargesLeft = 1,
        };
        var m2 = new SkipLoveCostBuff
        {
            CreatedAt = TimeUtils.AfterMarriage.AddDays(-1),
            ChargesLeft = 1
        };
        var m3 = new SkipLoveCostBuff
        {
            CreatedAt = TimeUtils.AfterMarriage.AddDays(-2),
            ChargesLeft = 0
        };
        marriage.Modifiers.AddRange([m1, m2, m3]);
        var foundModifier = _modifierService.GetModifierOrNull<SkipLoveCostBuff>(marriage);
        Assert.AreEqual(m2, foundModifier);
    }

    [TestMethod]
    public async Task AddModifierAsync_Works()
    {
        var marriage = MarriageUtils.GetMarriage();
        var modifier = new SkipLoveCostBuff { ChargesLeft = 3 };
        await _modifierService.AddModifierAsync(
            marriage,
            modifier,
            true
        );
        CollectionAssert.Contains(marriage.Modifiers, modifier);
        await _babyGameRepository.Received().SaveChangesAsync();

        await _modifierService.AddModifierAsync(
            marriage,
            new SkipKissCooldownBuff { EndsAt = _timeProvider.Now.AddDays(1.5)},
            true
        );
    }

    [TestMethod]
    public async Task UseModifierAsync_DepletesCharges()
    {
        var marriage = MarriageUtils.GetMarriage();
        var modifier = new SkipLoveCostBuff { ChargesLeft = 3 };
        await _modifierService.UseModifierAsync(modifier);
        await _babyGameRepository.Received().SaveChangesAsync();
        Assert.AreEqual(2, modifier.ChargesLeft);
    }
}