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

    using System.Collections.ObjectModel;
    using Framework.Model;
    using Framework.Persistence;

    /// <summary>
    /// Interface of objects that can be managed by a data provider.
    /// </summary>
    public interface IProvidableObject : IPresentableObject {

        /// <summary>
        /// Allowed groups for reading/writing this object.
        /// </summary>
        AllowedGroups AllowedGroups { get; }

        /// <summary>
        /// Indicates whether object has versions.
        /// </summary>
        bool HasVersions { get; }

        /// <summary>
        /// Enumerable of previous persistent versions of this
        /// object.
        /// </summary>
        ReadOnlyCollection<Version> Versions { get; }

        /// <summary>
        /// Gets the title of providable object.
        /// </summary>
        /// <returns>title of providable object</returns>
        string GetTitle();

    }

}