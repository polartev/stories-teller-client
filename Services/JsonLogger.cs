using System.Text.Json;
using System.Text.Json.Serialization;

namespace Story_Teller.Models;

public class JsonLogger
{
    private static readonly string logFilePath = Path.Combine(FileSystem.AppDataDirectory, "consolelog.json");
    private static readonly object _lock = new();

    private class LogEntry
    {
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
        public string Method { get; set; }
        public string Message { get; set; }
    }

    private class LogContainer
    {
        public List<LogEntry> Logs { get; set; } = new();
    }

    public static void LogMethodCall(string methodName, string message = "")
    {
        lock (_lock)
        {
            LogContainer container;

            // Если файл уже существует — загружаем логи
            if (File.Exists(logFilePath))
            {
                string existingJson = File.ReadAllText(logFilePath);
                try
                {
                    container = JsonSerializer.Deserialize<LogContainer>(existingJson) ?? new LogContainer();
                }
                catch
                {
                    container = new LogContainer(); // если что-то пошло не так
                }
            }
            else
            {
                container = new LogContainer();
            }

            container.Logs.Add(new LogEntry
            {
                Method = methodName,
                Message = message
            });

            string json = JsonSerializer.Serialize(container, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(logFilePath, json);
        }
    }
}