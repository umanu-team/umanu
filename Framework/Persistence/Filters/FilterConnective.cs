
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
    /// Logical connective to another filter.
    /// </summary>
    public enum FilterConnective {

        /// <summary>
        /// Logical connective is AND.
        /// </summary>
        [EnumMemberAttribute]
        And,

        /// <summary>
        /// There is no other filter.
        /// </summary>
        [EnumMemberAttribute]
        None,

        /// Logical connective is OR.
        [EnumMemberAttribute]
        Or

    }

}
