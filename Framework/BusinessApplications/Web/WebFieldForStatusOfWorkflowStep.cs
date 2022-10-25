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

    using Framework.BusinessApplications.Workflows;
    using Framework.BusinessApplications.Workflows.Forms;
    using Framework.Presentation;
    using Framework.Presentation.Web;
    using Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Control for rendering a read-only field for the status of the
    /// instance of a workflow step.
    /// </summary>
    public class WebFieldForStatusOfWorkflowStep : WebField {

        /// <summary>
        /// Topmost presentable parent object to build form for.
        /// </summary>
        public IPresentableObject TopmostParentPresentableObject { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForStatusOfWorkflowStep ViewField { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build list table row for</param>
        public WebFieldForStatusOfWorkflowStep(ViewFieldForStatusOfWorkflowStep viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject)
            : base(viewField, renderMode) {
            this.ViewField = viewField;
            this.TopmostParentPresentableObject = topmostParentPresentableObject;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            base.RenderChildControls(html);
            if (FieldRenderMode.Form == this.RenderMode) {
                html.AppendOpeningTag("p");
            }
            var workflowControlledObject = this.TopmostParentPresentableObject as WorkflowControlledObject;
            if (null != workflowControlledObject?.Workflow) {
                var workflowStepStatus = this.ViewField.GetStatusOfWorkflowStep(workflowControlledObject.Workflow);
                if (workflowStepStatus.HasValue) {
                    var cssClasses = new List<string>(2);
                    string value;
                    if (WorkflowStepStatus.Initial == workflowStepStatus.Value) {
                        value = "-";
                    } else if (WorkflowStepStatus.Active == workflowStepStatus.Value) {
                        cssClasses.Add("status");
                        cssClasses.Add("active");
                        value = "⌛";
                    } else if (WorkflowStepStatus.Completed == workflowStepStatus.Value) {
                        cssClasses.Add("status");
                        cssClasses.Add("completed");
                        value = "✔";
                    } else {
                        throw new WorkflowException("Workflow step status \"" + workflowStepStatus + "\" is unknown.");
                    }
                    html.AppendOpeningTag("span", cssClasses);
                    html.AppendHtmlEncoded(value);
                    html.AppendClosingTag("span");
                }
            }
            if (FieldRenderMode.Form == this.RenderMode) {
                html.AppendClosingTag("p");
            }
            return;
        }

    }

}