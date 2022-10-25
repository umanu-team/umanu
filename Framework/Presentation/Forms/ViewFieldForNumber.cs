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
    using System.Globalization;

    /// <summary>
    /// Field for a number to be presented in a view.
    /// </summary>
    public class ViewFieldForNumber : ViewFieldForNumberWithoutUnit {

        /// <summary>
        /// Unit label to display.
        /// </summary>
        public string Unit {
            get { return this.unit.Value; }
            set { this.unit.Value = value; }
        }
        private readonly PersistentFieldForString unit =
            new PersistentFieldForString(nameof(Unit));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForNumber()
            : base() {
            this.RegisterPersistentField(this.unit);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a value
        /// in this field is required</param>
        /// <param name="step">allowed size of the steps between
        /// values - set this to 0 to allow any value or to 1 to
        /// allow integer values only</param>
        public ViewFieldForNumber(string title, string key, Mandatoriness mandatoriness, decimal step)
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
        public ViewFieldForNumber(string title, string[] keyChain, Mandatoriness mandatoriness, decimal step)
            : this(title, mandatoriness, step) {
            this.KeyChain = keyChain;
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
        private ViewFieldForNumber(string title, Mandatoriness mandatoriness, decimal step)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
            this.Step = step;
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
            return new PresentableFieldForNullableDecimal(parentPresentableObject, this.Key);
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
            string readOnlyValue;
            if (long.TryParse(presentableField.ValueAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out long readOnlyLongValue)) {
                readOnlyValue = readOnlyLongValue.ToString(this.FormatString);
            } else if (decimal.TryParse(presentableField.ValueAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal readOnlyDecimalValue)) {
                readOnlyValue = readOnlyDecimalValue.ToString(this.FormatString);
            } else {
                readOnlyValue = string.Empty;
            }
            if (!string.IsNullOrEmpty(readOnlyValue) && !string.IsNullOrEmpty(this.Unit)) {
                if (!string.IsNullOrEmpty(this.Unit)) {
                    readOnlyValue += " " + this.Unit;
                }
            }
            return readOnlyValue;
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
            object value = null;
            string suffix;
            if (string.IsNullOrEmpty(this.Unit)) {
                suffix = string.Empty;
            } else {
                suffix = " " + this.Unit;
            }
            if (!string.IsNullOrEmpty(readOnlyValue) && readOnlyValue.EndsWith(suffix)) {
                var readOnlyValueWithoutSuffix = readOnlyValue.Substring(0, readOnlyValue.Length - suffix.Length);
                if (long.TryParse(readOnlyValueWithoutSuffix, out long readOnlyLongValue)) {
                    value = readOnlyLongValue;
                } else if (decimal.TryParse(readOnlyValueWithoutSuffix, out decimal readOnlyDecimalValue)) {
                    value = readOnlyDecimalValue;
                }
            }
            return value;
        }

    }

}