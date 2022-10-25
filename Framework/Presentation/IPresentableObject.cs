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

namespace Framework.Presentation {

    using Framework.Persistence;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface of objects that can be presented in a list or
    /// inside of a form.
    /// </summary>
    public interface IPresentableObject {

        /// <summary>
        /// Time of creation of this object.
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// User who created this object.
        /// </summary>
        IUser CreatedBy { get; }

        /// <summary>
        /// Globally unique identifier of this object.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// True if this is a new object, false otherwise.
        /// </summary>
        bool IsNew { get; }

        /// <summary>
        /// Time of last modification.
        /// </summary>
        DateTime ModifiedAt { get; }

        /// <summary>
        /// User who modified this object.
        /// </summary>
        IUser ModifiedBy { get; }

        /// <summary>
        /// Indicates whether to remove this object on update.
        /// </summary>
        RemovalType RemoveOnUpdate { get; set; }

        /// <summary>
        /// Finds the first presentable field for a specified key.
        /// </summary>
        /// <param name="key">key to find presentable field for</param>
        /// <returns>first presentable field for specified key or
        /// null</returns>
        IPresentableField FindPresentableField(string key);

        /// <summary>
        /// Finds the first presentable field for a specified key
        /// chain.
        /// </summary>
        /// <param name="keyChain">key chain to find presentable
        /// field for</param>
        /// <returns>first presentable field for specified key chain
        /// or null</returns>
        IPresentableField FindPresentableField(string[] keyChain);

        /// <summary>
        /// Finds all presentable fields for a specified key.
        /// </summary>
        /// <param name="key">key to find presentable fields for</param>
        /// <returns>all presentable fields for specified key or null</returns>
        IEnumerable<IPresentableField> FindPresentableFields(string key);

        /// <summary>
        /// Finds all presentable fields for a specified key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find presentable
        /// fields for</param>
        /// <returns>all presentable fields for specified key chain
        /// or null</returns>
        IEnumerable<IPresentableField> FindPresentableFields(string[] keyChain);

    }

}