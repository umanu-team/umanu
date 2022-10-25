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

    using Framework.Presentation.Forms;
    using Persistence;
    using Presentation;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Mail;
    using System.Resources;
    using System.Text;

    /// <summary>
    /// Represents an e-mail template.
    /// </summary>
    public class EmailTemplate : ProvidableObject {

        /// <summary>
        /// Collection of mail attachments.
        /// </summary>
        public ICollection<File> Attachments {
            get { return this.attachments; }
        }
        private readonly PresentableFieldForCollection<File> attachments;

        /// <summary>
        /// Blind copy receivers.
        /// </summary>
        public ICollection<IPerson> Bcc {
            get { return this.bcc; }
        }
        private readonly PresentableFieldForEmailPersonCollection bcc;

        /// <summary>
        /// Mail addresses of blind copy receivers as string.
        /// </summary>
        public string BccAsString {
            get {
                return EmailTemplate.GetAddressLineFor(this.Bcc);
            }
        }

        /// <summary>
        /// Body text of e-mail.
        /// </summary>
        public string BodyText {
            get { return this.bodyText.Value; }
            set { this.bodyText.Value = value; }
        }
        private readonly PresentableFieldForString bodyText;

        /// <summary>
        /// Copy receivers.
        /// </summary>
        public ICollection<IPerson> Cc {
            get { return this.cc; }
        }
        private readonly PresentableFieldForEmailPersonCollection cc;

        /// <summary>
        /// Mail addresses of copy receivers as string.
        /// </summary>
        public string CcAsString {
            get {
                return EmailTemplate.GetAddressLineFor(this.Cc);
            }
        }

        /// <summary>
        /// Sender.
        /// </summary>
        public IPerson From {
            get { return this.from.Value; }
            set { this.from.Value = value; }
        }
        private readonly PresentableFieldForEmailPerson from;

        /// <summary>
        /// Sender mail address as string.
        /// </summary>
        public string FromAsString {
            get {
                return Email.GetMailAddressStringFor(this.From);
            }
        }

        /// <summary>
        /// Subject of this mail.
        /// </summary>
        public string Subject {
            get { return this.subject.Value; }
            set { this.subject.Value = value; }
        }
        private readonly PresentableFieldForString subject;

        /// <summary>
        /// Primary receivers.
        /// </summary>
        public ICollection<IPerson> To {
            get { return this.to; }
        }
        private readonly PresentableFieldForEmailPersonCollection to;

        /// <summary>
        /// Mail addresses of primary receivers as string.
        /// </summary>
        public string ToAsString {
            get {
                return EmailTemplate.GetAddressLineFor(this.To);
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public EmailTemplate() {
            this.attachments = new PresentableFieldForPersistentObjectCollection<File>(this, nameof(Attachments));
            this.AddPresentableField(this.attachments);
            this.bcc = new PresentableFieldForEmailPersonCollection(this, nameof(Bcc));
            this.AddPresentableField(this.bcc);
            this.bodyText = new PresentableFieldForString(this, nameof(BodyText));
            this.AddPresentableField(this.bodyText);
            this.cc = new PresentableFieldForEmailPersonCollection(this, nameof(Cc));
            this.AddPresentableField(this.cc);
            this.from = new PresentableFieldForEmailPerson(this, nameof(From));
            this.AddPresentableField(this.from);
            this.subject = new PresentableFieldForString(this, nameof(Subject));
            this.AddPresentableField(this.subject);
            this.to = new PresentableFieldForEmailPersonCollection(this, nameof(To));
            this.AddPresentableField(this.to);
        }

        /// <summary>
        /// Gets an e-mail address line for a list of persons.
        /// </summary>
        /// <param name="persons">persons to get e-mail address line
        /// for</param>
        /// <returns>e-mail address line for persons</returns>
        protected static string GetAddressLineFor(IEnumerable<IPerson> persons) {
            var addressLineBuilder = new StringBuilder();
            bool isFirst = true;
            foreach (var person in persons) {
                string mailAdress = EmailTemplate.GetMailAddressStringFor(person);
                if (!string.IsNullOrEmpty(mailAdress)) {
                    if (isFirst) {
                        isFirst = false;
                    } else {
                        addressLineBuilder.Append("; ");
                    }
                    addressLineBuilder.Append(mailAdress);
                }
            }
            return addressLineBuilder.ToString();
        }

        /// <summary>
        /// Gets a culture specific value.
        /// </summary>
        /// <param name="resourceSource">type of resource source</param>
        /// <param name="culture">culture to get value for</param>
        /// <param name="resourceName">name of resource in resource
        /// source</param>
        private static string GetCultureSpecificValue(Type resourceSource, CultureInfo culture, string resourceName) {
            string cultureSpecificValue;
            var resourceManager = new ResourceManager(resourceSource);
            using (var resourceSet = resourceManager.GetResourceSet(culture, true, true)) {
                cultureSpecificValue = resourceSet.GetString(resourceName);
                if (null == cultureSpecificValue) {
                    using (var invariantResourceSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true)) {
                        cultureSpecificValue = invariantResourceSet.GetString(resourceName);
                    }
                }
            }
            return cultureSpecificValue;
        }

        /// <summary>
        /// Gets the mail address of a person.
        /// </summary>
        /// <param name="person">person to get mail address for</param>
        /// <returns>mail address of person</returns>
        public static MailAddress GetMailAddressFor(IPerson person) {
            MailAddress mailAdress;
            if (null == person || string.IsNullOrEmpty(person.EmailAddress)) {
                mailAdress = null;
            } else {
                string displayName = person.DisplayName;
                if (string.IsNullOrEmpty(displayName)) {
                    bool isFirstNameSet = !string.IsNullOrEmpty(person.FirstName);
                    bool isLastNameSet = !string.IsNullOrEmpty(person.LastName);
                    if (isFirstNameSet && !isLastNameSet) {
                        displayName = person.FirstName;
                    } else if (!isFirstNameSet && isLastNameSet) {
                        displayName = person.LastName;
                    } else if (isFirstNameSet && isLastNameSet) {
                        displayName = person.FirstName + " " + person.LastName;
                    }
                }
                if (string.IsNullOrEmpty(displayName)) {
                    mailAdress = new MailAddress(person.EmailAddress);
                } else {
                    mailAdress = new MailAddress(person.EmailAddress, displayName);
                }
            }
            return mailAdress;
        }

        /// <summary>
        /// Gets the mail address of a person as string.
        /// </summary>
        /// <param name="person">person to get mail address for</param>
        /// <returns>mail address of person as string</returns>
        public static string GetMailAddressStringFor(IPerson person) {
            string mailAddressAsString;
            MailAddress mailAdress = EmailTemplate.GetMailAddressFor(person);
            if (null == mailAdress) {
                mailAddressAsString = string.Empty;
            } else {
                mailAddressAsString = mailAdress.ToString();
            }
            return mailAddressAsString;
        }

        /// <summary>
        /// Replaces placeholders in a text like [Sender.FirstName]
        /// or [Receiver.LastName] by the actual values of a user.
        /// Possible values are defined in
        /// Framework.Persistence.Directories.Field plus
        /// &quot;CountryName&quot;.
        /// </summary>
        public void ReplacePlaceholders() {
            if (null != this.From) {
                if (null != this.Subject) {
                    this.Subject = EmailTemplate.ReplacePlaceholders(this.Subject, this.From, "Sender");
                }
                if (null != this.BodyText) {
                    this.BodyText = EmailTemplate.ReplacePlaceholders(this.BodyText, this.From, "Sender");
                }
            }
            if (1 == this.To.Count) {
                foreach (var to in this.To) {
                    if (null != to) {
                        if (null != this.Subject) {
                            this.Subject = EmailTemplate.ReplacePlaceholders(this.Subject, to, "Receiver");
                        }
                        if (null != this.BodyText) {
                            this.BodyText = EmailTemplate.ReplacePlaceholders(this.BodyText, to, "Receiver");
                        }
                        break;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Replaces placeholders in a text like [Sender.FirstName]
        /// or [Receiver.LastName] by the actual values of a user.
        /// </summary>
        /// <param name="text">text to replace placeholders in</param>
        /// <param name="person">person to pull new values from</param>
        /// <param name="placeholderPrefix">prefix of placeholder,
        /// e.g. "Sender" or "Receiver"</param>
        /// <returns>text with replaced placeholders</returns>
        private static string ReplacePlaceholders(string text, IPerson person, string placeholderPrefix) {
            text = EmailTemplate.ReplacePlaceholdersForCountryName(text, person, placeholderPrefix);
            var regex = new System.Text.RegularExpressions.Regex("\\[" + placeholderPrefix + "\\.([^\\]]+)\\]");
            var match = regex.Match(text);
            while (match.Success && 2 == match.Groups.Count) {
                string fieldKey = match.Groups[1].Value;
                var presentableField = person.FindPresentableField(fieldKey);
                if (null != presentableField) {
                    string value = null;
                    if (presentableField.IsForSingleElement) {
                        value = (presentableField as IPresentableFieldForElement).ValueAsString;
                    } else {
                        var presentableFieldForCollection = presentableField as IPresentableFieldForCollection;
                        foreach (var fieldValue in presentableFieldForCollection.GetValuesAsString()) {
                            value = fieldValue;
                            break;
                        }
                    }
                    if (null == value) {
                        value = string.Empty;
                    }
                    text = text.Replace(match.Value, value);
                }
                match = match.NextMatch();
            }
            return text;
        }

        /// <summary>
        /// Replaces the placeholder [PlaceholderPrefix.CountryName]
        /// by the actual value of a user.
        /// </summary>
        /// <param name="text">text to replace placeholder in</param>
        /// <param name="person">person to pull new value from</param>
        /// <param name="placeholderPrefix">prefix of placeholder,
        /// e.g. "Sender" or "Receiver"</param>
        /// <returns>text with replaced placeholder</returns>
        private static string ReplacePlaceholdersForCountryName(string text, IPerson person, string placeholderPrefix) {
            Country country;
            if (null == person.CountryAlpha2Code) {
                country = null;
            } else {
                country = Country.GetByAlpha2Code(person.CountryAlpha2Code);
            }
            string countryName;
            if (null == country) {
                countryName = string.Empty;
            } else {
                countryName = country.Name;
            }
            text = text.Replace("[" + placeholderPrefix + ".CountryName]", countryName);
            return text;
        }

        /// <summary>
        /// Sets a culture sprecific body text.
        /// </summary>
        /// <param name="resourceSource">type of resource source</param>
        /// <param name="culture">culture to set body text for</param>
        /// <param name="resourceName">name of resource in resource
        /// source</param>
        public void SetCultureSpecificBodyText(Type resourceSource, CultureInfo culture, string resourceName) {
            this.BodyText = EmailTemplate.GetCultureSpecificValue(resourceSource, culture, resourceName);
            return;
        }

        /// <summary>
        /// Sets a culture specific subject.
        /// </summary>
        /// <param name="resourceSource">type of resource source</param>
        /// <param name="culture">culture to set subject for</param>
        /// <param name="resourceName">name of resource in resource
        /// source</param>
        public void SetCultureSpecificSubject(Type resourceSource, CultureInfo culture, string resourceName) {
            this.Subject = EmailTemplate.GetCultureSpecificValue(resourceSource, culture, resourceName);
            return;
        }

    }

}