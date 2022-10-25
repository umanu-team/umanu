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

namespace Framework.Persistence {

    using System.Runtime.Serialization;

    /// <summary>
    /// Determines how to copy allowed groups.
    /// </summary>
    public enum CopyBehaviorForAllowedGroups {

        /// <summary>
        /// Do not copy allowed groups at all. This might cause
        /// automatic inheritance of allowed groups on save of taget
        /// object if no further allowed groups are set.
        /// </summary>
        [EnumMemberAttribute]
        DoNotCopy = 0,

        /// <summary>
        /// Reference the same allowed groups in target object as in
        /// source object.
        /// </summary>
        [EnumMemberAttribute]
        ShallowCopy = 1

    }

}