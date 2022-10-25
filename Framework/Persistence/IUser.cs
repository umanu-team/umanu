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

namespace Framework.Persistence {

    using Framework.Model;
    using System.Globalization;

    /// <summary>
    /// User that belongs to a user directory.
    /// </summary>
    public interface IUser : IPerson {

        /// <summary>
        /// Preferred culture.
        /// </summary>
        CultureInfo Culture { get; set; }

        /// <summary>
        /// Manager user is reporting to.
        /// </summary>
        IUser Manager { get; set; }

        /// <summary>
        /// Preferred language as ISO 639 code.
        /// </summary>
        string PreferredLanguage { get; set; }

        /// <summary>
        /// Room number.
        /// </summary>
        string RoomNumber { get; set; }

        /// <summary>
        /// User name used for login.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Changes the password of this user.
        /// </summary>
        /// <param name="oldPassword">old password</param>
        /// <param name="newPassword">new password</param>
        /// <returns>true if the password was updated successfully;
        /// otherwise, false</returns>
        bool ChangePassword(string oldPassword, string newPassword);

        /// <summary>
        /// Removes this user from directory.
        /// </summary>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        bool Remove();

        /// <summary>
        /// Retrieves all values of this user from directory. This
        /// can be used to refresh the values of this object.
        /// </summary>
        void Retrieve();

        /// <summary>
        /// Encrypts and sets a password using a non reversable
        /// algorithm.
        /// </summary>
        /// <param name="password">password to encrypt and set</param>
        void SetPassword(string password);

        /// <summary>
        /// Updates this user in directory.
        /// </summary>
        /// <returns>true if user was updated successfully in
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        bool Update();

    }

}