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

    /// <summary>
    /// Field for single elements to be presented in a view.
    /// </summary>
    public abstract class ViewFieldForElement : ViewFieldForEditableValue {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForElement()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Creates a presentable field that can hold the value of
        /// view field.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of created field</param>
        /// <returns>presentable field that can hold the value of
        /// view field</returns>
        public abstract IPresentableFieldForElement CreatePresentableField(IPresentableObject parentPresentableObject);

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
        public sealed override string GetReadOnlyValueFor(IPresentableField presentableField, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            return this.GetReadOnlyValueFor(presentableField as IPresentableFieldForElement, presentableObject, optionDataProvider);
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
        public virtual string GetReadOnlyValueFor(IPresentableFieldForElement presentableField, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            return presentableField.ValueAsString;
        }

        /// <summary>
        /// Gets the value as object for a read-only value.
        /// </summary>
        /// <param name="readOnlyValue">read-only value to get value
        /// as object for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>value as object for read-only value</returns>
        public virtual object ParseReadOnlyValue(string readOnlyValue, IOptionDataProvider optionDataProvider) {
            var presentableField = this.CreatePresentableField(new PresentableObject());
            presentableField.TrySetValueAsString(readOnlyValue);
            return presentableField.ValueAsObject;
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
        public virtual string Validate(IPresentableFieldForElement presentableField, ValidityCheck validityCheck, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string errorMessage = null;
            bool isMandatory = Mandatoriness.Required == this.Mandatoriness || (ValidityCheck.Strict == validityCheck && Mandatoriness.Desired == this.Mandatoriness);
            if (isMandatory && string.IsNullOrEmpty(presentableField.ValueAsString)) {
                errorMessage = this.GetDefaultErrorMessage();
            }
            return errorMessage;
        }

    }

}
