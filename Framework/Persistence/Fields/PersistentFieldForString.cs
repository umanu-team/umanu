﻿/*********************************************************************
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

    using Framework.Presentation.Forms;
    using System;
    using System.Data.Common;

    /// <summary>
    /// Wrapper class for a field of type string to be stored in
    /// persistence mechanism.
    /// </summary>
    public sealed class PersistentFieldForString : PersistentFieldForElement<string> {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.String;
            }
        }

        /// <summary>
        /// Value loaded from DbDataReader.
        /// </summary>
        private string valueFromDbDataReader;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        public PersistentFieldForString(string key)
            : base(key) {
            this.IsFullTextIndexed = true;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        /// <param name="value">value of persistent field</param>
        public PersistentFieldForString(string key, string value)
            : this(key) {
            this.Value = value;
        }

        /// <summary>
        /// Copies the value of persistent field.
        /// </summary>
        /// <returns>copy of value of persistent field</returns>
        internal override object CopyValue() {
            string copy;
            if (null == this.Value) {
                copy = null;
            } else {
                copy = string.Copy(this.Value);
            }
            return copy;
        }

        /// <summary>
        /// Gets the value of this presentable field as plain text.
        /// As an example, this is used for full-text indexing.
        /// </summary>
        /// <returns>value of this presentable field as plain text</returns>
        public override string GetValueAsPlainText() {
            string value = base.GetValueAsPlainText();
            if (!string.IsNullOrEmpty(value)) {
                value = XmlUtility.RemoveTags(value);
            }
            return value;
        }

        /// <summary>
        /// Converts the value of this field to a string value.
        /// </summary>
        /// <returns>value of this field as string</returns>
        protected internal override string GetValueAsString() {
            string value;
            if (null == this.Value) {
                value = string.Empty;
            } else {
                value = this.Value;
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
                this.valueFromDbDataReader = dataReader.GetString(ordinal);
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
            this.Value = value;
            return true;
        }

        /// <summary>
        /// Determines whether two value are different.
        /// </summary>
        /// <param name="x">first value to compare</param>
        /// <param name="y">second value to compare</param>
        /// <returns>true if the specified values are different,
        /// false otherwise</returns>
        protected override bool ValuesAreDifferent(string x, string y) {
            return x != y;
        }

    }

}
