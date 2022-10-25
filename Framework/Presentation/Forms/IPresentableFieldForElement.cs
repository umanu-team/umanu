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

    /// <summary>
    /// Interface for fields of elements to be presented.
    /// </summary>
    public interface IPresentableFieldForElement : IPresentableField {

        /// <summary>
        /// Sortable value of this presentable field.
        /// </summary>
        string SortableValue { get; }

        /// <summary>
        /// Value of this presentable field as Object.
        /// </summary>
        object ValueAsObject { get; set; }

        /// <summary>
        /// Value of this presentable field as string.
        /// </summary>
        string ValueAsString { get; set; }

        /// <summary>
        /// Gets the value of this presentable field as plain text.
        /// As an example, this is used for full-text indexing.
        /// </summary>
        /// <returns>value of this presentable field as plain text</returns>
        string GetValueAsPlainText();

        /// <summary>
        /// Gets the versioned field for a specific date.
        /// </summary>
        /// <param name="modificationDate">date to get versioned
        /// field for</param>
        /// <returns>versioned field if existing, null otherwise</returns>
        IPresentableFieldForElement GetVersionedField(DateTime? modificationDate);

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of value of this field and sets it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">new value to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        bool TrySetValueAsString(string value);

    }

    /// <summary>
    /// Interface for fields of elements to be presented.
    /// </summary>
    /// <typeparam name="TValue">type of value</typeparam>
    public interface IPresentableFieldForElement<TValue> : IPresentableFieldForElement {

        /// <summary>
        /// Value of this presentable field.
        /// </summary>
        TValue Value { get; set; }

    }

}
