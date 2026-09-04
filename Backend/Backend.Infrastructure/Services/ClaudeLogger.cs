using System.Text;

namespace Backend.Infrastructure.Services;

public class ClaudeLogger
{
    private readonly string _logDirectory;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public ClaudeLogger()
    {
        _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs", "claude");
        Directory.CreateDirectory(_logDirectory);
    }

    public async Task LogRequestAsync(string systemMessage, string userMessage)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC] === REQUEST ===");
        sb.AppendLine("--- SYSTEM ---");
        sb.AppendLine(systemMessage);
        sb.AppendLine("--- USER ---");
        sb.AppendLine(userMessage);
        sb.AppendLine();

        await WriteAsync(sb.ToString());
    }

    public async Task LogResponseAsync(string rawJson)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC] === RESPONSE ===");
        sb.AppendLine(rawJson);
        sb.AppendLine(new string('-', 80));
        sb.AppendLine();

        await WriteAsync(sb.ToString());
    }

    private async Task WriteAsync(string text)
    {
        var filePath = Path.Combine(_logDirectory, $"claude_{DateTime.UtcNow:yyyy-MM-dd}.log");

        await _lock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(filePath, text, Encoding.UTF8);
        }
        finally
        {
            _lock.Release();
        }
    }
}
