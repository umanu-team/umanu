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
    /// Type of control to use for displaying options.
    /// </summary>
    public enum OptionControlType {

        /// <summary>
        /// Determine control type automatically.
        /// </summary>
        [EnumMemberAttribute]
        Automatic = 0,

        /// <summary>
        /// Use radio buttons for displaying options.
        /// </summary>
        [EnumMemberAttribute]
        RadioButtons = 1,

        /// <summary>
        /// Use drop-down list for displaying options.
        /// </summary>
        [EnumMemberAttribute]
        DropDownList = 2

    }

}