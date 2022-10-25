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

    using Framework.Model;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Web;

    /// <summary>
    /// Represents a persistent user directory.
    /// </summary>
    public abstract class PersistentUserDirectory : UserDirectory {

        /// <summary>
        /// Gets the current user that is logged on.
        /// </summary>
        public override IUser CurrentUser {
            get {
                if (null == this.currentUser) {
                    this.LogOn();
                }
                return this.currentUser;
            }
        }
        private IUser currentUser;

        /// <summary>
        /// Gets the user of the current log-on attempt if password
        /// was expired.
        /// </summary>
        public PersistentUser ExpiredUser {
            get {
                if (null == this.currentUser) {
                    this.LogOn();
                }
                return this.expiredUser;
            }
            protected set {
                this.expiredUser = value;
            }
        }
        private PersistentUser expiredUser;

        /// <summary>
        /// Indicates whether email addresses are supposed to be set
        /// as user names automatically.
        /// </summary>
        public static bool HasEmailAddressAsUserName { get; set; } = true;

        /// <summary>
        /// Indicates whether user directory supports sorting of
        /// values within directory requests.
        /// </summary>
        protected override bool IsCapableToSortWithinRequests {
            get { return true; }
        }

        /// <summary>
        /// Log-on infos.
        /// </summary>
        protected static ConcurrentDictionary<Guid, LogOnInfo> LogOnInfos { get; private set; } = new ConcurrentDictionary<Guid, LogOnInfo>();

        /// <summary>
        /// Persistence mechanism to use as user store.
        /// </summary>
        public PersistenceMechanism PersistenceMechanism {
            set {
                if (SecurityModel.IgnorePermissions != value.SecurityModel) {
                    throw new ArgumentException("Security model \"" + value.SecurityModel.ToString() + "\" is not supported by persistent user directory.");
                }
                this.Users = value.FindContainer<PersistentUser>();
            }
        }

        /// <summary>
        /// Persistent container of all persistent users.
        /// </summary>
        protected PersistentContainer<PersistentUser> Users { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpContext">HTTP context to use as current
        /// user source</param>
        public PersistentUserDirectory(HttpContext httpContext) {
            this.HttpContext = httpContext;
        }

        /// <summary>
        /// Adds a user to the directory setting a given password.
        /// </summary>
        /// <param name="user">user to add to the directory</param>
        /// <param name="password">password to set for new user</param>
        protected override void AddNoCache(IUser user, string password) {
            var persistentUser = user as PersistentUser;
            if (null == persistentUser) {
                throw new ArgumentException("User is either null or not of type \"" + typeof(PersistentUser).FullName + "\".", nameof(user));
            }
            var filterCriteria = new FilterCriteria(nameof(PersistentUser.UserName), RelationalOperator.IsEqualTo, persistentUser.UserName, FilterTarget.IsOtherTextValue);
            if (this.Users.Contains(filterCriteria)) {
                throw new DirectoryException("New user cannot be added to user directory because the user name is not unique.");
            }
            persistentUser.SetPassword(password);
            this.Users.AddCascadedly(persistentUser);
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
            IEnumerable<object[]> distinctValueArrays = this.Users.FindDistinctValues(filterCriteria, sortCriteria, new string[] { fieldName });
            foreach (object[] distinctValueArray in distinctValueArrays) {
                if (distinctValueArray.LongLength > 0) {
                    string distinctValue = distinctValueArray[0] as string;
                    if (!string.IsNullOrEmpty(distinctValue)) {
                        yield return distinctValue;
                    }
                }
            }
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
            return this.Users.Find(filterCriteria, sortCriteria, startPosition, maxResults);
        }

        /// <summary>
        /// Refreshes the CurrentUser property by (re)loading the
        /// user from user source.
        /// </summary>
        /// <returns>current user that just logged on</returns>
        public abstract void LogOn();

        /// <summary>
        /// Refreshes the CurrentUser property by (re)loading the
        /// user from user source.
        /// </summary>
        /// <param name="httpCredential">user credential to log on</param>
        /// <returns>current user that just logged on</returns>
        protected IUser LogOn(NetworkCredential httpCredential) {
            IUser user = UserDirectory.AnonymousUser;
            if (null != httpCredential) {
                var filterCriteria = new FilterCriteria(nameof(PersistentUser.UserName), RelationalOperator.IsEqualTo, httpCredential.UserName, FilterTarget.IsOtherTextValue);
                var persistentUser = this.Users.FindOne(filterCriteria, SortCriterionCollection.Empty);
                if (null != persistentUser) {
                    var logOnInfo = PersistentUserDirectory.LogOnInfos.GetOrAdd(persistentUser.Id, delegate (Guid key) {
                        return new LogOnInfo();
                    });
                    lock (logOnInfo) {
                        var now = UtcDateTime.Now;
                        if (logOnInfo.LockedUntil <= now.AddSeconds(LogOnInfo.MinimumDelayInSeconds)) {
                            if (logOnInfo.LockedUntil > now) {
                                System.Threading.Thread.Sleep(logOnInfo.LockedUntil - now);
                            }
                            if (persistentUser.HasPassword(httpCredential.Password)) {
                                if (persistentUser.PasswordExpirationDate < UtcDateTime.Now) {
                                    this.ExpiredUser = persistentUser;
                                } else {
                                    user = persistentUser;
                                }
                                PersistentUserDirectory.LogOnInfos.TryRemove(persistentUser.Id, out _);
                            } else {
                                logOnInfo.AddFailedAttempt(httpCredential.Password);
                            }
                        }
                    }
                }
            }
            this.SetCurrentUser(user);
            return user;
        }

        /// <summary>
        /// Removes a specific user from the directory.
        /// </summary>
        /// <param name="item">specific user to remove from directory</param>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        protected override bool RemoveNoCache(IUser item) {
            bool result;
            var user = this.Users.FindOne(item.Id);
            if (null == user) {
                result = false;
            } else {
                result = user.RemoveCascadedly();
            }
            return result;
        }

        /// <summary>
        /// Sets the current user.
        /// </summary>
        /// <param name="currentUser">current user to set</param>
        protected void SetCurrentUser(IUser currentUser) {
            this.currentUser = currentUser;
        }

    }

}