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
    /// Control for rendering a read-only field for workflow status icon.
    /// </summary>
    public class WebFieldForWorkflowStepIcon : WebFieldForWorkflowStepTitle {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build list table row for</param>
        public WebFieldForWorkflowStepIcon(ViewFieldForWorkflowStepTitle viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject)
            : base(viewField, renderMode, topmostParentPresentableObject) {
            // nothing to do
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            if (FieldRenderMode.ListTable == this.RenderMode) {
                string sortableValue = this.GetReadOnlyValue();
                if (!string.IsNullOrEmpty(sortableValue)) {
                    sortableValue = System.Web.HttpUtility.HtmlEncode(sortableValue);
                    yield return new KeyValuePair<string, string>("data-value", sortableValue);
                }
            }
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            if (FieldRenderMode.Form == this.RenderMode) {
                this.RenderDisplayTitle(html);
                html.AppendOpeningTag("p");
            }
            string iconUrl;
            var workflowControlledObject = this.TopmostParentPresentableObject as WorkflowControlledObject;
            if (null != workflowControlledObject && null != workflowControlledObject.Workflow) {
                if (null != workflowControlledObject.Workflow.CurrentStep) {
                    iconUrl = workflowControlledObject.Workflow.GetIconUrl();
                } else if (null != workflowControlledObject.Workflow.LastStep) {
                    if (workflowControlledObject.Workflow.IsCanceled) {
                        iconUrl = workflowControlledObject.Workflow.LastStep.IconUrlForCanceledWorkflows;
                    } else {
                        iconUrl = workflowControlledObject.Workflow.LastStep.IconUrlForCompletedWorkflows;
                    }
                } else {
                    iconUrl = string.Empty;
                }
            } else {
                iconUrl = string.Empty;
            }
            if (!string.IsNullOrEmpty(iconUrl)) {
                var attributes = new Dictionary<string, string>(2);
                string workflowStepTitle = this.GetReadOnlyValue();
                if (!string.IsNullOrEmpty(workflowStepTitle)) {
                    attributes.Add("title", workflowStepTitle);
                }
                attributes.Add("src", iconUrl);
                html.AppendSelfClosingTag("img", attributes);
            }
            if (FieldRenderMode.Form == this.RenderMode) {
                html.AppendClosingTag("p");
            }
            return;
        }

    }

}