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
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Base class of user directory tokens.
    /// </summary>
    /// <typeparam name="T">type of user directoy token</typeparam>
    public abstract class UserDirectoryToken<T> : PersistentObject where T : UserDirectoryToken<T>, new() {

        /// <summary>
        /// Associated user.
        /// </summary>
        public IUser AssociatedUser {
            get {
                return this.associatedUser.Value;
            }
            protected set {
                this.associatedUser.Value = value;
                this.Reset();
            }
        }
        private readonly PersistentFieldForIUser associatedUser =
            new PersistentFieldForIUser(nameof(AssociatedUser));

        /// <summary>
        /// Encrypted token identifier.
        /// </summary>
        public string EncryptedIdentifier {
            get { return this.encryptedIdentifier.Value; }
            private set { this.encryptedIdentifier.Value = value; }
        }
        private readonly PersistentFieldForString encryptedIdentifier =
            new PersistentFieldForString(nameof(EncryptedIdentifier));

        /// <summary>
        /// Expiration date.
        /// </summary>
        public DateTime ExpirationDate {
            get { return this.expirationDate.Value; }
            private set { this.expirationDate.Value = value; }
        }
        private readonly PersistentFieldForDateTime expirationDate =
            new PersistentFieldForDateTime(nameof(ExpirationDate));

        /// <summary>
        /// Token identifier - this is not available for password
        /// reset tokens loaded from persistence mechanism.
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// Log-on infos.
        /// </summary>
        private static ConcurrentDictionary<string, LogOnInfo> LogOnInfos { get; set; } = new ConcurrentDictionary<string, LogOnInfo>();

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        protected UserDirectoryToken()
            : base() {
            this.RegisterPersistentField(this.associatedUser);
            this.RegisterPersistentField(this.encryptedIdentifier);
            this.RegisterPersistentField(this.expirationDate);
        }

        /// <summary>
        /// Cleans up all expired user directory tokens.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to clean up</param>
        /// <returns>matching user directory token or null</returns>
        private static void CleanUp(PersistenceMechanism persistenceMechanism) {
            foreach (var logOnInfoKey in UserDirectoryToken<T>.LogOnInfos.Keys) {
                var logOnInfo = UserDirectoryToken<T>.LogOnInfos[logOnInfoKey];
                lock (logOnInfo) {
                    if (logOnInfo.LockedUntil < UtcDateTime.Now) {
                        UserDirectoryToken<T>.LogOnInfos.TryRemove(logOnInfoKey, out _);
                    }
                }
            }
            var elevatedUserDirectoryTokens = persistenceMechanism.CopyWithElevatedPrivileges().FindContainer<T>();
            var filterCriteria = new FilterCriteria(nameof(UserDirectoryToken<T>.ExpirationDate), RelationalOperator.IsLessThan, UtcDateTime.Now);
            foreach (var expiredElevatedUserDirectoryToken in elevatedUserDirectoryTokens.Find(filterCriteria, SortCriterionCollection.Empty)) {
                expiredElevatedUserDirectoryToken.RemoveCascadedly();
            }
            return;
        }

        /// <summary>
        /// Creates a user directory token for a user with a specific
        /// user name or resets an existing one.
        /// </summary>
        /// <param name="userName">user name to create user directory
        /// token for</param>
        /// <param name="persistenceMechanism">persistence mechism to
        ///  query</param>
        /// <returns>matching user directory token or null</returns>
        public static T Create(string userName, PersistenceMechanism persistenceMechanism) {
            var userDirectoryToken = UserDirectoryToken<T>.FindOne(userName, persistenceMechanism);
            if (null == userDirectoryToken) {
                var userFilterCriteria = new FilterCriteria(nameof(IUser.UserName), RelationalOperator.IsEqualTo, userName, FilterTarget.IsOtherTextValue);
                var associatedUser = persistenceMechanism.UserDirectory.FindOne(userFilterCriteria, SortCriterionCollection.Empty);
                if (null != associatedUser) {
                    userDirectoryToken = new T {
                        AllowedGroups = new AllowedGroups()
                    };
                    userDirectoryToken.AllowedGroups.ForReading.Add(persistenceMechanism.AllUsers);
                    userDirectoryToken.AssociatedUser = associatedUser;
                    var userDirectoyTokens = persistenceMechanism.FindContainer<T>();
                    userDirectoyTokens.AddCascadedly(userDirectoryToken);
                }
            } else {
                var elevatedUserDirectoryTokens = persistenceMechanism.CopyWithElevatedPrivileges().FindContainer<T>();
                var elevatedUserDirectoryToken = elevatedUserDirectoryTokens.FindOne(userDirectoryToken.Id);
                if (null != elevatedUserDirectoryToken) {
                    elevatedUserDirectoryToken.Reset();
                    elevatedUserDirectoryToken.UpdateCascadedly();
                    userDirectoryToken.Retrieve();
                    userDirectoryToken.Identifier = elevatedUserDirectoryToken.Identifier;
                }
            }
            return userDirectoryToken;
        }

        /// <summary>
        /// Finds the most recent user directory token for a user
        /// name.
        /// </summary>
        /// <param name="userName">user name to find user directory
        /// token for</param>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to query</param>
        /// <returns>matching user directory token or null</returns>
        private static T FindOne(string userName, PersistenceMechanism persistenceMechanism) {
            UserDirectoryToken<T>.CleanUp(persistenceMechanism);
            T userDirectoryToken;
            var userFilterCriteria = new FilterCriteria(nameof(IUser.UserName), RelationalOperator.IsEqualTo, userName, FilterTarget.IsOtherTextValue);
            var associatedUser = persistenceMechanism.UserDirectory.FindOne(userFilterCriteria, SortCriterionCollection.Empty);
            if (null == associatedUser) {
                userDirectoryToken = null;
            } else {
                var userDirectoryTokens = persistenceMechanism.FindContainer<T>();
                var userDirectoryTokenFilterCriteria = new FilterCriteria(nameof(PasswordResetToken.AssociatedUser), RelationalOperator.IsEqualTo, associatedUser);
                var sortCriteria = new SortCriterionCollection {
                    new SortCriterion(nameof(UserDirectoryToken<T>.ExpirationDate), SortDirection.Descending)
                };
                userDirectoryToken = userDirectoryTokens.FindOne(userDirectoryTokenFilterCriteria, sortCriteria);
            }
            return userDirectoryToken;
        }

        /// <summary>
        /// Finds the user directory token for a pair of user name
        /// and token identifier.
        /// </summary>
        /// <param name="userName">user name to find user directory
        /// token for</param>
        /// <param name="identifier">user directory token identifier
        /// to find user directory token for</param>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to query</param>
        /// <returns>matching user directory token or null</returns>
        public static T FindOne(string userName, string identifier, PersistenceMechanism persistenceMechanism) {
            T userDirectoryToken = null;
            if (!string.IsNullOrEmpty(userName)) {
                var potentialToken = UserDirectoryToken<T>.FindOne(userName, persistenceMechanism);
                if (null != potentialToken) {
                    if (potentialToken.HasIdentifier(identifier)) { // user is logged in
                        userDirectoryToken = potentialToken;
                    } else {
                        var logOnInfo = UserDirectoryToken<T>.LogOnInfos.GetOrAdd(userName, delegate (string key) {
                            return new LogOnInfo();
                        });
                        lock (logOnInfo) {
                            if (!logOnInfo.IsLocked) {
                                logOnInfo.AddFailedAttempt(identifier);
                            }
                        }
                    }
                }
            }
            return userDirectoryToken;
        }

        /// <summary>
        /// Gets the time span until user directory tokens expire.
        /// </summary>
        /// <returns>time span until user directory tokens expire</returns>
        protected abstract TimeSpan GetExpirationTimeSpan();

        /// <summary>
        /// Gets the length of identifier to be generated.
        /// </summary>
        /// <returns>length of identifier to be generated</returns>
        protected abstract ushort GetIdentifierLength();

        /// <summary>
        /// Verifies a clear text identifier against the encrypted
        /// identifier of user directory token.
        /// </summary>
        /// <param name="identifier">clear text identifier to varify</param>
        /// <returns>true if clear text identifier matches for this
        /// user directory token, false otherwise</returns>
        public bool HasIdentifier(string identifier) {
            return UserDirectory.VerifyPassword(identifier, this.EncryptedIdentifier);
        }

        /// <summary>
        /// Resets the user directory token.
        /// </summary>
        private void Reset() {
            this.Identifier = PasswordGenerator.GeneratePassword(this.GetIdentifierLength(), true);
            this.EncryptedIdentifier = UserDirectory.EncryptPassword(this.Identifier);
            this.ExpirationDate = UtcDateTime.Now.Add(this.GetExpirationTimeSpan());
            return;
        }

    }

}