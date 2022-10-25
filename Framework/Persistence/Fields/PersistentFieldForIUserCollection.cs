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

    using Framework.Persistence.Directories;
    using Framework.Persistence.Filters;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// List of objects of type IUser that can be stored in
    /// persistence mechanism.
    /// </summary>
    public sealed class PersistentFieldForIUserCollection : PersistentFieldForCollection<IUser>, IPresentableFieldForIUserCollection {

        /// <summary>
        /// Indicates whether value from DbDataReader is set already.
        /// </summary>
        private bool? areValuesFromDbDataReaderSet;

        /// <summary>
        /// Indicates whether all users are contained.
        /// </summary>
        public bool ContainsAllUsers {
            get {
                return this.Contains(Guid.Empty);
            }
        }

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.IUser;
            }
        }

        /// <summary>
        /// Directory to use for user resolval.
        /// </summary>
        public UserDirectory UserDirectory {
            get {
                if (null == this.userDirectory) {
                    this.userDirectory = this.ParentPersistentObject.ParentPersistentContainer.ParentPersistenceMechanism.UserDirectory;
                }
                return this.userDirectory;
            }
            set {
                this.userDirectory = value;
            }
        }
        private UserDirectory userDirectory;

        /// <summary>
        /// Values loaded from DbDataReader.
        /// </summary>
        private List<Guid?> valuesFromDbDataReader;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        public PersistentFieldForIUserCollection(string key)
            : base(key) {
            this.userDirectory = null;
        }

        /// <summary>
        /// Adds a user if it is not contained yet.
        /// </summary>
        /// <param name="user">user to add</param>
        public void AddIfNotContained(IUser user) {
            bool isContained = (null != user && this.Contains(user.Id))
               || (null == user && this.Contains(null));
            if (!isContained) {
                this.Add(user);
            }
            return;
        }

        /// <summary>
        /// Adds users if they are not contained yet.
        /// </summary>
        /// <param name="users">users to add</param>
        public void AddRangeIfNotContained(IEnumerable<IUser> users) {
            foreach (var user in users) {
                this.AddIfNotContained(user);
            }
            return;
        }

        /// <summary>
        /// Determines whether the list contains a user with a
        /// specific id.
        /// </summary>
        /// <param name="id">id of user to locate</param>
        /// <returns>true if user with id is contained, false
        /// otherwise</returns>
        public bool Contains(Guid id) {
            bool isContained = false;
            foreach (var member in this) {
                if (member?.Id == id) {
                    isContained = true;
                    break;
                }
            }
            return isContained;
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
            foreach (var member in this) {
                if (null != member && null != user && (member.Id == user.Id || member.Id == Guid.Empty)) {
                    hasPermissionsContained = true;
                    break;
                }
            }
            return hasPermissionsContained;
        }

        /// <summary>
        /// Ensures that this field was retrieved from persistence at
        /// least once.
        /// </summary>
        protected override void EnsureRetrieval() {
            base.EnsureRetrieval();
            if (false == this.areValuesFromDbDataReaderSet) {
                this.areValuesFromDbDataReaderSet = true;
                var values = new List<IUser>(this.valuesFromDbDataReader.Count);
                DirectoryUserCache preloadedUsers = null;
                foreach (var id in this.valuesFromDbDataReader) {
                    if (null == id) {
                        values.Add(null);
                    } else if (Guid.Empty == id.Value) {
                        values.Add(UserDirectory.AnonymousUser);
                    } else if (null != this.ParentPersistentObject?.ParentPersistentContainer) {
                        if (null == preloadedUsers) {
                            preloadedUsers = new DirectoryUserCache();
                            var filterCriteria = FilterCriteria.Empty;
                            foreach (var idToPreload in this.valuesFromDbDataReader) {
                                if (null != idToPreload && Guid.Empty != idToPreload.Value) {
                                    filterCriteria = filterCriteria.Or(nameof(IUser.Id), RelationalOperator.IsEqualTo, idToPreload.Value);
                                }
                            }
                            preloadedUsers.AddRange(this.UserDirectory.Find(filterCriteria, SortCriterionCollection.Empty, nameof(IUser.DisplayName), nameof(IUser.UserName)));
                        }
                        if (preloadedUsers.Contains(id.Value)) {
                            values.Add(preloadedUsers[id.Value]);
                        }
                    }
                }
                this.SetValuesUnsafe(values);
                this.valuesFromDbDataReader = null;
            }
            return;
        }

        /// <summary>
        /// Gets a new instance of PersistentFieldForElement.
        /// </summary>
        /// <returns>new instance of PersistentFieldForElement</returns>
        protected override PersistentFieldForElement<IUser> GetNewPersistentFieldForElement() {
            return new PersistentFieldForIUser(this.Key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public override IEnumerable<string> GetValuesAsString() {
            foreach (var iUserValue in this) {
                yield return iUserValue?.UserName;
            }
        }

        /// <summary>
        /// Initializes the temporary list for DbDataReader.
        /// </summary>
        internal override void InitializeTemporaryCollectionForDbDataReader() {
            this.valuesFromDbDataReader = new List<Guid?>();
            return;
        }

        /// <summary>
        /// Loads the current value of a DbDataReader into a
        /// temporary collection but does not set it yet.
        /// </summary>
        /// <param name="dataReader">data reader to load value from</param>
        internal override void LoadValueFromDbDataReader(DbDataReader dataReader) {
            if (dataReader.IsDBNull(0)) {
                this.valuesFromDbDataReader.Add(null);
            } else {
                this.valuesFromDbDataReader.Add(dataReader.GetGuid(0));
            }
            return;
        }

        /// <summary>
        /// Removes the object with a specific ID from the list.
        /// </summary>
        /// <param name="id">ID of specific object to remove from
        /// list</param>
        /// <returns>true if object was successfully removed from
        /// list, false otherwise or if object with ID was not
        /// contained in list</returns>
        public bool Remove(Guid id) {
            bool success = false;
            for (var index = this.Count - 1; index > -1; index--) {
                var member = this[index];
                if (member?.Id == id) {
                    this.RemoveAt(index);
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Removes a specific object from the list.
        /// </summary>
        /// <param name="item">specific object to remove from list</param>
        /// <returns>true if object was successfully removed from
        /// list, false otherwise or if object was not contained in
        /// list</returns>
        public sealed override bool Remove(IUser item) {
            bool success = false;
            if (null != item) {
                success = this.Remove(item.Id);
            }
            return success;
        }

        /// <summary>
        /// Sets the new values for persistent field which have been
        /// loaded from DbDataReader before.
        /// </summary>
        internal override void SetValuesFromDbDataReader() {
            this.areValuesFromDbDataReaderSet = false;
            return;
        }

        /// <summary>
        /// Sorts the list by a given field.
        /// </summary>
        /// <param name="fieldKey">key of field to order list by</param>
        public void Sort(string fieldKey) {
            this.Sort(new UserValueComparer(fieldKey));
            return;
        }

        /// <summary>
        /// Synchronized the users of this list with an enumerable of
        /// users.
        /// </summary>
        /// <param name="users">enumerable of users to synchronize
        /// the users of this list with</param>
        public void SynchronizeWith(IEnumerable<IUser> users) {
            var usersToBeDeleted = new List<IUser>();
            foreach (var existingUser in this) {
                bool isExistingUserContainedInNewUsers = false;
                foreach (var newUser in users) {
                    if ((null == existingUser && null == newUser) || (null != existingUser && null != newUser && existingUser.Id == newUser.Id)) {
                        isExistingUserContainedInNewUsers = true;
                        break;
                    }
                }
                if (!isExistingUserContainedInNewUsers) {
                    usersToBeDeleted.Add(existingUser);
                }
            }
            foreach (var userToBeDeleted in usersToBeDeleted) {
                this.Remove(userToBeDeleted);
            }
            this.AddRangeIfNotContained(users);
            return;
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of values of this field and adds it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="item">new value to add</param>
        /// <returns>true if value could be added successfully, false
        /// otherwise</returns>
        public override bool TryAddString(string item) {
            IUser user = this.UserDirectory.FindOneByVagueTerm(item, FilterScope.UserName);
            this.Add(user);
            return string.IsNullOrEmpty(item) || null != user;
        }

    }

}