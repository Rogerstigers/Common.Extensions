using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Quisitive.Framework.Logging
{
    public interface ICommonLog
    {
        /// <summary>
        /// Logs a message with the specified LogLevel.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        void Log(LogLevel type, string message);

        /// <summary>
        /// Log debug LogLevel with the specified message
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug(string message);

        /// <summary>
        /// Log info LogLevel with the specified message
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Log warning LogLevel with the specified message
        /// </summary>
        /// <param name="message">The message.</param>
        void Warning(string message);

        /// <summary>
        /// Log warning LogLevel with the specified message including the exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Warning(string message, Exception exception);

        /// <summary>
        /// Log error LogLevel with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(string message);

        /// <summary>
        /// Logs error LogLevel described by the specified message including the exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Log error LogLevel with the specified messages from the collection
        /// </summary>
        /// <param name="errors">The list of messages</param>
        void LogErrors(List<string> messages, [CallerMemberNameAttribute] string method = "");
    }
}
