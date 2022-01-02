using Beef.Core.Chats.Interactions.Registration;
using Microsoft.Extensions.Hosting;

namespace Beef.Core.Chats;

public class ChatStartupService : IHostedService
{
    private readonly IEnumerable<IChatClientLauncher> _chatClientLaunchers;
    private readonly IEnumerable<ICommandRegistrationService> _commandRegistrationServices;
    private readonly IModuleRegistrar _moduleRegistrar;

    public ChatStartupService(
        IEnumerable<IChatClientLauncher> chatClientLaunchers,
        IEnumerable<ICommandRegistrationService> commandRegistrationServices,
        IModuleRegistrar moduleRegistrar
    )
    {
        _chatClientLaunchers = chatClientLaunchers;
        _commandRegistrationServices = commandRegistrationServices;
        _moduleRegistrar = moduleRegistrar;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _moduleRegistrar.RegisterModulesAsync();
        await Task.WhenAll(_chatClientLaunchers.Select(x => x.StartAsync(cancellationToken)));
        await Task.WhenAll(_commandRegistrationServices.Select(x => x.RegisterCommandsAsync()));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(_chatClientLaunchers.Select(x => x.StopAsync(cancellationToken)));
    }
}