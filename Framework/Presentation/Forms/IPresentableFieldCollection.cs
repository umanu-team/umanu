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

    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Interface for keyed collection of presentable fields. Keys of
    /// fields may not change.
    /// </summary>
    public interface IPresentableFieldCollection : IList<IPresentableField> {

        /// <summary>
        /// Gets the keys of all presentable fields of this
        /// collection.
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Gets the element with the specified key.
        /// </summary>
        /// <param name="key">key of the element to get</param>
        /// <returns>value of element with the specified key</returns>
        IPresentableField this[string key] { get; }

        /// <summary>
        /// Adds a range of presentable fields to this collection.
        /// </summary>
        /// <param name="items">range of fields to add</param>
        void AddRange(IEnumerable<IPresentableField> items);

        /// <summary>
        /// Determines whether the collection contains an element
        /// with the specified key chain.
        /// </summary>
        /// <param name="keyChain">key chain to look for</param>
        /// <returns>true if an element for the given key chain
        /// exists, false otherwise</returns>
        bool Contains(string keyChain);

        /// <summary>
        /// Determines whether the collection contains an element
        /// with the specified key chain.
        /// </summary>
        /// <param name="keyChain">key chain to look for</param>
        /// <returns>true if an element for the given key chain
        /// exists, false otherwise</returns>
        bool Contains(string[] keyChain);

        /// <summary>
        /// Finds a specific presentable field in this or any
        /// presentable child object.
        /// </summary>
        /// <param name="keyChain">key chain of presentable field to
        /// find</param>
        /// <returns>matching presentable field or null</returns>
        IPresentableField Find(string[] keyChain);

        /// <summary>
        /// Removes the element for a specific key.
        /// </summary>
        /// <param name="key">key to remove element for</param>
        /// <returns>true if the element is successfully removed,
        /// false otherwise - e.g. if no element for the given key
        /// exists</returns>
        bool Remove(string key);

    }

}