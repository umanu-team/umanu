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
    /// Primary color as defined in material design guidelines:
    /// http://www.google.com/design/spec/style/color.html
    /// </summary>
    public enum PrimaryColor {

        /// <summary>
        /// None.
        /// </summary>
        [EnumMemberAttribute]
        None = 0,

        /// <summary>
        /// Blue Grey.
        /// </summary>
        [EnumMemberAttribute]
        BlueGrey = 0x607d8b,

        /// <summary>
        /// Red.
        /// </summary>
        [EnumMemberAttribute]
        Red = 0xf44336,

        /// <summary>
        /// Pink.
        /// </summary>
        [EnumMemberAttribute]
        Pink = 0xe91e63,

        /// <summary>
        /// Purple.
        /// </summary>
        [EnumMemberAttribute]
        Purple = 0x9c27b0,

        /// <summary>
        /// Deep Purple.
        /// </summary>
        [EnumMemberAttribute]
        DeepPurple = 0x673ab7,

        /// <summary>
        /// Indigo.
        /// </summary>
        [EnumMemberAttribute]
        Indigo = 0x3f51b5,

        /// <summary>
        /// Blue.
        /// </summary>
        [EnumMemberAttribute]
        Blue = 0x2196f3,

        /// <summary>
        /// Light blue.
        /// </summary>
        [EnumMemberAttribute]
        LightBlue = 0x03a9f4,

        /// <summary>
        /// Cyan.
        /// </summary>
        [EnumMemberAttribute]
        Cyan = 0x00bcd4,

        /// <summary>
        /// Teal.
        /// </summary>
        [EnumMemberAttribute]
        Teal = 0x009688,

        /// <summary>
        /// Green.
        /// </summary>
        [EnumMemberAttribute]
        Green = 0x4caf50,

        /// <summary>
        /// Light Green.
        /// </summary>
        [EnumMemberAttribute]
        LightGreen = 0x8bc34a,

        /// <summary>
        /// Lime.
        /// </summary>
        [EnumMemberAttribute]
        Lime = 0xcddc39,

        /// <summary>
        /// Yellow.
        /// </summary>
        [EnumMemberAttribute]
        Yellow = 0xffeb3b,

        /// <summary>
        /// Amber.
        /// </summary>
        [EnumMemberAttribute]
        Amber = 0xffc107,

        /// <summary>
        /// Orange.
        /// </summary>
        [EnumMemberAttribute]
        Orange = 0xff9800,

        /// <summary>
        /// Deep Orange.
        /// </summary>
        [EnumMemberAttribute]
        DeepOrange = 0xff5722,

        /// <summary>
        /// Brown.
        /// </summary>
        [EnumMemberAttribute]
        Brown = 0x795548,

        /// <summary>
        /// Grey.
        /// </summary>
        [EnumMemberAttribute]
        Grey = 0x9e9e9e

    }

}