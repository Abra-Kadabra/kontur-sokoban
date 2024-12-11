using System;
using System.IO;

namespace sokoban;

public enum LogLevel
{
    DEBUG,
    INFO,
    WARN,
    ERROR,
    FATAL
}

public class Logger
{
    private readonly string logPath;

    public Logger()
    {
        logPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "sokoban.log";
    }

    public Logger(string logPath)
    {
        this.logPath = logPath;
    }

    public void Debug(string message)
    {
        Log(LogLevel.DEBUG, message);
    }

    public void Info(string message)
    {
        Log(LogLevel.INFO, message);
    }
    public void Error(string message)
    {
        Log(LogLevel.ERROR, message);
    }

    public void Log(LogLevel level, string message)
    {
        var logMessage = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} [{level}] {message}";
        File.AppendAllText(logPath, logMessage + Environment.NewLine);
    }
}
