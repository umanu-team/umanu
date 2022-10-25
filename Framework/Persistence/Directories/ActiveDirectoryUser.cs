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
    using Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// User that belongs to an Active Directory.
    /// </summary>
    public sealed class ActiveDirectoryUser : ActiveDirectoryContact, IUser {

        /// <summary>
        /// Preferred culture based on preferred language and
        /// country.
        /// </summary>
        public CultureInfo Culture {
            get {
                if (null == this.culture) {
                    var culture = CultureInfo.InvariantCulture;
                    if (!string.IsNullOrEmpty(this.PreferredLanguage)) {
                        string cultureName = this.PreferredLanguage;
                        if (cultureName.Length < 5 && !string.IsNullOrEmpty(this.CountryAlpha2Code)) {
                            cultureName += '-' + this.CountryAlpha2Code;
                        }
                        if (cultureName.Length > 4) {
                            var cultureInfos = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                            foreach (var cultureInfo in cultureInfos) {
                                if (cultureInfo.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase)) {
                                    culture = cultureInfo;
                                    break;
                                }
                            }
                        } else {
                            var cultureInfos = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
                            foreach (var cultureInfo in cultureInfos) {
                                if (cultureInfo.Name.Equals(this.PreferredLanguage, StringComparison.OrdinalIgnoreCase)) {
                                    culture = cultureInfo;
                                    culture.DateTimeFormat = new CultureInfo("de-DE").DateTimeFormat;
                                    break;
                                }
                            }
                        }
                    }
                    this.culture = culture;
                }
                return this.culture;
            }
            set {
                this.PreferredLanguage = value.Name;
            }
        }

        /// <summary>
        /// Collection of all field names that are created for new
        /// users.
        /// </summary>
        public static IEnumerable<string> FieldNames {
            get {
                if (null == ActiveDirectoryUser.fieldNames) {
                    ActiveDirectoryUser emptyUser = new ActiveDirectoryUser();
                    ActiveDirectoryUser.fieldNames = emptyUser.presentableFields.Keys;
                }
                return ActiveDirectoryUser.fieldNames;
            }
        }
        private static IEnumerable<string> fieldNames = null;

        /// <summary>
        /// Manager user is reporting to.
        /// </summary>
        public IUser Manager {
            get {
                IUser manager;
                if (string.IsNullOrEmpty(this.ManagerDistinguishedName)) {
                    manager = null;
                } else {
                    string distinguishedName = this.ManagerDistinguishedName;
                    var filter = new FilterCriteria(nameof(ActiveDirectoryUser.DistinguishedName), RelationalOperator.IsEqualTo, distinguishedName, FilterTarget.IsOtherTextValue);
                    manager = this.ParentUserDirectory.FindOne(filter, SortCriterionCollection.Empty, nameof(ActiveDirectoryUser.DisplayName), nameof(ActiveDirectoryUser.UserName));
                }
                return manager;
            }
            set {
                if (null == value) {
                    this.ManagerDistinguishedName = string.Empty;
                } else {
                    ActiveDirectoryUser manager = value as ActiveDirectoryUser;
                    if (null == manager) {
                        throw new ArgumentException("Manager is either null or not of type \"" + typeof(ActiveDirectoryUser).FullName + "\".");
                    } else {
                        this.ManagerDistinguishedName = manager.DistinguishedName;
                    }
                }
            }
        }

        /// <summary>
        /// Technical name of manager user is reporting to.
        /// </summary>
        public string ManagerDistinguishedName {
            get {
                return this.GetValue(nameof(ActiveDirectoryUser.ManagerDistinguishedName));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryUser.ManagerDistinguishedName), value);
            }
        }

        /// <summary>
        /// Preferred language as ISO 639 code.
        /// </summary>
        public string PreferredLanguage {
            get {
                return this.GetValue(nameof(ActiveDirectoryUser.PreferredLanguage));
            }
            set {
                this.culture = null;
                this.SetValue(nameof(ActiveDirectoryUser.PreferredLanguage), value);
            }
        }

        /// <summary>
        /// Room number.
        /// </summary>
        public string RoomNumber {
            get {
                return this.GetValue(nameof(ActiveDirectoryUser.RoomNumber));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryUser.RoomNumber), value);
            }
        }

        /// <summary>
        /// User name used for login.
        /// </summary>
        public string UserName {
            get {
                return this.GetValue(nameof(ActiveDirectoryUser.UserName));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryUser.UserName), value);
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ActiveDirectoryUser()
            : base() {
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryUser.ManagerDistinguishedName)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryUser.PreferredLanguage)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryUser.RoomNumber)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryUser.UserName)));
        }

        /// <summary>
        /// Instantiates a new instance (copy constructor).
        /// </summary>
        /// <param name="source">source instance to deep copy state
        /// from</param>
        /// <param name="includeId">true to copy ID of source as
        /// well, false otherwise</param>
        public ActiveDirectoryUser(IUser source, bool includeId)
            : this() {
            this.CopyFrom(source, includeId);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="firstName">first name of user</param>
        /// <param name="lastName">last name of user</param>
        public ActiveDirectoryUser(string firstName, string lastName)
            : this() {
            if (string.IsNullOrEmpty(firstName)) {
                this.LastName = lastName;
                this.FirstName = firstName;
            } else {
                this.FirstName = firstName;
                this.LastName = lastName;
            }
            this.SetNamePropertys();
        }

        /// <summary>
        /// Changes the password of this user.
        /// </summary>
        /// <param name="oldPassword">old password</param>
        /// <param name="newPassword">new password</param>
        /// <returns>true if password was updated successfully;
        /// otherwise, false</returns>
        public bool ChangePassword(string oldPassword, string newPassword) {
            return this.ParentUserDirectory.ChangePassword(this, oldPassword, newPassword);
        }

        /// <summary>
        /// Gets the value of an Active Directory specific attribute.
        /// </summary>
        /// <param name="attributeName">name of Active Directory
        /// specific attribute to get value for</param>
        /// <returns>value of Active Directory specific attribute</returns>
        public override string GetAttributeValue(string attributeName) {
            var activeDirectoryUser = new ActiveDirectoryUser();
            activeDirectoryUser.Id = this.Id;
            activeDirectoryUser.ParentUserDirectory = this.ParentUserDirectory;
            if (!activeDirectoryUser.presentableFields.Contains(attributeName)) {
                activeDirectoryUser.presentableFields.Add(new PresentableFieldForString(activeDirectoryUser, attributeName));
            }
            return activeDirectoryUser.GetValue(attributeName);
        }

        /// <summary>
        /// Removes this user from directory.
        /// </summary>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        public bool Remove() {
            if (this.IsNew) {
                throw new ObjectNotPersistentException("User cannot be deleted because it is not connected to a directory.");
            }
            return this.ParentUserDirectory.Remove(this);
        }

        /// <summary>
        /// Retrieves all values of this user from directory. This
        /// can be used to refresh the values of this object.
        /// </summary>
        public override void Retrieve() {
            if (this.IsNew) {
                throw new ObjectNotPersistentException("User cannot be retrieved because it is not connected to a directory.");
            } else {
                this.culture = null;
                this.ParentUserDirectory.Retrieve(this, this.presentableFields.Keys);
            }
            return;
        }

        /// <summary>
        /// Sets display name, initials and user name based on
        /// current first name and last name.
        /// </summary>
        public override void SetNamePropertys() {
            string userName = string.Empty;
            string lastName = this.LastName;
            if (!string.IsNullOrEmpty(lastName)) {
                userName = lastName;
            }
            string firstName = this.FirstName;
            if (!string.IsNullOrEmpty(firstName)) {
                userName += firstName.Substring(0, 1);
            }
            if (string.IsNullOrEmpty(userName)) {
                throw new DirectoryException("First name and last name may not be null/empty at the same time.");
            } else {
                base.SetNamePropertys();
                this.SetValue(nameof(ActiveDirectoryUser.UserName), userName);
            }
            return;
        }

        /// <summary>
        /// Encrypts and sets a password using a non reversable
        /// algorithm.
        /// </summary>
        /// <param name="password">password to encrypt and set</param>
        public void SetPassword(string password) {
            throw new NotSupportedException("Password of active directory users cannot be set.");
        }

        /// <summary>
        /// Updates this user in directory.
        /// </summary>
        /// <returns>true if user was updated successfully in
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        public bool Update() {
            bool success;
            if (this.IsNew) {
                throw new ObjectNotPersistentException("User cannot be updated because it is not connected to a directory.");
            } else {
                success = this.ParentUserDirectory.Update(this, this.presentableFields.Keys);
            }
            return success;
        }

    }

}