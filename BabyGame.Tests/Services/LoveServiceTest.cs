using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Models;
using BabyGame.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using NSubstitute;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace BabyGame.Tests.Services;

[TestClass]
[TestSubject(typeof(LoveService))]
public class LoveServiceTest
{
    private IOptionsSnapshot<BabyGameOptions> _optionsSnapshot;
    private IBabyGameRepository _babyGameRepository;
    private IBabyGachaService _babyGachaService;
    private IEventDispatcher _eventDispatcher;
    private IBabyGameLogger _babyGameLogger;
    private ITimeProvider _timeProvider;
    private LoveService _loveService;
    private BabyGameOptions _babyGameOptions;
    private Marriage _marriage;
    private Dictionary<Type, object> _eventDispatcherResults;

    [TestInitialize]
    public void Initialize()
    {
        _marriage = MarriageUtils.GetMarriage();
        _optionsSnapshot = Substitute.For<IOptionsSnapshot<BabyGameOptions>>();

        _babyGameOptions = Substitute.For<BabyGameOptions>();
        _babyGameOptions.MaxBabies.Returns(20);
        _babyGameOptions.BaseLoveCost.Returns(10);

        _optionsSnapshot.Value.Returns(_babyGameOptions);

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
    public async Task LoveAsync_Works()
    {
        _marriage.Chu = 100;
        _eventDispatcherResults[typeof(IChuCostMultiplierOnLove)] = 0.25;
        await _loveService.LoveAsync(_marriage.Spouse1, "Johnny");
    }
}