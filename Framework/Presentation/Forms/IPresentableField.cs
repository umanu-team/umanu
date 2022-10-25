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
    /// Interface for fields to be presented.
    /// </summary>
    public interface IPresentableField {

        /// <summary>
        /// Base type of value of this presentable field.
        /// </summary>
        Type ContentBaseType { get; }

        /// <summary>
        /// True if field is for a single element, false if it is for
        /// a collection of elements.
        /// </summary>
        bool IsForSingleElement { get; }

        /// <summary>
        /// Gets a value indicating whether this field is read-only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Internal key of this presentable field.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Parent presentable object of this field.
        /// </summary>
        IPresentableObject ParentPresentableObject { get; }

        /// <summary>
        /// Creates a new item that could be set to this field.
        /// </summary>
        /// <returns>new item that could be set to this field</returns>
        object NewItemAsObject();

    }

}
