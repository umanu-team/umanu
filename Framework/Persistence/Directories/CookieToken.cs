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
    /// Temporary token for cookies.
    /// </summary>
    public sealed class CookieToken : UserDirectoryToken<CookieToken> {

        /// <summary>
        /// Time span until cookie tokens expire.
        /// </summary>
        public static TimeSpan ExpirationTimeSpan = TimeSpan.FromDays(1);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public CookieToken()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets the time span until user directory tokens expire.
        /// </summary>
        /// <returns>time span until user directory tokens expire</returns>
        protected override TimeSpan GetExpirationTimeSpan() {
            return CookieToken.ExpirationTimeSpan;
        }

        /// <summary>
        /// Gets the length of identifier to be generated.
        /// </summary>
        /// <returns>length of identifier to be generated</returns>
        protected override ushort GetIdentifierLength() {
            return (ushort)PasswordGenerator.Random.Next(1024, 2048);
        }

    }

}