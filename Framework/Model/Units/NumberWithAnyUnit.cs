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

namespace Framework.Model.Units {

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using System;
    using System.Globalization;

    /// <summary>
    /// Combination of a decimal factor and any unit of measurement.
    /// </summary>
    public class NumberWithAnyUnit : PersistentObject, IEquatable<NumberWithAnyUnit> {

        /// <summary>
        /// True if a value was set to this object, false if number
        /// and unit have their default values.
        /// </summary>
        public bool HasValue {
            get {
                return this.Number.HasValue && !string.IsNullOrEmpty(this.Unit);
            }
        }

        /// <summary>
        /// Value as decimal number.
        /// </summary>
        public decimal? Number {
            get { return this.number.Value; }
            set { this.number.Value = value; }
        }
        private readonly PersistentFieldForNullableDecimal number =
            new PersistentFieldForNullableDecimal(nameof(Number));

        /// <summary>
        /// Value as decimal number as string.
        /// </summary>
        public string NumberAsString {
            get {
                string number;
                if (this.Number.HasValue) {
                    number = this.Number.Value.ToString(CultureInfo.InvariantCulture);
                } else {
                    number = string.Empty;
                }
                return number;
            }
        }

        /// <summary>
        /// Unit of value.
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
        public NumberWithAnyUnit()
            : base() {
            this.RegisterPersistentField(this.number);
            this.RegisterPersistentField(this.unit);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="number">value as decimal number</param>
        /// <param name="unit">unit of value</param>
        public NumberWithAnyUnit(decimal? number, string unit)
            : this() {
            this.Number = number;
            this.Unit = unit;
        }

        /// <summary>
        /// Clears number and unit.
        /// </summary>
        public void Clear() {
            this.Number = null;
            this.Unit = null;
            return;
        }

        /// <summary>
        /// Determines whether the current object is equal to another
        /// object of the same type.
        /// </summary>
        /// <param name="other">object to compare this object to</param>
        /// <returns>true if both objects are equal, false otherwise</returns>
        public bool Equals(NumberWithAnyUnit other) {
            return this.Number == other.Number && this.Unit == other.Unit;
        }

        /// <summary>
        /// Splits number and unit of a string representation of a
        /// number with a unit.
        /// </summary>
        /// <param name="value">string representation of a number
        /// with a unit</param>
        /// <param name="number">number part of string representation</param>
        /// <param name="unit">unit part of string representation</param>
        private static void SplitNumberAndUnit(string value, out string number, out string unit) {
            number = string.Empty;
            unit = string.Empty;
            if (!string.IsNullOrEmpty(value)) {
                string[] values = value.Trim().Split(new char[] { ' ' }, 2);
                if (values.LongLength > 0) {
                    number = values[0];
                }
                if (values.LongLength > 1) {
                    unit = values[1];
                }
            }
            return;
        }

        /// <summary>
        /// Gets number and unit as a combined string.
        /// </summary>
        /// <returns>number and unit as combined string</returns>
        public override string ToString() {
            string value;
            if (this.HasValue) {
                value = this.NumberAsString + " " + this.Unit;
            } else {
                value = string.Empty;
            }
            return value;
        }

        /// <summary>
        /// Gets number and unit as a combined string.
        /// </summary>
        /// <param name="format">standard or custom numeric format
        /// string for number</param>
        /// <returns>number and unit as combined string</returns>
        public string ToString(string format) {
            string value;
            if (this.HasValue) {
                value = this.Number.Value.ToString(format) + " " + this.Unit;
            } else {
                value = string.Empty;
            }
            return value;
        }

        /// <summary>
        /// Converts the specified string representation of a number
        /// with unit to an instance of NumberWithAnyUnit.
        /// </summary>
        /// <param name="value">string representation of unit</param>
        /// <param name="result">When this method returns, contains
        /// the NumberWithAnyUnit equivalent to the string
        /// repesentation, if the conversion succeeded, or null if
        /// the conversion failed. This parameter is passed
        /// uninitialized.</param>
        /// <returns>true if value was converted successfully;
        /// otherwise, false</returns>
        public static bool TryParse(string value, out NumberWithAnyUnit result) {
            NumberWithAnyUnit.SplitNumberAndUnit(value, out string numberString, out string unit);
            bool success = decimal.TryParse(numberString, out decimal parsedNumber) && !string.IsNullOrEmpty(unit);
            if (success) {
                result = new NumberWithAnyUnit(parsedNumber, unit);
            } else {
                result = null;
            }
            return success;
        }

        /// <summary>
        /// Converts the string representation of a number with a
        /// unit into the value of this object. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">new number with unit to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public virtual bool TrySetValueAsString(string value) {
            NumberWithAnyUnit.SplitNumberAndUnit(value, out string number, out string unit);
            return this.TrySetValueAsString(number, unit);
        }

        /// <summary>
        /// Converts the string representation of a number with a
        /// unit into the value of this object. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="number">new number to be set</param>
        /// <param name="unit">new unit to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public virtual bool TrySetValueAsString(string number, string unit) {
            return this.number.TrySetValueAsString(number) && this.unit.TrySetValueAsString(unit);
        }

    }

}