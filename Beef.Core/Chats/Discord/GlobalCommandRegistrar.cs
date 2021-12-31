namespace Beef.Core.Chats.Discord;

public class GlobalCommandRegistrar : ICommandRegistrar
{
    private readonly IEnumerable<IInteractionService> _interactionServices;

    public GlobalCommandRegistrar(IEnumerable<IInteractionService> interactionServices)
    {
        _interactionServices = interactionServices;
    }

    public async Task RegisterCommandsAsync(CancellationToken cancellationToken)
    {
        foreach (var interactionService in _interactionServices)
        {
            await interactionService.RegisterCommandsGloballyAsync();
        }
    }
}