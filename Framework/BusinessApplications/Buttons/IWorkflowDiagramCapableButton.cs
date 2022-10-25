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

    /// <summary>
    /// Interface for buttons that can be displayed on workflow
    /// diagram.
    /// </summary>
    internal interface IWorkflowDiagramCapableButton {

        /// <summary>
        /// Indicates whether workflow step is supposed to be visible
        /// on forms pages.
        /// </summary>
        bool IsVisibleOnFormPages { get; }

        /// <summary>
        /// Indicates whether workflow step is supposed to be visible
        /// on workflow diagram pages.
        /// </summary>
        bool IsVisibleOnWorkflowDiagramPages { get; }

    }

}