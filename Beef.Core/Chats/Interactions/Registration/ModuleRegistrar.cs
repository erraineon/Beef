using System.Reflection;
using Beef.Core.Chats.Interactions.Execution;

namespace Beef.Core.Chats.Interactions.Registration;

public class ModuleRegistrar : IModuleRegistrar
{
    private readonly IInteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;

    public ModuleRegistrar(IInteractionService interactionService, IServiceProvider serviceProvider)
    {
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
    }

    public async Task RegisterModulesAsync()
    {
        var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
        if (Assembly.GetEntryAssembly() is { } entryAssembly) assemblies.Add(entryAssembly);

        foreach (var assembly in assemblies) await _interactionService.AddModulesAsync(assembly, _serviceProvider);
    }
}