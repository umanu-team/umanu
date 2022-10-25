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

namespace Framework.BusinessApplications.Workflows.Forms {

    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Properties;

    /// <summary>
    /// Field for workflow status to be presented in a view.
    /// </summary>
    public class ViewFieldForWorkflowStepTitle : ViewField {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForWorkflowStepTitle()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        public ViewFieldForWorkflowStepTitle(string title)
            : this() {
            this.Title = title;
        }

        /// <summary>
        /// Gets the read-only value of a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to get
        /// read-only value of</param>
        /// <param name="topmostPresentableObject">topmost
        /// presentable parent object to get read-only value of</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>read-only value of presentable field</returns>
        public override string GetReadOnlyValueFor(IPresentableField presentableField, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            string value;
            var workflowControlledObject = topmostPresentableObject as WorkflowControlledObject;
            if (null != workflowControlledObject?.Workflow) {
                if (null != workflowControlledObject.Workflow.CurrentStep) {
                    value = workflowControlledObject.Workflow.GetTitle();
                } else {
                    if (workflowControlledObject.Workflow.IsCanceled) {
                        value = Resources.Cancelled;
                    } else {
                        value = Resources.Completed;
                    }
                }
            } else {
                value = string.Empty;
            }
            return value;
        }

    }

}