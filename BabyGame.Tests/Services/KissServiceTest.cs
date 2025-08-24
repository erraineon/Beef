using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Exceptions;
using BabyGame.Models;
using BabyGame.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BabyGame.Tests.Services;

[TestClass]
[TestSubject(typeof(KissService))]
public class KissServiceTest
{
    private IOptionsSnapshot<BabyGameOptions> _optionsSnapshot;
    private IBabyGameRepository _babyGameRepository;
    private IBabyGameLogger _babyGameLogger;
    private IEventDispatcher _eventDispatcher;
    private ITimeProvider _timeProvider;
    private KissService _kissService;
    private Marriage _marriage;
    private BabyGameOptions _babyGameOptions;
    private Dictionary<Type, object> _eventDispatcherResults;

    [TestInitialize]
    public void Initialize()
    {
        _marriage = MarriageUtils.GetMarriage();
        _optionsSnapshot = Substitute.For<IOptionsSnapshot<BabyGameOptions>>();

        _babyGameOptions = Substitute.For<BabyGameOptions>();
        _babyGameOptions.MinimumKissCooldown.Returns(TimeSpan.FromMinutes(1));
        _babyGameOptions.BaseKissCooldown.Returns(TimeSpan.FromMinutes(10));
        _babyGameOptions.MaxAffinityKissCooldownMultiplier.Returns(2);
        _babyGameOptions.AffinityKissCooldownMultiplierRate.Returns(4.5);

        _optionsSnapshot.Value.Returns(_babyGameOptions);

        _babyGameRepository = Substitute.For<IBabyGameRepository>();
        _babyGameRepository.GetMarriageAsync(Arg.Any<Player>()).Returns(_marriage);

        _babyGameLogger = new TestBabyGameLogger();

        _eventDispatcherResults = new Dictionary<Type, object>();
        _eventDispatcher = new TestEventDispatcher(_eventDispatcherResults);

        _timeProvider = Substitute.For<ITimeProvider>();
        _timeProvider.Now.Returns(TimeUtils.AfterMarriage);

        _kissService = new KissService(
            _optionsSnapshot,
            _babyGameRepository,
            _babyGameLogger,
            _eventDispatcher,
            _timeProvider
        );
    }

    [TestMethod]
    public async Task KissAsync_ChuCorrect()
    {
        _eventDispatcherResults[typeof(IKissCooldownMultiplierOnKiss)] = 0.5;
        _eventDispatcherResults[typeof(IChuOnKiss)] = 20.0;
        _eventDispatcherResults[typeof(IChuMultiplierOnKiss)] = 0.75;
        await _kissService.KissAsync(_marriage.Spouse1);
        // 20 + 15 (aggregators)
        Assert.AreEqual(35M, _marriage.Chu);
    }

    [TestMethod]
    public async Task KissAsync_Throws_OnCooldown()
    {
        _marriage.LastKissedAt = _timeProvider.Now - _babyGameOptions.BaseKissCooldown / 2;
        await Assert.ThrowsAsync<KissOnCooldownException>(() => _kissService.KissAsync(_marriage.Spouse1));
    }
}