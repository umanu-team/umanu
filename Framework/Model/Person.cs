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

namespace Framework.Model {

    using Framework.Persistence.Fields;
    using Persistence;
    using Persistence.Directories;
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Person with personal details.
    /// </summary>
    public class Person : PostalAddress, IPerson {

        /// <summary>
        /// Day of birth.
        /// </summary>
        public DateTime? Birthday {
            get { return this.birthday.Value; }
            set { this.birthday.Value = value; }
        }
        private readonly PersistentFieldForNullableDateTime birthday =
            new PersistentFieldForNullableDateTime(nameof(Birthday));

        /// <summary>
        /// Company name.
        /// </summary>
        public string Company {
            get { return this.company.Value; }
            set { this.company.Value = value; }
        }
        private readonly PersistentFieldForString company =
            new PersistentFieldForString(nameof(Company));

        /// <summary>
        /// Department of address.
        /// </summary>
        public string Department {
            get { return this.department.Value; }
            set { this.department.Value = value; }
        }
        private readonly PersistentFieldForString department =
            new PersistentFieldForString(nameof(Department));

        /// <summary>
        /// Short description of user.
        /// </summary>
        public string Description {
            get { return this.description.Value; }
            set { this.description.Value = value; }
        }
        private readonly PersistentFieldForString description =
            new PersistentFieldForString(nameof(Description));

        /// <summary>
        /// Name to be displayed.
        /// </summary>
        public string DisplayName {
            get { return this.displayName.Value; }
            set { this.displayName.Value = value; }
        }
        private readonly PersistentFieldForString displayName =
            new PersistentFieldForString(nameof(DisplayName));

        /// <summary>
        /// E-Mail address.
        /// </summary>
        public string EmailAddress {
            get { return this.emailAddress.Value; }
            set { this.emailAddress.Value = value; }
        }
        private readonly PersistentFieldForString emailAddress =
            new PersistentFieldForString(nameof(EmailAddress));

        /// <summary>
        /// Facsimile number.
        /// </summary>
        public string FaxNumber {
            get { return this.faxNumber.Value; }
            set { this.faxNumber.Value = value; }
        }
        private readonly PersistentFieldForString faxNumber =
            new PersistentFieldForString(nameof(FaxNumber));

        /// <summary>
        /// First name.
        /// </summary>
        public string FirstName {
            get { return this.firstName.Value; }
            set { this.firstName.Value = value; }
        }
        private readonly PersistentFieldForString firstName =
            new PersistentFieldForString(nameof(FirstName));

        /// <summary>
        /// Number of home phone.
        /// </summary>
        public string HomePhoneNumber {
            get { return this.homePhoneNumber.Value; }
            set { this.homePhoneNumber.Value = value; }
        }
        private readonly PersistentFieldForString homePhoneNumber =
            new PersistentFieldForString(nameof(HomePhoneNumber));

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
        /// Job title.
        /// </summary>
        public string JobTitle {
            get { return this.jobTitle.Value; }
            set { this.jobTitle.Value = value; }
        }
        private readonly PersistentFieldForString jobTitle =
            new PersistentFieldForString(nameof(JobTitle));

        /// <summary>
        /// Last name.
        /// </summary>
        public string LastName {
            get { return this.lastName.Value; }
            set { this.lastName.Value = value; }
        }
        private readonly PersistentFieldForString lastName =
            new PersistentFieldForString(nameof(LastName));

        /// <summary>
        /// Number of mobile phone.
        /// </summary>
        public string MobilePhoneNumber {
            get { return this.mobilePhoneNumber.Value; }
            set { this.mobilePhoneNumber.Value = value; }
        }
        private readonly PersistentFieldForString mobilePhoneNumber =
            new PersistentFieldForString(nameof(MobilePhoneNumber));

        /// <summary>
        /// "About me" notes.
        /// </summary>
        public string Notes {
            get { return this.notes.Value; }
            set { this.notes.Value = value; }
        }
        private readonly PersistentFieldForString notes =
            new PersistentFieldForString(nameof(Notes));

