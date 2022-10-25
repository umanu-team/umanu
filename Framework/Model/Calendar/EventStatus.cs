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

namespace Framework.Model.Calendar {

    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies the status of an event.
    /// </summary>
    public enum EventStatus {

        /// <summary>
        /// Event is tentative.
        /// </summary>
        [EnumMemberAttribute]
        Tentative = 0,

        /// <summary>
        /// Event is definite.
        /// </summary>
        [EnumMemberAttribute]
        Confirmed = 1,

        /// <summary>
        /// Event was cancelled.
        /// </summary>
        [EnumMemberAttribute]
        Cancelled = 2

    }

}