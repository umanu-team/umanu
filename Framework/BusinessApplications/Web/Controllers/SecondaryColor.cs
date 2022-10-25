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

namespace Framework.BusinessApplications.Web.Controllers {

    using System.Runtime.Serialization;

    /// <summary>
    /// Secondary color as defined in material design guidelines:
    /// http://www.google.com/design/spec/style/color.html
    /// </summary>
    public enum SecondaryColor {

        /// <summary>
        /// None.
        /// </summary>
        [EnumMemberAttribute]
        None = 0,

        /// <summary>
        /// Default.
        /// </summary>
        [EnumMemberAttribute]
        Cyan = 0x00acc1,

        /// <summary>
        /// Red.
        /// </summary>
        [EnumMemberAttribute]
        Red = 0xff5252,

        /// <summary>
        /// Pink.
        /// </summary>
        [EnumMemberAttribute]
        Pink = 0xff4081,

        /// <summary>
        /// Purple.
        /// </summary>
        [EnumMemberAttribute]
        Purple = 0xe040fb,

        /// <summary>
        /// Deep Purple.
        /// </summary>
        [EnumMemberAttribute]
        DeepPurple = 0x7c4dff,

        /// <summary>
        /// Indigo.
        /// </summary>
        [EnumMemberAttribute]
        Indigo = 0x536dfe,

        /// <summary>
        /// Blue.
        /// </summary>
        [EnumMemberAttribute]
        Blue = 0x448aff,

        /// <summary>
        /// Deep Orange.
        /// </summary>
        [EnumMemberAttribute]
        DeepOrange = 0xff6e40,

        /// <summary>
        /// Displays a logo.
        /// </summary>
        [EnumMemberAttribute]
        Logo = 0x24328b

    }

}