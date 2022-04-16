using System.Reflection;
using Beef.Core.Chats.Interactions.Execution;
using Beef.Core.Data;

namespace Beef.Core.Chats.Interactions.Registration;

public class ModuleRegistrar : IModuleRegistrar
{
    private readonly IChatScopeFactory _chatScopeFactory;
    private readonly IInteractionService _interactionService;

    public ModuleRegistrar(
        IInteractionService interactionService, IChatScopeFactory chatScopeFactory)
    {
        _interactionService = interactionService;
        _chatScopeFactory = chatScopeFactory;
    }

    public async Task RegisterModulesAsync()
    {
        var assemblies = new List<Assembly> {Assembly.GetExecutingAssembly()};
        if (Assembly.GetEntryAssembly() is { } entryAssembly) assemblies.Add(entryAssembly);

        using var scope = _chatScopeFactory.CreateScope(ChatType.Discord);
        foreach (var assembly in assemblies) await _interactionService.AddModulesAsync(assembly, scope.ServiceProvider);
    }
}