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

namespace Framework.Presentation.Forms {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for fields of collections to be presented.
    /// </summary>
    public interface IPresentableFieldForCollection : IPresentableField {

        /// <summary>
        /// Gets the number of objects contained.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds an object to the collection. 
        /// </summary>
        /// <param name="item">object to add to the collection</param>
        void AddObject(object item);

        /// <summary>
        /// Adds a string to the collection. 
        /// </summary>
        /// <param name="item">string value to add to the collection</param>
        void AddString(string item);

        /// <summary>
        /// Removes all objects from the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection of sortable values or null. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection
        /// of sortable values or null</returns>
        IEnumerable<string> GetSortableValues();

        /// <summary>
        /// Gets the values of presentable field as object.
        /// </summary>
        /// <returns>values of presentable field as object</returns>
        IEnumerable<object> GetValuesAsObject();

        /// <summary>
        /// Gets the values of this presentable field as plain text.
        /// As an example, this is used for full-text indexing.
        /// </summary>
        /// <returns>values of this presentable field as plain text</returns>
        IEnumerable<string> GetValuesAsPlainText();

        /// <summary>
        /// Gets the values of presentable field as string.
        /// </summary>
        /// <returns>values of presentable field as string</returns>
        IEnumerable<string> GetValuesAsString();

        /// <summary>
        /// Gets the versioned field for a specific date.
        /// </summary>
        /// <param name="modificationDate">date to get versioned
        /// field for</param>
        /// <returns>versioned field if existing, null otherwise</returns>
        IPresentableFieldForCollection GetVersionedField(DateTime? modificationDate);

        /// <summary>
        /// Sorts the elements in the list using the default
        /// comparer. This method performs an unstable sort, which
        /// means the order of equal elements may change.
        /// </summary>
        void Sort();

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer. This method performs an unstable sort, which
        /// means the order of equal elements may change.
        /// </summary>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        void SortObjects(Comparison<object> comparison);

        /// <summary>
        /// Swaps the items of two indexes.
        /// </summary>
        /// <param name="firstIndex">index to assign item of second
        /// index to</param>
        /// <param name="secondIndex">index to assign item of first
        /// index to</param>
        void Swap(int firstIndex, int secondIndex);

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of values of this field and adds it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="item">new value to add</param>
        /// <returns>true if value could be added successfully, false
        /// otherwise</returns>
        bool TryAddString(string item);

    }

    /// <summary>
    /// Interface for fields of collections to be presented.
    /// </summary>
    /// <typeparam name="TValue">type of values in collection</typeparam>
    public interface IPresentableFieldForCollection<TValue> : IPresentableFieldForCollection, ICollection<TValue> {

        /// <summary>
        /// Gets the value at a specific index.
        /// </summary>
        /// <param name="index">index to get value for</param>
        /// <returns>value at the specific index</returns>
        TValue this[int index] { get; set; }

    }

}