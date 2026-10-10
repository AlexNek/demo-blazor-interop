using System.Text;

namespace DemoInterop.Shared.Interop;

/// <summary>
/// Turns a caught exception into the text the wrong-usage panels display.
/// The demos show what the runtime really produced, including the inner exception.
/// Stack-frame lines are dropped: the exception type and its message are the point of
/// a ✗ panel, and on Blazor Server a single message carries ten lines of frames that
/// push that text off screen.
/// </summary>
internal static class ErrorText
{
    public static string From(Exception exception)
    {
        var text = new StringBuilder();

        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (text.Length > 0)
            {
                text.AppendLine().Append("  caused by ");
            }

            text.Append(current.GetType().Name).Append(": ").Append(WithoutFrames(current.Message));
        }

        return text.ToString();
    }

    private static string WithoutFrames(string message)
    {
        var kept = message
            .Replace("\r\n", "\n")
            .Split('\n')
            .Where(line => !line.TrimStart().StartsWith("at ", StringComparison.Ordinal));

        return string.Join('\n', kept);
    }
}
