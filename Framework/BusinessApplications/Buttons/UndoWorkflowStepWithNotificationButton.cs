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
    using Framework.Model;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Server side action button of action bar for undoing a step of
    /// the workflow and sending out a notification.
    /// </summary>
    public class UndoWorkflowStepWithNotificationButton<T> : UndoWorkflowStepButton where T : class, IProvidableObject {

        /// <summary>
        /// Body of the notification to send out on click.
        /// </summary>
        private string emailBody;

        /// <summary>
        /// Subject of the notification to send out on click.
        /// </summary>
        private string emailSubject;

        /// <summary>
        /// Placeholder dictionary
        /// </summary>
        private IDictionary<string, string> placeholders;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        /// <param name="workflowStep">workflow step to go to</param>
        /// <param name="confirmationMessage">confirmation message to
        /// display on click</param>
        /// <param name="emailSubject">subject of the notification to
        /// send out on click</param>
        /// <param name="emailBody">body of the notification to send out
        /// on click</param>
        public UndoWorkflowStepWithNotificationButton(string title, WorkflowStep workflowStep, string confirmationMessage, string emailSubject, string emailBody)
            : base(title, workflowStep, confirmationMessage) {
            this.emailSubject = emailSubject;
            this.emailBody = emailBody;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        /// <param name="workflowStep">workflow step to go to</param>
        /// <param name="confirmationMessage">confirmation message to
        /// display on click</param>
        /// <param name="emailSubject">subject of the notification to
        /// send out on click</param>
        /// <param name="emailBody">body of the notification to send out
        /// on click</param>
        /// <param name="placeholders">pairs of keys and
        /// values to replace in subject and body text</param>
        public UndoWorkflowStepWithNotificationButton(string title, WorkflowStep workflowStep, string confirmationMessage, string emailSubject, string emailBody, IDictionary<string, string> placeholders)
            : this(title, workflowStep, confirmationMessage, emailSubject, emailBody) {
            this.placeholders = placeholders;
        }

        /// <summary>
        /// Server-side action to execute on click.
        /// </summary>
        /// <param name="form">current form or null</param>
        /// <param name="promptInput">prompt input of user</param>
        public override void OnClick(IForm form, string promptInput) {
            var workflowControlledObject = form.PresentableObject as WorkflowControlledObject;
            var from = workflowControlledObject.ParentPersistentContainer.ParentPersistenceMechanism.UserDirectory.CurrentUser;
            var to = workflowControlledObject.Workflow.GetPreviouslyAndCurrentlyInvolvedUsers<T>();
            var cc = new IPerson[0];
            PlainTextEmail.SendAsync(from, to, cc, this.emailSubject, this.emailBody, this.placeholders);
            base.OnClick(form, promptInput);
            return;
        }

    }

}