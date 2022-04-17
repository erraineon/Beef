using System.Text.RegularExpressions;

namespace Beef.Core;

public static class StringExtensions
{
    public static IEnumerable<string> SplitBySpaceAndQuotes(this string value)
    {
        var allTokens = Regex
            .Matches(value, @"(?<match>\w+)|\""(?<match>[\w\s]*)""")
            .Select(m => m.Groups["match"].Value);
        return allTokens;
    }
}