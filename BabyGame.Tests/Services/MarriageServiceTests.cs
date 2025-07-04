using BabyGame.Data;
using BabyGame.Exceptions;
using BabyGame.Modifiers;
using BabyGame.Services;
using NSubstitute;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace BabyGame.Tests.Services;

[TestClass]
public class MarriageServiceTests
{
    private IBabyGameConfiguration _babyGameConfiguration;
    private IBabyGameLogger _babyGameLogger;
    private IBabyGameRepository _babyGameRepository;
    private IModifierService _modifierService;
    private IRandomProvider _randomProvider;
    private ITimeProvider _timeProvider;
    private MarriageService _marriageService;

    [TestInitialize]
    public void Initialize()
    {
        _babyGameConfiguration = Substitute.For<IBabyGameConfiguration>();
        _babyGameConfiguration.MaxInitialAffinity = 10;

        _babyGameRepository = Substitute.For<IBabyGameRepository>();

        _modifierService = Substitute.For<IModifierService>();

        _babyGameLogger = new TestBabyGameLogger();

        _randomProvider = Substitute.For<IRandomProvider>();
        _randomProvider.NextInt(null, Arg.Any<int>(), Arg.Any<int>()).Returns(123);
        _randomProvider
            .NextInt(Arg.Is<Marriage?>(x => x != null), Arg.Any<int>(), Arg.Any<int>())
            .Returns(x => x.ArgAt<int>(2));

        _timeProvider = Substitute.For<ITimeProvider>();
        _timeProvider.Now.Returns(TimeUtils.MarriageDay);

        _marriageService = new MarriageService(
            _babyGameConfiguration,
            _babyGameRepository,
            _modifierService,
            _babyGameLogger,
            _randomProvider,
            _timeProvider
        );
    }

    [TestMethod]
    public async Task Marriage_Works()
    {
        var spouse1 = PlayerUtils.GetAlice();
        var spouse2 = PlayerUtils.GetBob();
        var marriage = await _marriageService.MarryAsync(spouse1, spouse2);
        Assert.AreEqual(_timeProvider.Now, marriage.MarriedAt);
        Assert.IsNull(marriage.LastKissedAt);
        Assert.IsNull(marriage.LastLovedOn);
    }
    [TestMethod]
    public async Task Marriage_SetsAffinity()
    {
        _randomProvider
            .NextInt(Arg.Is<Marriage?>(x => x != null), Arg.Any<int>(), Arg.Any<int>())
            .Returns(3);
        var spouse1 = PlayerUtils.GetAlice();
        var spouse2 = PlayerUtils.GetBob();
        var marriage = await _marriageService.MarryAsync(spouse1, spouse2);
        Assert.AreEqual(3, marriage.Affinity);
    }

    [TestMethod]
    public async Task Marriage_SetsChu()
    {
        var spouse1 = PlayerUtils.GetAlice();
        var spouse2 = PlayerUtils.GetBob();
        _randomProvider
            .NextInt(Arg.Is<Marriage?>(x => x != null), Arg.Any<int>(), Arg.Any<int>())
            .Returns(3);
        var marriage = await _marriageService.MarryAsync(spouse1, spouse2);
        Assert.AreEqual(3, marriage.Chu);
    }

    [TestMethod]
    public async Task Marriage_GetsSaved()
    {
        var spouse1 = PlayerUtils.GetAlice();
        var spouse2 = PlayerUtils.GetBob();
        var marriage = await _marriageService.MarryAsync(spouse1, spouse2);
        await _babyGameRepository.Received().CreateMarriageAsync(marriage);
    }

    [TestMethod]
    public async Task Marriage_Adds_SkipLoveCostModifier()
    {
        var spouse1 = PlayerUtils.GetAlice();
        var spouse2 = PlayerUtils.GetBob();
        var marriage = await _marriageService.MarryAsync(spouse1, spouse2);
        await _modifierService
            .Received()
            .AddModifierAsync(marriage, Arg.Is<SkipLoveCostModifier>(m => m.ChargesLeft == 1), false);
    }

    [TestMethod]
    public async Task Marriage_Throws_AlreadyMarried()
    {
        _babyGameRepository.GetIsMarriedAsync(Arg.Any<Player>()).Returns(true);
        var spouse1 = PlayerUtils.GetAlice();
        var spouse2 = PlayerUtils.GetBob();
        await Assert.ThrowsExceptionAsync<AlreadyMarriedException>(() => _marriageService.MarryAsync(spouse1, spouse2));
    }

    [TestMethod]
    public async Task Marriage_Throws_NoSelfMarriage()
    {
        var spouse1 = PlayerUtils.GetAlice();
        var spouse2 = PlayerUtils.GetAlice();
        await Assert.ThrowsExceptionAsync<NoSelfMarriage>(() => _marriageService.MarryAsync(spouse1, spouse2));
    }
}