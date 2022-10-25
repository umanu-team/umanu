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
    /// Style of how to display an option.
    /// </summary>
    public enum OptionDisplayStyle {

        /// <summary>
        /// Display nothing.
        /// </summary>
        [EnumMemberAttribute]
        None = 0,

        /// <summary>
        /// Display icon only.
        /// </summary>
        [EnumMemberAttribute]
        IconOnly = 1,

        /// <summary>
        /// Display icon, use text as fallback.
        /// </summary>
        [EnumMemberAttribute]
        IconWithTextFallback = 2,

        /// <summary>
        /// Display text only.
        /// </summary>
        [EnumMemberAttribute]
        TextOnly = 3,

        /// <summary>
        /// Display text, use icon as fallback.
        /// </summary>
        [EnumMemberAttribute]
        TextWithIconFallback = 4

    }

}