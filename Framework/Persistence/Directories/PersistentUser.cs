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
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// User that belongs to a user directory in a persistence
    /// mechanism.
    /// </summary>
    public sealed class PersistentUser : Person, IProvidableUser {

        /// <summary>
        /// Preferred culture.
        /// </summary>
        public CultureInfo Culture {
            get {
                CultureInfo culture;
                if (null == this.culture.Value) {
                    culture = CultureInfo.InvariantCulture;
                } else {
                    culture = new CultureInfo(this.culture.Value);
                }
                return culture;
            }
            set {
                this.culture.Value = value.Name;
            }
        }
        private readonly PersistentFieldForString culture =
            new PersistentFieldForString(nameof(Culture));

        /// <summary>
        /// Encrypted password.
        /// </summary>
        private string EncryptedPassword {
            get { return this.encryptedPassword.Value; }
            set { this.encryptedPassword.Value = value; }
        }
        private readonly PersistentFieldForString encryptedPassword =
            new PersistentFieldForString(nameof(EncryptedPassword));

        /// <summary>
        /// Managers user is reporting to.
        /// </summary>
        public IUser Manager {
            get { return this.manager.Value; }
            set { this.manager.Value = value; }
        }
        private readonly PersistentFieldForIUser manager =
            new PersistentFieldForIUser(nameof(Manager));

        /// <summary>
        /// Expiration date of password.
        /// </summary>
        public DateTime PasswordExpirationDate {
            get { return this.passwordExpirationDate.Value; }
            set { this.passwordExpirationDate.Value = value; }
        }
        private readonly PersistentFieldForDateTime passwordExpirationDate =
            new PersistentFieldForDateTime(nameof(PasswordExpirationDate), UtcDateTime.Today);

        /// <summary>
        /// Preferred language as ISO 639 code.
        /// </summary>
        public string PreferredLanguage {
            get { return this.preferredLanguage.Value; }
            set { this.preferredLanguage.Value = value; }
        }
        private readonly PersistentFieldForString preferredLanguage =
            new PersistentFieldForString(nameof(PreferredLanguage));

        /// <summary>
        /// Collection of all presentable fields of this presentable
        /// object.
        /// </summary>
        private PresentableFieldCollection PresentableFields {
            get {
                if (null == this.presentableFields) {
                    this.presentableFields = new PresentableFieldCollection();
                }
                return this.presentableFields;
            }
        }
        private PresentableFieldCollection presentableFields;

        /// <summary>
        /// Room number.
        /// </summary>
        public string RoomNumber {
            get { return this.roomNumber.Value; }
            set { this.roomNumber.Value = value; }
        }
        private readonly PersistentFieldForString roomNumber =
            new PersistentFieldForString(nameof(RoomNumber));

        /// <summary>
        /// User name used for login.
        /// </summary>
        public string UserName {
            get { return this.userName.Value; }
            set { this.userName.Value = value; }
        }
        private readonly PersistentFieldForString userName =
            new PersistentFieldForString(nameof(UserName));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PersistentUser()
            : base() {
            this.RegisterPersistentField(this.culture);
            this.RegisterPersistentField(this.encryptedPassword);
            this.RegisterPersistentField(this.manager);
            this.RegisterPersistentField(this.passwordExpirationDate);
            this.RegisterPersistentField(this.preferredLanguage);
            this.RegisterPersistentField(this.roomNumber);
            this.UserName = this.Id.ToString("N");
            this.RegisterPersistentField(this.userName);
            this.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                if (nameof(PersistentUser.EmailAddress) == e.PropertyName) {
                    if (PersistentUserDirectory.HasEmailAddressAsUserName) {
                        if (string.IsNullOrEmpty(this.EmailAddress)) {
                            this.UserName = this.Id.ToString("N");
                        } else {
                            this.UserName = this.EmailAddress;
                        }
                    }
                } else if (nameof(PersistentUser.UserName) == e.PropertyName) {
                    if (true == this.UserName?.Contains(":")) {
                        this.UserName = this.UserName.Replace(":", string.Empty);
                    }
                }
                return;
            };
        }

        /// <summary>
        /// Instantiates a new instance (deep copy constructor
        /// including ID).
        /// </summary>
        /// <param name="source">source instance to deep copy state
        /// from including ID</param>
        internal PersistentUser(IUser source)
            : this() {
            this.Id = source.Id;
            if (source.Birthday.HasValue) {
                this.Birthday = new DateTime(source.Birthday.Value.Ticks, source.Birthday.Value.Kind);
            } else {
                this.Birthday = null;
            }
            if (null == source.City) {
                this.City = null;
            } else {
                this.City = string.Copy(source.City);
            }
            if (null == source.Company) {
                this.Company = null;
            } else {
                this.Company = string.Copy(source.Company);
            }
            if (null == source.CountryAlpha2Code) {
                this.CountryAlpha2Code = null;
            } else {
                this.CountryAlpha2Code = string.Copy(source.CountryAlpha2Code);
            }
            this.CreatedAt = new DateTime(source.CreatedAt.Ticks, source.CreatedAt.Kind);
            this.CreatedBy = source.CreatedBy;
            if (null == source.Culture) {
                this.Culture = null;
            } else {
                this.Culture = new CultureInfo(source.Culture.Name);
            }
            if (null == source.Department) {
                this.Department = null;
            } else {
                this.Department = string.Copy(source.Department);
            }
            if (null == source.Description) {
                this.Description = null;
            } else {
                this.Description = string.Copy(source.Description);
            }
            if (null == source.DisplayName) {
                this.DisplayName = null;
            } else {
                this.DisplayName = string.Copy(source.DisplayName);
            }
            if (null == source.EmailAddress) {
                this.EmailAddress = null;
            } else {
                this.EmailAddress = string.Copy(source.EmailAddress);
            }
            if (null == source.FaxNumber) {
                this.FaxNumber = null;
            } else {
                this.FaxNumber = string.Copy(source.FaxNumber);
            }
            if (null == source.FirstName) {
                this.FirstName = null;
            } else {
                this.FirstName = string.Copy(source.FirstName);
            }
            if (null == source.HomePhoneNumber) {
                this.HomePhoneNumber = null;
            } else {
                this.HomePhoneNumber = string.Copy(source.HomePhoneNumber);
            }
            if (null == source.HouseNumber) {
                this.HouseNumber = null;
            } else {
                this.HouseNumber = string.Copy(source.HouseNumber);
            }
            if (null == source.JobTitle) {
                this.JobTitle = null;
            } else {
                this.JobTitle = string.Copy(source.JobTitle);
            }
            if (null == source.LastName) {
                this.LastName = null;
            } else {
                this.LastName = string.Copy(source.LastName);
            }
            this.Manager = source.Manager;
            if (null == source.MobilePhoneNumber) {
                this.MobilePhoneNumber = null;
            } else {
                this.MobilePhoneNumber = string.Copy(source.MobilePhoneNumber);
            }
            this.ModifiedAt = new DateTime(source.ModifiedAt.Ticks, source.ModifiedAt.Kind);
            this.ModifiedBy = source.ModifiedBy;
            if (null == source.Notes) {
                this.Notes = null;
            } else {
                this.Notes = string.Copy(source.Notes);
            }
            if (null == source.Office) {
                this.Office = null;
            } else {
                this.Office = string.Copy(source.Office);
            }
            if (null == source.OfficePhoneNumber) {
                this.OfficePhoneNumber = null;
            } else {
                this.OfficePhoneNumber = string.Copy(source.OfficePhoneNumber);
            }
            if (null == source.PersonalTitle) {
                this.PersonalTitle = null;
            } else {
                this.PersonalTitle = string.Copy(source.PersonalTitle);
            }
            if (null == source.Photo) {
                this.Photo = null;
            } else {
                this.Photo = new ImageFile();
                this.Photo.CopyFrom(source.Photo, CopyBehaviorForAllowedGroups.DoNotCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.DeepCopy);
            }
            if (null == source.PostOfficeBox) {
                this.PostOfficeBox = null;
            } else {
                this.PostOfficeBox = string.Copy(source.PostOfficeBox);
            }
            if (null == source.PreferredLanguage) {
                this.PreferredLanguage = null;
            } else {
                this.PreferredLanguage = string.Copy(source.PreferredLanguage);
            }
            if (null == source.RoomNumber) {
                this.RoomNumber = null;
            } else {
                this.RoomNumber = string.Copy(source.RoomNumber);
            }
            if (null == source.State) {
                this.State = null;
            } else {
                this.State = string.Copy(source.State);
            }
            if (null == source.Street) {
                this.Street = null;
            } else {
                this.Street = string.Copy(source.Street);
            }
            if (null == source.UserName) {
                this.UserName = null;
            } else {
                this.UserName = string.Copy(source.UserName);
            }
            if (null == source.WebSite) {
                this.WebSite = null;
            } else {
                this.WebSite = string.Copy(source.WebSite);
            }
            if (null == source.ZipCode) {
                this.ZipCode = null;
            } else {
                this.ZipCode = string.Copy(source.ZipCode);
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="firstName">first name of user</param>
        /// <param name="lastName">last name of user</param>
        public PersistentUser(string firstName, string lastName)
            : this() {
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        /// <summary>
        /// Adds a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to add</param>
        public void AddPresentableField(IPresentableField presentableField) {
            var existingField = this.FindPresentableField(presentableField.Key);
            if (null != existingField) {
                throw new ArgumentException("Presentable field with key \"" + presentableField.Key + "\" cannot be added because a field with the same key was added already.", nameof(presentableField));
            }
            this.PresentableFields.Add(presentableField);
            return;
        }

        /// <summary>
        /// Changes the password of this user and updates it.
        /// </summary>
        /// <param name="oldPassword">old password</param>
        /// <param name="newPassword">new password</param>
        /// <returns>true if user was updated successfully;
        /// otherwise, false</returns>
        public bool ChangePassword(string oldPassword, string newPassword) {
            bool success = this.HasPassword(oldPassword);
            if (success) {
                this.SetPassword(newPassword);
                success = this.Update();
            }
            return success;
        }

        /// <summary>
        /// Clears the password of this user.
        /// </summary>
        public void ClearPassword() {
            this.EncryptedPassword = null;
            return;
        }

        /// <summary>
        /// Gets all presentable fields.
        /// </summary>
        /// <returns>enumerable of presentable fields</returns>
        protected override IEnumerable<IPresentableField> GetPresentableFields() {
            foreach (var presentableField in base.GetPresentableFields()) {
                yield return presentableField;
            }
            foreach (var presentableField in this.PresentableFields) {
                yield return presentableField;
            }
        }

        /// <summary>
        /// Gets the title of providable object.
        /// </summary>
        /// <returns>title of providable object</returns>
        public string GetTitle() {
            return this.DisplayName;
        }

        /// <summary>
        /// Verifies a clear text password against the encrypted
        /// password of this user.
        /// </summary>
        /// <param name="password">clear text password to varify</param>
        /// <returns>true if clear text password matches for this
        /// user, false otherwise</returns>
        public bool HasPassword(string password) {
            return UserDirectory.VerifyPassword(password, this.EncryptedPassword);
        }

        /// <summary>
        /// Encrypts and sets a password using a non reversable
        /// algorithm.
        /// </summary>
        /// <param name="password">password to encrypt and set</param>
        public void SetPassword(string password) {
            this.EncryptedPassword = UserDirectory.EncryptPassword(password);
            return;
        }

    }

}