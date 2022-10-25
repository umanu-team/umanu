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

    /// <summary>
    /// Type of a field with info about previous keys to be stored in
    /// persistence mechanism.
    /// </summary>
    public class PersistentFieldTypeWithPreviousKeys : PersistentFieldType {

        /// <summary>
        /// List of previous keys of field.
        /// </summary>
        public IList<string> PreviousKeys {
            get {
                if (null == this.previousKeys) {
                    this.previousKeys = new List<string>(0);
                }
                return this.previousKeys;
            }
        }
        private IList<string> previousKeys;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldType">persistent field type to copy</param>
        public PersistentFieldTypeWithPreviousKeys(PersistentFieldTypeWithPreviousKeys fieldType)
            : this(fieldType.Key, fieldType.PreviousKeys, fieldType.IsForSingleElement, fieldType.ContentBaseType) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldType">persistent field type to copy</param>
        /// <param name="previousKeys">previous keys of field</param>
        public PersistentFieldTypeWithPreviousKeys(PersistentFieldType fieldType, IEnumerable<string> previousKeys)
            : this(fieldType.Key, previousKeys, fieldType.IsForSingleElement, fieldType.ContentBaseType) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of the field created based on
        /// this persistent field type</param>
        /// <param name="previousKeys">previous keys of field</param>
        /// <param name="isForSingleElement">true if field is for a
        /// single element, false if it is for a collection of
        /// elements</param>
        /// <param name="contentBaseType">base type of value of
        /// persistent field</param>
        public PersistentFieldTypeWithPreviousKeys(string key, IEnumerable<string> previousKeys, bool isForSingleElement, Type contentBaseType)
            : base(key, isForSingleElement, contentBaseType) {
            foreach (var previousKey in previousKeys) {
                this.PreviousKeys.Add(previousKey);
            }
        }

        /// <summary>
        /// Determines whether the current object is equal to another
        /// object of the same type.
        /// </summary>
        /// <param name="other">object to compare this object to</param>
        /// <returns>true if both objects are equal, false otherwise</returns>
        public bool Equals(PersistentFieldTypeWithPreviousKeys other) {
            bool isEqual = base.Equals(other) && this.PreviousKeys.Count.Equals(other.PreviousKeys.Count);
            if (isEqual) {
                for (int i = 0; i < this.PreviousKeys.Count; i++) {
                    if (!this.PreviousKeys[i].Equals(other.PreviousKeys[i], StringComparison.Ordinal)) {
                        isEqual = false;
                        break;
                    }
                }
            }
            return isEqual;
        }

    }

}