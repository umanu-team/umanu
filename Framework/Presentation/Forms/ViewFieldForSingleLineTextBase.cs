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

namespace Framework.Presentation.Forms {

    using Framework.Persistence.Fields;
    using Framework.Properties;

    /// <summary>
    /// Base field for single-line text to be presented in a view.
    /// </summary>
    public abstract class ViewFieldForSingleLineTextBase : ViewFieldForElementWithPlaceholder {

        /// <summary>
        /// Maximum number of characters of value.
        /// </summary>
        public ulong MaxLength {
            get { return this.maxLength.Value; }
            set { this.maxLength.Value = value; }
        }
        private readonly PersistentFieldForULong maxLength =
            new PersistentFieldForULong(nameof(MaxLength), ulong.MaxValue);

        /// <summary>
        /// Minimum number of characters of value.
        /// </summary>
        public ulong MinLength {
            get { return this.minLength.Value; }
            set { this.minLength.Value = value; }
        }
        private readonly PersistentFieldForULong minLength =
            new PersistentFieldForULong(nameof(MinLength), ulong.MinValue);

        /// <summary>
        /// Client action to execute on key down - may be null or
        /// empty.
        /// </summary>
        public string OnClientKeyDown {
            get { return this.onClientKeyDown.Value; }
            set { this.onClientKeyDown.Value = value; }
        }
        private readonly PersistentFieldForString onClientKeyDown =
            new PersistentFieldForString(nameof(OnClientKeyDown));

        /// <summary>
        /// Regular expression that must match for values of this
        /// field to be valid.
        /// </summary>
        public string ValidationPattern {
            get { return this.validationPattern.Value; }
            set { this.validationPattern.Value = value; }
        }
        private readonly PersistentFieldForString validationPattern =
            new PersistentFieldForString(nameof(ValidationPattern));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForSingleLineTextBase()
            : base() {
            this.RegisterPersistentField(this.maxLength);
            this.RegisterPersistentField(this.minLength);
            this.RegisterPersistentField(this.onClientKeyDown);
            this.RegisterPersistentField(this.validationPattern);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForSingleLineTextBase(string title, string key, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.Key = key;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForSingleLineTextBase(string title, string[] keyChain, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        private ViewFieldForSingleLineTextBase(string title, Mandatoriness mandatoriness)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
        }

        /// <summary>
        /// Creates a presentable field that can hold the value of
        /// view field.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of created field</param>
        /// <returns>presentable field that can hold the value of
        /// view field</returns>
        public override IPresentableFieldForElement CreatePresentableField(IPresentableObject parentPresentableObject) {
            return new PresentableFieldForString(parentPresentableObject, this.Key);
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage;
            if (ulong.MinValue == this.MinLength && ulong.MaxValue == this.MaxLength) {
                errorMessage = base.GetDefaultErrorMessage();
            } else {
                if (ulong.MinValue == this.MinLength) {
                    if (1 == this.MaxLength) {
                        errorMessage = Resources.PleaseEnterAValidValueWithAtMostOneCharacterForThisField;
                    } else {
                        errorMessage = string.Format(Resources.PleaseEnterAValidValueWithAtMost0CharactersForThisField, this.MaxLength);
                    }
                } else if (ulong.MaxValue == this.MaxLength) {
                    if (1 == this.MinLength) {
                        errorMessage = Resources.PleaseEnterAValidValueWithAtLeastOneCharacterForThisField;
                    } else {
                        errorMessage = string.Format(Resources.PleaseEnterAValidValueWithAtLeast0CharactersForThisField, this.MinLength);
                    }
                } else {
                    errorMessage = string.Format(Resources.PleaseEnterAValidValueWithAtLeast0AndAtMost1CharactersForThisField, this.MinLength, this.MaxLength);
                }
                errorMessage += " " + this.GetInfoMessageAboutManditoriness();
            }
            return errorMessage;
        }

        /// <summary>
        /// Returns null if the specified value is valid, an error
        /// message otherwise.
        /// </summary>
        /// <param name="presentableField">presentable field to be
        /// validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>null if the specified value is valid, error
        /// message otherwise</returns>
        public override string Validate(IPresentableFieldForElement presentableField, ValidityCheck validityCheck, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string errorMessage = base.Validate(presentableField, validityCheck, presentableObject, optionDataProvider);
            string value = presentableField.ValueAsString;
            if (string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(value)) {
                ulong valueLength = (ulong)value.Length;
                if (valueLength > this.MaxLength
                    || valueLength < this.MinLength
                    || (!string.IsNullOrEmpty(this.ValidationPattern) && !System.Text.RegularExpressions.Regex.IsMatch(value, this.ValidationPattern))) {
                    errorMessage = this.GetDefaultErrorMessage();
                }
            }
            return errorMessage;
        }

    }

}