namespace Beef.Twitter;

public record Tweet(ulong Id, string AuthorName, string Status, bool HasMedia);