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
    /// Surrounding HTML tag(s) to use for rendering items of
    /// navigation sections.
    /// </summary>
    public enum NavigationSectionTag {

        /// <summary>
        /// Do not surround items of navigation sections with HTML
        /// tags.
        /// </summary>
        [EnumMemberAttribute]
        None = 0,

        /// <summary>
        /// Surround items of navigation sections with &lt;ul&gt; and
        /// &lt;li&gt; HTML tags.
        /// </summary>
        [EnumMemberAttribute]
        UlLi = 1,

        /// <summary>
        /// Surround items of navigation sections with &lt;div&gt;
        /// HTML tags.
        /// </summary>
        [EnumMemberAttribute]
        Div = 2,

        /// <summary>
        /// Surround items of navigation sections with &lt;span&gt;
        /// HTML tags.
        /// </summary>
        [EnumMemberAttribute]
        Span = 3

    }

}