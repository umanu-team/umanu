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

namespace Framework.BusinessApplications.DataProviders {

    using Framework.Persistence;
    using Framework.Persistence.Directories;

    /// <summary>
    /// Data provider for views of Active Directory localization
    /// settings.
    /// </summary>
    internal sealed class ActiveDirectoryLocalizationSettingsDataProvider : LocalizationSettingsDataProvider {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        public ActiveDirectoryLocalizationSettingsDataProvider(PersistenceMechanism persistenceMechanism)
            : base(persistenceMechanism) {
            // nothing to do
        }

        /// <summary>
        /// Adds or updates an object in persistence mechanism.
        /// </summary>
        /// <param name="element">object to add or update</param>
        public override void AddOrUpdate(LocalizationSettings element) {
            var activeDirectory = this.PersistenceMechanism.UserDirectory as ActiveDirectory;
            if (null == activeDirectory) {
                var mixedUserDirectory = this.PersistenceMechanism.UserDirectory as MixedUserDirectory;
                foreach (var userDirectory in mixedUserDirectory.UserDirectories) {
                    activeDirectory = userDirectory as ActiveDirectory;
                    if (null != activeDirectory) {
                        break;
                    }
                }
            }
            using (var impersonalizedActiveDirectory = new ActiveDirectory(activeDirectory.HttpContext, activeDirectory.CurrentUser.UserName, element.Password)) {
                IUser currentUser = impersonalizedActiveDirectory.CurrentUser;
                currentUser.Culture = element.Culture;
                currentUser.Update();
            }
            return;
        }

    }

}