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

    /// <summary>
    /// Unit of measurement.
    /// </summary>
    public class Unit : PersistentObject, IEquatable<Unit> {

        /// <summary>
        /// True if a values were set to this object, false if
        /// default values are set.
        /// </summary>
        public virtual bool HasValue {
            get {
                return null != this.BaseUnit && null != this.Symbol;
            }
        }

        /// <summary>
        /// True if this is a base unit, false otherwise.
        /// </summary>
        public bool IsBaseUnit {
            get {
                return this.Equals(this.BaseUnit);
            }
        }

        /// <summary>
        /// Base unit of this unit.
        /// </summary>
        public Unit BaseUnit {
            get { return this.baseUnit.Value; }
            private set { this.baseUnit.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<Unit> baseUnit =
            new PersistentFieldForPersistentObject<Unit>(nameof(BaseUnit), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Factor for converting a value of this unit into its base
        /// unit.
        /// </summary>
        public decimal ConversionFactor {
            get { return this.conversionFactor.Value; }
            private set { this.conversionFactor.Value = value; }
        }
        private readonly PersistentFieldForDecimal conversionFactor =
            new PersistentFieldForDecimal(nameof(ConversionFactor));

        /// <summary>
        /// Symbol of the unit.
        /// </summary>
        public string Symbol {
            get { return this.symbol.Value; }
            private set { this.symbol.Value = value; }
        }
        private readonly PersistentFieldForString symbol =
            new PersistentFieldForString(nameof(Symbol));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Unit()
            : base() {
            this.RegisterPersistentField(this.baseUnit);
            this.RegisterPersistentField(this.conversionFactor);
            this.RegisterPersistentField(this.symbol);
        }

        /// <summary>
        /// Instantiates a new instance for a base unit.
        /// </summary>
        /// <param name="symbol">symbol of the base unit</param>
        public Unit(string symbol)
            : this() {
            this.Symbol = symbol;
            this.ConversionFactor = 1m;
            this.BaseUnit = this;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="symbol">symbol of the unit</param>
        /// <param name="conversionFactor">factor for converting a
        /// value of this unit into its base unit</param>
        /// <param name="baseUnit">base unit of this unit</param>
        public Unit(string symbol, decimal conversionFactor, Unit baseUnit)
            : this(symbol) {
            this.ConversionFactor = conversionFactor;
            this.BaseUnit = baseUnit;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="unit">unit to copy - base unit is copied as
        /// shallow copy</param>
        protected Unit(Unit unit)
            : this(unit.Symbol, unit.ConversionFactor, unit.BaseUnit) {
            // nothing to do
        }

        /// <summary>
        /// Returns true the current unit is equal to another unit,
        /// false otherwise.
        /// </summary>
        /// <param name="other">other unit</param>
        /// <returns>true the current object is equal to another
        /// unit, false otherwise</returns>
        public virtual bool Equals(Unit other) {
            return this.BaseUnit.ConversionFactor == other.BaseUnit.ConversionFactor
                && this.BaseUnit.Symbol == other.BaseUnit.Symbol
                && this.ConversionFactor == other.ConversionFactor
                && this.Symbol == other.Symbol;
        }

        /// <summary>
        /// Converts the specified string representation of a unit to
        /// an instance of Unit.
        /// </summary>
        /// <param name="unit">string representation of unit</param>
        /// <returns>Unit equivalent to the string repesentation</returns>
        public static Unit Parse(string unit) {
            Unit result;
            if (null == unit) {
                throw new ArgumentNullException(nameof(unit));
            } else if (!Unit.TryParse(unit, out result)) {
                throw new FormatException("Unit \"" + unit + "\" cannot be interpreted.");
            }
            return result;
        }

        /// <summary>
        /// Converts the specified string representation of a unit to
        /// an instance of Unit.
        /// </summary>
        /// <param name="unit">string representation of unit</param>
        /// <param name="result">When this method returns, contains
        /// the Unit equivalent to the string repesentation, if the
        /// conversion succeeded, or null if the conversion failed.
        /// This parameter is passed uninitialized.</param>
        /// <returns>true if value was converted successfully;
        /// otherwise, false</returns>
        public static bool TryParse(string unit, out Unit result) {
            return AmountOfSubstance.TryParse(unit, out result)
                || ElectricCurrent.TryParse(unit, out result)
                || Length.TryParse(unit, out result)
                || LuminousIntensity.TryParse(unit, out result)
                || Mass.TryParse(unit, out result)
                || Memory.TryParse(unit, out result)
                || Temperature.TryParse(unit, out result)
                || Time.TryParse(unit, out result)
                || Volume.TryParse(unit, out result);
        }

    }

}