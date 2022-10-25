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
    using Framework.BusinessApplications.Workflows.Steps;
    using Presentation.Forms;

    /// <summary>
    /// Server side action button of action bar for jumping to
    /// another step of the workflow.
    /// </summary>
    public class GoToWorkflowStepButton : ServerSideButton {

        /// <summary>
        /// Workflow step to go to.
        /// </summary>
        public WorkflowStep WorkflowStep { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        /// <param name="workflowStep">workflow step  to go to</param>
        public GoToWorkflowStepButton(string title, WorkflowStep workflowStep)
            : base(title) {
            this.WorkflowStep = workflowStep;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        /// <param name="workflowStep">workflow step  to go to</param>
        /// <param name="confirmationMessage">confirmation message to
        /// display on click</param>
        public GoToWorkflowStepButton(string title, WorkflowStep workflowStep, string confirmationMessage)
            : this(title, workflowStep) {
            this.ConfirmationMessage = confirmationMessage;
        }

        /// <summary>
        /// Server-side action to execute on click.
        /// </summary>
        /// <param name="form">current form or null</param>
        /// <param name="promptInput">prompt input of user</param>
        public override void OnClick(IForm form, string promptInput) {
            var workflowControlledObject = (WorkflowControlledObject)form.PresentableObject;
            var elevatedWorkflowStep = this.WorkflowStep.ParentPersistentContainer.ParentPersistenceMechanism.CopyWithElevatedPrivileges().FindContainer<WorkflowStep>().FindOne(this.WorkflowStep.Id);
            workflowControlledObject.WorkflowWithElevatedPrivileges.GoTo(elevatedWorkflowStep);
            return;
        }

    }

}