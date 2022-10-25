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

    /// <summary>
    /// Represents the trace log for logging.
    /// </summary>
    public sealed class TraceLog : ConsoleLog {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public TraceLog()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="minLevel">minimum level for logging - all
        /// messages below this level will be suppressed</param>
        public TraceLog(LogLevel minLevel)
            : base(minLevel) {
            // nothing to do
        }

        /// <summary>
        /// Writes an entry with the given message text to the log.
        /// </summary>
        /// <param name="message">message to write to log</param>
        /// <param name="logLevel">log level of entry to write</param>
        public override void WriteEntry(string message, LogLevel logLevel) {
            if (LogLevel.Debug == logLevel || LogLevel.Information == logLevel) {
                System.Diagnostics.Trace.TraceInformation(message);
            } else if (LogLevel.Error == logLevel) {
                System.Diagnostics.Trace.TraceError(message);
            } else if (LogLevel.Warning == logLevel) {
                System.Diagnostics.Trace.TraceWarning(message);
            }
            return;
        }

    }

}