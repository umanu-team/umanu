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

namespace Framework.Presentation.Web {

    using System.Runtime.Serialization;

    /// <summary>
    /// Type of a redirection rule.
    /// </summary>
    public enum RedirectionRuleType {

        /// <summary>
        /// Source URL matches if current URL equals it.
        /// </summary>
        [EnumMemberAttribute]
        Equals = 0,

        /// <summary>
        /// Source URL matches if current URL begins with it.
        /// </summary>
        [EnumMemberAttribute]
        StartsWith = 1,

        /// <summary>
        /// Source URL matches if current URL contains it.
        /// </summary>
        [EnumMemberAttribute]
        Contains = 2,

        /// <summary>
        /// Source URL matches if current URL ends with it.
        /// </summary>
        [EnumMemberAttribute]
        EndsWith = 3,

        /// <summary>
        /// Source URL is a regular expression that needs to match
        /// the current URL.
        /// </summary>
        [EnumMemberAttribute]
        RegularExpression = 4

    }

}