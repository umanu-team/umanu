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

namespace Framework.BusinessApplications.Buttons {

    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies the insertion point of an ad-hoc workflow.
    /// </summary>
    public enum InsertionPoint {

        /// <summary>
        /// Inserts an ad-hoc workflow prior to the current step of
        /// root workflow.
        /// </summary>
        [EnumMemberAttribute]
        BeforeCurrentStep = 0,

        /// <summary>
        /// Inserts an ad-hoc workflow prior to the current step of
        /// leaf workflow.
        /// </summary>
        [EnumMemberAttribute]
        BeforeCurrentLeafStep = 1,

        /// <summary>
        /// Inserts an ad-hoc workflow parallel to the current step
        /// of root workflow.
        /// </summary>
        [EnumMemberAttribute]
        ParallelToCurrentStep = 2,

        /// <summary>
        /// Inserts an ad-hoc workflow after the current step of root
        /// workflow.
        /// </summary>
        [EnumMemberAttribute]
        AfterCurrentStep = 3,

        /// <summary>
        /// Inserts an ad-hoc workflow after the current step of leaf
        /// workflow.
        /// </summary>
        [EnumMemberAttribute]
        AfterCurrentLeafStep = 4

    }

}