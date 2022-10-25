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

namespace Framework.BusinessApplications.Interchange.JsonRpc {

    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  Base class of data providers for persistent users.
    /// </summary>
    public abstract class UserDataProvider : DataProvider<PersistentUser> {

        /// <summary>
        /// Persistence mechanism client to be used.
        /// </summary>
        protected PersistenceMechanismClient PersistenceMechansimClient { get; private set; }

        /// <summary>
        /// Persistent container for persistent users.
        /// </summary>
        protected PersistentContainer<PersistentUser> PersistentUsers {
            get {
                if (null == this.persistentUsers) {
                    this.persistentUsers = this.PersistenceMechansimClient.FindContainer<PersistentUser>();
                }
                return this.persistentUsers;
            }
        }
        private PersistentContainer<PersistentUser> persistentUsers;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanismClient">persistence
        /// mechanism client to be used</param>
        public UserDataProvider(PersistenceMechanismClient persistenceMechanismClient)
            : base() {
            this.PersistenceMechansimClient = persistenceMechanismClient;
        }

        /// <summary>
        /// Adds or updates an object in persistence mechanism.
        /// </summary>
        /// <param name="persistentUser">persistent user to add or
        /// update</param>
        public override void AddOrUpdate(PersistentUser persistentUser) {
            var filterCriteriaForUniqueMailAddress = new FilterCriteria(nameof(IUser.Id), RelationalOperator.IsNotEqualTo, persistentUser.Id)
                .And(nameof(PersistentUser.EmailAddress), RelationalOperator.IsEqualTo, persistentUser.EmailAddress, FilterTarget.IsOtherTextValue);
            if (this.PersistentUsers.Contains(filterCriteriaForUniqueMailAddress)) {
                throw new ObjectNotUniqueException("Persistent user cannot be saved because the e-mail address is not unique.");
            } else {
                var persistentUserIsNew = persistentUser.IsNew;
                this.PersistentUsers.AddOrUpdateCascadedly(persistentUser);
                if (persistentUserIsNew) {
                    var allowedGroup = new Group(persistentUser.DisplayName);
                    allowedGroup.Members.Add(persistentUser);
                    persistentUser.AllowedGroups.ForReading.Add(allowedGroup);
                    persistentUser.AllowedGroups.ForWriting.Add(allowedGroup);
                    var password = PasswordGenerator.GeneratePassword(15);
                    persistentUser.SetPassword(password);
                    persistentUser.UpdateCascadedly();
                    this.SendWelcomeEmail(persistentUser, password);
                }
                this.PersistenceMechansimClient.RemoveExpiredTemporaryFiles();
            }
            return;
        }

        /// <summary>
        /// Creates a new persistent user.
        /// </summary>
        /// <param name="type">type of persistent user</param>
        /// <returns>new persistent user or null if it is not
        /// supposed to be created</returns>
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
        /// Deletes a persistent user in persistence mechanism.
        /// </summary>
        /// <param name="persistentUser">persistent user to delete</param>
        public override void Delete(PersistentUser persistentUser) {
            persistentUser.AllowedGroups.RemoveCascadedly();
            persistentUser.RemoveCascadedly();
            return;
        }

        /// <summary>
        /// Finds all matching results of this data provider.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>all matching objects from this container</returns>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <returns>matching objects from this data provider</returns>
        public override ICollection<PersistentUser> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            var byCompanySortCriteria = new SortCriterionCollection {
                    { nameof(PersistentUser.Company), SortDirection.Ascending }
                };
            return this.PersistentUsers.Find(filterCriteria, SortCriterionCollection.Concat(sortCriteria, byCompanySortCriteria), startPosition, maxResults);
        }

        /// <summary>
        /// Finds the file with a specific ID and file name.
        /// </summary>
        /// <param name="fileId">ID to get file for</param>
        /// <param name="fileName">name of requested file</param>
        /// <returns>file with specific ID and file name</returns>
        public override File FindFile(System.Guid fileId, string fileName) {
            // nothing to do
            return null;
        }

        /// <summary>
        /// Finds the persistent user with a specific ID.
        /// </summary>
        /// <param name="id">ID to get persistent user for</param>
        /// <returns>persistent user with specific ID or null if no
        /// match was found</returns>
        public override PersistentUser FindOne(string id) {
            PersistentUser persistentUser;
            if (Guid.TryParse(id, out Guid guid)) {
                var filterCriteria = new FilterCriteria(new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, guid));
                persistentUser = this.PersistentUsers.FindOne(filterCriteria, SortCriterionCollection.Empty);
            } else {
                persistentUser = null;
            }
            return persistentUser;
        }

        /// <summary>
        /// Preloads the state of multiple persistent users.
        /// </summary>
        /// <param name="persistentUsers">persistent users to preload</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        public override void Preload(IEnumerable<PersistentUser> persistentUsers, IEnumerable<string[]> keyChains) {
            // not supported
            return;
        }

        /// <summary>
        /// Sends a welcome email to a new user.
        /// </summary>
        /// <param name="persistentUser">persistent user to add or
        /// update</param>
        /// <param name="password">password of new user</param>
        public abstract void SendWelcomeEmail(PersistentUser persistentUser, string password);

    }

}