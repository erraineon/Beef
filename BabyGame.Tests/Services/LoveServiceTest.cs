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

    [TestInitialize]
    public void Initialize()
    {
        _optionsSnapshot = Substitute.For<IOptionsSnapshot<IBabyGameConfiguration>>();
        _babyGameRepository = Substitute.For<IBabyGameRepository>();
        _babyGachaService = Substitute.For<IBabyGachaService>();
        _eventDispatcher = Substitute.For<IEventDispatcher>();
        _babyGameLogger = Substitute.For<IBabyGameLogger>();
        _timeProvider = Substitute.For<ITimeProvider>();
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
        await _loveService.LoveAsync(PlayerUtils.GetAlice(), "Johnny");
    }
}