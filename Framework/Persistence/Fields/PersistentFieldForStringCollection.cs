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

    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// List of objects of type string that can be stored in
    /// persistence mechanism.
    /// </summary>
    public sealed class PersistentFieldForStringCollection : PersistentFieldForCollection<string> {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.String;
            }
        }

        /// <summary>
        /// Values loaded from DbDataReader.
        /// </summary>
        private List<string> valuesFromDbDataReader;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        public PersistentFieldForStringCollection(string key)
            : base(key) {
            this.IsFullTextIndexed = true;
        }

        /// <summary>
        /// Copies the values of persistent field.
        /// </summary>
        /// <returns>copies of values of persistent field</returns>
        internal override IEnumerable<object> CopyValues() {
            foreach (var value in this) {
                string copy;
                if (null == value) {
                    copy = null;
                } else {
                    copy = string.Copy(value);
                }
                yield return copy;
            }
        }

        /// <summary>
        /// Gets a new instance of PersistentFieldForElement.
        /// </summary>
        /// <returns>new instance of PersistentFieldForElement</returns>
        protected override PersistentFieldForElement<string> GetNewPersistentFieldForElement() {
            return new PersistentFieldForString(this.Key);
        }

        /// <summary>
        /// Gets the values of this presentable field as plain text.
        /// As an example, this is used for full-text indexing.
        /// </summary>
        /// <returns>values of this presentable field as plain text</returns>
        public override IEnumerable<string> GetValuesAsPlainText() {
            foreach (string value in base.GetValuesAsPlainText()) {
                if (string.IsNullOrEmpty(value)) {
                    yield return value;
                } else {
                    yield return XmlUtility.RemoveTags(value);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public override IEnumerable<string> GetValuesAsString() {
            return this;
        }

        /// <summary>
        /// Initializes the temporary list for DbDataReader.
        /// </summary>
        internal override void InitializeTemporaryCollectionForDbDataReader() {
            this.valuesFromDbDataReader = new List<string>();
            return;
        }

        /// <summary>
        /// Loads the current value of a DbDataReader into a
        /// temporary collection but does not set it yet.
        /// </summary>
        /// <param name="dataReader">data reader to load value from</param>
        internal override void LoadValueFromDbDataReader(DbDataReader dataReader) {
            if (dataReader.IsDBNull(0)) {
                this.valuesFromDbDataReader.Add(null);
            } else {
                this.valuesFromDbDataReader.Add(dataReader.GetString(0));
            }
            return;
        }

        /// <summary>
        /// Sets the new values for persistent field which have been
        /// loaded from DbDataReader before.
        /// </summary>
        internal override void SetValuesFromDbDataReader() {
            this.SetValuesUnsafe(this.valuesFromDbDataReader);
            this.valuesFromDbDataReader = null;
            return;
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of values of this field and adds it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="item">new value to add</param>
        /// <returns>true if value could be added successfully, false
        /// otherwise</returns>
        public override bool TryAddString(string item) {
            this.Add(item);
            return true;
        }

    }

}