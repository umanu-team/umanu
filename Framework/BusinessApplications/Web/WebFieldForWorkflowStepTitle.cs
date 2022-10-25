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

namespace Framework.BusinessApplications.Web {

    using Framework.BusinessApplications.Workflows.Forms;
    using Framework.Presentation;
    using Framework.Presentation.Web;
    using Presentation.Forms;

    /// <summary>
    /// Control for rendering a read-only field for workflow status.
    /// </summary>
    public class WebFieldForWorkflowStepTitle : WebFieldForReadOnlyValue {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build list table row for</param>
        public WebFieldForWorkflowStepTitle(ViewFieldForWorkflowStepTitle viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject)
            : base(viewField, renderMode, topmostParentPresentableObject, null) {
            // nothing to do
        }

    }

}