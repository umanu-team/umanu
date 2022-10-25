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

namespace Framework.Presentation.Web.Controllers {

    using Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// List of file locks.
    /// </summary>
    internal sealed class FileLockList : KeyedCollection<Guid, FileLock> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public FileLockList()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Adds a file lock.
        /// </summary>
        /// <param name="fileId">ID of file to lock</param>
        /// <param name="owner">owner of file lock</param>
        /// <returns>true if lock succeeded, false otherwise</returns>
        public bool AddFileLock(Guid fileId, string owner) {
            bool success;
            this.CleanUp();
            if (this.Contains(fileId)) {
                success = false;
            } else {
                success = true;
                this.Add(new FileLock(fileId, owner));
            }
            return success;
        }

        /// <summary>
        /// Removes all expired file locks from list.
        /// </summary>
        private void CleanUp() {
            var now = UtcDateTime.Now;
            var expiredFileLocks = new List<FileLock>();
            foreach (var fileLock in this) {
                if (fileLock.Timeout < now) {
                    expiredFileLocks.Add(fileLock);
                }
            }
            foreach (var expiredFileLock in expiredFileLocks) {
                this.Remove(expiredFileLock);
            }
            return;
        }

        /// <summary>
        /// Extracts the key of the specified item.
        /// </summary>
        /// <param name="item">item to extract key of</param>
        /// <returns>key of the specified item</returns>
        protected override Guid GetKeyForItem(FileLock item) {
            return item.FileId;
        }

        /// <summary>
        /// Gets the lock for a file ID or null.
        /// </summary>
        /// <param name="fileId">ID of file to lock</param>
        /// <returns>lock for file ID or null</returns>
        public FileLock TryGetItemFor(Guid fileId) {
            FileLock item;
            if (this.Contains(fileId)) {
                item = this[fileId];
            } else {
                item = null;
            }
            return item;
        }

    }

}