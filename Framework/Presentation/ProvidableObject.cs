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

namespace Framework.Presentation {

    using Forms;
    using Persistence;
    using Persistence.Directories;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Object that can be managed by a data provider.
    /// </summary>
    public class ProvidableObject : PresentableObject, IProvidableObject {

        /// <summary>
        /// Allowed groups for reading/writing this object.
        /// </summary>
        public virtual AllowedGroups AllowedGroups {
            get {
                var allowedGroups = new AllowedGroups();
                var allUsers = new Group("All users");
                allUsers.Members.Add(UserDirectory.AnonymousUser);
                allowedGroups.ForReading.Add(allUsers);
                return allowedGroups;
            }
        }

        /// <summary>
        /// Indicates whether providable object has versions.
        /// </summary>
        public bool HasVersions {
            get { return this.Versions.Count > 0; }
        }

        /// <summary>
        /// Title of this object.
        /// </summary>
        public string Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PresentableFieldForString title;

        /// <summary>
        /// Enumerable of previous persistent versions of this
        /// object.
        /// </summary>
        public ReadOnlyCollection<Model.Version> Versions {
            get { return new List<Model.Version>(0).AsReadOnly(); }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ProvidableObject()
            : base() {
            this.title = new PresentableFieldForString(this, nameof(ProvidableObject.Title));
            this.AddPresentableField(this.title);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="id">ID to set if object is not new</param>
        /// <param name="createdAt">time of creation of object</param>
        /// <param name="createdBy">user who created object</param>
        /// <param name="modifiedAt">time of last modification</param>
        /// <param name="modifiedBy">user who modified object</param>
        public ProvidableObject(Guid id, DateTime createdAt, IUser createdBy, DateTime modifiedAt, IUser modifiedBy)
            : this() {
            this.Id = id;
            this.CreatedAt = createdAt;
            this.CreatedBy = createdBy;
            this.ModifiedAt = modifiedAt;
            this.ModifiedBy = modifiedBy;
            this.IsNew = false;
        }

        /// <summary>
        /// Gets the title of providable object.
        /// </summary>
        public string GetTitle() {
            return this.Title;
        }

    }

}