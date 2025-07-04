using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.Extensions;

namespace BabyGame.Tests.Services;

[TestClass]
[TestSubject(typeof(KissService))]
public class KissServiceTest
{
    private IOptionsSnapshot<IBabyGameConfiguration> _optionsSnapshot;
    private IModifierService _modifierService;
    private IBabyGameRepository _babyGameRepository;
    private IBabyGameLogger _babyGameLogger;
    private IEventDispatcher _eventDispatcher;
    private ITimeProvider _timeProvider;
    private KissService _kissService;
    private IAsyncDisposable _transaction;
    private Marriage _marriage;
    private IBabyGameConfiguration _babyGameConfiguration;
    private IEventAggregate<double> _cooldownMultiplierAggregate;
    private Dictionary<Type, object> _eventDispatcherResults;

    [TestInitialize]
    public void Initialize()
    {
        _marriage = MarriageUtils.GetMarriage();
        _optionsSnapshot = Substitute.For<IOptionsSnapshot<IBabyGameConfiguration>>();

        _babyGameConfiguration = Substitute.For<IBabyGameConfiguration>();
        _babyGameConfiguration.MinimumKissCooldown.Returns(TimeSpan.FromMinutes(1));
        _babyGameConfiguration.BaseKissCooldown.Returns(TimeSpan.FromMinutes(10));
        _babyGameConfiguration.MaxAffinityKissCooldownMultiplier.Returns(2);
        _babyGameConfiguration.AffinityKissCooldownMultiplierRate.Returns(4.5);

        _optionsSnapshot.Value.Returns(_babyGameConfiguration);
        _modifierService = Substitute.For<IModifierService>();
        _transaction = Substitute.For<IAsyncDisposable>();

        _babyGameRepository = Substitute.For<IBabyGameRepository>();
        _babyGameRepository.BeginTransactionAsync().Returns(_transaction);
        _babyGameRepository.GetMarriageAsync(Arg.Any<Player>()).Returns(_marriage);

        _babyGameLogger = new TestBabyGameLogger();

        _cooldownMultiplierAggregate = Substitute.For<IEventAggregate<double>>();

        _eventDispatcherResults = new Dictionary<Type, object>();
        _eventDispatcher = new TestEventDispatcher(_eventDispatcherResults);

        _timeProvider = Substitute.For<ITimeProvider>();
        _timeProvider.Now.Returns(TimeUtils.AfterMarriage);

        _kissService = new KissService(
            _optionsSnapshot,
            _modifierService,
            _babyGameRepository,
            _babyGameLogger,
            _eventDispatcher,
            _timeProvider
        );
    }
    [TestMethod]
    public async Task KissAsync_ChuCorrect_NoAffinity()
    {
        _marriage.Affinity = 50;
        _eventDispatcherResults[typeof(IKissCooldownMultiplierOnKiss)] = 0.5;
        _eventDispatcherResults[typeof(IChuOnKiss)] = 20.0;
        _eventDispatcherResults[typeof(IChuMultiplierOnKiss)] = 0.75;
        await _kissService.KissAsync(_marriage.Spouse1);
        // 20 + 10 (affinity) + 15 (aggregators)
        Assert.AreEqual(45M, _marriage.Chu);
        await _transaction.Received().DisposeAsync();
    }
}