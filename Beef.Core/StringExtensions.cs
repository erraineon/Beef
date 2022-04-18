using System.Text.RegularExpressions;

namespace Beef.Core;

public static class StringExtensions
{
    public static IEnumerable<string> SplitBySpaceAndQuotes(this string value)
    {
        var allTokens = Regex
            .Split(value, @"[ ](?=(?:[^""]*""[^""]*"")*[^""]*$)");
        return allTokens;
    }
}