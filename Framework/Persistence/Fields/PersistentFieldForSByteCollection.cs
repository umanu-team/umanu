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
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Globalization;

    /// <summary>
    /// List of objects of type sbyte that can be stored in
    /// persistence mechanism.
    /// </summary>
    public sealed class PersistentFieldForSByteCollection : PersistentFieldForCollection<sbyte> {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.SByte;
            }
        }

        /// <summary>
        /// Values loaded from DbDataReader.
        /// </summary>
        private List<sbyte> valuesFromDbDataReader;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        public PersistentFieldForSByteCollection(string key)
            : base(key) {
            // nothing to do
        }

        /// <summary>
        /// Gets a new instance of PersistentFieldForElement.
        /// </summary>
        /// <returns>new instance of PersistentFieldForElement</returns>
        protected override PersistentFieldForElement<sbyte> GetNewPersistentFieldForElement() {
            return new PersistentFieldForSByte(this.Key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public override IEnumerable<string> GetValuesAsString() {
            foreach (var sByteValue in this) {
                yield return sByteValue.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Initializes the temporary list for DbDataReader.
        /// </summary>
        internal override void InitializeTemporaryCollectionForDbDataReader() {
            this.valuesFromDbDataReader = new List<sbyte>();
            return;
        }

        /// <summary>
        /// Loads the current value of a DbDataReader into a
        /// temporary collection but does not set it yet.
        /// </summary>
        /// <param name="dataReader">data reader to load value from</param>
        internal override void LoadValueFromDbDataReader(DbDataReader dataReader) {
            this.valuesFromDbDataReader.Add(Convert.ToSByte(dataReader.GetValue(0), CultureInfo.InvariantCulture.NumberFormat));
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
            bool success = sbyte.TryParse(item, NumberStyles.Any, CultureInfo.InvariantCulture, out sbyte parsedItem);
            this.Add(parsedItem);
            return success;
        }

    }

}