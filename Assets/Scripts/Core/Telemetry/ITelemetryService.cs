namespace Barbu.Core
{
    using System;

    public interface ITelemetryService
    {
        void Log(LogLevel level, string message);

        void LogInfo(string message);

        void LogError(string message);

        void LogError(Exception exception, string message);
    }
}
