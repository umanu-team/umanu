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
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of logs.
    /// </summary>
    public class CombinedLog : ILog {

        /// <summary>
        /// List of logs to use.
        /// </summary>
        public IList<ILog> Logs { get; private set; }

        /// <summary>
        /// Minimum level for logging. All messages below this level
        /// will be suppressed.
        /// </summary>
        public LogLevel MinLevel {
            get {
                LogLevel minLevel = LogLevel.Error;
                foreach (var log in this.Logs) {
                    if (log.MinLevel < minLevel) {
                        minLevel = log.MinLevel;
                    }
                }
                return minLevel;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public CombinedLog() {
            this.Logs = new List<ILog>();
        }

        /// <summary>
        /// Writes an entry with the given message text to the log.
        /// </summary>
        /// <param name="exception">exception to write to log</param>
        /// <param name="logLevel">log level of entry to write</param>
        /// <returns>correlation ID of entry</returns>
        public virtual Guid WriteEntry(Exception exception, LogLevel logLevel) {
            var correlatedException = exception as CorrelatedException;
            if (null == correlatedException) {
                correlatedException = new CorrelatedException(exception.Message, exception);
            }
            foreach (var log in this.Logs) {
                log.WriteEntry(correlatedException, logLevel);
            }
            return correlatedException.CorrelationId;
        }

        /// <summary>
        /// Writes an entry with the given message text to the log.
        /// </summary>
        /// <param name="message">message to write to log</param>
        /// <param name="logLevel">log level of entry to write</param>
        public virtual void WriteEntry(string message, LogLevel logLevel) {
            foreach (var log in this.Logs) {
                log.WriteEntry(message, logLevel);
            }
            return;
        }

    }

}