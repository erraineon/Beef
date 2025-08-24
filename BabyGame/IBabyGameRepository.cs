using BabyGame.Data;
using BabyGame.Services;

namespace BabyGame;

public interface IBabyGameRepository
{
    Task SaveMarriageAsync(Marriage marriage);
    Task<Marriage> GetMarriageAsync(Player player);
    Task<bool> GetIsMarriedAsync(Player player);
    Task CreateSpouseAsync(Player player);
    Task<Proposal?> GetProposalOrNullAsync(Player proposer, Player fiance);
    IQueryable<Proposal> GetProposals(Player proposer);
    Task SaveProposalAsync(Proposal proposal);
}