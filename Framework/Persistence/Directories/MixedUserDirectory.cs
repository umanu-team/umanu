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

    using Exceptions;
    using Filters;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a combination of user directories.
    /// </summary>
    public sealed class MixedUserDirectory : UserDirectory {

        /// <summary>
        /// Number of users contained in directory.
        /// </summary>
        public override int Count {
            get {
                int count = 0;
                foreach (var userDirectory in this.UserDirectories) {
                    count += userDirectory.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// Gets the current user that is logged on.
        /// </summary>
        public override IUser CurrentUser {
            get {
                IUser currentUser = UserDirectory.AnonymousUser;
                foreach (var userDirectory in this.UserDirectories) {
                    var user = userDirectory.CurrentUser;
                    if (user.Id != UserDirectory.AnonymousUser.Id) {
                        currentUser = user;
                        break;
                    }
                }
                return currentUser;
            }
        }

        /// <summary>
        /// Indicates whether user directory supports sorting of
        /// values within directory requests.
        /// </summary>
        protected override bool IsCapableToSortWithinRequests {
            get { return true; }
        }

        /// <summary>
        /// Nested user directories.
        /// </summary>
        public IEnumerable<UserDirectory> UserDirectories { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="userDirectories">nested user directories to
        /// mix</param>
        public MixedUserDirectory(IEnumerable<UserDirectory> userDirectories)
            : base() {
            this.UserDirectories = userDirectories;
        }

        /// <summary>
        /// Adds a user to the directory. The diplay name is used as
        /// common name.
        /// </summary>
        /// <param name="user">user to add to the directory</param>
        /// <param name="password">password to set for new user</param>
        protected override void AddNoCache(IUser user, string password) {
            throw new DirectoryException("It is not possible to add users to mixed user directories.");
        }

        /// <summary>
        /// Clears the cache of users in local memory.
        /// </summary>
        public override void ClearCache() {
            base.ClearCache();
            foreach (var userDirectory in this.UserDirectories) {
                userDirectory.ClearCache();
            }
            return;
        }

        /// <summary>
        /// Finds all matching users from this directory.
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
        /// prior to the actual retrieval of contacts - this can also
        /// be used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>all matching contacts from this directory</returns>
        protected override IEnumerable<IUser> FindNoCache(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, params string[] propertiesToPreLoad) {
            uint results = 0;
            foreach (var userDirectory in this.UserDirectories) {
                foreach (var user in userDirectory.Find(filterCriteria, sortCriteria, startPosition, maxResults, propertiesToPreLoad)) {
                    yield return user;
                    results++;
                    if (results >= maxResults) {
                        break;
                    }
                }
                if (results >= maxResults) {
                    break;
                }
            }
        }

        /// <summary>
        /// Finds all distinct values of a specific property.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// users for</param>
        /// <param name="sortCriteria">criteria to sort the distinct
        /// result values by</param>
        /// <param name="fieldName">specific property to find
        /// distinct values for</param>
        /// <returns>all distinct values of a specific property</returns>
        public override IEnumerable<string> FindDistinctValues(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string fieldName) {
            var distinctValues = new List<string>();
            foreach (var userDirectory in this.UserDirectories) {
                foreach (var distinctValue in userDirectory.FindDistinctValues(filterCriteria, sortCriteria, fieldName)) {
                    if (!distinctValues.Contains(distinctValue)) {
                        distinctValues.Add(distinctValue);
                        yield return distinctValue;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the first nexted user directory of a type.
        /// </summary>
        /// <typeparam name="T">type of nested user directory to get</typeparam>
        /// <returns>first nexted user directory of type or null</returns>
        public T GetNestedUserDirectory<T>() where T : UserDirectory {
            T matchingUserDirectory = null;
            foreach (var userDirectory in this.UserDirectories) {
                matchingUserDirectory = userDirectory as T;
                if (null != matchingUserDirectory) {
                    break;
                }
            }
            return matchingUserDirectory;
        }

        /// <summary>
        /// Removes a specific user from the directory.
        /// </summary>
        /// <param name="item">specific user to remove from directory</param>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        protected override bool RemoveNoCache(IUser item) {
            bool isRemoved = false;
            foreach (var userDirectory in this.UserDirectories) {
                isRemoved = userDirectory.Remove(item) || isRemoved;
            }
            return isRemoved;
        }

    }

}