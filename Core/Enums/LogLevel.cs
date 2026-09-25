using DinaCSharp.Services.Logs;

namespace DinaCSharp.Core.Enums
{
    /// <summary>
    /// Defines severity levels for logging.
    /// </summary>
    /// <remarks>
    /// These levels are used by <see cref="LogManager"/> to filter and categorize
    /// log messages. Consumers can set <c>LogManager.MinLogLevel</c> to control
    /// which messages are recorded or displayed.
    /// </remarks>
    public enum LogLevel
    {
        /// <summary>
        /// Detailed debugging information. Normally enabled only during development
        /// or when diagnosing issues.
        /// </summary>
        Debug,
        /// <summary>
        /// Informational messages that describe normal application flow.
        /// </summary>
        Info,
        /// <summary>
        /// Indications of possible problems or unexpected situations that do not
        /// stop program execution but may require attention.
        /// </summary>
        Warning,
        /// <summary>
        /// Error events that represent failures in components or operations and
        /// typically require investigation.
        /// </summary>
        Error,
    }
}
