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

    using Directories;
    using System;

    /// <summary>
    /// Dummy persistence machanism.
    /// </summary>
    public sealed partial class DummyPersistenceMechanism : PersistenceMechanism {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="userDirectory">directory to use for user
        /// resolval</param>
        /// <param name="securityModel">security model to use for
        /// accessing this persistence mechanism</param>
        public DummyPersistenceMechanism(UserDirectory userDirectory, SecurityModel securityModel)
            : base(userDirectory, securityModel) {
            // nothing to do
        }

        /// <summary>
        /// Copys this persistence mechanism and sets the security
        /// model of the copied instance to apply permissions.
        /// </summary>
        /// <returns>copy of this persistence mechanism that applies
        /// all permissions or current instance if it applies
        /// permissions already</returns>
        protected override PersistenceMechanism CopyWithCurrentUserPrivilegesNoCache() {
            var restrictedCopy = new DummyPersistenceMechanism(this.UserDirectory, SecurityModel.ApplyPermissions);
            restrictedCopy.CopyPartiallyFrom(this);
            return restrictedCopy;
        }

        /// <summary>
        /// Copys this persistence mechanism and sets the security
        /// model of the copied instance to ignore permissions. This
        /// method does not cache the copied elevated persistence
        /// mechanism.
        /// </summary>
        /// <returns>copy of this persistence mechanism that ignores
        /// all permissions</returns>
        protected internal override PersistenceMechanism CopyWithElevatedPrivilegesNoCache() {
            var elevatedCopy = new DummyPersistenceMechanism(this.UserDirectory, SecurityModel.IgnorePermissions);
            elevatedCopy.CopyPartiallyFrom(this);
            return elevatedCopy;
        }

        /// <summary>
        /// Copys this persistence mechanism but replaces the user
        /// directory. This method does not cache the copied
        /// persistence mechanism.
        /// </summary>
        /// <param name="userDirectory">user directory to associate
        /// to copied persistence mechanism</param>
        /// <returns>copy of this persistence mechanism with replaced
        /// user directory</returns>
        public override PersistenceMechanism CopyWithReplacedUserDirectoryNoCache(UserDirectory userDirectory) {
            var modifiedCopy = new DummyPersistenceMechanism(userDirectory, this.SecurityModel);
            modifiedCopy.CopyPartiallyFrom(this);
            return modifiedCopy;
        }

        /// <summary>
        /// Gets the unique ID of this persistence mechanism.
        /// </summary>
        /// <returns>unique ID of this persistence mechanism</returns>
        protected override string GetId() {
            throw new NotImplementedException();
        }

    }

}