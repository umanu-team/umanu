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

namespace Framework.Persistence.Rdbms {

    using Framework.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// Logger for SQL database commands.
    /// </summary>
    public sealed class DbCommandLogger {

        /// <summary>
        /// True if this logger is active, false otherwise.
        /// </summary>
        public bool IsActive {
            get {
                if (!this.isActive.HasValue) {
                    this.isActive = null != this.Log && LogLevel.Debug == this.Log.MinLevel;
                }
                return this.isActive.Value;
            }
        }
        private bool? isActive;

        /// <summary>
        /// Log to use for logging.
        /// </summary>
        public ILog Log { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public DbCommandLogger() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="log">log to use for logging</param>
        public DbCommandLogger(ILog log)
            : this() {
            this.Log = log;
        }

        /// <summary>
        /// Logs an SQL command, if minimum level of log is set to
        /// debug.
        /// </summary>
        /// <param name="commandText">command text to log</param>
        /// <param name="parameters">SQL parameters to apply</param>
        public void LogCommand(string commandText, IEnumerable<DbParameter> parameters) {
            if (this.IsActive) {
                commandText = DbFilterBuilder.ApplyParameters(commandText, parameters);
                this.Log.WriteEntry("Executing SQL command:" + Environment.NewLine + commandText, LogLevel.Debug);
            }
            return;
        }

    }

}