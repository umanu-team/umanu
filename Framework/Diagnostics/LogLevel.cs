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

namespace Framework.Diagnostics {

    using System.Runtime.Serialization;

    /// <summary>
    /// Enumeration of log levels.
    /// </summary>
    public enum LogLevel {

        /// <summary>
        /// Debug message message which can help during the
        /// development process or for detailed debugging purposes.
        /// </summary>
        [EnumMemberAttribute]
        Debug = 0,

        /// <summary>
        /// Information message indicating that the solution works as
        /// supposed.
        /// </summary>
        [EnumMemberAttribute]
        Information = 1,

        /// <summary>
        /// Warning message indicating an irregular issue which does
        /// not necessarily need human interaction to be fixed.
        /// </summary>
        [EnumMemberAttribute]
        Warning = 2,

        /// <summary>
        /// Error message indicating a malfunction which probably
        /// needs human interaction to be fixed.
        /// </summary>
        [EnumMemberAttribute]
        Error = 3

    }

}