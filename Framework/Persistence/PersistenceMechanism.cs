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

    using Framework.Diagnostics;
    using Framework.Persistence.Directories;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Filters;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for all persistence machanisms like relaional
    /// databases, document based databases, flat files or whatever.
    /// </summary>
    public abstract partial class PersistenceMechanism {

        /// <summary>
        /// Finds the oldest group of all users with title "All
        /// users". If such a group does not exist, it will be
        /// created automatically.
        /// </summary>
        public Group AllUsers {
            get {
                if (null == this.groupOfAllUsers) {
                    var groups = this.FindContainer<Group>();
                    var filterCriteria = new FilterCriteria(nameof(Group.Title), RelationalOperator.IsEqualTo, "All users", FilterTarget.IsOtherTextValue)
                        .And(nameof(Group.Members), RelationalOperator.IsEqualTo, UserDirectory.AnonymousUser);
                    var sortCriteria = new SortCriterionCollection() {
                        new SortCriterion(nameof(Group.CreatedAt), SortDirection.Ascending)
                    };
                    this.groupOfAllUsers = groups.FindOne(filterCriteria, sortCriteria);
                    if (null == this.groupOfAllUsers) {
                        var elevatedPersistenceMechnaism = this.CopyWithElevatedPrivileges();
                        var elevatedGroups = elevatedPersistenceMechnaism.FindContainer<Group>();
                        var allUsers = new Group("All users");
                        allUsers.Members.Add(UserDirectory.AnonymousUser);
                        allUsers.AllowedGroups = new AllowedGroups();
                        allUsers.AllowedGroups.ForReading.Add(allUsers);
                        elevatedGroups.AddCascadedly(allUsers);
                        this.groupOfAllUsers = groups.FindOne(filterCriteria, sortCriteria);
                    }
                }
                return this.groupOfAllUsers;
            }
        }
        private Group groupOfAllUsers;

        /// <summary>
        /// Copy of this persistence mechanism with elevated
        /// privileges.
        /// </summary>
        private PersistenceMechanism elevatedCopy;

        /// <summary>
        /// Indicates whether persistence mechanism has full-text
        /// search enabled.
        /// </summary>
        public bool HasFullTextSearchEnabled { get; set; }

        /// <summary>
        /// True if this persistentence mechanism has native support
        /// for byte values, false otherwise.
        /// </summary>
        protected internal virtual bool HasNativeSupportForByte {
            get { return true; }
        }

        /// <summary>
        /// True if this persistentence mechanism has native support
        /// for decimal values, false otherwise.
        /// </summary>
        protected internal virtual bool HasNativeSupportForDecimal {
            get { return true; }
        }

        /// <summary>
        /// Allowed groups always must have themselves as allowed
        /// groups. Usually the persistence mechnism would throw an
        /// exception if this is not the case. However, when setting
        /// this property to TRUE invalid allowed groups of allowed
        /// groups are allowed anyway.
        /// </summary>
        public bool HasSupportForIndividualAllowedGroupsOfAllowedGroups { get; set; } = false;

        /// <summary>
        /// Indicates whether persistence mechanism has versioning
        /// enabled.
        /// </summary>
        public bool HasVersioningEnabled {
            get { return null != this.VersioningRepository; }
        }

        /// <summary>
        /// Log to use for logging.
        /// </summary>
        protected ILog Log { get; set; }

        /// <summary>
        /// Copy of this persistence mechanism with privileges of
        /// current user.
        /// </summary>
        private PersistenceMechanism restrictedCopy;

        /// <summary>
        /// Security model to use for accessing this persistence
        /// mechanism.
        /// </summary>
        public SecurityModel SecurityModel { get; private set; }

        /// <summary>
        /// Directory to use for user resolval.
        /// </summary>
        public UserDirectory UserDirectory { get; private set; }

        /// <summary>
        /// Repository for versioning.
        /// </summary>
        protected internal PersistenceMechanism VersioningRepository {
            get {
                return this.versioningRepository;
            }
            protected set {
                if (null != value && SecurityModel.IgnorePermissions != value.SecurityModel) {
                    throw new PersistenceException("Versioning repositories may not apply permissions.");
                }
                this.versioningRepository = value;
            }
        }
        private PersistenceMechanism versioningRepository;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="userDirectory">directory to use for user
        /// resolval</param>
        /// <param name="securityModel">security model to use for
        /// accessing this persistence mechanism</param>
        protected PersistenceMechanism(UserDirectory userDirectory, SecurityModel securityModel) {
            this.SecurityModel = securityModel;
            this.UserDirectory = userDirectory;
        }

        /// <summary>
        /// Cleans up this persistence mechanism.
        /// </summary>
        public virtual void CleanUp() {
            this.RemoveExpiredTemporaryFiles();
            return;
        }

        /// <summary>
        /// Copys the permission independent state of a source
        /// instance into this one.
        /// </summary>
        /// <param name="source">source instance to copy permission
        /// independent state from</param>
        protected void CopyPartiallyFrom(PersistenceMechanism source) {
            this.HasFullTextSearchEnabled = source.HasFullTextSearchEnabled;
            this.HasSupportForIndividualAllowedGroupsOfAllowedGroups = source.HasSupportForIndividualAllowedGroupsOfAllowedGroups;
            this.Log = source.Log;
            this.PersistentObjectInitializer = source.PersistentObjectInitializer;
            return;
        }

        /// <summary>
        /// Copys this persistence mechanism and sets the security
        /// model of the copied instance to apply permissions.
        /// </summary>
        /// <returns>copy of this persistence mechanism that applies
        /// all permissions or current instance if it applies
        /// permissions already</returns>
        public PersistenceMechanism CopyWithCurrentUserPrivileges() {
            if (null == this.restrictedCopy) {
                if (SecurityModel.ApplyPermissions == this.SecurityModel) {
                    this.restrictedCopy = this;
                } else {
                    this.restrictedCopy = this.CopyWithCurrentUserPrivilegesNoCache();
                }
            }
            return this.restrictedCopy;
        }

        /// <summary>
        /// Copys this persistence mechanism and sets the security
        /// model of the copied instance to apply permissions. This
        /// method does not cache the copied persistence mechanism.
        /// </summary>
        /// <returns>copy of this persistence mechanism that applies
        /// all permissions</returns>
        protected abstract PersistenceMechanism CopyWithCurrentUserPrivilegesNoCache();

        /// <summary>
        /// Copys this persistence mechanism and sets the security
        /// model of the copied instance to ignore permissions.
        /// </summary>
        /// <returns>copy of this persistence mechanism that ignores
        /// all permissions or current instance if it ignores
        /// permissions already</returns>
        public PersistenceMechanism CopyWithElevatedPrivileges() {
            if (null == this.elevatedCopy) {
                if (SecurityModel.IgnorePermissions == this.SecurityModel) {
                    this.elevatedCopy = this;
                } else {
                    this.elevatedCopy = this.CopyWithElevatedPrivilegesNoCache();
                }
            }
            return this.elevatedCopy;
        }

        /// <summary>
        /// Copys this persistence mechanism and sets the security
        /// model of the copied instance to ignore permissions. This
        /// method does not cache the copied elevated persistence
        /// mechanism.
        /// </summary>
        /// <returns>copy of this persistence mechanism that ignores
        /// all permissions</returns>
        protected internal abstract PersistenceMechanism CopyWithElevatedPrivilegesNoCache();

        /// <summary>
        /// Copys this persistence mechanism but replaces the user
        /// directory. This method does not cache the copied
        /// persistence mechanism.
        /// </summary>
        /// <param name="userDirectory">user directory to associate
        /// to copied persistence mechanism</param>
        /// <returns>copy of this persistence mechanism with replaced
        /// user directory</returns>
        public abstract PersistenceMechanism CopyWithReplacedUserDirectoryNoCache(UserDirectory userDirectory);

        /// <summary>
        /// Gets the unique ID of this persistence mechanism.
        /// </summary>
        /// <returns>unique ID of this persistence mechanism</returns>
        protected abstract string GetId();

        /// <summary>
        /// Gets the inventory of persistence mechanism as dictionary
        /// of assembly qualified type name and number of elements of
        /// type in persistence mechanism.
        /// </summary>
        /// <returns>inventory of persistence mechanism as dictionary
        /// of assembly qualified type name and number of elements of
        /// type in persistence mechanism</returns>
        protected IDictionary<string, int> GetInventory() {
            var containerInfos = this.GetContainerInfos();
            var inventory = new Dictionary<string, int>(containerInfos.Count);
            foreach (var containerInfo in containerInfos) {
                var persistentContainer = this.FindContainer(containerInfo.AssemblyQualifiedTypeName);
                inventory.Add(containerInfo.AssemblyQualifiedTypeName, persistentContainer.Count);
            }
            return inventory;
        }

    }

}