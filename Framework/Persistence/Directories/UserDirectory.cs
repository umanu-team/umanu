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

    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Represents a user directory.
    /// </summary>
    public abstract class UserDirectory : ICollection<IUser> {

        /// <summary>
        /// Gets an anonymous user.
        /// </summary>
        public static readonly IUser AnonymousUser = new AnonymousUser();

        /// <summary>
        /// Number of users contained in directory.
        /// </summary>
        public virtual int Count {
            get {
                int count = 0;
                foreach (var user in this.Find(FilterCriteria.Empty, SortCriterionCollection.Empty)) {
                    count++;
                }
                return count;
            }
        }

        /// <summary>
        /// Gets the current user that is logged on.
        /// </summary>
        public abstract IUser CurrentUser { get; }

        /// <summary>
        /// Indicates whether display names of users are generated as
        /// "FirstName LastName" if false or invertedly as "LastName,
        /// FirstName" if true.
        /// </summary>
        public static bool HasInvertedDisplayNameGeneration { get; set; }

        /// <summary>
        /// HTTP context to use as current user source.
        /// </summary>
        public HttpContext HttpContext { get; protected set; }

        /// <summary>
        /// Indicates whether user directory supports sorting of
        /// values within directory requests.
        /// </summary>
        protected abstract bool IsCapableToSortWithinRequests { get; }

        /// <summary>
        /// Indicates whether current user is supposed to be
        /// anonymized for saving data.
        /// </summary>
        public bool IsCurrentUserAnonymized { get; set; }

        /// <summary>
        /// Indicates whether current user is anonymous.
        /// </summary>
        public bool IsCurrentUserAnonymous {
            get { return null == this.CurrentUser || this.CurrentUser.Id == UserDirectory.AnonymousUser.Id; }
        }

        /// <summary>
        /// True if directory is read-only, false otherwise.
        /// </summary>
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <summary>
        /// Time span for user passwords to expire after.
        /// </summary>
        public static TimeSpan PasswordExpirationTimeSpan = TimeSpan.FromDays(365);

        /// <summary>
        /// Cache for directory users.
        /// </summary>
        private readonly DirectoryUserCache userCache;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public UserDirectory() {
            this.IsCurrentUserAnonymized = false;
            this.userCache = new DirectoryUserCache();
        }

        /// <summary>
        /// Adds a user to the directory setting a random password.
        /// </summary>
        /// <param name="user">user to add to the directory</param>
        public void Add(IUser user) {
            this.Add(user, PasswordGenerator.GeneratePassword(16));
        }

        /// <summary>
        /// Adds a user to the directory setting a given password.
        /// </summary>
        /// <param name="user">user to add to the directory</param>
        /// <param name="password">password to set for new user</param>
        public void Add(IUser user, string password) {
            if (null != user && !this.userCache.Contains(user.Id)) {
                this.userCache.Add(user);
            }
            this.AddNoCache(user, password);
        }

        /// <summary>
        /// Adds a user to the directory setting a given password.
        /// </summary>
        /// <param name="user">user to add to the directory</param>
        /// <param name="password">password to set for new user</param>
        protected abstract void AddNoCache(IUser user, string password);

        /// <summary>
        /// Removes all users from directory.
        /// </summary>
        public void Clear() {
            throw new NotSupportedException("Deletion of whole directorys is not supported. Please delete all users one after the other if required.");
        }

        /// <summary>
        /// Clears the cache of users in local memory.
        /// </summary>
        public virtual void ClearCache() {
            this.userCache.Clear();
            return;
        }

        /// <summary>
        /// Determines whether the directory contains a specific
        /// user.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to look for</param>
        /// <returns>true if specific user is contained, false
        /// otherwise</returns>
        public bool Contains(FilterCriteria filterCriteria) {
            return null != this.FindOne(filterCriteria, SortCriterionCollection.Empty);
        }

        /// <summary>
        /// Determines whether the directory contains a specific
        /// user.
        /// </summary>
        /// <param name="id">ID of specific user to look for</param>
        /// <returns>true if specific user is contained, false
        /// otherwise</returns>
        public bool Contains(Guid id) {
            var filterCriteria = new FilterCriteria(nameof(IUser.Id), RelationalOperator.IsEqualTo, id);
            return this.Contains(filterCriteria);
        }

        /// <summary>
        /// Determines whether the directory contains a specific
        /// user.
        /// </summary>
        /// <param name="user">specific user to look for by ID</param>
        /// <returns>true if specific user is contained, false
        /// otherwise</returns>
        public bool Contains(IUser user) {
            bool isContained;
            if (null == user) {
                isContained = false;
            } else {
                isContained = this.Contains(user.Id);
            }
            return isContained;
        }

        /// <summary>
        /// Copies all users of the directory to an array, starting
        /// at a particular array index. 
        /// </summary>
        /// <param name="array">array to copy the users into</param>
        /// <param name="arrayIndex">array index to start at</param>
        public void CopyTo(IUser[] array, int arrayIndex) {
            var users = new List<IUser>(this.Find(FilterCriteria.Empty, SortCriterionCollection.Empty));
            users.CopyTo(array, arrayIndex);
            return;
        }

        /// <summary>
        /// Encrypts a password using a non reversable algorithm.
        /// </summary>
        /// <param name="password">password to encrypt</param>
        /// <returns>encrypted password</returns>
        public static string EncryptPassword(string password) {
            return UserDirectory.EncryptPassword(password, PasswordGenerator.GeneratePassword(16));
        }

        /// <summary>
        /// Encrypts a password using a non reversable algorithm.
        /// </summary>
        /// <param name="password">password to encrypt</param>
        /// <param name="salt">salt to be used for encryption</param>
        /// <returns>encrypted password</returns>
        public static string EncryptPassword(string password, string salt) {
            if (password.Length > 4096) {
                throw new ArgumentException("Password is too long.", nameof(password));
            }
            byte[] hashBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            using (var sha = Mono.Security.Cryptography.SHA224Managed.Create()) {
                for (int i = 0; i < 50777; i++) {
                    byte[] saledHashBytes = new byte[saltBytes.LongLength + hashBytes.LongLength];
                    Array.Copy(hashBytes, saledHashBytes, hashBytes.LongLength);
                    Array.Copy(saltBytes, 0, saledHashBytes, hashBytes.LongLength, saltBytes.LongLength);
                    hashBytes = sha.ComputeHash(saledHashBytes);
                }
            }
            var hashBuilder = new StringBuilder(hashBytes.Length);
            foreach (var hashByte in hashBytes) {
                hashBuilder.Append(hashByte.ToString("x2"));
            }
            return "$0$" + salt + hashBuilder.ToString();
        }

        /// <summary>
        /// Finds all matching users from this directory.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// users for</param>
        /// <param name="sortCriteria">criteria to sort users by</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>all matching users from this directory</returns>
        public IEnumerable<IUser> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, params string[] propertiesToPreLoad) {
            return this.Find(filterCriteria, sortCriteria, 0, ulong.MaxValue, propertiesToPreLoad);
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
        public IEnumerable<IUser> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, params string[] propertiesToPreLoad) {
            List<IUser> matchingUsers;
            if (1 == maxResults) {
                matchingUsers = new List<IUser>(1);
            } else {
                matchingUsers = new List<IUser>();
            }
            var isRetrievedFromCache = false;
            var cacheIds = UserDirectory.GetCacheIdsFor(filterCriteria);
            if (cacheIds.Count > 0 && this.userCache.Contains(cacheIds)) {
                foreach (var cacheId in cacheIds) {
                    matchingUsers.Add(this.userCache[cacheId]);
                }
                isRetrievedFromCache = true;
            } else if (cacheIds.Count < 1 || !this.userCache.AreRegisteredAsNotExisting(cacheIds)) {
                foreach (var matchingUser in this.FindNoCache(filterCriteria, sortCriteria, startPosition, maxResults, propertiesToPreLoad)) {
                    if (this.userCache.Contains(matchingUser.Id)) {
                        matchingUsers.Add(this.userCache[matchingUser.Id]);
                    } else {
                        matchingUsers.Add(matchingUser);
                        this.userCache.Add(matchingUser);
                    }
                    cacheIds.Remove(matchingUser.Id);
                }
                this.userCache.RegisterAsNotExisting(cacheIds);
            }
            if ((isRetrievedFromCache || !this.IsCapableToSortWithinRequests) && sortCriteria.Count > 1) {
                matchingUsers.Sort(new SortCriteriaComparer(sortCriteria));
            }
            if (isRetrievedFromCache) {
                new Range(startPosition, maxResults).ApplyTo(matchingUsers);
            }
            return matchingUsers;
        }

        /// <summary>
        /// Finds users by a vague search term containing either the
        /// user name or the display name.
        /// </summary>
        /// <param name="vagueTerm">search term for user</param>
        /// <param name="filterScope">filter scope for search term</param>
        /// <returns>matching users</returns>
        public IEnumerable<IUser> FindByVagueTerm(string vagueTerm, FilterScope filterScope) {
            return this.FindByVagueTerm(vagueTerm, filterScope, 0, ulong.MaxValue);
        }

        /// <summary>
        /// Finds users by a vague search term containing either the
        /// user name or the display name.
        /// </summary>
        /// <param name="vagueTerm">search term for user</param>
        /// <param name="filterScope">filter scope for search term</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to return</param>
        /// <returns>matching users</returns>
        public IEnumerable<IUser> FindByVagueTerm(string vagueTerm, FilterScope filterScope, ulong startPosition, ulong maxResults) {
            if (!string.IsNullOrEmpty(vagueTerm)) {
                vagueTerm = vagueTerm.Trim();
            }
            string anonymousUserValue = new PersistentFieldForIUser("User", UserDirectory.AnonymousUser).ValueAsString;
            if (string.IsNullOrEmpty(vagueTerm)) {
                // nothing to do
            } else if (anonymousUserValue == vagueTerm) {
                yield return UserDirectory.AnonymousUser;
            } else {
                string userName;
                int leftParenthesisIndex = vagueTerm.LastIndexOf('(');
                if (leftParenthesisIndex > -1 && vagueTerm.EndsWith(")", StringComparison.Ordinal)) {
                    userName = vagueTerm.Substring(leftParenthesisIndex + 1, vagueTerm.Length - leftParenthesisIndex - 2);
                } else {
                    userName = string.Empty;
                }
                FilterCriteria filterCriteria;
                if (string.IsNullOrEmpty(userName)) {
                    filterCriteria = new FilterCriteria(nameof(IUser.UserName), RelationalOperator.IsEqualTo, vagueTerm, FilterTarget.IsOtherTextValue);
                    if (FilterScope.UserNameAndDisplayName == filterScope) {
                        filterCriteria = filterCriteria.Or(nameof(IUser.DisplayName), RelationalOperator.StartsWith, vagueTerm, FilterTarget.IsOtherTextValue);
                    }
                } else {
                    filterCriteria = new FilterCriteria(nameof(IUser.UserName), RelationalOperator.IsEqualTo, userName, FilterTarget.IsOtherTextValue);
                }
                var sortCriteria = new SortCriterionCollection {
                    { nameof(IUser.UserName), SortDirection.Ascending }
                };
                foreach (var user in this.Find(filterCriteria, sortCriteria, startPosition, maxResults, nameof(IUser.DisplayName), nameof(IUser.UserName))) {
                    yield return user;
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
        public abstract IEnumerable<string> FindDistinctValues(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string fieldName);

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
        protected abstract IEnumerable<IUser> FindNoCache(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, params string[] propertiesToPreLoad);

        /// <summary>
        /// Finds the first matching user from this directory.
        /// </summary>
        /// <param name="id">ID to find user for</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>first matching user from this directory or null
        /// if no match was found</returns>
        public IUser FindOne(Guid id, params string[] propertiesToPreLoad) {
            var filterCriteria = new FilterCriteria(nameof(IUser.Id), RelationalOperator.IsEqualTo, id);
            return this.FindOne(filterCriteria, SortCriterionCollection.Empty, propertiesToPreLoad);
        }

        /// <summary>
        /// Finds the first matching user from this directory.
        /// </summary>
        /// <param name="user">user to find by ID</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>first matching user from this directory or
        /// null if no match was found</returns>
        public IUser FindOne(IUser user, params string[] propertiesToPreLoad) {
            return this.FindOne(user.Id, propertiesToPreLoad);
        }

        /// <summary>
        /// Finds the first matching user from this directory.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// user for</param>
        /// <param name="sortCriteria">criteria to sort users by</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>first matching user from this directory or null
        /// if no match was found</returns>
        public IUser FindOne(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, params string[] propertiesToPreLoad) {
            IUser match = null;
            foreach (var result in this.Find(filterCriteria, sortCriteria, 0, 1, propertiesToPreLoad)) {
                match = result;
                break;
            }
            return match;
        }

        /// <summary>
        /// Finds a user by a vague search term containing either the
        /// user name or the display name.
        /// </summary>
        /// <param name="vagueTerm">search term for user</param>
        /// <param name="filterScope">filter scope for search term</param>
        /// <returns>matching user</returns>
        public IUser FindOneByVagueTerm(string vagueTerm, FilterScope filterScope) {
            IUser match = null;
            foreach (var result in this.FindByVagueTerm(vagueTerm, filterScope, 0, 1)) {
                match = result;
                break;
            }
            return match;
        }

        /// <summary>
        /// Gets the cache IDs for filter criteria.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to get cache
        /// IDs for</param>
        /// <returns>cache IDs for filter criteria</returns>
        private static IList<Guid> GetCacheIdsFor(FilterCriteria filterCriteria) {
            var cacheIds = new List<Guid>();
            if (!filterCriteria.IsEmpty) {
                var filterCriterion = filterCriteria;
                do {
                    bool isOrOrNotConnected = filterCriterion.Connective == FilterConnective.None || filterCriterion.Connective == FilterConnective.Or;
                    if (filterCriterion.HasSubFilterCriteria) {
                        IList<Guid> subCacheIds = null;
                        if (isOrOrNotConnected) {
                            subCacheIds = UserDirectory.GetCacheIdsFor(filterCriterion.SubFilterCriteria);
                        }
                        if (null != subCacheIds && subCacheIds.Count > 0) {
                            cacheIds.AddRange(subCacheIds);
                        } else {
                            cacheIds.Clear();
                            break;
                        }
                    } else {
                        if (nameof(IUser.Id) == filterCriterion.FieldName && RelationalOperator.IsEqualTo == filterCriterion.RelationalOperator && isOrOrNotConnected) {
                            var cacheId = Guid.Parse(filterCriterion.ValueOrOtherFieldName.ToString());
                            if (!cacheIds.Contains(cacheId)) {
                                cacheIds.Add(cacheId);
                            }
                        } else {
                            cacheIds.Clear();
                            break;
                        }
                    }
                    filterCriterion = filterCriterion.Next;
                } while (null != filterCriterion);
            }
            return cacheIds;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// directory.
        /// </summary>
        /// <returns>enumerator that iterates through the directory</returns>
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
        /// Gets the user to be set as created by and/or modified by
        /// on create and update operations.
        /// </summary>
        /// <returns>user to be set as created by and/or modified by
        /// on create and update operations</returns>
        internal IUser GetUserForModifications() {
            IUser userForModifications;
            if (this.IsCurrentUserAnonymized) {
                userForModifications = UserDirectory.AnonymousUser;
            } else {
                userForModifications = this.CurrentUser;
            }
            return userForModifications;
        }

        /// <summary>
        /// Removes a specific user from the directory.
        /// </summary>
        /// <param name="user">specific user to remove from directory</param>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        public bool Remove(IUser user) {
            bool isRemoved = this.RemoveNoCache(user);
            if (isRemoved && this.userCache.Contains(user.Id)) {
                this.userCache.Remove(user.Id);
                this.userCache.RegisterAsNotExisting(user.Id);
            }
            return isRemoved;
        }

        /// <summary>
        /// Removes a specific user from the directory.
        /// </summary>
        /// <param name="user">specific user to remove from directory</param>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        protected abstract bool RemoveNoCache(IUser user);

        /// <summary>
        /// Verifies a clear text password against an encrypted
        /// password.
        /// </summary>
        /// <param name="password">clear text password to varify</param>
        /// <param name="encryptedPassword">encrypted password</param>
        /// <returns>true if clear text password matches encrypted
        /// one, false otherwise</returns>
        public static bool VerifyPassword(string password, string encryptedPassword) {
            bool isMatch = false;
            if (!string.IsNullOrEmpty(encryptedPassword) && encryptedPassword.Length > 19) {
                string salt = encryptedPassword.Substring(3, 16);
                isMatch = UserDirectory.EncryptPassword(password, salt) == encryptedPassword;
            }
            return isMatch;
        }

    }

}