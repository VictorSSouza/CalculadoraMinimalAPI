namespace CalculatorAPI.Logging;

public class CustomLogger : ILogger
{
    readonly string loggerName;
    readonly CustomLoggerProviderConfiguration config;
    readonly IHttpContextAccessor httpContextAccessor;
    private static readonly object _lock = new();

    public CustomLogger(string loggerName, CustomLoggerProviderConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        this.loggerName = loggerName;
        this.config = config;
        this.httpContextAccessor = httpContextAccessor;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None && logLevel >= config.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        // Obtém o método HTTP atual (ex: "GET", "POST"). Se for um log do sistema fora de requisição, usa "SYSTEM"
        string httpMethod = httpContextAccessor.HttpContext?.Request.Method ?? "SYSTEM";

        string message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {eventId.Id} - {formatter(state, exception)}";
        if (exception != null) message += $"\n{exception}";

        WriteTextInFile(message, httpMethod);
    }

    public static void WriteTextInFile(string message, string httpMethod)
    {
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

        // O nome do arquivo será baseado no método: GET.log, POST.log, PUT.log, etc.
        string fileName = $"{httpMethod.ToUpper()}.log";
        string filePath = Path.Combine(directoryPath, fileName);

        lock (_lock)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using var writer = new StreamWriter(filePath, append: true);
                writer.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao escrever no log: {ex.Message}");
            }
        }
    }
}