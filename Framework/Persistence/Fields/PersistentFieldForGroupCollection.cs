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

namespace Framework.Persistence.Fields {

    using System.Collections.Generic;

    /// <summary>
    /// List of objects of type group that can be stored in
    /// persistence mechanism.
    /// </summary>
    public class PersistentFieldForGroupCollection : PersistentFieldForPersistentObjectCollection<Group> {

        /// <summary>
        /// Indicates whether all users are contained.
        /// </summary>
        public bool ContainsAllUsers {
            get {
                bool hasAllUsersContained = false;
                foreach (var group in this) {
                    if (group.Members.ContainsAllUsers) {
                        hasAllUsersContained = true;
                        break;
                    }
                }
                return hasAllUsersContained;
            }
        }

        /// <summary>
        /// Unique members of all contained groups.
        /// </summary>
        public IEnumerable<IUser> Members {
            get { return Group.MembersOf(this); }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        /// <param name="cascadedRemovalBehavior">specifies the
        /// behavior to apply on cascaded removal of parent object</param>
        public PersistentFieldForGroupCollection(string key, CascadedRemovalBehavior cascadedRemovalBehavior)
            : base(key, cascadedRemovalBehavior) {
            // nothing to do
        }

        /// <summary>
        /// Indicates whether a specific user or anonymous user is
        /// contained.
        /// </summary>
        /// <param name="user">user to look for</param>
        /// <returns>true if given user or anonymous user is
        /// contained, false otherwise</returns>
        public bool ContainsPermissionsFor(IUser user) {
            bool hasPermissionsContained = false;
            foreach (var group in this) {
                if (group.Members.ContainsPermissionsFor(user)) { // it is intended to have no null-check for group here - if group is null then something is wrong in the application code, not in framework code
                    hasPermissionsContained = true;
                    break;
                }
            }
            return hasPermissionsContained;
        }

        /// <summary>
        /// Get all groups of collection except for the one to be
        /// excluded.
        /// </summary>
        /// <param name="excludedGroup">group to be excluded</param>
        /// <returns>all groups of collection except for the one to
        /// be excluded</returns>
        public IEnumerable<Group> ExceptFor(Group excludedGroup) {
            return this.ExceptFor(new Group[] { excludedGroup });
        }

        /// <summary>
        /// Get all groups of collection except for the ones to be
        /// excluded.
        /// </summary>
        /// <param name="excludedGroups">groups to be excluded</param>
        /// <returns>all groups of collection except for the ones to
        /// be excluded</returns>
        public IEnumerable<Group> ExceptFor(IEnumerable<Group> excludedGroups) {
            foreach (var group in this) {
                bool isGroupExcluded = false;
                foreach (var excludedGroup in excludedGroups) {
                    if (group == excludedGroup) {
                        isGroupExcluded = true;
                        break;
                    }
                }
                if (!isGroupExcluded) {
                    yield return group;
                }
            }
        }

    }

}