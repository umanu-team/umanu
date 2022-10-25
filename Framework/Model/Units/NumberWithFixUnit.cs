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

    using Framework.Persistence.Fields;
    using System;
    using System.Globalization;

    /// <summary>
    /// Combination of a decimal factor and a defined unit of
    /// measurement.
    /// </summary>
    public class NumberWithFixUnit : Unit, IEquatable<NumberWithFixUnit> {

        /// <summary>
        /// True if a value was set to this object, false if number
        /// and unit have their default values.
        /// </summary>
        public override bool HasValue {
            get {
                return base.HasValue && this.Number.HasValue;
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
        /// Value as decimal number in base unit.
        /// </summary>
        public decimal? NumberInBaseUnit {
            get {
                decimal? numberInBaseUnit;
                if (this.Number.HasValue) {
                    numberInBaseUnit = this.Number.Value * this.ConversionFactor;
                } else {
                    numberInBaseUnit = null;
                }
                return numberInBaseUnit;
            }
            set {
                if (value.HasValue) {
                    this.Number = value.Value / this.ConversionFactor;
                } else {
                    this.Number = null;
                }
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public NumberWithFixUnit()
            : base() {
            this.RegisterPersistentField(this.number);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="unit">unit of value</param>
        public NumberWithFixUnit(Unit unit)
            : base(unit) {
            this.RegisterPersistentField(this.number);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="number">value as decimal number</param>
        /// <param name="unit">unit of value</param>
        public NumberWithFixUnit(decimal? number, Unit unit)
            : this(unit) {
            this.Number = number;
        }

        /// <summary>
        /// Converts this value to a value of another unit.
        /// </summary>
        /// <param name="targetUnit">unit to convert value to</param>
        /// <returns>value converted to other unit</returns>
        public virtual NumberWithFixUnit ConvertToUnit(Unit targetUnit) {
            NumberWithFixUnit convertedValue;
            if (this.BaseUnit.Equals(targetUnit.BaseUnit)) {
                convertedValue = new NumberWithFixUnit(decimal.Zero, targetUnit);
                convertedValue.NumberInBaseUnit = this.NumberInBaseUnit;
            } else {
                throw new ArgumentException("Units with different base units cannot be converted to each other.", nameof(targetUnit));
            }
            return convertedValue;
        }

        /// <summary>
        /// Gets this value in another unit.
        /// </summary>
        /// <param name="targetUnit">unit to get value in</param>
        /// <returns>value in other unit</returns>
        public virtual decimal? ConvertValueToUnit(Unit targetUnit) {
            return this.ConvertToUnit(targetUnit).Number;
        }

        /// <summary>
        /// Returns true the current unit is equal to another number
        /// and unit, false otherwise.
        /// </summary>
        /// <param name="other">other number and unit</param>
        /// <returns>true the current object is equal to another
        /// number and unit, false otherwise</returns>
        public virtual bool Equals(NumberWithFixUnit other) {
            return this.Number == other.Number && base.Equals(other);
        }

        /// <summary>
        /// Converts the specified representation of a decimal number
        /// with a string unit to an instance of number with fix
        /// unit.
        /// </summary>
        /// <param name="numberWithAnyUnit">number with any unit</param>
        /// <returns>new instance of number with fix unit</returns>
        public static NumberWithFixUnit Parse(NumberWithAnyUnit numberWithAnyUnit) {
            NumberWithFixUnit result;
            if (null == numberWithAnyUnit || !numberWithAnyUnit.HasValue) {
                result = null;
            } else {
                result = NumberWithFixUnit.Parse(numberWithAnyUnit.Number, numberWithAnyUnit.Unit);
            }
            return result;
        }

        /// <summary>
        /// Converts the specified representation of a decimal number
        /// with a string unit to an instance of number with fix
        /// unit.
        /// </summary>
        /// <param name="number">value as decimal number</param>
        /// <param name="unit">string representation of unit</param>
        /// <returns>new instance of number with fix unit</returns>
        public static NumberWithFixUnit Parse(decimal? number, string unit) {
            return new NumberWithFixUnit(number, Unit.Parse(unit));
        }

        /// <summary>
        /// Sets this value to a value from another unit.
        /// </summary>
        /// <param name="value">value from another unit</param>
        public virtual void SetValueFromUnit(NumberWithFixUnit value) {
            this.Number = value.ConvertValueToUnit(this);
            return;
        }

        /// <summary>
        /// Sets this value to a value from another unit.
        /// </summary>
        /// <param name="number">number from another unit</param>
        /// <param name="sourceUnit">unit to get value from</param>
        public virtual void SetValueFromUnit(decimal? number, Unit sourceUnit) {
            this.SetValueFromUnit(new NumberWithFixUnit(number, sourceUnit));
            return;
        }

        /// <summary>
        /// Gets number and unit as a combined string.
        /// </summary>
        /// <returns>number and unit as combined string</returns>
        public override string ToString() {
            string value;
            if (this.HasValue) {
                value = this.NumberAsString + " " + this.Symbol;
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
                value = this.Number.Value.ToString(format) + " " + this.Symbol;
            } else {
                value = string.Empty;
            }
            return value;
        }

        /// <summary>
        /// Converts the specified representation of a decimal number
        /// with a string unit to an instance of number with fix
        /// unit.
        /// </summary>
        /// <param name="numberWithAnyUnit">number with any unit</param>
        /// <param name="result">When this method returns, contains
        /// a new instance of number with fix unit, if the conversion
        /// succeeded, or null if the conversion failed. This
        /// parameter is passed uninitialized.</param>
        /// <returns>true if value was converted successfully;
        /// otherwise, false</returns>
        public static bool TryParse(NumberWithAnyUnit numberWithAnyUnit, out NumberWithFixUnit result) {
            bool success;
            if (null == numberWithAnyUnit || !numberWithAnyUnit.HasValue) {
                result = null;
                success = false;
            } else {
                success = NumberWithFixUnit.TryParse(numberWithAnyUnit.Number, numberWithAnyUnit.Unit, out result);
            }
            return success;
        }

        /// <summary>
        /// Converts the specified representation of a decimal number
        /// with a string unit to an instance of number with fix
        /// unit.
        /// </summary>
        /// <param name="number">value as decimal number</param>
        /// <param name="unit">string representation of unit</param>
        /// <param name="result">When this method returns, contains
        /// a new instance of number with fix unit, if the conversion
        /// succeeded, or null if the conversion failed. This
        /// parameter is passed uninitialized.</param>
        /// <returns>true if value was converted successfully;
        /// otherwise, false</returns>
        public static bool TryParse(decimal? number, string unit, out NumberWithFixUnit result) {
            bool success;
            if (Unit.TryParse(unit, out Unit fixUnit)) {
                result = new NumberWithFixUnit(number, fixUnit);
                success = true;
            } else {
                result = null;
                success = false;
            }
            return success;
        }

    }

}