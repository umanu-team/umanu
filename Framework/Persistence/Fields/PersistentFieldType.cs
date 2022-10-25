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

    /// <summary>
    /// Type of a field to be stored in persistence mechanism.
    /// </summary>
    public class PersistentFieldType : IEquatable<PersistentFieldType> {

        /// <summary>
        /// Base type of value of this persistent field type.
        /// </summary>
        public Type ContentBaseType { get; set; }

        /// <summary>
        /// True if type is for a single element, false if it is for
        /// a collection of elements.
        /// </summary>
        public bool IsForSingleElement { get; set; }

        /// <summary>
        /// Name of the field created based on this persistent field
        /// type.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldType">persistent field type to copy</param>
        public PersistentFieldType(PersistentFieldType fieldType)
            : this(fieldType.Key, fieldType.IsForSingleElement, fieldType.ContentBaseType) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of the field created based on
        /// this persistent field type</param>
        /// <param name="isForSingleElement">true if field is for a
        /// single element, false if it is for a collection of
        /// elements</param>
        /// <param name="contentBaseType">base type of value of
        /// persistent field</param>
        public PersistentFieldType(string key, bool isForSingleElement, Type contentBaseType) {
            this.Key = key;
            this.IsForSingleElement = isForSingleElement;
            this.ContentBaseType = contentBaseType;
        }

        /// <summary>
        /// Determines whether the current object is equal to another
        /// object of the same type.
        /// </summary>
        /// <param name="other">object to compare this object to</param>
        /// <returns>true if both objects are equal, false otherwise</returns>
        public bool Equals(PersistentFieldType other) {
            bool isEqual;
            if (null == other) {
                isEqual = false;
            } else {
                isEqual = this.ContentBaseType.Equals(other.ContentBaseType)
                    && this.Key.Equals(other.Key, StringComparison.Ordinal)
                    && this.IsForSingleElement.Equals(other.IsForSingleElement);
            }
            return isEqual;
        }

    }

}
