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
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Represents an anonymous user.
    /// </summary>
    internal sealed class AnonymousUser : IUser {

        /// <summary>
        /// Day of birth.
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// City of address.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Company name.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Country of address.
        /// </summary>
        public Country Country { get; set; }

        /// <summary>
        /// ISO 3166-1 alpha-2 country code of address.
        /// </summary>
        public string CountryAlpha2Code { get; set; }

        /// <summary>
        /// Time of creation of this user in directory.
        /// </summary>
        public DateTime CreatedAt {
            get {
                return UtcDateTime.MinValue;
            }
        }

        /// <summary>
        /// User who created this user in directory.
        /// </summary>
        public IUser CreatedBy {
            get {
                return null;
            }
        }

        /// <summary>
        /// Preferred culture.
        /// </summary>
        public CultureInfo Culture {
            get {
                var culture = new CultureInfo("en-GB");
                if (null != System.Web.HttpContext.Current?.Request?.UserLanguages) {
                    foreach (var userLanguage in System.Web.HttpContext.Current.Request.UserLanguages) {
                        try {
                            culture = new CultureInfo(userLanguage); // culture has to be set on the fly because anonymous user might be static
                            break;
                        } catch (Exception) {
                            // ignore exceptions
                        }
                    }
                }
                return culture;
            }
            set {
                throw new NotSupportedException("Culture of anonymous user cannot be changed.");
            }
        }

        /// <summary>
        /// Department of address.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Short description of user.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Name to be displayed.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// E-Mail address.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Facsimile number.
        /// </summary>
        public string FaxNumber { get; set; }

        /// <summary>
        /// First name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Number of home phone.
        /// </summary>
        public string HomePhoneNumber { get; set; }

        /// <summary>
        /// House number of address.
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        /// Globally unique identifier of this object.
        /// </summary>
        public Guid Id {
            get {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Thumbnail photo.
        /// </summary>
        public ImageFile Photo { get; set; }

        /// <summary>
        /// Initials of name.
        /// </summary>
        public string Initials {
            get {
                string initials = string.Empty;
                if (!string.IsNullOrEmpty(this.FirstName)) {
                    initials = this.FirstName.Substring(0, 1);
                }
                if (!string.IsNullOrEmpty(this.LastName)) {
                    initials += this.LastName.Substring(0, 1);
                }
                return initials;
            }
        }

        /// <summary>
        /// True if this is a new object, false otherwise.
        /// </summary>
        public bool IsNew {
            get {
                return false;
            }
        }

        /// <summary>
        /// True if this object is read-only, false otherwise.
        /// </summary>
        public bool IsWriteProtected {
            get {
                return true;
            }
        }

        /// <summary>
        /// Job title.
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Managers user is reporting to.
        /// </summary>
        public IUser Manager { get; set; }

        /// <summary>
        /// Number of mobile phone.
        /// </summary>
        public string MobilePhoneNumber { get; set; }

        /// <summary>
        /// Time of last modification in directory.
        /// </summary>
        public DateTime ModifiedAt {
            get {
                return UtcDateTime.MinValue;
            }
        }

        /// <summary>
        /// User who modified this user in directory.
        /// </summary>
        public IUser ModifiedBy {
            get {
                return null;
            }
        }

        /// <summary>
        /// "About me" notes.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Name of office building.
        /// </summary>
        public string Office { get; set; }

        /// <summary>
        /// Number of office phone.
        /// </summary>
        public string OfficePhoneNumber { get; set; }

        /// <summary>
        /// Personal titles like academic titles or aristocratic
        /// titles.
        /// </summary>
        public string PersonalTitle { get; set; }

        /// <summary>
        /// P.O. box of address.
        /// </summary>
        public string PostOfficeBox { get; set; }

        /// <summary>
        /// Preferred language as ISO 639 code.
        /// </summary>
        public string PreferredLanguage { get; set; }

        /// <summary>
        /// Indicates whether to remove this object on update.
        /// </summary>
        public RemovalType RemoveOnUpdate { get; set; }

        /// <summary>
        /// Room number.
        /// </summary>
        public string RoomNumber { get; set; }

        /// <summary>
        /// State of address.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Street of address.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// User name used for login.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// URL of web site.
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// Zip code of address.
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        internal AnonymousUser() {
            this.DisplayName = "Anonymous";
        }

        /// <summary>
        /// Changes the password of this user.
        /// </summary>
        /// <param name="oldPassword">old password</param>
        /// <param name="newPassword">new password</param>
        /// <returns>true if the password was updated successfully;
        /// otherwise, false</returns>
        public bool ChangePassword(string oldPassword, string newPassword) {
            throw new NotSupportedException("Password of anonymous user cannot be changed.");
        }

        /// <summary>
        /// Deep copies the state of another instance of this type
        /// into this instance, as far as all child objects implement
        /// ICopyable&lt;T&gt;.
        /// </summary>
        /// <param name="source">source instance to deep copy state
        /// from</param>
        public void CopyFrom(IUser source) {
            throw new NotSupportedException("Anonymous user cannot copy values of other user.");
        }

        /// <summary>
        /// Finds the first presentable field for a specified key.
        /// </summary>
        /// <param name="key">key to find presentable field for</param>
        /// <returns>first presentable field for specified key or
        /// null</returns>
        public IPresentableField FindPresentableField(string key) {
            return this.FindPresentableField(KeyChain.FromKey(key));
        }

        /// <summary>
        /// Finds the first presentable field for a specified key
        /// chain.
        /// </summary>
        /// <param name="keyChain">key chain to find presentable
        /// field for</param>
        /// <returns>first presentable field for specified key chain
        /// or null</returns>
        public IPresentableField FindPresentableField(string[] keyChain) {
            IPresentableField presentableField;
            if (1L == keyChain.LongLength) {
                presentableField = this.FindPresentableField(keyChain[1]);
            } else {
                presentableField = null;
            }
            return presentableField;
        }

        /// <summary>
        /// Finds all presentable fields for a specified key.
        /// </summary>
        /// <param name="key">key to find presentable fields for</param>
        /// <returns>all presentable fields for specified key or null</returns>
        public IEnumerable<IPresentableField> FindPresentableFields(string key) {
            yield return this.FindPresentableField(key);
        }

        /// <summary>
        /// Finds all presentable fields for a specified key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find presentable
        /// fields for</param>
        /// <returns>all presentable fields for specified key chain
        /// or null</returns>
        public IEnumerable<IPresentableField> FindPresentableFields(string[] keyChain) {
            yield return this.FindPresentableField(keyChain);
        }

        /// <summary>
        /// Removes this user from directory.
        /// </summary>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        public bool Remove() {
            throw new NotSupportedException("Anonymous user cannot be removed.");
        }

        /// <summary>
        /// Retrieves all values of this user from directory. This
        /// can be used to refresh the values of this object.
        /// </summary>
        public void Retrieve() {
            // nothing to do
            return;
        }

        /// <summary>
        /// Encrypts and sets a password using a non reversable
        /// algorithm.
        /// </summary>
        /// <param name="password">password to encrypt and set</param>
        public void SetPassword(string password) {
            throw new NotSupportedException("Password of anonymous user cannot be set.");
        }

        /// <summary>
        /// Updates this user in directory.
        /// </summary>
        /// <returns>true if user was updated successfully in
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        public bool Update() {
            throw new NotSupportedException("Anonymous user cannot be updated.");
        }

    }

}