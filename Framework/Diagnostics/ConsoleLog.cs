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
    /// Represents the system console for logging.
    /// </summary>
    public class ConsoleLog : ILog {

        /// <summary>
        /// True to add date and time of message to console output,
        /// false otherwise.
        /// </summary>
        public bool IsSupposedToAddTime { get; set; }

        /// <summary>
        /// Minimum level for logging. All messages below this level
        /// will be suppressed.
        /// </summary>
        public LogLevel MinLevel { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ConsoleLog()
            : this(LogLevel.Debug) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="minLevel">minimum level for logging - all
        /// messages below this level will be suppressed</param>
        public ConsoleLog(LogLevel minLevel) {
            this.IsSupposedToAddTime = false;
            this.MinLevel = minLevel;
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
                messageBuilder.Append("Correlation ID: ");
                messageBuilder.Append(correlationId.ToString());
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append("Exception: ");
                messageBuilder.Append(correlatedException.InnerException.GetType().FullName);
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append("Message: ");
                messageBuilder.Append(correlatedException.InnerException.Message);
                messageBuilder.Append(Environment.NewLine);
                messageBuilder.Append("StackTrace: ");
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
                bool isColorChanged = true;
                if (LogLevel.Information == logLevel) {
                    Console.ForegroundColor = ConsoleColor.Green;
                } else if (LogLevel.Warning == logLevel) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                } else if (LogLevel.Error == logLevel) {
                    Console.ForegroundColor = ConsoleColor.Red;
                } else {
                    isColorChanged = false;
                }
                if (this.IsSupposedToAddTime) {
                    Console.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff "));
                }
                Console.WriteLine(message);
                Console.WriteLine();
                if (isColorChanged) {
                    Console.ResetColor();
                }
            }
            return;
        }

    }

}