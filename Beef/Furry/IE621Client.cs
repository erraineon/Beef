using E621;

namespace Beef.Furry;

public interface IE621Client
{
    Task<IList<E621Post>> Search(E621SearchOptions options);
}