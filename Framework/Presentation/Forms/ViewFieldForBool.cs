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

    using Framework.Properties;

    /// <summary>
    /// Field for boolean value to be presented in a view.
    /// </summary>
    public class ViewFieldForBool : ViewFieldForElement {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForBool()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForBool(string title, string key, Mandatoriness mandatoriness)
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
        public ViewFieldForBool(string title, string[] keyChain, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        private ViewFieldForBool(string title, Mandatoriness mandatoriness)
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
            return new PresentableFieldForNullableBool(parentPresentableObject, this.Key);
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage = Resources.PleaseSelectAValidValueForThisField;
            errorMessage += " " + this.GetInfoMessageAboutManditoriness();
            return errorMessage;
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
            if (bool.TrueString == presentableField.ValueAsString) {
                value = Resources.Yes;
            } else if (bool.FalseString == presentableField.ValueAsString) {
                value = Resources.No;
            } else {
                value = string.Empty;
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
            bool? value;
            if (Resources.Yes == readOnlyValue) {
                value = true;
            } else if (Resources.No == readOnlyValue) {
                value = false;
            } else {
                value = null;
            }
            return value;
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
                if (!bool.TryParse(value, out bool valueAsBool)) {
                    errorMessage = this.GetDefaultErrorMessage();
                }
            }
            return errorMessage;
        }

    }

}
