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
    /// Represents a detection state of an email address.
    /// </summary>
    internal enum EmailAddressDetectionState {

        /// <summary>
        /// Detection is outside of word.
        /// </summary>
        [EnumMemberAttribute]
        WhiteSpace = 0,

        /// <summary>
        /// Detection is inside of word.
        /// </summary>
        [EnumMemberAttribute]
        InWord = 1,

        /// <summary>
        /// Detection is inside of word with @ character.
        /// </summary>
        [EnumMemberAttribute]
        InWordWithAtCharacter = 2,

        /// <summary>
        /// Detection is inside of email address.
        /// </summary>
        [EnumMemberAttribute]
        InEmailAddress = 3,

        /// <summary>
        /// Detection is inside of invalid email address.
        /// </summary>
        [EnumMemberAttribute]
        InInvalidEmailAddress = 4,

        /// <summary>
        /// Detection is inside of an html tag.
        /// </summary>
        [EnumMemberAttribute]
        InHtmlTag = 5

    }

}