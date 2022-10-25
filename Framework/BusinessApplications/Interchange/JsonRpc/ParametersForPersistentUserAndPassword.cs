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

namespace Framework.BusinessApplications.Interchange.JsonRpc {

    using Framework.Persistence.Fields;
    using Persistence.Directories;

    /// <summary>
    /// JSON-RPC parameters for user and password.
    /// </summary>
    internal sealed class ParametersForPersistentUserAndPassword : ParametersForPersistentUser {

        /// <summary>
        /// Password for user.
        /// </summary>
        public string Password {
            get { return this.password.Value; }
            set { this.password.Value = value; }
        }
        private readonly PersistentFieldForString password =
            new PersistentFieldForString(nameof(Password));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForPersistentUserAndPassword()
            : base() {
            this.RegisterPersistentField(this.password);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="password">password of user</param>
        public ParametersForPersistentUserAndPassword(PersistentUser user, string password)
            : this() {
            this.User = user;
            this.Password = password;
        }

    }

}