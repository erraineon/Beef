using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BabyGame.Services;

public class MarriageService(
    IBabyGameRepository babyGameRepository,
    IRandomProvider randomProvider,
    ITimeProvider timeProvider,
    IEventDispatcher eventDispatcher) : IMarriageService
{
    public async Task<Marriage> MarryAsync(Player spouse1, Player spouse2)
    {
        EnsureNoSelfMarriage(spouse1, spouse2);
        await EnsureNotAlreadyMarriedAsync(spouse1);
        await EnsureNotAlreadyMarriedAsync(spouse2);
        var now = timeProvider.Now;
        var marriage = new Marriage
        {
            Spouse1 = spouse1,
            Spouse2 = spouse2,
            MarriedAt = now,
            Seed = randomProvider.NextInt(null, int.MinValue, int.MaxValue)
        };
        marriage.Chu = randomProvider.NextInt(marriage, 1, 31);
        eventDispatcher.Aggregate<IMarriageComplete, int>(marriage);
        await babyGameRepository.SaveMarriageAsync(marriage);
        return marriage;
    }

    private async Task EnsureNotAlreadyMarriedAsync(Player player)
    {
        if (await babyGameRepository.GetIsMarriedAsync(player))
            throw new AlreadyMarriedException(player);
    }

    private static void EnsureNoSelfMarriage(Player spouse1, Player spouse2)
    {
        if (spouse1.Id == spouse2.Id)
            throw new NoSelfMarriage(spouse1);
    }
}

public class ProposalService(IBabyGameRepository babyGameRepository, ITimeProvider timeProvider)
{
    public async Task<Proposal> ProposeAsync(Player proposer, Player fiance)
    {
        var proposal = await babyGameRepository.GetProposalOrNullAsync(proposer, fiance);
        if (proposal != null)
        {
            if (proposal.Fiance.Id == fiance.Id)
                throw new AlreadyProposedException(proposal);

            if (proposal.Proposer.Id == fiance.Id)
            {
                proposal.Accepted = true;
                proposal.DecidedAt = timeProvider.Now;
                await babyGameRepository.SaveProposalAsync(proposal);

                await DenyOtherProposalsAsync(proposer, fiance, proposal);
            }
        }
        else
        {
            proposal = new Proposal
            {
                CreatedAt = timeProvider.Now,
                Proposer = proposer,
                Fiance = fiance
            };
            await babyGameRepository.SaveProposalAsync(proposal);
        }

        return proposal;
    }

    private async Task DenyOtherProposalsAsync(Player proposer, Player fiance, Proposal currentProposal)
    {
        var otherProposals = await babyGameRepository.GetProposals(proposer)
            .Union(babyGameRepository.GetProposals(fiance))
            .Where(x => x.Accepted == null)
            .Except([currentProposal])
            .ToListAsync();

        foreach (var otherProposal in otherProposals)
        {
            otherProposal.Accepted = false;
            otherProposal.DecidedAt = timeProvider.Now;
            await babyGameRepository.SaveProposalAsync(otherProposal);
        }

    }
}

public class AlreadyProposedException(Proposal existingProposal) : Exception
{
    public Proposal ExistingProposal { get; } = existingProposal;
}

public class Proposal
{
    public Guid Id { get; set; }
    public Player Proposer { get; set; }
    public Player Fiance { get; set; }
    public bool? Accepted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset DecidedAt { get; set; }
}