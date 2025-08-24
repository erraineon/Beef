using System.Text;

namespace BabyGame.Services;

public class BabyGameLogger : IBabyGameLogger
{
    private readonly StringBuilder _lines = new();

    public void Log(string message)
    {
        _lines.AppendLine(message);
    }
}