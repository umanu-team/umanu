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

    /// <summary>
    /// Interface of any log.
    /// </summary>
    public interface ILog {

        /// <summary>
        /// Minimum level for logging. All messages below this level
        /// will be suppressed.
        /// </summary>
        LogLevel MinLevel { get; }

        /// <summary>
        /// Writes an entry with the given message text to the log.
        /// </summary>
        /// <param name="exception">exception to write to log</param>
        /// <param name="logLevel">log level of entry to write</param>
        /// <returns>correlation ID of entry</returns>
        Guid WriteEntry(Exception exception, LogLevel logLevel);

        /// <summary>
        /// Writes an entry with the given message text to the log.
        /// </summary>
        /// <param name="message">message to write to log</param>
        /// <param name="logLevel">log level of entry to write</param>
        void WriteEntry(string message, LogLevel logLevel);

    }

}