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

namespace Framework.Persistence.Filters {

    using System.Runtime.Serialization;

    /// <summary>
    /// Determins whether the target of a comparison is another value
    /// or another field.
    /// </summary>
    public enum FilterTarget {

        /// <summary>
        /// Target of comparison is another field.
        /// </summary>
        [EnumMemberAttribute]
        IsOtherField,

        /// <summary>
        /// Target of comparison is another boolean value.
        /// </summary>
        [EnumMemberAttribute]
        IsOtherBoolValue,

        /// <summary>
        /// Target of comparison is another GUID value.
        /// </summary>
        [EnumMemberAttribute]
        IsOtherGuidValue,

        /// <summary>
        /// Target of comparison is another numeric value.
        /// </summary>
        [EnumMemberAttribute]
        IsOtherNumericValue,

        /// <summary>
        /// Target of comparison is another text value.
        /// </summary>
        [EnumMemberAttribute]
        IsOtherTextValue

    }

}
