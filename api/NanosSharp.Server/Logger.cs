using Serilog;

namespace NanosSharp.Server;

/// <summary>
/// The nanos logger is a wrapper for the serilog logger adding a prefix for a special category.
/// </summary>
public sealed class Logger
{
    private readonly Serilog.Core.Logger _logger;
    private readonly string _prefix;
    
    internal Logger(string prefix, bool isDebug)
    {
        _prefix = prefix;

        if (isDebug)
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }
        else
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
    
    /// <summary>
    /// Prints a debug message.
    /// </summary>
    public void Debug(string message, params object[] args)
    {
        _logger.Debug($"[{_prefix}] {message}", args);
    }
    
    /// <summary>
    /// Prints an info message.
    /// </summary>
    public void Info(string message, params object[] args)
    {
        _logger.Information($"[{_prefix}] {message}", args);
    }
    
    /// <summary>
    /// Prints a warning message.
    /// </summary>
    public void Warn(string message, params object[] args)
    {
        _logger.Warning($"[{_prefix}] {message}", args);
    }
    
    /// <summary>
    /// Prints an error message.
    /// </summary>
    public void Error(string message, params object[] args)
    {
        _logger.Error($"[{_prefix}] {message}", args);
    }
    
    /// <summary>
    /// Prints a fatal message.
    /// </summary>
    public void Fatal(string message, params object[] args)
    {
        _logger.Fatal($"[{_prefix}] {message}", args);
    }
}