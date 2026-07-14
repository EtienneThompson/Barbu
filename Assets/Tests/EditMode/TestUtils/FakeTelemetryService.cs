namespace Barbu.Tests.EditMode.TestUtils
{
    using System;
    using Barbu.Core.Telemetry;

    /// <summary>No-op ITelemetryService so tests don't touch real logging/analytics.</summary>
    public class FakeTelemetryService : ITelemetryService
    {
        public void Log(LogLevel level, string message)
        {
        }

        public void LogInfo(string message)
        {
        }

        public void LogError(string message)
        {
        }

        public void LogError(Exception exception, string message)
        {
        }
    }
}
