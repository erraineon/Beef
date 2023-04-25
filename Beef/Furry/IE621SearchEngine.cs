using E621;

namespace Beef.Furry;

public interface IE621SearchEngine
{
    Task<IList<E621Post>> SearchLatestAsync(string contextKey, string tags);
}