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
    using Properties;
    using System;

    /// <summary>
    /// Field for dimensions to be presented in a view. The connected
    /// data field must be an
    /// IPresentableFieldForElement&lt;Dimensions&gt;.
    /// </summary>
    public class ViewFieldForDimensions : ViewFieldForNumber {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForDimensions()
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
        /// <param name="step">allowed size of the steps between
        /// values - set this to 0 to allow any value or to 1 to
        /// allow integer values only</param>
        public ViewFieldForDimensions(string title, string key, Mandatoriness mandatoriness, decimal step)
            : this() {
            this.Key = key;
            this.Mandatoriness = mandatoriness;
            this.Step = step;
            this.Title = title;
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
        public ViewFieldForDimensions(string title, string[] keyChain, Mandatoriness mandatoriness, decimal step)
            : this() {
            this.KeyChain = keyChain;
            this.Mandatoriness = mandatoriness;
            this.Step = step;
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
                    errorMessage = string.Format(Resources.PleaseEnterAValidValueLessThan0ForThisField, this.MaxValue);
                } else if (decimal.MaxValue == this.MaxValue) {
                    errorMessage = string.Format(Resources.PleaseEnterAValidValueGreaterThan0ForThisField, this.MinValue);
                } else {
                    errorMessage = string.Format(Resources.PleaseEnterAValidValueBetween0And1ForThisField, this.MinValue, this.MaxValue);
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
            string value;
            var dimensions = presentableField.ValueAsObject as Dimensions;
            if (null == dimensions || (!dimensions.Length.HasValue && !dimensions.Width.HasValue && !dimensions.Height.HasValue)) {
                value = null;
            } else {
                string length;
                if (dimensions.Length.HasValue) {
                    length = dimensions.Length.Value.ToString(this.FormatString);
                } else {
                    length = "?";
                }
                string width;
                if (dimensions.Width.HasValue) {
                    width = dimensions.Width.Value.ToString(this.FormatString);
                } else {
                    width = "?";
                }
                string height;
                if (dimensions.Height.HasValue) {
                    height = dimensions.Height.Value.ToString(this.FormatString);
                } else {
                    height = "?";
                }
                value = length + " x " + width + " x " + height;
                if (!string.IsNullOrEmpty(this.Unit)) {
                    value += " " + this.Unit;
                }
                value += " " + Resources.LengthXWidthXHeightAbbr;
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
            Dimensions value = null;
            string suffix = " " + Resources.LengthXWidthXHeightAbbr;
            if (!string.IsNullOrEmpty(this.Unit)) {
                suffix = " " + this.Unit + suffix;
            }
            if (!string.IsNullOrEmpty(readOnlyValue) && readOnlyValue.EndsWith(suffix)) {
                var readOnlyValueWithoutSuffix = readOnlyValue.Substring(0, readOnlyValue.Length - suffix.Length);
                var splitReadOnlyValue = readOnlyValueWithoutSuffix.Split(new string[] { " x " }, StringSplitOptions.None);
                if (3L == splitReadOnlyValue.LongLength) {
                    value = new Dimensions();
                    if (decimal.TryParse(splitReadOnlyValue[0], out decimal length)) {
                        value.Length = length;
                    }
                    if (decimal.TryParse(splitReadOnlyValue[1], out decimal width)) {
                        value.Width = width;
                    }
                    if (decimal.TryParse(splitReadOnlyValue[2], out decimal height)) {
                        value.Height = height;
                    }
                }
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
            string errorMessage = null;
            var viewFieldForElement = new ViewFieldForNumber(this.Key, this.Title, this.Mandatoriness, this.Step);
            viewFieldForElement.DescriptionForEditMode = this.DescriptionForEditMode;
            viewFieldForElement.DescriptionForViewMode = this.DescriptionForViewMode;
            viewFieldForElement.IsRange = this.IsRange;
            viewFieldForElement.MaxValue = this.MaxValue;
            viewFieldForElement.MinValue = this.MinValue;
            viewFieldForElement.OptionProvider = this.OptionProvider;
            viewFieldForElement.Placeholder = this.Placeholder;
            var dimensions = presentableField.ValueAsObject as Dimensions;
            foreach (var fieldValue in new string[] { dimensions.LengthAsString, dimensions.WidthAsString, dimensions.HeightAsString }) {
                var presentableFieldForElement = new PresentableFieldForString(presentableField.ParentPresentableObject, presentableField.Key, fieldValue);
                presentableFieldForElement.IsReadOnly = presentableField.IsReadOnly;
                errorMessage = viewFieldForElement.Validate(presentableFieldForElement, validityCheck, presentableObject, optionDataProvider);
                if (!string.IsNullOrEmpty(errorMessage)) {
                    break;
                }
            }
            return errorMessage;
        }

    }

}