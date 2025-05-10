namespace Barbu.Core.Telemetry
{
    using System;
    using UnityEngine;

    public class TelemetryService : ITelemetryService
    {
        public TelemetryService()
        {
        }

        public void Log(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Info:
                    this.LogInfo(message);
                    return;
                case LogLevel.Error:
                    this.LogError(message);
                    return;
                case LogLevel.None:
                case LogLevel.Debug:
                case LogLevel.Warning:
                case LogLevel.Fatal:
                default:
                    return;
            }
        }

        public void LogInfo(string message)
        {
            Debug.Log(message);
        }

        public void LogError(Exception exception, string message)
        {
            this.LogError($"{message}. Exception: {exception}");
        }

        public void LogError(string message)
        {
            Debug.LogError(message);
        }
    }
}
