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

namespace Framework.BusinessApplications.Workflows.Steps {

    using System.Runtime.Serialization;

    /// <summary>
    /// Condition to be fulfilled in order to pass a
    /// WaitForFieldValuesAction.
    /// </summary>
    public enum WaitForFieldValuesCondition {

        /// <summary>
        /// All field values must match.
        /// </summary>
        [EnumMemberAttribute]
        AllFieldValuesMustMatch = 0,

        /// <summary>
        /// All field values must not match.
        /// </summary>
        [EnumMemberAttribute]
        AllFieldValuesMustNotMatch = 1,

        /// <summary>
        /// One of the field values must match.
        /// </summary>
        [EnumMemberAttribute]
        OneFieldValueMustMatch = 2,

        /// <summary>
        /// One of the field values must not match.
        /// </summary>
        [EnumMemberAttribute]
        OneFieldValueMustNotMatch = 3

    }

}