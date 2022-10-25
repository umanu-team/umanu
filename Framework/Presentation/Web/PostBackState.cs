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
    /// Post backstate of a form.
    /// </summary>
    public enum PostBackState {

        /// <summary>
        /// Current instance of form is created initially.
        /// </summary>
        [EnumMemberAttribute]
        NoPostBack,

        /// <summary>
        /// The form was submitted by the client validly.
        /// </summary>
        [EnumMemberAttribute]
        ValidPostBack,

        /// <summary>
        /// The form was submitted by the client, but something seems
        /// to be wrong. A machine might have submitted the post back
        /// or this instance of the form has been submitted before
        /// already.
        /// </summary>
        [EnumMemberAttribute]
        InvalidPostBack

    }

}