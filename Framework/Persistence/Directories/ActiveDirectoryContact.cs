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
    using Model;
    using Presentation;
    using Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.DirectoryServices;
    using System.Globalization;

    /// <summary>
    /// Contact that belongs to an Active Directory.
    /// </summary>
    public class ActiveDirectoryContact : IPerson, IProvidableObject {

        /// <summary>
        /// Allowed groups for reading/writing this object.
        /// </summary>
        public AllowedGroups AllowedGroups {
            get { return null; }
        }

        /// <summary>
        /// Day of birth - this is not supported by Active Directory.
        /// </summary>
        public DateTime? Birthday {
            get {
                return null;
            }
            set {
                // not supported
            }
        }

        /// <summary>
        /// City of address.
        /// </summary>
        public string City {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.City));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.City), value);
            }
        }

        /// <summary>
        /// Backing field of culture.
        /// </summary>
        protected CultureInfo culture;

        /// <summary>
        /// Company name.
        /// </summary>
        public string Company {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.Company));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.Company), value);
            }
        }

        /// <summary>
        /// Country of address. If this property is set, the country
        /// alpha 2 code, the country name and the numeric country
        /// code will be adjusted automatically.
        /// </summary>
        public Country Country {
            get {
                return Country.GetByAlpha2Code(this.CountryAlpha2Code);
            }
            set {
                this.culture = null;
                this.SetValue(nameof(ActiveDirectoryContact.CountryAlpha2Code), value.Alpha2Code);
                this.SetValue(nameof(ActiveDirectoryContact.CountryName), value.Name);
                this.SetValue(nameof(ActiveDirectoryContact.CountryNumericCode), value.NumericCode.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// ISO 3166-1 alpha-2 country code of address. If this
        /// property is set, the country name as well as the numeric
        /// country code will be adjusted automatically.
        /// </summary>
        public string CountryAlpha2Code {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.CountryAlpha2Code));
            }
            set {
                var country = Country.GetByAlpha2Code(value);
                if (null == country) {
                    throw new DirectoryException("\"" + value + "\" is not a valid ISO 3166-1 alpha-2 country code.");
                } else {
                    this.Country = country;
                }
            }
        }

        /// <summary>
        /// Name of country of address. If this property is set, the
        /// alpha-2 country code as well as the numeric country code
        /// will be adjusted automatically.
        /// </summary>
        public string CountryName {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.CountryName));
            }
            set {
                var country = Country.GetByName(value);
                if (null == country) {
                    throw new DirectoryException("\"" + value + "\" is not a valid country name.");
                } else {
                    this.Country = country;
                }
            }
        }

        /// <summary>
        /// ISO 3166-1 numeric country code of address. If this
        /// property is set, the alpha-2 country code as well as the
        /// country name will be adjusted automatically.
        /// </summary>
        public ushort? CountryNumericCode {
            get {
                var presentableField = this.presentableFields[nameof(ActiveDirectoryContact.CountryNumericCode)] as PresentableFieldForNullableUShort;
                return presentableField.Value;
            }
            set {
                if (value.HasValue) {
                    var country = Country.GetByNumericCode(value.Value);
                    if (null == country) {
                        throw new DirectoryException("\"" + value + "\" is not a valid ISO 3166-1 numeric country code.");
                    } else {
                        this.Country = country;
                    }
                } else {
                    this.Country = null;
                }
            }
        }

        /// <summary>
        /// Time of creation of this user in directory.
        /// </summary>
        public DateTime CreatedAt {
            get {
                return UtcDateTime.Parse(this.GetValue(nameof(ActiveDirectoryContact.CreatedAt)));
            }
        }

        /// <summary>
        /// User who created this user in directory - this is not
        /// supported by Active Directory.
        /// </summary>
        public IUser CreatedBy {
            get {
                return null;
            }
        }

        /// <summary>
        /// Department of address.
        /// </summary>
        public string Department {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.Department));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.Department), value);
            }
        }

        /// <summary>
        /// Short description of user.
        /// </summary>
        public string Description {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.Description));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.Description), value);
            }
        }

        /// <summary>
        /// Name to be displayed. This is also used as common name.
        /// </summary>
        public string DisplayName {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.DisplayName));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.DisplayName), value);
            }
        }

        /// <summary>
        /// Technical name of user in LDAP directory.
        /// </summary>
        public string DistinguishedName {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.DistinguishedName));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.DistinguishedName), value);
            }
        }

        /// <summary>
        /// E-Mail address.
        /// </summary>
        public string EmailAddress {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.EmailAddress));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.EmailAddress), value);
            }
        }

        /// <summary>
        /// Facsimile number.
        /// </summary>
        public string FaxNumber {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.FaxNumber));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.FaxNumber), value);
            }
        }

        /// <summary>
        /// First name.
        /// </summary>
        public string FirstName {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.FirstName));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.FirstName), value);
            }
        }

        /// <summary>
        /// Indicates whether object has versions.
        /// </summary>
        public bool HasVersions {
            get { return false; }
        }

        /// <summary>
        /// Enumerable of previous persistent versions of this
        /// object.
        /// </summary>
        public ReadOnlyCollection<Model.Version> Versions {
            get { return new List<Model.Version>(0).AsReadOnly(); }
        }

        /// <summary>
        /// Number of home phone.
        /// </summary>
        public string HomePhoneNumber {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.HomePhoneNumber));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.HomePhoneNumber), value);
            }
        }

        /// <summary>
        /// House number of address - this is not supported by Active
        /// Directory.
        /// </summary>
        public string HouseNumber {
            get {
                return null;
            }
            set {
                // not supported
            }
        }

        /// <summary>
        /// Globally unique identifier of this user in directory.
        /// </summary>
        public Guid Id {
            get {
                var presentableField = this.presentableFields[nameof(ActiveDirectoryContact.Id)] as PresentableFieldForGuid;
                return presentableField.Value;
            }
            internal set {
                var presentableField = this.presentableFields[nameof(ActiveDirectoryContact.Id)] as PresentableFieldForGuid;
                presentableField.Value = value;
            }
        }

        /// <summary>
        /// Initials of name.
        /// </summary>
        public string Initials {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.Initials));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.Initials), value);
            }
        }

        /// <summary>
        /// True if this object was not read from persistence
        /// mechanism before, false otherwise.
        /// </summary>
        public bool IsNew {
            get {
                return null == this.ParentUserDirectory;
            }
        }

        /// <summary>
        /// True if this user is persistent and initialized with
        /// values from directory, false otherwise.
        /// </summary>
        public bool IsRetrieved { get; internal set; }

        /// <summary>
        /// True if this object is read-only, false otherwise.
        /// </summary>
        public bool IsWriteProtected {
            get {
                return false;
            }
        }

        /// <summary>
        /// Job title.
        /// </summary>
        public string JobTitle {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.JobTitle));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.JobTitle), value);
            }
        }

        /// <summary>
        /// Last name.
        /// </summary>
        public string LastName {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.LastName));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.LastName), value);
            }
        }

        /// <summary>
        /// Number of mobile phone.
        /// </summary>
        public string MobilePhoneNumber {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.MobilePhoneNumber));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.MobilePhoneNumber), value);
            }
        }

        /// <summary>
        /// Time of last modification in directory.
        /// </summary>
        public DateTime ModifiedAt {
            get {
                return UtcDateTime.Parse(this.GetValue(nameof(ActiveDirectoryContact.ModifiedAt)));
            }
        }

        /// <summary>
        /// User who modified this user in directory - this is not
        /// supported by Active Directory.
        /// </summary>
        public IUser ModifiedBy {
            get {
                return null;
            }
        }

        /// <summary>
        /// "About me" notes.
        /// </summary>
        public string Notes {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.Notes));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.Notes), value);
            }
        }

        /// <summary>
        /// Name of office building.
        /// </summary>
        public string Office {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.Office));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.Office), value);
            }
        }

        /// <summary>
        /// Number of office phone.
        /// </summary>
        public string OfficePhoneNumber {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.OfficePhoneNumber));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.OfficePhoneNumber), value);
            }
        }

        /// <summary>
        /// Parent user directory of this user.
        /// </summary>
        public ActiveDirectory ParentUserDirectory { get; internal set; }

        /// <summary>
        /// Personal titles like academic titles or aristocratic
        /// titles.
        /// </summary>
        public string PersonalTitle {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.PersonalTitle));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.PersonalTitle), value);
            }
        }

        /// <summary>
        /// Thumbnail photo.
        /// </summary>
        public ImageFile Photo {
            get {
                if (!this.IsRetrieved) {
                    this.Retrieve();
                }
                var presentableField = this.presentableFields[nameof(ActiveDirectoryContact.Photo)] as PresentableFieldForImageFile;
                return presentableField.Value;
            }
            set {
                var presentableField = this.presentableFields[nameof(ActiveDirectoryContact.Photo)] as PresentableFieldForImageFile;
                if (null == value) {
                    presentableField.Value = null;
                } else {
                    var imageFile = new ImageFile();
                    imageFile.CopyFrom(value, CopyBehaviorForAllowedGroups.DoNotCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.DeepCopy);
                    imageFile.Id = this.Id;
                    imageFile.Name = "thumbnailPhoto.jpg";
                    presentableField.Value = imageFile;
                }
            }
        }

        /// <summary>
        /// P.O. box of address.
        /// </summary>
        public string PostOfficeBox {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.PostOfficeBox));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.PostOfficeBox), value);
            }
        }

        /// <summary>
        /// Enumerable of all presentable fields of this presentable
        /// object.
        /// </summary>
        protected PresentableFieldCollection presentableFields;

        /// <summary>
        /// Indicates whether to remove this object on update.
        /// </summary>
        public RemovalType RemoveOnUpdate { get; set; }

        /// <summary>
        /// State of address.
        /// </summary>
        public string State {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.State));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.State), value);
            }
        }

        /// <summary>
        /// Street and house number of address.
        /// </summary>
        public string Street {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.Street));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.Street), value);
            }
        }

        /// <summary>
        /// URL of web site.
        /// </summary>
        public string WebSite {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.WebSite));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.WebSite), value);
            }
        }

        /// <summary>
        /// Zip code of address.
        /// </summary>
        public string ZipCode {
            get {
                return this.GetValue(nameof(ActiveDirectoryContact.ZipCode));
            }
            set {
                this.SetValue(nameof(ActiveDirectoryContact.ZipCode), value);
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        internal ActiveDirectoryContact() {
            this.culture = null;
            this.presentableFields = new PresentableFieldCollection {
                new PresentableFieldForString(this, nameof(ActiveDirectoryContact.City)),
                new PresentableFieldForString(this, nameof(ActiveDirectoryContact.Company)),
                new PresentableFieldForString(this, nameof(ActiveDirectoryContact.CountryAlpha2Code)),
                new PresentableFieldForString(this, nameof(ActiveDirectoryContact.CountryName)),
                new PresentableFieldForNullableUShort(this, nameof(ActiveDirectoryContact.CountryNumericCode))
            };
            var createdAtField = new PresentableFieldForDateTime(this, nameof(ActiveDirectoryContact.CreatedAt)) {
                IsReadOnly = true
            };
            this.presentableFields.Add(createdAtField);
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.Department)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.Description)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.DisplayName)));
            var distinguishedNameField = new PresentableFieldForString(this, nameof(ActiveDirectoryContact.DistinguishedName)) {
                IsReadOnly = true
            };
            this.presentableFields.Add(distinguishedNameField);
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.EmailAddress)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.FaxNumber)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.FirstName)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.HomePhoneNumber)));
            var idField = new PresentableFieldForGuid(this, nameof(ActiveDirectoryContact.Id)) {
                IsReadOnly = true
            };
            this.presentableFields.Add(idField);
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.Initials)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.JobTitle)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.LastName)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.MobilePhoneNumber)));
            var modifiedAtField = new PresentableFieldForDateTime(this, nameof(ActiveDirectoryContact.ModifiedAt)) {
                IsReadOnly = true
            };
            this.presentableFields.Add(modifiedAtField);
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.Notes)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.Office)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.OfficePhoneNumber)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.PersonalTitle)));
            this.presentableFields.Add(new PresentableFieldForImageFile(this, nameof(ActiveDirectoryContact.Photo)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.PostOfficeBox)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.State)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.Street)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.WebSite)));
            this.presentableFields.Add(new PresentableFieldForString(this, nameof(ActiveDirectoryContact.ZipCode)));
            this.IsRetrieved = false;
        }

        /// <summary>
        /// Deep copies the state of another instance of this type
        /// into this instance, as far as all child objects implement
        /// ICopyable&lt;T&gt;.
        /// </summary>
        /// <param name="source">source instance to deep copy state
        /// from</param>
        public void CopyFrom(IPerson source) {
            this.CopyFrom(source, false);
            return;
        }

        /// <summary>
        /// Deep copies the state of another instance of this type
        /// into this instance, as far as all child objects implement
        /// ICopyable&lt;T&gt;.
        /// </summary>
        /// <param name="source">source instance to deep copy state
        /// from</param>
        /// <param name="includeId">true to copy ID of source as
        /// well, false otherwise</param>
        public void CopyFrom(IPerson source, bool includeId) {
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
            if (includeId) {
                this.Id = source.Id;
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
            if (null == source.MobilePhoneNumber) {
                this.MobilePhoneNumber = null;
            } else {
                this.MobilePhoneNumber = string.Copy(source.MobilePhoneNumber);
            }
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
            if (null == source.PostOfficeBox) {
                this.PostOfficeBox = null;
            } else {
                this.PostOfficeBox = string.Copy(source.PostOfficeBox);
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
            return;
        }

        /// <summary>
        /// Ensures that all properties of this user were retrieved
        /// from directory at least once.
        /// </summary>
        protected void EnsureRetrieval() {
            if (!this.IsRetrieved && !this.IsNew) {
                this.Retrieve();
            }
            return;
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
            this.EnsureRetrieval();
            return this.presentableFields.Find(keyChain);
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
        /// Gets the value of an Active Directory specific attribute.
        /// </summary>
        /// <param name="attributeName">name of Active Directory
        /// specific attribute to get value for</param>
        /// <returns>value of Active Directory specific attribute</returns>
        public virtual string GetAttributeValue(string attributeName) {
            var activeDirectoryContact = new ActiveDirectoryContact {
                Id = this.Id,
                ParentUserDirectory = this.ParentUserDirectory
            };
            if (!activeDirectoryContact.presentableFields.Contains(attributeName)) {
                activeDirectoryContact.presentableFields.Add(new PresentableFieldForString(activeDirectoryContact, attributeName));
            }
            return activeDirectoryContact.GetValue(attributeName);
        }

        /// <summary>
        /// Gets the title of providable object.
        /// </summary>
        /// <returns>title of providable object</returns>
        public string GetTitle() {
            return this.DisplayName;
        }

        /// <summary>
        /// Gets the value of a specific LDAP key for this user.
        /// </summary>
        /// <param name="key">LDAP key to get value for</param>
        /// <returns>value of LDAP key for this user</returns>
        protected string GetValue(string key) {
            var presentableField = this.presentableFields[key] as IPresentableFieldForElement;
            if (null == presentableField.ValueAsObject) {
                this.EnsureRetrieval();
            }
            return presentableField.ValueAsString;
        }

        /// <summary>
        /// Commits the values of this user to the directory entry of
        /// an Active Directory search result.
        /// </summary>
        /// <param name="searchResult">Active Directory search result
        /// to set values to</param>
        /// <param name="dictionary">dictionary of translations to
        /// apply</param>
        internal void GetValues(SearchResult searchResult, IDictionary<string, string> dictionary) {
            if (null == searchResult) {
                throw new PersistenceException("User does not exist in directory.");
            } else {
                try {
                    using (DirectoryEntry entry = searchResult.GetDirectoryEntry()) {
                        string cn = null;
                        Type typeOfImageFile = typeof(ImageFile);
                        foreach (IPresentableFieldForElement presentableField in this.presentableFields) {
                            if (!dictionary.TryGetValue(presentableField.Key, out string translatedKey)) {
                                translatedKey = presentableField.Key;
                            }
                            if (presentableField.ContentBaseType == typeOfImageFile) {
                                PropertyValueCollection property = entry.Properties[translatedKey];
                                var imageFile = presentableField.ValueAsObject as ImageFile;
                                ActiveDirectoryContact.GetValuesForImageFile(property, imageFile);
                            } else {
                                ActiveDirectoryContact.GetValuesForField(entry, presentableField, translatedKey);
                                if (nameof(ActiveDirectoryContact.DisplayName) == presentableField.Key) {
                                    cn = "CN=" + presentableField.ValueAsString.Replace(",", "\\,");
                                }
                            }
                        }
                        entry.CommitChanges();
                        if (!string.IsNullOrEmpty(cn)) {
                            string cnToUpperInvariant = cn.ToUpperInvariant();
                            if (cnToUpperInvariant != "CN=" && cnToUpperInvariant != entry.Name.ToUpperInvariant()) {
                                entry.Rename(cn);
                            }
                        }
                    }
                } catch (UnauthorizedAccessException exception) {
                    throw new DirectoryException(exception.Message, exception);
                }
                this.Retrieve();
            }
            return;
        }

        /// <summary>
        /// Commits the value of a field to a directory entry.
        /// </summary>
        /// <param name="property">property to set value for</param>
        /// <param name="imageFile">image file to get as value</param>
        private static void GetValuesForImageFile(PropertyValueCollection property, ImageFile imageFile) {
            if (null == imageFile || null == imageFile.Bytes) {
                property.Clear();
            } else {
                var propertyValue = property[0] as byte[];
                bool isValueChanged = null == propertyValue || propertyValue.LongLength != imageFile.Bytes.LongLength;
                if (!isValueChanged) {
                    for (long i = 0; i < propertyValue.LongLength; i++) {
                        if (propertyValue[i] != imageFile.Bytes[i]) {
                            isValueChanged = true;
                            break;
                        }
                    }
                }
                if (isValueChanged) {
                    const int sideLength = 96;
                    imageFile = imageFile.ToJpegImageFile(ResizeType.CropToSquare, sideLength);
                    property[0] = imageFile.Bytes;
                }
            }
            return;
        }

        /// <summary>
        /// Commits the value of a field to a directory entry.
        /// </summary>
        /// <param name="entry">entry to set value for</param>
        /// <param name="presentableField">presentable field to get
        /// value from</param>
        /// <param name="translatedKey">translated key of field</param>
        private static void GetValuesForField(DirectoryEntry entry, IPresentableFieldForElement presentableField, string translatedKey) {
            if (!presentableField.IsReadOnly) {
                bool isPresentableFieldKeyContainedInFieldNames = false;
                foreach (string fieldName in ActiveDirectoryUser.FieldNames) {
                    if (presentableField.Key == fieldName) {
                        isPresentableFieldKeyContainedInFieldNames = true;
                        break;
                    }
                }
                if (isPresentableFieldKeyContainedInFieldNames) {
                    PropertyValueCollection property = entry.Properties[translatedKey];
                    string value = presentableField.ValueAsString;
                    if (property.Count > 1) {
                        if (property[0].ToString() != value) {
                            property[0] = value;
                        }
                    } else if (1 == property.Count) {
                        if (string.IsNullOrEmpty(value)) {
                            property.Clear();
                        } else if (property[0].ToString() != value) {
                            property[0] = value;
                        }
                    } else { // 0 == property.Count
                        if (!string.IsNullOrEmpty(value)) {
                            property.Add(value);
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Retrieves all values of this user from directory. This
        /// can be used to refresh the values of this object.
        /// </summary>
        public virtual void Retrieve() {
            if (this.IsNew) {
                throw new ObjectNotPersistentException("Contact cannot be retrieved because it is not connected to a directory.");
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
        public virtual void SetNamePropertys() {
            string displayName = string.Empty;
            string firstName = this.FirstName;
            string lastName = this.LastName;
            if (UserDirectory.HasInvertedDisplayNameGeneration) {
                if (!string.IsNullOrEmpty(lastName)) {
                    displayName = lastName;
                }
                if (!string.IsNullOrEmpty(firstName)) {
                    if (!string.IsNullOrEmpty(displayName)) {
                        displayName += ", ";
                    }
                    displayName += firstName;
                }
            } else {
                if (!string.IsNullOrEmpty(firstName)) {
                    displayName = firstName;
                }
                if (!string.IsNullOrEmpty(lastName)) {
                    if (!string.IsNullOrEmpty(displayName)) {
                        displayName += " ";
                    }
                    displayName += lastName;
                }
            }
            this.SetValue(nameof(ActiveDirectoryContact.DisplayName), displayName);
            string initials = string.Empty;
            if (!string.IsNullOrEmpty(lastName)) {
                initials = lastName.Substring(0, 1);
            }
            if (!string.IsNullOrEmpty(firstName)) {
                initials = firstName.Substring(0, 1) + initials;
            }
            this.SetValue(nameof(ActiveDirectoryContact.Initials), initials);
            return;
        }

        /// <summary>
        /// Sets the value of a specific LDAP key for this user.
        /// </summary>
        /// <param name="key">LDAP key to set value for</param>
        /// <param name="value">value to set</param>
        protected void SetValue(string key, string value) {
            this.EnsureRetrieval();
            var presentableField = this.presentableFields[key] as IPresentableFieldForElement;
            presentableField.ValueAsString = value;
            return;
        }

        /// <summary>
        /// Sets the values of an Active Directory search result to
        /// this user.
        /// </summary>
        /// <param name="searchResult">Active Directory search result
        /// to get value from</param>
        /// <param name="propertiesToLoad">collection of properties
        /// to load from search result</param>
        /// <param name="dictionary">dictionary of translations to
        /// apply</param>
        internal void SetValues(SearchResult searchResult, StringCollection propertiesToLoad, IDictionary<string, string> dictionary) {
            if (null == searchResult) {
                throw new PersistenceException("User does not exist in directory.");
            } else {
                Type typeOfImageFile = typeof(ImageFile);
                foreach (string key in propertiesToLoad) {
                    if (!dictionary.TryGetValue(key, out string translatedKey)) {
                        translatedKey = key;
                    }
                    IPresentableFieldForElement presentableField;
                    if (this.presentableFields.Contains(translatedKey)) {
                        presentableField = this.presentableFields[translatedKey] as IPresentableFieldForElement;
                    } else {
                        presentableField = new PresentableFieldForString(this, translatedKey);
                        this.presentableFields.Add(presentableField);
                    }
                    if (searchResult.Properties[key].Count > 0) {
                        if (presentableField.ContentBaseType == TypeOf.Guid) {
                            byte[] value = searchResult.Properties[key][0] as byte[];
                            presentableField.ValueAsObject = new Guid(value);
                        } else if (presentableField.ContentBaseType == typeOfImageFile) {
                            var imageFile = new ImageFile {
                                Bytes = searchResult.Properties[key][0] as byte[],
                                Id = this.Id,
                                Name = key + ".jpg"
                            };
                            presentableField.ValueAsObject = imageFile;
                        } else {
                            presentableField.ValueAsString = searchResult.Properties[key][0].ToString();
                        }
                    } else {
                        presentableField.ValueAsString = string.Empty;
                    }
                }
            }
            return;
        }

    }

}