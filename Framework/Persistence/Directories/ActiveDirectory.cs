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
    using Framework.Presentation;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.DirectoryServices;
    using System.Globalization;
    using System.Security.Principal;
    using System.Web;

    /// <summary>
    /// Represents an Active Directory. Instances of this class have
    /// to be disposed after usage. This class is not thread safe.
    /// </summary>
    public sealed class ActiveDirectory : UserDirectory, IDisposable {

        /// <summary>
        /// Number of users contained in directory.
        /// </summary>
        public override int Count {
            get {
                return this.GetCount(FilterCriteria.Empty);
            }
        }

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
        /// Source to use for resolving the current user.
        /// </summary>
        public CurrentUserSource CurrentUserSource {
            get {
                return this.currentUserSource;
            }
            set {
                if (CurrentUserSource.HttpContext == value && null == this.HttpContext) {
                    throw new ArgumentException("Current user source cannot be set to HttpContext because no HTTP context was provided in constructor.");
                }
                this.currentUserSource = value;
            }
        }
        private CurrentUserSource currentUserSource;

        /// <summary>
        /// Directory searcher to use for search operations.
        /// </summary>
        private DirectorySearcher directorySearcher;

        /// <summary>
        /// Distinguished name of OU for adding new users to Active
        /// Directory, e.g. &quot;CN=Users,DC=domain,DC=lan&quot;.
        /// </summary>
        public string DistinguishedNameOfOUForAddingUsers { get; set; }

        /// <summary>
        /// Dictionary of framework field names and Active Directory
        /// field names.
        /// </summary>
        private static readonly ReadOnlyDictionary<string, string> fieldNameDictionary; // concurrent dictionary is not necessary because contents won't change after initialization in static constructor

        /// <summary>
        /// Indicates whether user directory supports sorting of
        /// values within directory requests.
        /// </summary>
        protected override bool IsCapableToSortWithinRequests {
            get { return false; }
        }

        /// <summary>
        /// Dictionary of Active Directory field names and framework
        /// field names.
        /// </summary>
        private static readonly ReadOnlyDictionary<string, string> reversedFieldNameDictionary; // concurrent dictionary is not necessary because contents won't change after initialization in static constructor

        /// <summary>
        /// User name for accessing directory.
        /// </summary>
        private readonly string userName;

        /// <summary>
        /// Password for accessing directory.
        /// </summary>
        private readonly string password;

        /// <summary>
        /// Root path of directory to use, e.g.
        /// LDAP://123.45.67.89/
        /// </summary>
        private readonly string rootPath;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ActiveDirectory() {
            ActiveDirectory.fieldNameDictionary = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
                { nameof(ActiveDirectoryContact.City), "l" },
                { nameof(ActiveDirectoryContact.Company), "company" },
                { nameof(ActiveDirectoryContact.CountryAlpha2Code), "c" },
                { nameof(ActiveDirectoryContact.CountryName), "co" },
                { nameof(ActiveDirectoryContact.CountryNumericCode), "countryCode" },
                { nameof(ActiveDirectoryContact.CreatedAt), "whenCreated" },
                { nameof(ActiveDirectoryContact.Department), "department" },
                { nameof(ActiveDirectoryContact.Description), "description" },
                { nameof(ActiveDirectoryContact.DisplayName), "displayName" },
                { nameof(ActiveDirectoryContact.DistinguishedName), "distinguishedName" },
                { nameof(ActiveDirectoryContact.EmailAddress), "mail" },
                { nameof(ActiveDirectoryContact.FaxNumber), "facsimileTelephoneNumber" },
                { nameof(ActiveDirectoryContact.FirstName), "givenName" },
                { nameof(ActiveDirectoryContact.HomePhoneNumber), "homePhone" },
                { nameof(ActiveDirectoryContact.Id), "objectGUID" },
                { nameof(ActiveDirectoryContact.Initials), "initials" },
                { nameof(ActiveDirectoryContact.JobTitle), "title" },
                { nameof(ActiveDirectoryContact.LastName), "sn" },
                { nameof(ActiveDirectoryContact.MobilePhoneNumber), "mobile" },
                { nameof(ActiveDirectoryContact.ModifiedAt), "whenChanged" },
                { nameof(ActiveDirectoryContact.Notes), "info" },
                { nameof(ActiveDirectoryContact.Office), "physicalDeliveryOfficeName" },
                { nameof(ActiveDirectoryContact.OfficePhoneNumber), "telephoneNumber" },
                { nameof(ActiveDirectoryContact.PersonalTitle), "personalTitle" },
                { nameof(ActiveDirectoryContact.Photo), "thumbnailPhoto" },
                { nameof(ActiveDirectoryContact.PostOfficeBox), "postOfficeBox" },
                { nameof(ActiveDirectoryContact.State), "st" },
                { nameof(ActiveDirectoryContact.Street), "streetAddress" },
                { nameof(ActiveDirectoryContact.WebSite), "wWWHomePage" },
                { nameof(ActiveDirectoryContact.ZipCode), "postalCode" },
                { nameof(ActiveDirectoryUser.ManagerDistinguishedName), "manager" },
                { nameof(ActiveDirectoryUser.PreferredLanguage), "preferredLanguage" },
                { nameof(ActiveDirectoryUser.RoomNumber), "roomNumber" },
                { nameof(ActiveDirectoryUser.UserName), "sAMAccountName" }
            });
            var reversedFieldNameDictionary = new Dictionary<string, string>(ActiveDirectory.fieldNameDictionary.Count);
            foreach (var fieldName in ActiveDirectory.fieldNameDictionary) {
                reversedFieldNameDictionary.Add(fieldName.Value, fieldName.Key);
            }
            ActiveDirectory.reversedFieldNameDictionary = new ReadOnlyDictionary<string, string>(reversedFieldNameDictionary);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="userName">user name for accessing directory</param>
        /// <param name="password">password for accessing directory</param>
        /// <param name="rootPath">root path of directory to use, e.g.
        /// LDAP://123.45.67.89/</param>
        private ActiveDirectory(string userName, string password, string rootPath)
            : base() {
            this.currentUser = null;
            this.directorySearcher = new DirectorySearcher();
            this.userName = userName;
            this.password = password;
            this.rootPath = rootPath;
            this.InitializeDirectorySearcher();
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="currentUserSource">source to use for
        /// resolving the current user</param>
        public ActiveDirectory(CurrentUserSource currentUserSource)
            : this(currentUserSource, string.Empty, string.Empty) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpContext">HTTP context to use as current
        /// user source</param>
        public ActiveDirectory(HttpContext httpContext)
            : this(httpContext, string.Empty, string.Empty) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="currentUserSource">source to use for
        /// resolving the current user</param>
        /// <param name="userName">user name for accessing directory</param>
        /// <param name="password">password for accessing directory</param>
        public ActiveDirectory(CurrentUserSource currentUserSource, string userName, string password)
            : this(currentUserSource, userName, password, string.Empty) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpContext">HTTP context to use as current
        /// user source</param>
        /// <param name="userName">user name for accessing directory</param>
        /// <param name="password">password for accessing directory</param>
        public ActiveDirectory(HttpContext httpContext, string userName, string password)
            : this(httpContext, userName, password, string.Empty) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="currentUserSource">source to use for
        /// resolving the current user</param>
        /// <param name="userName">user name for accessing directory</param>
        /// <param name="password">password for accessing directory</param>
        /// <param name="rootPath">root path of directory to use, e.g.
        /// LDAP://123.45.67.89/</param>
        public ActiveDirectory(CurrentUserSource currentUserSource, string userName, string password, string rootPath)
            : this(userName, password, rootPath) {
            this.CurrentUserSource = currentUserSource;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpContext">HTTP context to use as current
        /// user source</param>
        /// <param name="userName">user name for accessing directory</param>
        /// <param name="password">password for accessing directory</param>
        /// <param name="rootPath">root path of directory to use, e.g.
        /// LDAP://123.45.67.89/</param>
        public ActiveDirectory(HttpContext httpContext, string userName, string password, string rootPath)
            : this(userName, password, rootPath) {
            this.HttpContext = httpContext;
            this.CurrentUserSource = CurrentUserSource.HttpContext;
        }

        /// <summary>
        /// Adds a user to the directory. The diplay name is used as
        /// common name.
        /// </summary>
        /// <param name="user">user to add to the directory</param>
        /// <param name="password">password to set for new user</param>
        protected override void AddNoCache(IUser user, string password) {
            var activeDirectoryUser = user as ActiveDirectoryUser;
            if (null == activeDirectoryUser) {
                throw new ArgumentException("User is either null or not of type \"" + typeof(ActiveDirectoryUser).FullName + "\".", nameof(user));
            }
            if (string.IsNullOrEmpty(this.DistinguishedNameOfOUForAddingUsers)) {
                throw new DirectoryException("Property ActiveDirectory.DistinguishedNameOfOUForAddingUsers may not be null or empty.");
            }
            using (DirectoryEntry container = new DirectoryEntry("LDAP://" + this.DistinguishedNameOfOUForAddingUsers)) {
                using (DirectoryEntry entry = container.Children.Add("CN=" + user.DisplayName.Replace(",", "\\,"), "user")) {
                    entry.Properties["sAMAccountName"].Value = user.UserName;
                    entry.CommitChanges();
                    entry.Invoke("SetPassword", new object[] { password });
                    entry.Properties["pwdLastSet"].Value = 0;
                    entry.Properties["userAccountControl"].Value = 512;
                    entry.CommitChanges();
                    activeDirectoryUser.Id = entry.Guid;
                }
            }
            activeDirectoryUser.ParentUserDirectory = this;
            this.Update(activeDirectoryUser, ActiveDirectoryUser.FieldNames);
            return;
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="user">user to change password for</param>
        /// <param name="oldPassword">old password</param>
        /// <param name="newPassword">new password</param>
        /// <returns>true if the password was updated successfully;
        /// otherwise, false</returns>
        internal bool ChangePassword(ActiveDirectoryUser user, string oldPassword, string newPassword) {
            bool result;
            if (null == this.directorySearcher) { // object is disposed
                using (this.directorySearcher = new DirectorySearcher()) {
                    this.InitializeDirectorySearcher();
                    result = this.ChangePasswordUnsafe(user, oldPassword, newPassword);
                }
                this.directorySearcher = null;
            } else {
                result = this.ChangePasswordUnsafe(user, oldPassword, newPassword);
            }
            return result;
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="user">user to change password for</param>
        /// <param name="oldPassword">old password</param>
        /// <param name="newPassword">new password</param>
        /// <returns>true if the password was updated successfully;
        /// otherwise, false</returns>
        private bool ChangePasswordUnsafe(ActiveDirectoryUser user, string oldPassword, string newPassword) {
            bool isChanged = false;
            var filterCriteria = new FilterCriteria(nameof(ActiveDirectoryUser.Id), RelationalOperator.IsEqualTo, user.Id);
            this.SetupDirectorySearcher(new string[] { "user", "person" }, filterCriteria, SortCriterionCollection.Empty, 1, new string[0]);
            SearchResult searchResult = this.directorySearcher.FindOne();
            if (null != searchResult) {
                using (var directoryEntry = searchResult.GetDirectoryEntry()) {
                    try {
                        directoryEntry.Invoke("ChangePassword", new object[] { oldPassword, newPassword });
                        isChanged = true;
                    } catch (Exception) {
                        // ignore exceptions
                    }
                }
            }
            return isChanged;
        }

        /// <summary>
        /// Disposes the connection to the Active Directory.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
            return;
        }

        /// <summary>
        /// Frees all unmanaged resources and also managed resources
        /// if desired.
        /// </summary>
        /// <param name="freeManagedResources">true to free unmanaged
        /// resources only, false to free managed resources as well</param>
        private void Dispose(bool freeManagedResources) {
            if (freeManagedResources) {
                if (null != this.directorySearcher) {
                    this.directorySearcher.Dispose();
                    this.directorySearcher = null;
                }
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
            return this.FindListOfUsers(filterCriteria, sortCriteria, startPosition, maxResults, propertiesToPreLoad);
        }

        /// <summary>
        /// Finds all matching contacts from this directory.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// contacts for</param>
        /// <param name="sortCriteria">criteria to sort contacts by</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of contacts - this can also
        /// be used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>all matching contacts from this directory</returns>
        public IEnumerable<ActiveDirectoryContact> FindContacts(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, params string[] propertiesToPreLoad) {
            return this.FindContacts(filterCriteria, sortCriteria, 0, ulong.MaxValue, propertiesToPreLoad);
        }

        /// <summary>
        /// Finds all matching contacts from this directory.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// contacts for</param>
        /// <param name="sortCriteria">criteria to sort contacts by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to return</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// be prior to the actual retrieval of contacts - this can also
        /// used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>all matching contacts from this directory</returns>
        public IEnumerable<ActiveDirectoryContact> FindContacts(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, params string[] propertiesToPreLoad) {
            return this.FindListOfContacts(filterCriteria, sortCriteria, startPosition, maxResults, propertiesToPreLoad);
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
            IEnumerable<string> results;
            if (null == this.directorySearcher) { // object is disposed
                using (this.directorySearcher = new DirectorySearcher()) {
                    this.InitializeDirectorySearcher();
                    results = this.FindDistinctValuesUnsafe(filterCriteria, sortCriteria, fieldName);
                }
                this.directorySearcher = null;
            } else {
                results = this.FindDistinctValuesUnsafe(filterCriteria, sortCriteria, fieldName);
            }
            return results;
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
        private IEnumerable<string> FindDistinctValuesUnsafe(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string fieldName) {
            if (sortCriteria.Count > 1) {
                throw new ArgumentException("More than one sort criterion is not supported for Active Directory queries of distinct values.");
            }
            var distinctValues = new List<string>();
            this.SetupDirectorySearcher(new string[] { "user", "person" }, filterCriteria, sortCriteria, int.MaxValue, new string[] { fieldName });
            using (SearchResultCollection searchResults = this.directorySearcher.FindAll()) {
                foreach (SearchResult searchResult in searchResults) {
                    var property = searchResult.Properties[fieldName];
                    if (property.Count > 0) {
                        string value = property[0].ToString();
                        if (!distinctValues.Contains(value)) {
                            distinctValues.Add(value);
                        }
                    }
                }
            }
            return distinctValues.AsReadOnly();
        }

        /// <summary>
        /// Finds a specific Active Directory group from this
        /// directory.
        /// </summary>
        /// <param name="distinguishedName">distinguished name of
        /// group to find</param>
        /// <returns>specific Active Directory group or null</returns>
        public ActiveDirectoryGroup FindGroupByDistinguishedName(string distinguishedName) {
            ActiveDirectoryGroup group;
            if (string.IsNullOrEmpty(distinguishedName)) {
                group = null;
            } else {
                group = new ActiveDirectoryGroup(distinguishedName, this);
            }
            return group;
        }

        /// <summary>
        /// Finds a specific Active Directory group from this
        /// directory.
        /// </summary>
        /// <param name="groupName">name of group to find</param>
        /// <returns>specific Active Directory group or null</returns>
        public ActiveDirectoryGroup FindGroupByGroupName(string groupName) {
            ActiveDirectoryGroup result;
            if (null == this.directorySearcher) { // object is disposed
                using (this.directorySearcher = new DirectorySearcher()) {
                    this.InitializeDirectorySearcher();
                    result = this.FindGroupByGroupNameUnsafe(groupName);
                }
                this.directorySearcher = null;
            } else {
                result = this.FindGroupByGroupNameUnsafe(groupName);
            }
            return result;
        }

        /// <summary>
        /// Finds a specific Active Directory group from this
        /// directory.
        /// </summary>
        /// <param name="groupName">name of group to find</param>
        /// <returns>specific Active Directory group or null</returns>
        private ActiveDirectoryGroup FindGroupByGroupNameUnsafe(string groupName) {
            string distinguishedName = null;
            var filterCriteria = new FilterCriteria("sAMAccountName", RelationalOperator.IsEqualTo, groupName, FilterTarget.IsOtherTextValue);
            this.SetupDirectorySearcher(new string[] { "group" }, filterCriteria, SortCriterionCollection.Empty, 1, new string[] { groupName });
            using (SearchResultCollection searchResults = this.directorySearcher.FindAll()) {
                foreach (SearchResult searchResult in searchResults) {
                    if (searchResult.Path.StartsWith("ldap://", StringComparison.OrdinalIgnoreCase)) {
                        distinguishedName = searchResult.Path.Substring(7);
                    } else {
                        distinguishedName = searchResult.Path;
                    }
                    break;
                }
            }
            return this.FindGroupByDistinguishedName(distinguishedName);
        }

        /// <summary>
        /// Finds all matching contacts from this directory.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// contacts for</param>
        /// <param name="sortCriteria">criteria to sort contacts by</param>
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
        private IList<ActiveDirectoryContact> FindListOfContacts(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, params string[] propertiesToPreLoad) {
            IList<ActiveDirectoryContact> results;
            if (null == this.directorySearcher) { // object is disposed
                using (this.directorySearcher = new DirectorySearcher()) {
                    this.InitializeDirectorySearcher();
                    results = this.FindListOfContactsUnsafe(filterCriteria, sortCriteria, startPosition, maxResults, propertiesToPreLoad);
                }
                this.directorySearcher = null;
            } else {
                results = this.FindListOfContactsUnsafe(filterCriteria, sortCriteria, startPosition, maxResults, propertiesToPreLoad);
            }
            return results;
        }

        /// <summary>
        /// Finds all matching contacts from this directory.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// contacts for</param>
        /// <param name="sortCriteria">criteria to sort contacts by</param>
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
        private IList<ActiveDirectoryContact> FindListOfContactsUnsafe(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, string[] propertiesToPreLoad) {
            if (startPosition > int.MaxValue) {
                throw new ArgumentException("Start position " + startPosition.ToString(CultureInfo.InvariantCulture) + " is too big - it may not be > " + int.MaxValue + " for Active Directory queries.", nameof(startPosition));
            }
            IList<ActiveDirectoryContact> matchingContacts;
            if (filterCriteria.TrySplitIntoChainsOf(46, out IList<FilterCriteria> splitFilterCriteriaList) || (ulong.MaxValue != maxResults && sortCriteria.Count > 1)) { // Active Directory cannot handle filters with > 46 chains or combinations of more than one sort criterion and a limited number of maximum results.
                matchingContacts = this.FindListOfContactsUnsafe(splitFilterCriteriaList, sortCriteria, startPosition, maxResults, propertiesToPreLoad);
            } else {
                int startPositionAsInt = Convert.ToInt32(startPosition, CultureInfo.InvariantCulture.NumberFormat);
                ulong sizeLimit = startPosition + maxResults;
                int sizeLimitAsInt;
                if (sizeLimit > int.MaxValue) {
                    sizeLimitAsInt = int.MaxValue;
                } else {
                    sizeLimitAsInt = Convert.ToInt32(sizeLimit, CultureInfo.InvariantCulture.NumberFormat);
                }
                this.SetupDirectorySearcher(new string[] { "contact", "person" }, filterCriteria, sortCriteria, sizeLimitAsInt, propertiesToPreLoad);
                try {
                    using (var searchResults = this.directorySearcher.FindAll()) {
                        matchingContacts = new List<ActiveDirectoryContact>(searchResults.Count);
                        for (int i = startPositionAsInt; i < searchResults.Count; i++) {
                            var matchingContact = new ActiveDirectoryContact {
                                ParentUserDirectory = this
                            };
                            matchingContact.SetValues(searchResults[i], this.directorySearcher.PropertiesToLoad, ActiveDirectory.reversedFieldNameDictionary);
                            matchingContacts.Add(matchingContact);
                        }
                    }
                } catch (DirectoryServicesCOMException exception) {
                    throw new DirectoryException(exception.Message, exception);
                } catch (System.Runtime.InteropServices.COMException exception) {
                    throw new DirectoryException("Active Directory request could not be processed. Either there is an infrastructural problem or the filter criteria are too long: " + filterCriteria.ToString(), exception);
                }
            }
            return matchingContacts;
        }

        /// <summary>
        /// Finds all matching contacts from this directory.
        /// </summary>
        /// <param name="filterCriteria">enumerable of filter
        /// criteria to select contacts for</param>
        /// <param name="sortCriteria">criteria to sort contacts by</param>
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
        private IList<ActiveDirectoryContact> FindListOfContactsUnsafe(IEnumerable<FilterCriteria> filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, string[] propertiesToPreLoad) {
            var matchingContacts = new List<ActiveDirectoryContact>();
            foreach (var currentFilterCriteria in filterCriteria) {
                matchingContacts.AddRange(this.FindListOfContactsUnsafe(currentFilterCriteria, SortCriterionCollection.Empty, 0, ulong.MaxValue, propertiesToPreLoad));
            }
            matchingContacts.Sort(new SortCriteriaComparer(sortCriteria));
            var selectedContacts = new List<ActiveDirectoryContact>(matchingContacts.Count);
            ulong maxPosition = startPosition + maxResults;
            ulong position = 0;
            foreach (var matchingContact in matchingContacts) {
                if (position >= startPosition && position < maxPosition) {
                    selectedContacts.Add(matchingContact);
                }
                position++;
            }
            return selectedContacts;
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
        private IList<IUser> FindListOfUsers(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, params string[] propertiesToPreLoad) {
            IList<IUser> results;
            if (null == this.directorySearcher) { // object is disposed
                using (this.directorySearcher = new DirectorySearcher()) {
                    this.InitializeDirectorySearcher();
                    results = this.FindListOfUsersUnsafe(filterCriteria, sortCriteria, startPosition, maxResults, propertiesToPreLoad);
                }
                this.directorySearcher = null;
            } else {
                results = this.FindListOfUsersUnsafe(filterCriteria, sortCriteria, startPosition, maxResults, propertiesToPreLoad);
            }
            return results;
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
        private IList<IUser> FindListOfUsersUnsafe(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, string[] propertiesToPreLoad) {
            if (startPosition > int.MaxValue) {
                throw new ArgumentException("Start position " + startPosition.ToString(CultureInfo.InvariantCulture) + " is too big - it may not be > " + int.MaxValue + " for Active Directory queries.", nameof(startPosition));
            }
            IList<IUser> matchingUsers;
            if (filterCriteria.TrySplitIntoChainsOf(46, out IList<FilterCriteria> splitFilterCriteriaList) || (ulong.MaxValue != maxResults && sortCriteria.Count > 1)) { // Active Directory cannot handle filters with > 46 chains or combinations of more than one sort criterion and a limited number of maximum results.
                matchingUsers = this.FindListOfUsersUnsafe(splitFilterCriteriaList, sortCriteria, startPosition, maxResults, propertiesToPreLoad);
            } else {
                int startPositionAsInt = Convert.ToInt32(startPosition, CultureInfo.InvariantCulture.NumberFormat);
                ulong sizeLimit = startPosition + maxResults;
                int sizeLimitAsInt;
                if (sizeLimit > int.MaxValue) {
                    sizeLimitAsInt = int.MaxValue;
                } else {
                    sizeLimitAsInt = Convert.ToInt32(sizeLimit, CultureInfo.InvariantCulture.NumberFormat);
                }
                this.SetupDirectorySearcher(new string[] { "user", "person" }, filterCriteria, sortCriteria, sizeLimitAsInt, propertiesToPreLoad);
                try {
                    using (var searchResults = this.directorySearcher.FindAll()) {
                        matchingUsers = new List<IUser>(searchResults.Count);
                        for (int i = startPositionAsInt; i < searchResults.Count; i++) {
                            var matchingUser = new ActiveDirectoryUser {
                                ParentUserDirectory = this
                            };
                            matchingUser.SetValues(searchResults[i], this.directorySearcher.PropertiesToLoad, ActiveDirectory.reversedFieldNameDictionary);
                            matchingUsers.Add(matchingUser);
                        }
                    }
                } catch (DirectoryServicesCOMException exception) {
                    throw new DirectoryException(exception.Message, exception);
                } catch (System.Runtime.InteropServices.COMException exception) {
                    throw new DirectoryException("Active Directory request could not be processed. Either there is an infrastructural problem or the filter criteria are too long: " + filterCriteria.ToString(), exception);
                }
            }
            return matchingUsers;
        }

        /// <summary>
        /// Finds all matching users from this directory.
        /// </summary>
        /// <param name="filterCriteria">enumerable of filter criteria
        /// to select users for</param>
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
        private IList<IUser> FindListOfUsersUnsafe(IEnumerable<FilterCriteria> filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, string[] propertiesToPreLoad) {
            var matchingUsers = new List<IUser>();
            foreach (var currentFilterCriteria in filterCriteria) {
                matchingUsers.AddRange(this.FindListOfUsersUnsafe(currentFilterCriteria, SortCriterionCollection.Empty, 0, ulong.MaxValue, propertiesToPreLoad));
            }
            matchingUsers.Sort(new SortCriteriaComparer(sortCriteria));
            var selectedUsers = new List<IUser>(matchingUsers.Count);
            ulong maxPosition = startPosition + maxResults;
            uint position = 0;
            foreach (var matchingUser in matchingUsers) {
                if (position >= startPosition && position < maxPosition) {
                    selectedUsers.Add(matchingUser);
                }
                position++;
            }
            return selectedUsers;
        }

        /// <summary>
        /// Finds the first matching contact from this directory.
        /// </summary>
        /// <param name="id">ID to find contact for</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of contacts - this can also
        /// be used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>first matching contact from this directory or
        /// null if no match was found</returns>
        public ActiveDirectoryContact FindOneContact(Guid id, params string[] propertiesToPreLoad) {
            var filterCriteria = new FilterCriteria(nameof(IUser.Id), RelationalOperator.IsEqualTo, id);
            return this.FindOneContact(filterCriteria, SortCriterionCollection.Empty, propertiesToPreLoad);
        }

        /// <summary>
        /// Finds the first matching contact from this directory.
        /// </summary>
        /// <param name="contact">contact to find by ID</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of contacts - this can also
        /// be used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>first matching contact from this directory or
        /// null if no match was found</returns>
        public ActiveDirectoryContact FindOneContact(IPerson contact, params string[] propertiesToPreLoad) {
            return this.FindOneContact(contact.Id, propertiesToPreLoad);
        }

        /// <summary>
        /// Finds the first matching contact from this directory.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// contact for</param>
        /// <param name="sortCriteria">criteria to sort contacts by</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of contacts - this can also
        /// be used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>first matching contact from this directory or
        /// null if no match was found</returns>
        public ActiveDirectoryContact FindOneContact(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, params string[] propertiesToPreLoad) {
            ActiveDirectoryContact match = null;
            foreach (var result in this.FindContacts(filterCriteria, sortCriteria, 0, 1, propertiesToPreLoad)) {
                match = result;
                break;
            }
            return match;
        }

        /// <summary>
        /// Gets the number of users matching specific filter
        /// criteria.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// users for</param>
        /// <returns>number of objects matching filter criteria</returns>
        internal int GetCount(FilterCriteria filterCriteria) {
            return this.FindListOfUsers(filterCriteria, SortCriterionCollection.Empty, 0, ulong.MaxValue).Count;
        }

        /// <summary>
        /// Initializes the directory searcher.
        /// </summary>
        private void InitializeDirectorySearcher() {
            this.directorySearcher.Asynchronous = false;
            this.directorySearcher.SearchRoot.AuthenticationType = AuthenticationTypes.Secure;
            if (!string.IsNullOrEmpty(this.userName)) {
                this.directorySearcher.SearchRoot.Username = this.userName;
            }
            if (!string.IsNullOrEmpty(this.password)) {
                this.directorySearcher.SearchRoot.Password = this.password;
            }
            if (!string.IsNullOrEmpty(this.rootPath)) {
                this.directorySearcher.SearchRoot.Path = this.rootPath;
            }
            return;
        }

        /// <summary>
        /// Refreshes the CurrentUser property by (re)loading the
        /// user from user source.
        /// </summary>
        /// <returns>current user that just logged on</returns>
        public IUser LogOn() {
            string userName;
            if (CurrentUserSource.HttpContext == this.CurrentUserSource) {
                var principal = this.HttpContext.User;
                if (null == principal) {
                    userName = string.Empty;
                } else {
                    userName = principal.Identity.Name;
                }
            } else if (CurrentUserSource.Environment == this.CurrentUserSource) {
                userName = Environment.UserName;
            } else if (CurrentUserSource.WindowsIdentity == this.CurrentUserSource) {
                var identity = WindowsIdentity.GetCurrent();
                userName = identity.Name;
            } else {
                throw new DirectoryException("Source \"" + this.CurrentUserSource.ToString() + "\" for resolving current user is unknown.");
            }
            int lastSlashIndex = userName.LastIndexOf('\\');
            if (lastSlashIndex > -1) {
                userName = userName.Substring(lastSlashIndex + 1);
            }
            if (string.IsNullOrEmpty(userName)) {
                this.currentUser = UserDirectory.AnonymousUser;
            } else {
                var filterCriteria = new FilterCriteria(nameof(ActiveDirectoryUser.UserName), RelationalOperator.IsEqualTo, userName, FilterTarget.IsOtherTextValue);
                this.currentUser = this.FindOne(filterCriteria, SortCriterionCollection.Empty, nameof(ActiveDirectoryUser.DisplayName), nameof(ActiveDirectoryUser.UserName), nameof(ActiveDirectoryUser.CountryAlpha2Code), nameof(ActiveDirectoryUser.PreferredLanguage));
                if (null == this.currentUser) {
                    this.currentUser = UserDirectory.AnonymousUser;
                }
            }
            return this.currentUser;
        }

        /// <summary>
        /// Sets up the directory searcher for finding specific
        /// users.
        /// </summary>
        /// <param name="objectClasses">object classes to query</param>
        /// <param name="filterCriteria">filter criteria to select
        /// users for</param>
        /// <param name="sortCriteria">criteria to sort users by</param>
        /// <param name="sizeLimit">maximum number of results to return</param>
        /// <param name="propertiesToLoad">properties to load</param>
        private void SetupDirectorySearcher(IEnumerable<string> objectClasses, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, int sizeLimit, IEnumerable<string> propertiesToLoad) {
            FilterCriteria ldapFilter = null;
            foreach (var objectClass in objectClasses) {
                if (null == ldapFilter) {
                    ldapFilter = new FilterCriteria("objectClass", RelationalOperator.IsEqualTo, objectClass, FilterTarget.IsOtherTextValue);
                } else {
                    ldapFilter = ldapFilter.And("objectClass", RelationalOperator.IsEqualTo, objectClass, FilterTarget.IsOtherTextValue);
                }
            }
            var translatedFilterCriteria = filterCriteria.TranslateFieldNames(ActiveDirectory.fieldNameDictionary);
            if (null == ldapFilter) {
                ldapFilter = translatedFilterCriteria;
            } else {
                ldapFilter = ldapFilter.And(translatedFilterCriteria);
            }
            this.directorySearcher.Filter = new ActiveDirectoryFilterBuilder(ldapFilter).ToFilter();
            this.directorySearcher.PageSize = sizeLimit;
            this.directorySearcher.SizeLimit = sizeLimit;
            this.directorySearcher.PropertiesToLoad.Clear();
            this.directorySearcher.PropertiesToLoad.Add(ActiveDirectory.fieldNameDictionary[nameof(ActiveDirectoryUser.Id)]);
            var translatedSortCriteria = sortCriteria.TranslateFieldNames(ActiveDirectory.fieldNameDictionary);
            foreach (var sortCriterion in translatedSortCriteria) {
                if (!this.directorySearcher.PropertiesToLoad.Contains(sortCriterion.FieldName)) {
                    this.directorySearcher.PropertiesToLoad.Add(sortCriterion.FieldName);
                }
            }
            foreach (var property in propertiesToLoad) {
                if (!ActiveDirectory.fieldNameDictionary.TryGetValue(property, out string translatedProperty)) {
                    translatedProperty = property;
                }
                if (!this.directorySearcher.PropertiesToLoad.Contains(translatedProperty)) {
                    this.directorySearcher.PropertiesToLoad.Add(translatedProperty);
                }
            }
            this.directorySearcher.SearchScope = SearchScope.Subtree;
            if (1 == translatedSortCriteria.Count) {
                this.directorySearcher.Sort.PropertyName = translatedSortCriteria[0].FieldName;
                if (Framework.Persistence.SortDirection.Ascending == translatedSortCriteria[0].SortDirection) {
                    this.directorySearcher.Sort.Direction = SortDirection.Ascending;
                } else if (Framework.Persistence.SortDirection.Descending == translatedSortCriteria[0].SortDirection) {
                    this.directorySearcher.Sort.Direction = SortDirection.Descending;
                } else {
                    throw new DirectoryException("\"" + translatedSortCriteria[0].SortDirection.ToString() + "\" is not a valid sort direction.");
                }
            }
            return;
        }

        /// <summary>
        /// Removes a specific user from the directory.
        /// </summary>
        /// <param name="user">specific user to remove from directory</param>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        protected override bool RemoveNoCache(IUser user) {
            bool result;
            if (null == this.directorySearcher) { // object is disposed
                using (this.directorySearcher = new DirectorySearcher()) {
                    this.InitializeDirectorySearcher();
                    result = this.RemoveNoCacheUnsafe(user);
                }
                this.directorySearcher = null;
            } else {
                result = this.RemoveNoCacheUnsafe(user);
            }
            return result;
        }

        /// <summary>
        /// Removes a specific user from the directory.
        /// </summary>
        /// <param name="user">specific user to remove from directory</param>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        private bool RemoveNoCacheUnsafe(IUser user) {
            bool isRemoved = false;
            var filterCriteria = new FilterCriteria(nameof(ActiveDirectoryUser.Id), RelationalOperator.IsEqualTo, user.Id);
            this.SetupDirectorySearcher(new string[] { "user", "person" }, filterCriteria, SortCriterionCollection.Empty, 1, new string[0]);
            SearchResult searchResult = this.directorySearcher.FindOne();
            if (null != searchResult) {
                using (var directoryEntry = searchResult.GetDirectoryEntry()) {
                    using (var parentEntry = directoryEntry.Parent) {
                        try {
                            parentEntry.Children.Remove(directoryEntry);
                            isRemoved = true;
                        } catch (Exception) {
                            // ignore exceptions
                        }
                    }
                }
            }
            return isRemoved;
        }

        /// <summary>
        /// Retrieves a specific contact from directory.
        /// </summary>
        /// <param name="contact">specific contact to retrieve from
        /// directory</param>
        /// <param name="propertiesToLoad">properties to load</param>
        internal void Retrieve(ActiveDirectoryContact contact, IEnumerable<string> propertiesToLoad) {
            if (null == this.directorySearcher) { // object is disposed
                using (this.directorySearcher = new DirectorySearcher()) {
                    this.InitializeDirectorySearcher();
                    this.RetrieveUnsafe(contact, propertiesToLoad);
                }
                this.directorySearcher = null;
            } else {
                this.RetrieveUnsafe(contact, propertiesToLoad);
            }
            return;
        }

        /// <summary>
        /// Retrieves a specific user from directory.
        /// </summary>
        /// <param name="user">specific user to retrieve from
        /// directory</param>
        /// <param name="propertiesToLoad">properties to load</param>
        internal void Retrieve(ActiveDirectoryUser user, IEnumerable<string> propertiesToLoad) {
            if (null == this.directorySearcher) { // object is disposed
                using (this.directorySearcher = new DirectorySearcher()) {
                    this.InitializeDirectorySearcher();
                    this.RetrieveUnsafe(user, propertiesToLoad);
                }
                this.directorySearcher = null;
            } else {
                this.RetrieveUnsafe(user, propertiesToLoad);
            }
            return;
        }

        /// <summary>
        /// Retrieves a specific contact from directory.
        /// </summary>
        /// <param name="contact">specific contact to retrieve from
        /// directory</param>
        /// <param name="propertiesToLoad">properties to load</param>
        private void RetrieveUnsafe(ActiveDirectoryContact contact, IEnumerable<string> propertiesToLoad) {
            var filterCriteria = new FilterCriteria(nameof(ActiveDirectoryUser.Id), RelationalOperator.IsEqualTo, contact.Id);
            this.SetupDirectorySearcher(new string[] { "contact", "person" }, filterCriteria, SortCriterionCollection.Empty, 1, propertiesToLoad);
            SearchResult searchResult = this.directorySearcher.FindOne();
            contact.SetValues(searchResult, this.directorySearcher.PropertiesToLoad, ActiveDirectory.reversedFieldNameDictionary);
            contact.IsRetrieved = true;
            return;
        }

        /// <summary>
        /// Retrieves a specific user from directory.
        /// </summary>
        /// <param name="user">specific user to retrieve from
        /// directory</param>
        /// <param name="propertiesToLoad">properties to load</param>
        private void RetrieveUnsafe(ActiveDirectoryUser user, IEnumerable<string> propertiesToLoad) {
            var filterCriteria = new FilterCriteria(nameof(ActiveDirectoryUser.Id), RelationalOperator.IsEqualTo, user.Id);
            this.SetupDirectorySearcher(new string[] { "user", "person" }, filterCriteria, SortCriterionCollection.Empty, 1, propertiesToLoad);
            SearchResult searchResult = this.directorySearcher.FindOne();
            user.SetValues(searchResult, this.directorySearcher.PropertiesToLoad, ActiveDirectory.reversedFieldNameDictionary);
            user.IsRetrieved = true;
            return;
        }

        /// <summary>
        /// Updates a specific user in directory.
        /// </summary>
        /// <param name="user">specific user to update in directory</param>
        /// <param name="propertiesToLoad">properties to load</param>
        /// <returns>true if user was updated successfully in
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        internal bool Update(ActiveDirectoryUser user, IEnumerable<string> propertiesToLoad) {
            bool result;
            if (null == this.directorySearcher) { // object is disposed
                using (this.directorySearcher = new DirectorySearcher()) {
                    this.InitializeDirectorySearcher();
                    result = this.UpdateUnsafe(user, propertiesToLoad);
                }
                this.directorySearcher = null;
            } else {
                result = this.UpdateUnsafe(user, propertiesToLoad);
            }
            return result;
        }

        /// <summary>
        /// Updates a specific user in directory.
        /// </summary>
        /// <param name="user">specific user to update in directory</param>
        /// <param name="propertiesToLoad">properties to load</param>
        /// <returns>true if user was updated successfully in
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        private bool UpdateUnsafe(ActiveDirectoryUser user, IEnumerable<string> propertiesToLoad) {
            bool success;
            if (RemovalType.Remove == user.RemoveOnUpdate || RemovalType.RemoveCascadedly == user.RemoveOnUpdate) {
                success = this.Remove(user);
            } else {
                var filterCriteria = new FilterCriteria(nameof(ActiveDirectoryUser.Id), RelationalOperator.IsEqualTo, user.Id);
                this.SetupDirectorySearcher(new string[] { "user", "person" }, filterCriteria, SortCriterionCollection.Empty, 1, propertiesToLoad);
                var searchResult = this.directorySearcher.FindOne();
                if (null == searchResult) {
                    success = false;
                } else {
                    user.GetValues(searchResult, ActiveDirectory.fieldNameDictionary);
                    success = true;
                }
            }
            return success;
        }

    }

}