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
    /// Defindes the frequency of a recurrence.
    /// </summary>
    public enum RecurrenceFrequency {

        /// <summary>
        /// Secondly recurrence.
        /// </summary>
        [EnumMemberAttribute]
        Secondly = 0,

        /// <summary>
        /// Minutely recurrence.
        /// </summary>
        [EnumMemberAttribute]
        Minutely = 1,

        /// <summary>
        /// Hourly recurrence.
        /// </summary>
        [EnumMemberAttribute]
        Hourly = 2,

        /// <summary>
        /// Daily recurrence.
        /// </summary>
        [EnumMemberAttribute]
        Daily = 3,

        /// <summary>
        /// Weekly recurrence.
        /// </summary>
        [EnumMemberAttribute]
        Weekly = 4,

        /// <summary>
        /// Monthly recurrence.
        /// </summary>
        [EnumMemberAttribute]
        Monthly = 5,

        /// <summary>
        /// Yearly recurrence.
        /// </summary>
        [EnumMemberAttribute]
        Yearly = 6

    }

}