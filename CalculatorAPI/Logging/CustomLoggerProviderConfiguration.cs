namespace CalculatorAPI.Logging;

public class CustomLoggerProviderConfiguration
{
    // Padrão para Warning, mas pode ser configurado para outros níveis de log
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;
    // Identificador do evento de log, útil para rastrear logs específicos
    public int EventId { get; set; } = 0;
}