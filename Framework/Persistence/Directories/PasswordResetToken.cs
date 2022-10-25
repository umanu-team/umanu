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

    using System;

    /// <summary>
    /// Temporary token for resetting passwords.
    /// </summary>
    public sealed class PasswordResetToken : UserDirectoryToken<PasswordResetToken> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PasswordResetToken()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="associatedUser">associated user</param>
        public PasswordResetToken(IUser associatedUser)
            : this() {
            this.AssociatedUser = associatedUser;
        }

        /// <summary>
        /// Gets the time span until user directory tokens expire.
        /// </summary>
        /// <returns>time span until user directory tokens expire</returns>
        protected override TimeSpan GetExpirationTimeSpan() {
            return TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Gets the length of identifier to be generated.
        /// </summary>
        /// <returns>length of identifier to be generated</returns>
        protected override ushort GetIdentifierLength() {
            return (ushort)PasswordGenerator.Random.Next(15, 20);
        }

    }

}