namespace Beef.Google;

public interface IGoogleSearchEngine
{
    Task<string?> FindWebpageLinkAsync(string query);
    Task<string?> FindImageLinkAsync(string query);
}