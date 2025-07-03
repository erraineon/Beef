using BabyGame.Data;
using BabyGame.Models;
using BabyGame.Services;
using NSubstitute;

namespace BabyGame.Tests;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
[TestClass]
public class BabyGachaServiceTests
{
    private IRandomProvider _randomProvider;
    private IBabyGameRepository _babyGameRepository;
    private ITimeProvider _timeProvider;
    private BabyGachaService _babyGachaService;
    private IBabyGameLogger _babyGameLogger;

    [TestInitialize]
    public void Initialize()
    {
        _randomProvider = Substitute.For<IRandomProvider>();
        _randomProvider.NextInt(null, Arg.Any<int>(), Arg.Any<int>()).Returns(123);
        _randomProvider.NextDouble(Arg.Any<Marriage>()).Returns(0.5);
        _randomProvider.NextItem(Arg.Any<Marriage>(), Arg.Any<Type[]>()).Returns(x => x.Arg<Type[]>().First());

        _babyGameRepository = Substitute.For<IBabyGameRepository>();

        _timeProvider = Substitute.For<ITimeProvider>();
        _timeProvider.Now.Returns(TimeUtils.AfterMarriage.AddDays(2));

        _babyGameLogger = new TestBabyGameLogger();

        _babyGachaService = new BabyGachaService(_randomProvider, _babyGameRepository, _timeProvider, _babyGameLogger);
    }

    [TestMethod]
    public async Task CreateBabyAsync_Works()
    {
        var marriage = MarriageUtils.GetMarriage();
        await _babyGachaService.CreateBabyAsync(marriage);
    }

    [TestMethod]
    public void GetRandomRarity_Works()
    {
        var marriage = MarriageUtils.GetMarriage();
        _randomProvider.NextDouble(marriage).Returns(
            BabyRarities.Common - 0.01,
            BabyRarities.Common + BabyRarities.Rare - 0.01,
            BabyRarities.Common + BabyRarities.Rare + BabyRarities.SuperRare - 0.01,
            BabyRarities.Common + BabyRarities.Rare + BabyRarities.SuperRare + BabyRarities.Legendary - 0.01
        );
        Assert.AreEqual(BabyRarities.Common, _babyGachaService.GetRandomRarity(marriage, out _));
        Assert.AreEqual(BabyRarities.Rare, _babyGachaService.GetRandomRarity(marriage, out _));
        Assert.AreEqual(BabyRarities.SuperRare, _babyGachaService.GetRandomRarity(marriage, out _));
        Assert.AreEqual(BabyRarities.Legendary, _babyGachaService.GetRandomRarity(marriage, out _));
    }

    [TestMethod]
    public void GetRandomRarity_Pity_Works()
    {
        var marriage = MarriageUtils.GetMarriage();
        marriage.Pity = 0.9;
        _randomProvider.NextDouble(marriage).Returns(
            0,
            0.1
        );
        Assert.AreEqual(BabyRarities.Common, _babyGachaService.GetRandomRarity(marriage, out var pityReset));
        Assert.IsFalse(pityReset);
        Assert.AreEqual(BabyRarities.Legendary, _babyGachaService.GetRandomRarity(marriage, out pityReset));
        Assert.IsTrue(pityReset);
    }
}