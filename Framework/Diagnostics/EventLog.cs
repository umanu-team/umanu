/*********************************************************************
 * Umanu Framework / (C) Umanu Team / http://www.umanu.de/           *
 *                                                                   *
 * This program is free software: you can redistribute it and/or     *
 * modify it under the terms of the GNU Lesser General Public        *
 * License as published by the Free Software Foundation, either      *
 * version 3 of the License, or (at your option) any later version.  *
 *                                                                   *
 * This program is distributed in the hope that it will be useful,   *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of    *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the     *
 * GNU Lesser General Public License for more details.               *
 *                                                                   *
 * You should have received a copy of the GNU Lesser General Public  *
 * License along with this program.                                  *
 * If not, see <http://www.gnu.org/licenses/>.                       *
 *********************************************************************/

namespace Framework.Diagnostics {

    using System;
    using System.Text;

    /// <summary>
    /// Represents the Windows application event log.
    /// </summary>
    public class EventLog : ILog {

        /// <summary>
        /// Name of application to log events for.
        /// </summary>
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Minimum level for logging. All messages below this level
        /// will be suppressed.
        /// </summary>
        public LogLevel MinLevel { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="applicationName">name of application to log
        /// events for</param>
        public EventLog(string applicationName)
            : this(applicationName, LogLevel.Debug) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="applicationName">name of application to log
        /// events for</param>
        /// <param name="minLevel">minimum level for logging - all
        /// messages below this level will be suppressed</param>
        public EventLog(string applicationName, LogLevel minLevel) {
            this.ApplicationName = applicationName;
            this.MinLevel = minLevel;
        }

        /// <summary>
        /// Initializes access to event log.
        /// </summary>
        public virtual void Initialize() {
            if (!System.Diagnostics.EventLog.SourceExists(this.ApplicationName)) {
                System.Diagnostics.EventLog.CreateEventSource(this.ApplicationName, "Application");
            }
            return;
        }

        /// <summary>
        /// Writes an entry with the given message text to the log.
        /// </summary>
        /// <param name="exception">exception to write to log</param>
        /// <param name="logLevel">log level of entry to write</param>
        /// <returns>correlation ID of entry</returns>
        public virtual Guid WriteEntry(Exception exception, LogLevel logLevel) {
            Guid correlationId;
            if (logLevel >= this.MinLevel) {
                var correlatedException = exception as CorrelatedException;
                if (null == correlatedException) {
                    correlatedException = new CorrelatedException(exception.Message, exception);
                }
                correlationId = correlatedException.CorrelationId;
                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.Append(correlatedException.InnerException.GetType().FullName);
                messageBuilder.Append(" with correlation ID ");
                messageBuilder.Append(correlationId.ToString());
                messageBuilder.AppendLine();
                messageBuilder.AppendLine();
                messageBuilder.Append(correlatedException.InnerException.Message);
                messageBuilder.AppendLine();
                messageBuilder.Append(correlatedException.InnerException.StackTrace);
                this.WriteEntry(messageBuilder.ToString(), logLevel);
            } else {
                correlationId = Guid.Empty;
            }
            return correlationId;
        }

        /// <summary>
        /// Writes an entry with the given message text to the log.
        /// </summary>
        /// <param name="message">message to write to log</param>
        /// <param name="logLevel">log level of entry to write</param>
        public virtual void WriteEntry(string message, LogLevel logLevel) {
            if (logLevel >= this.MinLevel) {
                if (message.Length > 30000) {
                    message = message.Substring(0, 29997) + "...";
                }
                System.Diagnostics.EventLog.WriteEntry(this.ApplicationName, message, this.GetEventLogEntryTypeForLogLevel(logLevel));
            }
            return;
        }

        /// <summary>
        /// Gets the matching event log entry type for a specified
        /// log level.
        /// </summary>
        /// <param name="logLevel">log level to get event log entry
        /// type for</param>
        /// <returns>event log entry type for specified log level</returns>
        protected virtual System.Diagnostics.EventLogEntryType GetEventLogEntryTypeForLogLevel(LogLevel logLevel) {
            System.Diagnostics.EventLogEntryType eventLogEntryType;
            if (LogLevel.Debug == logLevel || LogLevel.Information == logLevel) {
                eventLogEntryType = System.Diagnostics.EventLogEntryType.Information;
            } else if (LogLevel.Warning == logLevel) {
                eventLogEntryType = System.Diagnostics.EventLogEntryType.Warning;
            } else if (LogLevel.Error == logLevel) {
                eventLogEntryType = System.Diagnostics.EventLogEntryType.Error;
            } else {
                throw new ArgumentException("Log level \"" + logLevel.ToString() + "\" is unknown.", nameof(logLevel));
            }
            return eventLogEntryType;
        }

    }

}