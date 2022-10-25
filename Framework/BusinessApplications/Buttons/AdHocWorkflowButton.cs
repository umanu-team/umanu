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

    using Framework.BusinessApplications.Workflows;
    using Framework.Presentation.Forms;

    /// <summary>
    /// Server side action button of action bar for insertion of an
    /// ad-hoc workflow into the running one.
    /// </summary>
    public class AdHocWorkflowButton : ServerSideButton {

        /// <summary>
        /// Ad-hoc workflow step sequence to insert into the running
        /// workflow.
        /// </summary>
        public WorkflowStepSequence AdHocWorkflow { get; set; }

        /// <summary>
        /// Specifies the insertion point.
        /// </summary>
        public InsertionPoint InsertionPoint { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        /// <param name="adHocWorkflow">ad-hoc workflow step sequence
        /// to insert into the running workflow</param>
        /// <param name="insertionPoint">insertion point</param>
        public AdHocWorkflowButton(string title, WorkflowStepSequence adHocWorkflow, InsertionPoint insertionPoint)
            : base(title) {
            this.AdHocWorkflow = adHocWorkflow;
            this.InsertionPoint = insertionPoint;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        /// <param name="adHocWorkflow">ad-hoc workflow step sequence
        /// to insert into the running workflow</param>
        /// <param name="insertionPoint">insertion point</param>
        /// <param name="confirmationMessage">confirmation message to
        /// display on click</param>
        public AdHocWorkflowButton(string title, WorkflowStepSequence adHocWorkflow, InsertionPoint insertionPoint, string confirmationMessage)
            : this(title, adHocWorkflow, insertionPoint) {
            this.ConfirmationMessage = confirmationMessage;
        }

        /// <summary>
        /// Server-side action to execute on click.
        /// </summary>
        /// <param name="form">current form or null</param>
        /// <param name="promptInput">prompt input of user</param>
        public override void OnClick(IForm form, string promptInput) {
            var workflowControlledObject = (WorkflowControlledObject)form.PresentableObject;
            workflowControlledObject.WorkflowWithElevatedPrivileges.InsertAdHocWorkflow(this);
            return;
        }

    }

}