using DinaCSharp.Core.Enums;
using DinaCSharp.Core.Interfaces;
using DinaCSharp.Services.Keys;

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Security;
using System.Threading;

namespace DinaCSharp.Services.Logs
{
    /// <summary>
    /// Global logger manager that accepts log messages from the application,
    /// optionally writes them to a file and/or to the console, and processes
    /// messages on a background thread.
    /// </summary>
    /// <remarks>
    /// - Messages are enqueued and processed by a dedicated background thread to
    ///   avoid blocking producers.
    /// - If a file path is provided, the manager will attempt to open the file in
    ///   append mode and write each log line. File open failures are silently
    ///   ignored and logging to file is disabled in that case.
    /// - Use <see cref="MinLogLevel"/> to filter out low-severity messages.
    /// - Call <see cref="Dispose"/> when the application is shutting down to flush
    ///   and stop the background thread cleanly.
    /// </remarks>
    public sealed class LogManager : IRegister, IDisposable
    {
        private readonly Thread _logThread;
        private readonly string? _logFilePath;
        private readonly ConcurrentQueue<string> _logQueue = [];
        private readonly AutoResetEvent _logEvent = new(false);
        private bool _running = true;
        private readonly StreamWriter? _writer;
        /// <summary>
        /// Minimum level that will be recorded. Messages below this level are ignored.
        /// Default is <see cref="LogLevel.Debug"/>.
        /// </summary>
        public LogLevel MinLogLevel { get; set; } = LogLevel.Debug;
        /// <summary>
        /// When true, messages are also written to the console./>.
        /// </summary>
        public bool LogToConsole { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of <see cref="LogManager"/>.
        /// </summary>
        /// <param name="logFilePath">
        /// Optional file path to append logs to. If null or if opening the file fails,
        /// file logging is disabled but console logging may still occur.
        /// </param>
        /// <param name="logToConsole">Whether to write logs to the console in addition to file.</param>
        public LogManager(string logFilePath, bool logToConsole = true)
        {
            _logFilePath = logFilePath;
            LogToConsole = logToConsole;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
                _writer = new StreamWriter(_logFilePath, append: true)
                {
                    AutoFlush = true // flush automatique après chaque WriteLine
                };
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException 
                                          or ArgumentNullException or PathTooLongException or DirectoryNotFoundException
                                          or NotSupportedException or SecurityException)
            {
                _writer = null;
            }

            _logThread = new Thread(ProcessQueue)
            {
                IsBackground = true,
                Name = "LogManagerThread"
            };
            _logThread.Start();

            Log($"--- Log démarré : {DateTime.Now} ---{Environment.NewLine}");
        }

        /// <summary>
        /// Enqueues a log message with the specified severity.
        /// </summary>
        /// <param name="message">Log message text.</param>
        /// <param name="level">Severity level (default is <see cref="LogLevel.Info"/>).</param>
        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (level < MinLogLevel)
                return;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            string formatted = $"[{timestamp}] [{level}] {message}";
            _logQueue.Enqueue(formatted);
            _logEvent.Set();
        }

        /// <summary>
        /// Helper to log a debug-level message.
        /// </summary>
        public void Debug(string message) => Log(message, LogLevel.Debug);
        /// <summary>
        /// Helper to log an informational message.
        /// </summary>
        public void Info(string message) => Log(message, LogLevel.Info);
        /// <summary>
        /// Helper to log a warning message.
        /// </summary>
        public void Warning(string message) => Log(message, LogLevel.Warning);
        /// <summary>
        /// Helper to log an error message.
        /// </summary>
        public void Error(string message) => Log(message, LogLevel.Error);

        private void ProcessQueue()
        {
            while (_running)
            {
                _logEvent.WaitOne();

                while (_logQueue.TryDequeue(out var log))
                {
                    if (LogToConsole)
                        Console.WriteLine(log);

#pragma warning disable CA1031 // Ne pas intercepter les types d'exception générale
                    try
                    {
                        _writer?.WriteLine(log);
                    }
                    catch { }
#pragma warning restore CA1031 // Ne pas intercepter les types d'exception générale
                }
            }
        }

        /// <summary>
        /// Registers this instance into the service locator using the provided key.
        /// </summary>
        /// <param name="key">Service registration key.</param>
        public void Register(Key<ServiceTag> key)
        {
            ServiceLocator.Register(key, this);
        }

        /// <summary>
        /// Stops the background logging thread and releases resources.
        /// </summary>
        public void Dispose()
        {
            _running = false;
            _logEvent?.Set();
            _logThread?.Join();
            _logEvent?.Dispose();
            _writer?.Dispose();
        }
    }
}
