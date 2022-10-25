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

    using Framework.Persistence.Fields;
    using Framework.Presentation;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Pesistent group of users.
    /// </summary>
    public sealed class Group : PersistentObject, IProvidableObject, IEquatable<Group> {

        /// <summary>
        /// Members of group.
        /// </summary>
        public PersistentFieldForIUserCollection Members { get; private set; }

        /// <summary>
        /// Title of group.
        /// </summary>
        public string Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PersistentFieldForString title =
            new PersistentFieldForString(nameof(Title));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Group()
            : base() {
            this.Members = new PersistentFieldForIUserCollection(nameof(Members));
            this.RegisterPersistentField(this.Members);
            this.title.IsFullTextIndexed = false;
            this.RegisterPersistentField(this.title);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">language invariant title of group</param>
        public Group(string title)
            : this() {
            this.Title = title;
        }

        /// <summary>
        /// Determines whether the current object is equal to another
        /// object of the same type by ID.
        /// </summary>
        /// <param name="other">object to compare this object to</param>
        /// <returns>true if both objects are equal by ID, false
        /// otherwise</returns>
        public bool Equals(Group other) {
            bool isEqual;
            if (null == other) {
                isEqual = false;
            } else {
                isEqual = this.Id.Equals(other.Id);
            }
            return isEqual;
        }

        /// <summary>
        /// Gets the unique members of an enumerable of groups.
        /// </summary>
        /// <param name="groups">groups to get unique members of</param>
        /// <returns>unique members of enumerable of groups</returns>
        public static IEnumerable<IUser> MembersOf(IEnumerable<Persistence.Group> groups) {
            var memberIds = new List<Guid>();
            foreach (var group in groups) {
                foreach (var member in group.Members) {
                    if (null != member && !memberIds.Contains(member.Id)) {
                        memberIds.Add(member.Id);
                        yield return member;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the title of providable object.
        /// </summary>
        public string GetTitle() {
            return this.Title;
        }

    }

}