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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Keyed collection for directory users, optimized for fast
    /// access by ID. IDs of users may not change.
    /// </summary>
    internal sealed class DirectoryUserCache : KeyedCollection<Guid, IUser> {

        /// <summary>
        /// List of IDs of non-existent users.
        /// </summary>
        private readonly List<Guid> notExistingUserIds;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public DirectoryUserCache() {
            this.notExistingUserIds = new List<Guid>();
        }

        /// <summary>
        /// Adds a collection of objects to the list. 
        /// </summary>
        /// <param name="items">collection of objects to add to the
        /// cache</param>
        public void AddRange(IEnumerable<IUser> items) {
            foreach (var item in items) {
                this.Add(item);
            }
            return;
        }

        /// <summary>
        /// Indicates whether all IDs are not registered as
        /// not existing.
        /// </summary>
        /// <param name="ids">IDs to check registration of
        /// non-existence of</param>
        /// <returns>true if all IDs are registered as not existing,
        /// false if at least one ID is not registered as not
        /// existing</returns>
        public bool AreRegisteredAsNotExisting(IEnumerable<Guid> ids) {
            bool areRegisteredAsNotExisting = false;
            foreach (var id in ids) {
                areRegisteredAsNotExisting = this.notExistingUserIds.Contains(id);
                if (!areRegisteredAsNotExisting) {
                    break;
                }
            }
            return areRegisteredAsNotExisting;
        }

        /// <summary>
        /// Removes all elements.
        /// </summary>
        protected override void ClearItems() {
            base.ClearItems();
            this.notExistingUserIds.Clear();
            return;
        }

        /// <summary>
        /// Indicates whether all IDs are contained in list.
        /// </summary>
        /// <param name="ids">IDs to check existence in list of</param>
        /// <returns>true if all IDs are contained in list, false
        /// otherwise</returns>
        public bool Contains(IEnumerable<Guid> ids) {
            bool areAllContained = true;
            foreach (var id in ids) {
                if (!this.Contains(id)) {
                    areAllContained = false;
                    break;
                }
            }
            return areAllContained;
        }

        /// <summary>
        /// Extracts the ID of the specified user.
        /// </summary>
        /// <param name="user">user to extract ID of</param>
        /// <returns>ID of the specified user</returns>
        protected override Guid GetKeyForItem(IUser user) {
            return user.Id;
        }

        /// <summary>
        /// Registers an ID as not existing.
        /// </summary>
        /// <param name="id">ID to register as not existing</param>
        public void RegisterAsNotExisting(Guid id) {
            if (!this.notExistingUserIds.Contains(id)) {
                this.notExistingUserIds.Add(id);
            }
            return;
        }

        /// <summary>
        /// Registers IDs as not existing.
        /// </summary>
        /// <param name="ids">IDs to register as not existing</param>
        public void RegisterAsNotExisting(IEnumerable<Guid> ids) {
            foreach (var id in ids) {
                this.RegisterAsNotExisting(id);
            }
            return;
        }

    }

}