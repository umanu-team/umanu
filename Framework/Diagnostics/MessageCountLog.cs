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

    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Represents the system debug output for logging number of
    /// messages.
    /// </summary>
    public class MessageCountLog : DebugLog {

        /// <summary>
        /// Number of log messages.
        /// </summary>
        private ulong count;

        /// <summary>
        /// Prefix to add to log output.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public MessageCountLog()
            : this(LogLevel.Debug) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="minLevel">minimum level for logging - all
        /// messages below this level will be suppressed</param>
        public MessageCountLog(LogLevel minLevel)
            : base(minLevel) {
            this.count = 0;
        }

        /// <summary>
        /// Writes an entry with the given message text to the log.
        /// </summary>
        /// <param name="message">message to write to log</param>
        /// <param name="logLevel">log level of entry to write</param>
        public override void WriteEntry(string message, LogLevel logLevel) {
            if (logLevel >= this.MinLevel) {
                string logText = string.Empty;
                if (!string.IsNullOrEmpty(this.Label)) {
                    logText += this.Label + ": ";
                }
                this.count = this.count + 1;
                logText += this.count.ToString(CultureInfo.InvariantCulture);
                Debug.WriteLine(logText);
            }
            return;
        }

    }

}