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
    /// Type of resize operation to perform.
    /// </summary>
    public enum ResizeType {

        /// <summary>
        /// No resize, ignore given side length.
        /// </summary>
        [EnumMemberAttribute]
        None = 0,

        /// <summary>
        /// Clip a square with given side length.
        /// </summary>
        [EnumMemberAttribute]
        CropToSquare = 1,

        /// <summary>
        /// Keep aspect ratio and take given side length as side
        /// length of longer side.
        /// </summary>
        [EnumMemberAttribute]
        KeepAspectRatioSetLongerSide = 2,

        /// <summary>
        /// Keep aspect ratio and take given side length as side
        /// length of longer side. Crop image to make the remainder
        /// of division of length of shorter side by 16 zero.
        /// </summary>
        [EnumMemberAttribute]
        OptimizeAspectRatioSetLongerSide = 3,

        /// <summary>
        /// Keep aspect ratio and take given side length as width.
        /// </summary>
        [EnumMemberAttribute]
        KeepAspectRatioSetWidth = 4,

        /// <summary>
        /// Keep aspect ratio and take given side length as width.
        /// Crop image to make the remainder of division of length of
        /// shorter side by 16 zero.
        /// </summary>
        [EnumMemberAttribute]
        OptimizeAspectRatioSetWidth = 5,

        /// <summary>
        /// Keep aspect ratio and take given side length as height.
        /// </summary>
        [EnumMemberAttribute]
        KeepAspectRatioSetHeight = 6,

        /// <summary>
        /// Keep aspect ratio and take given side length as height.
        /// Crop image to make the remainder of division of length of
        /// shorter side by 16 zero.
        /// </summary>
        [EnumMemberAttribute]
        OptimizeAspectRatioSetHeight = 7

    }

}