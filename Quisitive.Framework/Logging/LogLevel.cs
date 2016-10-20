namespace Quisitive.Framework.Logging
{
    public enum LogLevel
    {
        /// <summary>
        /// Used for debugging only
        /// </summary>
        Debug,
        /// <summary>
        /// Used for tracing, debugging and informational messages
        /// </summary>
        Info,
        /// <summary>
        /// Used in situations that might need attention, but not critical errors
        /// </summary>
        Warning,
        /// <summary>
        /// Used for messages from a critical error or exception
        /// </summary>
        Error
    }
}
