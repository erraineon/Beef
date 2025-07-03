using System.Diagnostics;

namespace BabyGame.Tests;

public class TestBabyGameLogger : IBabyGameLogger
{
    public List<string> LogLines { get; } = new();
    public void Log(string message)
    {
        LogLines.Add(message);
        Debug.WriteLine(message);
    }
}