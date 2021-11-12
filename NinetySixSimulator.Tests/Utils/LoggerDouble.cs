using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NinetySixSimulator.Tests.Utils;

/// <summary>
/// Inspired by https://stackoverflow.com/questions/52707702/how-do-you-mock-ilogger-loginformation
/// </summary>
/// <typeparam name="T"></typeparam>
public class LoggerDouble<T> : ILogger, ILogger<T>
{
    public List<LogEntry> LogEntries { get; } = new List<LogEntry>();

    public IEnumerable<LogEntry> TraceEntries => LogEntries.Where(e => e.LogLevel == LogLevel.Trace);
    public IEnumerable<LogEntry> DebugEntries => LogEntries.Where(e => e.LogLevel == LogLevel.Debug);
    public IEnumerable<LogEntry> InformationEntries => LogEntries.Where(e => e.LogLevel == LogLevel.Information);

    public bool HasBeenLogged(LogLevel logLevel, string msg)
    {
        return LogEntries.Any(item => item.LogLevel == logLevel && item.State.ToString() == msg);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        LogEntries.Add(new LogEntry(logLevel, eventId, state, exception));
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return new LoggingScope();
    }

    public class LoggingScope : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

public class LogEntry
{
    public LogEntry(LogLevel logLevel, EventId eventId, object state, Exception exception)
    {
        LogLevel = logLevel;
        EventId = eventId;
        State = state;
        Exception = exception;
    }

    public LogLevel LogLevel { get; }
    public EventId EventId { get; }
    public object State { get; }
    public Exception Exception { get; }
}
