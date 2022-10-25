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

    using System.Runtime.Serialization;

    /// <summary>
    /// Characters to use for separating values.
    /// </summary>
    public enum ValueSeparator {

        /// <summary>
        /// None.
        /// </summary>
        [EnumMemberAttribute]
        None = 0,

        /// <summary>
        /// Comma.
        /// </summary>
        [EnumMemberAttribute]
        Comma = 1,

        /// <summary>
        /// Semicolon.
        /// </summary>
        [EnumMemberAttribute]
        Semicolon = 2,

        /// <summary>
        /// Space.
        /// </summary>
        [EnumMemberAttribute]
        Space = 3,

        /// <summary>
        /// Line break.
        /// </summary>
        [EnumMemberAttribute]
        LineBreak = 4

    }

}