        /// <summary>
        /// Name of office building.
        /// </summary>
        public string Office {
            get { return this.office.Value; }
            set { this.office.Value = value; }
        }
        private readonly PersistentFieldForString office =
            new PersistentFieldForString(nameof(Office));

        /// <summary>
        /// Number of office phone.
        /// </summary>
        public string OfficePhoneNumber {
            get { return this.officePhoneNumber.Value; }
            set { this.officePhoneNumber.Value = value; }
        }
        private readonly PersistentFieldForString officePhoneNumber =
            new PersistentFieldForString(nameof(OfficePhoneNumber));

        /// <summary>
        /// Personal titles like academic titles or aristocratic
        /// titles.
        /// </summary>
        public string PersonalTitle {
            get { return this.personalTitle.Value; }
            set { this.personalTitle.Value = value; }
        }
        private readonly PersistentFieldForString personalTitle =
            new PersistentFieldForString(nameof(PersonalTitle));

        /// <summary>
        /// Thumbnail photo.
        /// </summary>
        public ImageFile Photo {
            get { return this.photo.Value; }
            set { this.photo.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<ImageFile> photo =
            new PersistentFieldForPersistentObject<ImageFile>(nameof(Photo), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// URL of web site.
        /// </summary>
        public string WebSite {
            get { return this.webSite.Value; }
            set { this.webSite.Value = value; }
        }
        private readonly PersistentFieldForString webSite =
            new PersistentFieldForString(nameof(WebSite));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Person()
            : base() {
            this.RegisterPersistentField(this.birthday);
            this.RegisterPersistentField(this.company);
            this.RegisterPersistentField(this.department);
            this.RegisterPersistentField(this.description);
            this.RegisterPersistentField(this.displayName);
            this.RegisterPersistentField(this.emailAddress);
            this.RegisterPersistentField(this.faxNumber);
            this.RegisterPersistentField(this.firstName);
            this.RegisterPersistentField(this.homePhoneNumber);
            this.RegisterPersistentField(this.jobTitle);
            this.RegisterPersistentField(this.lastName);
            this.RegisterPersistentField(this.mobilePhoneNumber);
            this.RegisterPersistentField(this.notes);
            this.RegisterPersistentField(this.office);
            this.RegisterPersistentField(this.officePhoneNumber);
            this.RegisterPersistentField(this.personalTitle);
            this.RegisterPersistentField(this.photo);
            this.RegisterPersistentField(this.webSite);
            this.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                if (nameof(Person.FirstName) == e.PropertyName || nameof(Person.LastName) == e.PropertyName) {
                    string displayName = string.Empty;
                    if (UserDirectory.HasInvertedDisplayNameGeneration) {
                        if (!string.IsNullOrEmpty(this.LastName)) {
                            displayName = this.LastName;
                        }
                        if (!string.IsNullOrEmpty(this.FirstName)) {
                            if (!string.IsNullOrEmpty(displayName)) {
                                displayName += ", ";
                            }
                            displayName += this.FirstName;
                        }
                    } else {
                        if (!string.IsNullOrEmpty(this.FirstName)) {
                            displayName = this.FirstName;
                        }
                        if (!string.IsNullOrEmpty(this.LastName)) {
                            if (!string.IsNullOrEmpty(displayName)) {
                                displayName += ' ';
                            }
                            displayName += this.LastName;
                        }
                    }
                    this.DisplayName = displayName;
                }
                return;
            };
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="displayName">name to be displayed</param>
        /// <param name="emailAddress">e-mail address</param>
        public Person(string displayName, string emailAddress)
            : this() {
            this.DisplayName = displayName;
            this.EmailAddress = emailAddress;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="firstName">first name</param>
        /// <param name="lastName">last name</param>
        /// <param name="emailAddress">e-mail address</param>
        public Person(string firstName, string lastName, string emailAddress)
            : this() {
            if (UserDirectory.HasInvertedDisplayNameGeneration) {
                this.DisplayName = lastName + ", " + firstName;
            } else {
                this.DisplayName = firstName + " " + lastName;
            }
            this.FirstName = firstName;
            this.LastName = lastName;
            this.EmailAddress = emailAddress;
        }

    }

}