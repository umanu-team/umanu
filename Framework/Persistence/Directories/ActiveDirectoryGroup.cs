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

    using Framework.Persistence.Filters;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.DirectoryServices;

    /// <summary>
    /// Security group that belongs to an Active Directory.
    /// </summary>
    public sealed class ActiveDirectoryGroup : ICollection<IUser> {

        /// <summary>
        /// Number of users contained in group.
        /// </summary>
        public int Count {
            get {
                var filterCriteria = new FilterCriteria(this.MemberOfFieldName, RelationalOperator.IsEqualTo, this.DistinguishedName, FilterTarget.IsOtherTextValue);
                return this.ParentUserDirectory.GetCount(filterCriteria);
            }
        }

        /// <summary>
        /// Distinguished name of group.
        /// </summary>
        public string DistinguishedName { get; private set; }

        /// <summary>
        /// True if group is read-only, false otherwise.
        /// </summary>
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <summary>
        /// Indicates whether groups in groups are supposed to be
        /// queried.
        /// </summary>
        public bool IsResolvingGroupsInGroups { get; set; }

        /// <summary>
        /// Name of member of field.
        /// </summary>
        private string MemberOfFieldName {
            get {
                string memberOfFieldName;
                if (this.IsResolvingGroupsInGroups) {
                    memberOfFieldName = "memberOf:1.2.840.113556.1.4.1941:";
                } else {
                    memberOfFieldName = "memberOf";
                }
                return memberOfFieldName;
            }
        }

        /// <summary>
        /// Parent user directory of this group.
        /// </summary>
        public ActiveDirectory ParentUserDirectory { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="distinguishedName">distinguished name of
        /// group</param>
        /// <param name="parentUserDirectory">parent user directory
        /// of group</param>
        internal ActiveDirectoryGroup(string distinguishedName, ActiveDirectory parentUserDirectory) {
            this.DistinguishedName = distinguishedName;
            this.ParentUserDirectory = parentUserDirectory;
        }

        /// <summary>
        /// Adds a user to the group.
        /// </summary>
        /// <param name="user">user to add to the group</param>
        public void Add(IUser user) {
            var activeDirectoryUser = user as ActiveDirectoryUser;
            if (null == activeDirectoryUser) {
                throw new ArgumentException("User is either null or not of type \"" + typeof(ActiveDirectoryUser).FullName + "\".", nameof(user));
            }
            this.Add(activeDirectoryUser);
            return;
        }

        /// <summary>
        /// Adds a user to the group.
        /// </summary>
        /// <param name="user">user to add to the group</param>
        public void Add(ActiveDirectoryUser user) {
            using (DirectoryEntry container = new DirectoryEntry("LDAP://" + this.DistinguishedName)) {
                container.Properties["member"].Add(user.DistinguishedName);
                container.CommitChanges();
            }
            return;
        }

        /// <summary>
        /// Removes all users from group.
        /// </summary>
        public void Clear() {
            foreach (IUser user in this) {
                this.Remove(user);
            }
            return;
        }

        /// <summary>
        /// Determines whether this group contains a specific user.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to look for</param>
        /// <returns>true if specific user is contained, false
        /// otherwise</returns>
        public bool Contains(FilterCriteria filterCriteria) {
            return null != this.FindOne(filterCriteria, SortCriterionCollection.Empty, new string[0]);
        }

        /// <summary>
        /// Determines whether the group contains a specific user.
        /// </summary>
        /// <param name="id">ID of specific user to look for</param>
        /// <returns>true if specific user is contained, false
        /// otherwise</returns>
        public bool Contains(Guid id) {
            var filterCriteria = new FilterCriteria(nameof(ActiveDirectoryUser.Id), RelationalOperator.IsEqualTo, id);
            return this.Contains(filterCriteria);
        }

        /// <summary>
        /// Determines whether the directory contains a specific
        /// user.
        /// </summary>
        /// <param name="item">specific user to look for by ID</param>
        /// <returns>true if specific user is contained, false
        /// otherwise</returns>
        public bool Contains(IUser item) {
            return this.Contains(item.Id);
        }

        /// <summary>
        /// Copies all users of the group to an array, starting at a
        /// particular array index. 
        /// </summary>
        /// <param name="array">array to copy the users into</param>
        /// <param name="arrayIndex">array index to start at</param>
        public void CopyTo(IUser[] array, int arrayIndex) {
            var users = new List<IUser>(this.Find(FilterCriteria.Empty, SortCriterionCollection.Empty));
            users.CopyTo(array, arrayIndex);
            return;
        }

        /// <summary>
        /// Finds all matching users from this group.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// users for</param>
        /// <param name="sortCriteria">criteria to sort users by</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional LDAP properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>all matching users from this group</returns>
        public IEnumerable<IUser> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, params string[] propertiesToPreLoad) {
            return this.Find(filterCriteria, sortCriteria, 0, ulong.MaxValue, propertiesToPreLoad);
        }

        /// <summary>
        /// Finds all matching users from this group.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// users for</param>
        /// <param name="sortCriteria">criteria to sort users by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to return</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional LDAP properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>all matching users from this group</returns>
        public IEnumerable<IUser> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, params string[] propertiesToPreLoad) {
            var groupFilterCriteria = new FilterCriteria(this.MemberOfFieldName, RelationalOperator.IsEqualTo, this.DistinguishedName, FilterTarget.IsOtherTextValue).And(filterCriteria);
            return this.ParentUserDirectory.Find(groupFilterCriteria, sortCriteria, startPosition, maxResults, propertiesToPreLoad);
        }

        /// <summary>
        /// Finds all distinct values from this group of a specific
        /// property.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// users for</param>
        /// <param name="sortCriteria">criteria to sort the distinct
        /// result values by</param>
        /// <param name="fieldName">specific property to find
        /// distinct values for</param>
        /// <returns>all distinct values from this group of a
        /// specific property</returns>
        public IEnumerable<string> FindDistinctValues(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string fieldName) {
            var groupFilterCriteria = new FilterCriteria(this.MemberOfFieldName, RelationalOperator.IsEqualTo, this.DistinguishedName, FilterTarget.IsOtherTextValue).And(filterCriteria);
            return this.ParentUserDirectory.FindDistinctValues(groupFilterCriteria, sortCriteria, fieldName);
        }

        /// <summary>
        /// Finds the first matching user from this group.
        /// </summary>
        /// <param name="id">ID to find user for</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional LDAP properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>first matching user from this group or null if
        /// no match was found</returns>
        public IUser FindOne(Guid id, params string[] propertiesToPreLoad) {
            var filterCriteria = new FilterCriteria(nameof(ActiveDirectoryUser.Id), RelationalOperator.IsEqualTo, id);
            return this.FindOne(filterCriteria, SortCriterionCollection.Empty, propertiesToPreLoad);
        }

        /// <summary>
        /// Finds the first matching user from this group.
        /// </summary>
        /// <param name="user">user to find by ID</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional LDAP properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>first matching user from this group or null if
        /// no match was found</returns>
        public IUser FindOne(IUser user, params string[] propertiesToPreLoad) {
            return this.FindOne(user.Id, propertiesToPreLoad);
        }

        /// <summary>
        /// Finds the first matching user from this group.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// user for</param>
        /// <param name="sortCriteria">criteria to sort users by</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional LDAP properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>first matching user from this group or null if
        /// no match was found</returns>
        public IUser FindOne(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, params string[] propertiesToPreLoad) {
            IUser match = null;
            foreach (var result in this.Find(filterCriteria, sortCriteria, 0, 1, propertiesToPreLoad)) {
                match = result;
                break;
            }
            return match;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// group.
        /// </summary>
        /// <returns>enumerator that iterates through the group</returns>
        public IEnumerator<IUser> GetEnumerator() {
            return this.Find(FilterCriteria.Empty, SortCriterionCollection.Empty).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// directory.
        /// </summary>
        /// <returns>enumerator that iterates through the directory</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            foreach (var value in this.Find(FilterCriteria.Empty, SortCriterionCollection.Empty)) {
                yield return value;
            }
        }

        /// <summary>
        /// Removes a specific user from this group.
        /// </summary>
        /// <param name="user">specific user to remove from group</param>
        /// <returns>true if user was successfully removed from
        /// group, false otherwise or if user was not contained in
        /// group</returns>
        public bool Remove(IUser user) {
            var activeDirectoryUser = user as ActiveDirectoryUser;
            if (null == activeDirectoryUser) {
                throw new ArgumentException("User is either null or not of type \"" + typeof(ActiveDirectoryUser).FullName + "\".", nameof(user));
            }
            return this.Remove(activeDirectoryUser);
        }

        /// <summary>
        /// Removes a specific user from this group.
        /// </summary>
        /// <param name="user">specific user to remove from group</param>
        /// <returns>true if user was successfully removed from
        /// group, false otherwise or if user was not contained in
        /// group</returns>
        public bool Remove(ActiveDirectoryUser user) {
            bool isRemoved = false;
            using (DirectoryEntry container = new DirectoryEntry("LDAP://" + this.DistinguishedName)) {
                try {
                    container.Properties["member"].Remove(user.DistinguishedName);
                    container.CommitChanges();
                    isRemoved = true;
                } catch (Exception) {
                    // ignore exceptions
                }
            }
            return isRemoved;
        }

    }

}