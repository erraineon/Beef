using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Models;
using BabyGame.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BabyGame.Tests.Services;

[TestClass]
[TestSubject(typeof(LoveService))]
public class LoveServiceTest
{
    private IOptionsSnapshot<IBabyGameConfiguration> _optionsSnapshot;
    private IBabyGameRepository _babyGameRepository;
    private IBabyGachaService _babyGachaService;
    private IEventDispatcher _eventDispatcher;
    private IBabyGameLogger _babyGameLogger;
    private ITimeProvider _timeProvider;
    private LoveService _loveService;
    private IBabyGameConfiguration _babyGameConfiguration;
    private Marriage _marriage;
    private Dictionary<Type, object> _eventDispatcherResults;

    [TestInitialize]
    public void Initialize()
    {
        _marriage = MarriageUtils.GetMarriage();
        _optionsSnapshot = Substitute.For<IOptionsSnapshot<IBabyGameConfiguration>>();

        _babyGameConfiguration = Substitute.For<IBabyGameConfiguration>();
        _babyGameConfiguration.MaxBabies.Returns(20);
        _babyGameConfiguration.BaseLoveCost.Returns(10);

        _optionsSnapshot.Value.Returns(_babyGameConfiguration);

        _babyGameRepository = Substitute.For<IBabyGameRepository>();
        _babyGameRepository.GetMarriageAsync(Arg.Any<Player>()).Returns(_marriage);

        _babyGachaService = Substitute.For<IBabyGachaService>();

        _eventDispatcherResults = new Dictionary<Type, object>();
        _eventDispatcher = new TestEventDispatcher(_eventDispatcherResults);
        _babyGameLogger = new TestBabyGameLogger();

        _timeProvider = Substitute.For<ITimeProvider>();
        _timeProvider.Now.Returns(TimeUtils.AfterMarriage);
        _timeProvider.Today.Returns(_ => _timeProvider.Now.Date);

        _loveService = new LoveService(
            _optionsSnapshot,
            _babyGameRepository,
            _babyGachaService,
            _eventDispatcher,
            _babyGameLogger,
            _timeProvider
        );
    }
    [TestMethod]
    public async Task Test()
    {
        _marriage.Chu = 100;
        _eventDispatcherResults[typeof(IChuCostMultiplierOnLove)] = 0.25;
        await _loveService.LoveAsync(_marriage.Spouse1, "Johnny");
    }
}