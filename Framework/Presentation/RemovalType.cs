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

    using System.Runtime.Serialization;

    /// <summary>
    /// Type of removal of an object.
    /// </summary>
    public enum RemovalType {

        /// <summary>
        /// Do no remove the object.
        /// </summary>
        [EnumMemberAttribute]
        False = 0,

        /// <summary>
        /// Remove the object only - do not remove child objects.
        /// </summary>
        [EnumMemberAttribute]
        Remove = 1,

        /// <summary>
        /// Remove the object and all child objects to be removed
        /// cascadedly.
        /// </summary>
        [EnumMemberAttribute]
        RemoveCascadedly = 2

    }

}