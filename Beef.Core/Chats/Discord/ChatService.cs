using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace Beef.Core.Chats.Discord;

public class ChatService : IHostedService
{
    private readonly IEnumerable<IChatClientLauncher> _chatClientLaunchers;
    private readonly IEnumerable<ICommandRegistrar> _commandRegistrars;
    private readonly IModuleRegistrar _moduleRegistrar;

    public ChatService(
        IModuleRegistrar moduleRegistrar, 
        IEnumerable<IChatClientLauncher> chatClientLaunchers,
        IEnumerable<ICommandRegistrar> commandRegistrars
        )
    {
        _chatClientLaunchers = chatClientLaunchers;
        _commandRegistrars = commandRegistrars;
        _moduleRegistrar = moduleRegistrar;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _moduleRegistrar.RegisterModules();

        foreach (var chatClientLauncher in _chatClientLaunchers)
        {
            await chatClientLauncher.StartAsync(cancellationToken);
        }

        foreach (var commandRegistrar in _commandRegistrars)
        {
            await commandRegistrar.RegisterCommandsAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var chatClientLauncher in _chatClientLaunchers)
        {
            await chatClientLauncher.StopAsync(cancellationToken);
        }
    }
}

public interface IModuleRegistrar
{
    void RegisterModules();
}

public class ModuleRegistrar : IModuleRegistrar
{
    private readonly IEnumerable<IInteractionService> _interactionServices;
    private readonly IServiceProvider _serviceProvider;

    public ModuleRegistrar(IEnumerable<IInteractionService> interactionServices, IServiceProvider serviceProvider)
    {
        _interactionServices = interactionServices;
        _serviceProvider = serviceProvider;
    }


    public void RegisterModules()
    {
        var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly()};
        if (Assembly.GetEntryAssembly() is { } entryAssembly) assemblies.Add(entryAssembly);

        foreach (var interactionService in _interactionServices)
        foreach (var assembly in assemblies)
        {
            interactionService.AddModulesAsync(assembly, _serviceProvider);
        }
    }
}