namespace CalculatorAPI.Logging;

public class CustomLoggerProvider : ILoggerProvider
{
    private readonly CustomLoggerProviderConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomLoggerProvider(CustomLoggerProviderConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _config = config;
        _httpContextAccessor = httpContextAccessor;
    }

    public ILogger CreateLogger(string CategoryName)
    {
        return new CustomLogger(CategoryName, _config, _httpContextAccessor);
    }

    public void Dispose()
    {
    }
}