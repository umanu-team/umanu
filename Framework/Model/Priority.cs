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

namespace Framework.Model {

    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a priority.
    /// </summary>
    public enum Priority {

        /// <summary>
        /// Priority is not defined.
        /// </summary>
        [EnumMemberAttribute]
        NotDefined = 0,

        /// <summary>
        /// Priority is A1 (high).
        /// </summary>
        [EnumMemberAttribute]
        A1 = 1,

        /// <summary>
        /// Priority is A2 (high).
        /// </summary>
        [EnumMemberAttribute]
        A2 = 2,

        /// <summary>
        /// Priority is A3 (high).
        /// </summary>
        [EnumMemberAttribute]
        A3 = 3,

        /// <summary>
        /// Priority is B1 (medium).
        /// </summary>
        [EnumMemberAttribute]
        B1 = 4,

        /// <summary>
        /// Priority is B2 (medium).
        /// </summary>
        [EnumMemberAttribute]
        B2 = 5,

        /// <summary>
        /// Priority is B3 (medium).
        /// </summary>
        [EnumMemberAttribute]
        B3 = 6,

        /// <summary>
        /// Priority is C1 (low).
        /// </summary>
        [EnumMemberAttribute]
        C1 = 7,

        /// <summary>
        /// Priority is C2 (low).
        /// </summary>
        [EnumMemberAttribute]
        C2 = 8,

        /// <summary>
        /// Priority is C3 (low).
        /// </summary>
        [EnumMemberAttribute]
        C3 = 9

    }

}