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

    using Framework.Model.Units;
    using Framework.Persistence.Fields;
    using Framework.Properties;

    /// <summary>
    /// Field for a factor and a choice of units to be presented in a
    /// view. The connected data field must be an
    /// IPresentableFieldForElement&lt;NumberWithFixUnit&gt;.
    /// </summary>
    public class ViewFieldForNumberWithUnitChoice : ViewFieldForNumberWithoutUnit {

        /// <summary>
        /// Provider for suggested values.
        /// </summary>
        public OptionProvider UnitOptionProvider {
            get { return this.unitOptionProvider.Value; }
            set { this.unitOptionProvider.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<OptionProvider> unitOptionProvider =
            new PersistentFieldForPersistentObject<OptionProvider>(nameof(UnitOptionProvider), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForNumberWithUnitChoice()
            : base() {
            this.RegisterPersistentField(this.unitOptionProvider);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="step">allowed size of the steps between
        /// values - set this to 0 to allow any value or to 1 to
        /// allow integer values only</param>
        private ViewFieldForNumberWithUnitChoice(string title, Mandatoriness mandatoriness, decimal step)
            : this() {
            this.Title = title;
            this.Mandatoriness = mandatoriness;
            this.Step = step;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="step">allowed size of the steps between
        /// values - set this to 0 to allow any value or to 1 to
        /// allow integer values only</param>
        public ViewFieldForNumberWithUnitChoice(string title, string key, Mandatoriness mandatoriness, decimal step)
            : this(title, mandatoriness, step) {
            this.Key = key;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="step">allowed size of the steps between
        /// values - set this to 0 to allow any value or to 1 to
        /// allow integer values only</param>
        public ViewFieldForNumberWithUnitChoice(string title, string[] keyChain, Mandatoriness mandatoriness, decimal step)
            : this(title, mandatoriness, step) {
            this.KeyChain = keyChain;
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
            return new PresentableFieldForObject(parentPresentableObject, this.Key);
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage;
            if (decimal.MinValue == this.MinValue && decimal.MaxValue == this.MaxValue) {
                errorMessage = base.GetDefaultErrorMessage();
            } else {
                if (decimal.MinValue == this.MinValue) {
                    errorMessage = string.Format(Resources.PleaseEnterAValidValueLessThan0AndSelectAUnitForThisField, this.MaxValue);
                } else if (decimal.MaxValue == this.MaxValue) {
                    errorMessage = string.Format(Resources.PleaseEnterAValidValueGreaterThan0AndSelectAUnitForThisField, this.MinValue);
                } else {
                    errorMessage = string.Format(Resources.PleaseEnterAValidValueBetween0And1AndSelectAUnitForThisField, this.MinValue, this.MaxValue);
                }
                errorMessage += " " + this.GetInfoMessageAboutManditoriness();
            }
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
            string formattedValue;
            var numberWithUnit = presentableField.ValueAsObject as NumberWithAnyUnit;
            if (null == numberWithUnit || !numberWithUnit.HasValue) {
                formattedValue = null;
            } else {
                formattedValue = numberWithUnit.Number.Value.ToString(this.FormatString);
                if (!string.IsNullOrEmpty(formattedValue)) {
                    var unitString = this.UnitOptionProvider.FindReadOnlyValueForKey(numberWithUnit.Unit, presentableField.ParentPresentableObject, presentableObject, optionDataProvider);
                    if (!string.IsNullOrEmpty(unitString)) {
                        formattedValue += ' ' + unitString;
                    }
                }
            }
            return formattedValue;
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
            if (NumberWithAnyUnit.TryParse(readOnlyValue, out NumberWithAnyUnit value)) {
                value.Unit = this.UnitOptionProvider.FindKeyForValue(value.Unit, new PresentableObject(), new PresentableObject(), optionDataProvider);
            } else {
                value = null;
            }
            return value;
        }

        /// <summary>
        /// Returns null if the specified value is valid, an error message
        /// otherwise.
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
            string errorMessage = null;
            var numberWithUnit = presentableField.ValueAsObject as NumberWithAnyUnit;
            if (null == numberWithUnit) {
                errorMessage = this.GetDefaultErrorMessage();
            } else {
                string number = numberWithUnit.NumberAsString;
                var presentableFieldForElement = new PresentableFieldForString(presentableField.ParentPresentableObject, presentableField.Key, number);
                presentableFieldForElement.IsReadOnly = presentableField.IsReadOnly;
                errorMessage = base.Validate(presentableFieldForElement, validityCheck, presentableObject, optionDataProvider);
                bool isMandatory = Mandatoriness.Required == this.Mandatoriness || (ValidityCheck.Strict == validityCheck && Mandatoriness.Desired == this.Mandatoriness);
                if (string.IsNullOrEmpty(errorMessage) && (((isMandatory || !string.IsNullOrEmpty(number)) && string.IsNullOrEmpty(numberWithUnit.Unit))
                    || (!string.IsNullOrEmpty(numberWithUnit.Unit) && !this.UnitOptionProvider.ContainsKey(numberWithUnit.Unit, presentableField.ParentPresentableObject, presentableObject, optionDataProvider)))) {
                    errorMessage = this.GetDefaultErrorMessage();
                }
            }
            return errorMessage;
        }

    }

}