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

    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;
    using System.Web;

    /// <summary>
    /// Represents a user directory that exists in memory only.
    /// </summary>
    public class InMemoryUserDirectory : UserDirectory {

        /// <summary>
        /// Gets the current user that is logged on.
        /// </summary>
        public override IUser CurrentUser {
            get { return this.currentUser; }
        }
        private readonly IUser currentUser;

        /// <summary>
        /// Indicates whether user directory supports sorting of
        /// values within directory requests.
        /// </summary>
        protected override bool IsCapableToSortWithinRequests {
            get { return false; }
        }

        /// <summary>
        /// List of users of directory.
        /// </summary>
        private readonly List<IUser> users;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        private InMemoryUserDirectory()
            : base() {
            this.users = new List<IUser>();
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="currentUser">user to be set as current user</param>
        public InMemoryUserDirectory(IUser currentUser)
            : this() {
            this.currentUser = currentUser;
            this.users.Add(currentUser);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="currentUserSource">source to use for
        /// resolving the current user</param>
        public InMemoryUserDirectory(CurrentUserSource currentUserSource)
            : this(InMemoryUserDirectory.GetCurrentUserFrom(currentUserSource, null)) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpContext">HTTP context to use as current
        /// user source</param>
        public InMemoryUserDirectory(HttpContext httpContext)
            : this(InMemoryUserDirectory.GetCurrentUserFrom(CurrentUserSource.HttpContext, httpContext)) {
            // nothing to do
        }

        /// <summary>
        /// Adds a user to the directory setting a given password.
        /// </summary>
        /// <param name="user">user to add to the directory</param>
        /// <param name="password">password to set for new user</param>
        protected override void AddNoCache(IUser user, string password) {
            if (null == user) {
                throw new ArgumentException("User may not be null.", nameof(user));
            }
            foreach (var existingUser in this.users) {
                if (existingUser.UserName == user.UserName) {
                    throw new DirectoryException("New user cannot be added to user directory because the user name is not unique.");
                }
            }
            // TODO: user.SetPassword(password);
            this.users.Add(user);
            return;
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
            throw new NotImplementedException("Finding distinct values is not implemented yet.");
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
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>all matching users from this directory</returns>
        protected override IEnumerable<IUser> FindNoCache(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, params string[] propertiesToPreLoad) {
            var matchingUsers = new List<IUser>(this.users);
            filterCriteria?.Filter(matchingUsers);
            if (null != sortCriteria) {
                matchingUsers.Sort(new SortCriteriaComparer(sortCriteria));
            }
            if (startPosition > ulong.MinValue || maxResults < ulong.MaxValue) {
                new Range(startPosition, maxResults).ApplyTo(matchingUsers);
            }
            return matchingUsers;
        }

        /// <summary>
        /// Gets the current user from a specified source.
        /// </summary>
        /// <param name="currentUserSource">source to use for
        /// resolving the current user</param>
        /// <param name="httpContext">HTTP context to use as current
        /// user source - may be null if HTTP context is not used as
        /// current user source</param>
        /// <returns>current user</returns>
        private static IUser GetCurrentUserFrom(CurrentUserSource currentUserSource, HttpContext httpContext) {
            string userName;
            if (CurrentUserSource.HttpContext == currentUserSource) {
                var principal = httpContext.User;
                if (null == principal) {
                    userName = string.Empty;
                } else {
                    userName = principal.Identity.Name;
                }
            } else if (CurrentUserSource.Environment == currentUserSource) {
                userName = Environment.UserName;
            } else if (CurrentUserSource.WindowsIdentity == currentUserSource) {
                var identity = WindowsIdentity.GetCurrent();
                userName = identity.Name;
            } else {
                throw new DirectoryException("Source \"" + currentUserSource.ToString() + "\" for resolving current user is unknown.");
            }
            int lastSlashIndex = userName.LastIndexOf('\\');
            if (lastSlashIndex > -1) {
                userName = userName.Substring(lastSlashIndex + 1);
            }
            IUser currentUser;
            if (string.IsNullOrEmpty(userName)) {
                currentUser = UserDirectory.AnonymousUser;
            } else {
                currentUser = new InMemoryUser(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1), userName);
            }
            return currentUser;
        }

        /// <summary>
        /// Removes a specific user from the directory.
        /// </summary>
        /// <param name="user">specific user to remove from directory</param>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        protected override bool RemoveNoCache(IUser user) {
            return this.users.Remove(user);
        }

    }

}