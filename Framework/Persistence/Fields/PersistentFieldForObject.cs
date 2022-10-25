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

    /// <summary>
    /// Wrapper class for a field of type object to be stored in
    /// persistence mechanism (for internal use only - does not fire
    /// any event handlers).
    /// </summary>
    internal sealed class PersistentFieldForObject : PersistentFieldForElement {

        /// <summary>
        /// Type of value of this persistent field.
        /// </summary>
        private Type contentType;

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return this.contentType;
            }
        }

        /// <summary>
        /// Value of this persistent field as object.
        /// </summary>
        public override object ValueAsObject { get; set; }

        /// <summary>
        /// Value loaded from DbDataReader.
        /// </summary>
        private object valueFromDbDataReader;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        /// <param name="type">type of value of this persistent field</param>
        public PersistentFieldForObject(string key, Type type)
            : base(key) {
            this.contentType = type;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        /// <param name="type">type of value of this persistent field</param>
        /// <param name="value">value of persistent field</param>
        public PersistentFieldForObject(string key, Type type, object value)
            : this(key, type) {
            this.ValueAsObject = value;
        }

        /// <summary>
        /// Copies the value of persistent field.
        /// </summary>
        /// <returns>copy of value of persistent field</returns>
        internal override object CopyValue() {
            throw new InvalidOperationException("Value of persistent field for object may not be copied this way.");
        }

        /// <summary>
        /// Converts the value of this field to a string value.
        /// </summary>
        /// <returns>value of this field as string</returns>
        internal protected override string GetValueAsString() {
            string value;
            if (null == this.ValueAsObject) {
                value = string.Empty;
            } else {
                value = this.ValueAsObject.ToString();
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
            this.valueFromDbDataReader = dataReader.GetValue(ordinal);
            return;
        }

        /// <summary>
        /// Creates a new item that could be set to this field.
        /// </summary>
        /// <returns>new item that could be set to this field</returns>
        public override object NewItemAsObject() {
            return Activator.CreateInstance(this.contentType);
        }

        /// <summary>
        /// Sets the new value for persistent field which has been
        /// loaded from DbDataReader before.
        /// </summary>
        internal override void SetValueFromDbDataReader() {
            this.ValueAsObject = this.valueFromDbDataReader;
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
            return false;
        }

    }

}