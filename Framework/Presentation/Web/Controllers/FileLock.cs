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

    /// <summary>
    /// Represents a file lock.
    /// </summary>
    internal sealed class FileLock {

        /// <summary>
        /// ID of locked file.
        /// </summary>
        public Guid FileId { get; private set; }

        /// <summary>
        /// ID of lock.
        /// </summary>
        public Guid LockId { get; private set; }

        /// <summary>
        /// Token of lock.
        /// </summary>
        public string LockToken {
            get { return "opaquelocktoken:" + this.LockId.ToString("D"); }
        }

        /// <summary>
        /// Owner of lock.
        /// </summary>
        public string Owner { get; private set; }

        /// <summary>
        /// Expiration date and time of lock.
        /// </summary>
        public DateTime Timeout { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fileId">ID of locked file</param>
        /// <param name="owner">owner of lock</param>
        public FileLock(Guid fileId, string owner) {
            this.FileId = fileId;
            this.LockId = Guid.NewGuid();
            this.Owner = owner;
            this.RefreshTimeout();
        }

        /// <summary>
        /// Refreshed the expiration date and time of lock.
        /// </summary>
        public void RefreshTimeout() {
            this.Timeout = UtcDateTime.Now.AddMinutes(2);
            return;
        }

    }

}