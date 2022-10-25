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

namespace Framework.BusinessApplications.DataProviders {

    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Persistence.Filters;
    using System;
    using System.Transactions;

    /// <summary>
    /// Data provider for persistent users.
    /// </summary>
    public abstract class UserDataProvider : PersistentDataProvider<PersistentUser> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to save users in and load users from</param>
        /// <param name="baseSortCriteria">base sort criteria to
        /// apply</param>
        public UserDataProvider(PersistenceMechanism persistenceMechanism, SortCriterionCollection baseSortCriteria)
            : base(persistenceMechanism, FilterCriteria.Empty, baseSortCriteria) {
            // nothing to do
        }

        /// <summary>
        /// Adds a user to persistence mechanism.
        /// </summary>
        /// <param name="persistentUser">user to add</param>
        /// <param name="password">password to be set for new users</param>
        protected abstract void Add(PersistentUser persistentUser, string password);

        /// <summary>
        /// Adds or updates a user in persistence mechanism.
        /// </summary>
        /// <param name="persistentUser">user to add or update</param>
        public sealed override void AddOrUpdate(PersistentUser persistentUser) {
            if (persistentUser.IsNew) {
                this.Add(persistentUser, PasswordGenerator.GeneratePassword(15));
            } else {
                this.Update(persistentUser);
            }
            return;
        }

        /// <summary>
        /// Creates a new user of a specific type.
        /// </summary>
        /// <param name="type">type of new user</param>
        /// <returns>new user of specified type</returns>
        public override PersistentUser Create(Type type) {
            PersistentUser persistentUser;
            if (typeof(PersistentUser) == type) {
                persistentUser = new PersistentUser {
                    AllowedGroups = new AllowedGroups()
                };
            } else {
                persistentUser = null;
            }
            return persistentUser;
        }

        /// <summary>
        /// Deletes a user in persistence mechanism.
        /// </summary>
        /// <param name="persistentUser">user to delete</param>
        public override void Delete(PersistentUser persistentUser) {
            if (!persistentUser.IsWriteProtected) {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                    var elevatedPersistentUsers = this.PersistenceMechanism.CopyWithElevatedPrivileges().FindContainer<PersistentUser>();
                    var elevatedPersistentUser = elevatedPersistentUsers.FindOne(persistentUser.Id);
                    elevatedPersistentUser.AllowedGroups.RemoveCascadedly();
                    elevatedPersistentUser.RemoveCascadedly();
                    transactionScope.Complete();
                    if (elevatedPersistentUser.IsRemoved) {
                        persistentUser.IsRemoved = true;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Makes sure that administrator group contains at least an
        /// auto-generated default administrator.
        /// </summary>
        /// <param name="elevatedApplicationSettings">application
        /// settings with elevated privileges</param>
        /// <param name="adminEmailAddress">email address to send
        /// password of auto-generated default administrator to</param>
        public void EnsureAccessForAdministrator(ApplicationSettings elevatedApplicationSettings, string adminEmailAddress) {
            if (elevatedApplicationSettings.Administrators.Members.Count < 1 && !string.IsNullOrEmpty(adminEmailAddress)) {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                    var adminUser = this.PersistenceMechanism.UserDirectory.FindOne(new FilterCriteria(nameof(IUser.UserName), RelationalOperator.IsEqualTo, adminEmailAddress, FilterTarget.IsOtherTextValue), SortCriterionCollection.Empty) as PersistentUser;
                    if (null == adminUser) {
                        adminUser = this.Create(typeof(PersistentUser));
                        adminUser.FirstName = "Delete";
                        adminUser.LastName = "Me";
                        adminUser.UserName = adminEmailAddress;
                        adminUser.EmailAddress = adminEmailAddress;
                        var password = PasswordGenerator.GeneratePassword(64);
#if DEBUG
                        adminUser.Culture = System.Globalization.CultureInfo.GetCultureInfo("de-DE");
                        adminUser.CountryAlpha2Code = "DE";
                        adminUser.PasswordExpirationDate = Model.UtcDateTime.MaxValue;
                        password = "admin";
#endif
                        this.Add(adminUser, password);
                    }
                    if (!elevatedApplicationSettings.Administrators.Members.ContainsPermissionsFor(adminUser)) {
                        elevatedApplicationSettings.Administrators.Members.Add(adminUser);
                        elevatedApplicationSettings.UpdateCascadedly();
                    }
                    transactionScope.Complete();
                }
            }
            return;
        }

        /// <summary>
        /// Updates a user in persistence mechanism.
        /// </summary>
        /// <param name="persistentUser">user to update</param>
        protected virtual void Update(PersistentUser persistentUser) {
            persistentUser.UpdateCascadedly();
            return;
        }

    }

}