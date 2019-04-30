using System;

namespace Microsoft.Extensions.Logging
{
    internal static class LoggingExtensions
    {
        private static Action<ILogger, Exception> _errorProcessingMessage;

        static LoggingExtensions()
        {
            _errorProcessingMessage = LoggerMessage.Define(
                eventId: 3,
                logLevel: LogLevel.Error,
                formatString: "Exception occurred while processing message.");
        }

        public static void ErrorProcessingMessage(this ILogger logger, Exception ex)
            => _errorProcessingMessage(logger, ex);
    }
}
