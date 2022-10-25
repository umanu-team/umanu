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
    using System;
    using System.Globalization;

    /// <summary>
    /// Field for a password to be presented in a view.
    /// </summary>
    public class ViewFieldForPassword : ViewFieldForSingleLineTextBase {

        /// <summary>
        /// Indicates whether different sets of charaters have to be
        /// contained.
        /// </summary>
        public bool IsCharacterVarianceRequired {
            get { return this.isCharacterVarianceRequired.Value; }
            set { this.isCharacterVarianceRequired.Value = value; }
        }
        private readonly PersistentFieldForBool isCharacterVarianceRequired =
            new PersistentFieldForBool(nameof(IsCharacterVarianceRequired), false);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForPassword()
            : base() {
            this.RegisterPersistentField(this.isCharacterVarianceRequired);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForPassword(string title, string key, Mandatoriness mandatoriness)
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
        public ViewFieldForPassword(string title, string[] keyChain, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        private ViewFieldForPassword(string title, Mandatoriness mandatoriness)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
        }

        /// <summary>
        /// Gets the read-only value of a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to get
        /// read-only value of</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to get read-only value of</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>read-only value of presentable field</returns>
        public override string GetReadOnlyValueFor(IPresentableFieldForElement presentableField, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string value;
            if (presentableField.ValueAsString.Length > 0) {
                value = Resources.Yes;
            } else {
                value = Resources.No;
            }
            return value;
        }

        /// <summary>
        /// Gets the value as object for a read-only value.
        /// </summary>
        /// <param name="readOnlyValue">read-only value to get value
        /// as object for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>value as object for read-only value</returns>
        public override object ParseReadOnlyValue(string readOnlyValue, IOptionDataProvider optionDataProvider) {
            throw new InvalidOperationException("Passwords must not be parsed.");
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
                if (this.IsCharacterVarianceRequired) {
                    const string upperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    const string lowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
                    const string numbers = "0123456789";
                    bool isUpperCaseLetterContained = false;
                    bool isLowerCaseLetterContained = false;
                    bool isNumberContained = false;
                    bool isOtherCharacterContained = false;
                    foreach (var c in value) {
                        string s = c.ToString(CultureInfo.InvariantCulture);
                        if (upperCaseLetters.Contains(s)) {
                            isUpperCaseLetterContained = true;
                        } else if (lowerCaseLetters.Contains(s)) {
                            isLowerCaseLetterContained = true;
                        } else if (numbers.Contains(s)) {
                            isNumberContained = true;
                        } else {
                            isOtherCharacterContained = true;
                        }
                    }
                    if (!isUpperCaseLetterContained || !isLowerCaseLetterContained || !isNumberContained || !isOtherCharacterContained) {
                        errorMessage = Resources.PleaseMakeSureTheValueContainsAtLeastAnUpperCaseCharacterALowerCaseCharacterANumericCharacterAndASpecialCharacter + " " + this.GetInfoMessageAboutManditoriness();
                    }
                }
            }
            return errorMessage;
        }

    }

}