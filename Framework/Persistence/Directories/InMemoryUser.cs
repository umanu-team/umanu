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
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents an in-memory user.
    /// </summary>
    internal class InMemoryUser : PresentableObject, IUser {

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
        /// Preferred culture.
        /// </summary>
        public CultureInfo Culture { get; set; }

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
        /// True if this object is read-only, false otherwise.
        /// </summary>
        public bool IsWriteProtected {
            get { return false; }
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
        /// <param name="id">globally unique identifier of this
        /// object</param>
        /// <param name="userName">user name used for login</param>
        internal InMemoryUser(Guid id, string userName) {
            this.DisplayName = this.UserName = userName;
            this.Id = id;
            this.IsNew = false;
        }

        /// <summary>
        /// Changes the password of this user.
        /// </summary>
        /// <param name="oldPassword">old password</param>
        /// <param name="newPassword">new password</param>
        /// <returns>true if the password was updated successfully;
        /// otherwise, false</returns>
        public bool ChangePassword(string oldPassword, string newPassword) {
            throw new NotSupportedException("Password of in-memory user cannot be changed.");
        }

        /// <summary>
        /// Deep copies the state of another instance of this type
        /// into this instance, as far as all child objects implement
        /// ICopyable&lt;T&gt;.
        /// </summary>
        /// <param name="source">source instance to deep copy state
        /// from</param>
        public void CopyFrom(IUser source) {
            throw new NotSupportedException("In-memory user cannot copy values of other user.");
        }

        /// <summary>
        /// Removes this user from directory.
        /// </summary>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        public bool Remove() {
            throw new NotSupportedException("In-memory user cannot be removed.");
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
            throw new NotSupportedException("Password of in-memory user cannot be set.");
        }

        /// <summary>
        /// Updates this user in directory.
        /// </summary>
        /// <returns>true if user was updated successfully in
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        public bool Update() {
            throw new NotSupportedException("In-memory user cannot be updated.");
        }

    }

}