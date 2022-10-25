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

namespace Framework.Persistence.Fields {

    using System;
    using System.Data.Common;
    using System.Globalization;

    /// <summary>
    /// Wrapper class for a field of type nullable short to be stored
    /// in persistence mechanism.
    /// </summary>
    public sealed class PersistentFieldForNullableShort : PersistentFieldForElement<short?> {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.NullableShort;
            }
        }

        /// <summary>
        /// Value loaded from DbDataReader.
        /// </summary>
        private short? valueFromDbDataReader;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        public PersistentFieldForNullableShort(string key)
            : base(key) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        /// <param name="value">value of persistent field</param>
        public PersistentFieldForNullableShort(string key, short? value)
            : this(key) {
            this.Value = value;
        }

        /// <summary>
        /// Copies the value of persistent field.
        /// </summary>
        /// <returns>copy of value of persistent field</returns>
        internal override object CopyValue() {
            short? copy;
            if (this.Value.HasValue) {
                copy = this.Value.Value;
            } else {
                copy = null;
            }
            return copy;
        }

        /// <summary>
        /// Converts the value of this field to a string value.
        /// </summary>
        /// <returns>value of this field as string</returns>
        protected internal override string GetValueAsString() {
            string value;
            if (this.Value.HasValue) {
                value = this.Value.Value.ToString(CultureInfo.InvariantCulture);
            } else {
                value = string.Empty;
            }
            return value;
        }

        /// <summary>
        /// Loads a new value for persistent field from a specified
        /// index of a DbDataReader, but does not set the value yet.
        /// </summary>
        /// <param name="dataReader">data reader to load value from</param>
        /// <param name="ordinal">index of data reader to load value
        /// from</param>
        internal override void LoadValueFromDbDataReader(DbDataReader dataReader, int ordinal) {
            if (dataReader.IsDBNull(ordinal)) {
                this.valueFromDbDataReader = null;
            } else {
                this.valueFromDbDataReader = dataReader.GetInt16(ordinal);
            }
            return;
        }

        /// <summary>
        /// Sets the new value for persistent field which has been
        /// loaded from DbDataReader before.
        /// </summary>
        internal override void SetValueFromDbDataReader() {
            this.SetValueUnsafe(this.valueFromDbDataReader);
            this.valueFromDbDataReader = null;
            return;
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of value of this field and sets it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">new value to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public override bool TrySetValueAsString(string value) {
            bool success;
            if (string.IsNullOrEmpty(value)) {
                this.Value = null;
                success = true;
            } else {
                success = short.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out short parsedValue);
                this.Value = parsedValue;
            }
            return success;
        }

        /// <summary>
        /// Determines whether two value are different.
        /// </summary>
        /// <param name="x">first value to compare</param>
        /// <param name="y">second value to compare</param>
        /// <returns>true if the specified values are different,
        /// false otherwise</returns>
        protected override bool ValuesAreDifferent(short? x, short? y) {
            return !x.Equals(y);
        }

    }

}