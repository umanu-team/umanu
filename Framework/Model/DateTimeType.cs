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

namespace Framework.Model {

    using System.Runtime.Serialization;

    /// <summary>
    /// Type of date/time value of a date/time field.
    /// </summary>
    public enum DateTimeType {

        /// <summary>
        /// A date and a time whis is displayed converted to local
        /// timezone on client side but saved as UTC date/time.
        /// </summary>
        [EnumMemberAttribute]
        DateAndTime = 0,

        /// <summary>
        /// A local date and a time which is not converted
        /// automatically. Usually it is better to use DateAndTime
        /// instead. LocalDateAndTime should only be used if
        /// date/times are supposed to be displayed in other time
        /// zones than the time zone of the client.
        /// </summary>
        [EnumMemberAttribute]
        LocalDateAndTime = 1,

        /// <summary>
        /// A date without a time.
        /// </summary>
        [EnumMemberAttribute]
        Date = 2,

        /// <summary>
        /// A month of a specific year.
        /// </summary>
        [EnumMemberAttribute]
        Month = 3,

        /// <summary>
        /// A time without a date.
        /// </summary>
        [EnumMemberAttribute]
        Time = 4,

        /// <summary>
        /// A week of a specific year.
        /// </summary>
        [EnumMemberAttribute]
        Week = 5

    }

}