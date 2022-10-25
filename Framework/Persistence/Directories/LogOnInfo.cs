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

namespace Framework.Persistence.Directories {

    using Framework.Model;
    using System;

    /// <summary>
    /// Log-on info of a user.
    /// </summary>
    public sealed class LogOnInfo {

        /// <summary>
        /// Number of failed attempts to log on.
        /// </summary>
        public uint FailedAttempts { get; private set; }

        /// <summary>
        /// Hashed failed password of last failed attempt.
        /// </summary>
        private int hashedFailedPassword;

        /// <summary>
        /// Indicated whether lock is active for the user of this
        /// log-on info.
        /// </summary>
        public bool IsLocked {
            get { return this.LockedUntil > UtcDateTime.Now; }
        }

        /// <summary>
        /// Date/time the user account is locked until.
        /// </summary>
        public DateTime LockedUntil { get; private set; }

        /// <summary>
        /// Minimum delay in seconds after failed attempt to log on.
        /// </summary>
        public const double MinimumDelayInSeconds = 7;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public LogOnInfo() {
            this.FailedAttempts = 0;
            this.LockedUntil = UtcDateTime.MinValue;
        }

        /// <summary>
        /// Adds a failed attempt.
        /// </summary>
        /// <param name="password">failed password</param>
        public void AddFailedAttempt(string password) {
            int hashedPassword = password.GetHashCode();
            if (hashedPassword != this.hashedFailedPassword) {
                this.FailedAttempts++;
                if (this.FailedAttempts > 3) {
                    this.LockedUntil = this.LockedUntil.AddMinutes(1);
                }
                var minLockedUntil = UtcDateTime.Now.AddSeconds(LogOnInfo.MinimumDelayInSeconds);
                if (this.LockedUntil < minLockedUntil) {
                    this.LockedUntil = minLockedUntil;
                }
                this.hashedFailedPassword = hashedPassword;
            }
            return;
        }

    }

}