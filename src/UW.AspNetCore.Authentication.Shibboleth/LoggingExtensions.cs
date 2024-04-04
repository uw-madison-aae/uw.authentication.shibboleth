namespace Microsoft.Extensions.Logging;

#if NET6_0_OR_GREATER
internal static partial class LoggingExtensions
#else
internal static class LoggingExtensions
#endif
{
#if !NET6_0_OR_GREATER
    private static readonly Action<ILogger, string, Exception?> _shibbolethAuthenticationError;
    private static readonly Action<ILogger, Exception?> _signInHandled;
    private static readonly Action<ILogger, Exception?> _signInSkipped;

#endif
    static LoggingExtensions()
    {
#if !NET6_0_OR_GREATER
        _shibbolethAuthenticationError = LoggerMessage.Define<string>(
            eventId: new EventId(990, "ShibbolethAuthenticationFailed"),
            logLevel: LogLevel.Information,
            formatString: "Error from ShibbolethAuthentication: {ErrorMessage}.");
        _signInHandled = LoggerMessage.Define(
            eventId: new EventId(5, "SignInHandled"),
            logLevel: LogLevel.Debug,
            formatString: "The SigningIn event returned Handled.");
        _signInSkipped = LoggerMessage.Define(
            eventId: new EventId(6, "SignInSkipped"),
            logLevel: LogLevel.Debug,
            formatString: "The SigningIn event returned Skipped.");
#endif
    }

#if NET6_0_OR_GREATER
    [LoggerMessage(990, LogLevel.Information, "Error from ShibbolethAuthentication: {ErrorMessage}.", EventName = "ShibbolethAuthenticationFailed")]
    public static partial void ShibbolethAuthenticationFailed(this ILogger logger, string errorMessage);

    [LoggerMessage(5, LogLevel.Debug, "The SigningIn event returned Handled.", EventName = "SignInHandled")]
    public static partial void SignInHandled(this ILogger logger);

    [LoggerMessage(6, LogLevel.Debug, "The SigningIn event returned Skipped.", EventName = "SignInSkipped")]
    public static partial void SignInSkipped(this ILogger logger);
#else
    public static void ShibbolethAuthenticationFailed(this ILogger logger, string errorMessage)
    {
        _shibbolethAuthenticationError(logger, errorMessage, null);
    }

    public static void SignInHandled(this ILogger logger)
    {
        _signInHandled(logger, null);
    }

    public static void SignInSkipped(this ILogger logger)
    {
        _signInSkipped(logger, null);
    }
#endif
}
