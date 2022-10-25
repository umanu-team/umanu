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

namespace Framework.BusinessApplications.Workflows.Steps {

    using Framework.BusinessApplications.Buttons;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using System.Collections.Generic;

    /// <summary>
    /// Last workflow step to be executed when the workflow ends or
    /// is canceled.
    /// </summary>
    public class LastStep : WorkflowState {

        /// <summary>
        /// Url of icon to display for canceled workflows.
        /// </summary>
        public virtual string IconUrlForCanceledWorkflows {
            get { return string.Empty; }
        }

        /// <summary>
        /// Url of icon to display for completed workflows.
        /// </summary>
        public virtual string IconUrlForCompletedWorkflows {
            get { return string.Empty; }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public LastStep()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets called on click of a button of the workflow.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="sender">button that was clicked</param>
        /// <param name="promptInput">prompt input of user</param>
        /// <typeparam name="T">type of objects server side workflow
        /// button is for</typeparam>
        internal virtual void ButtonClick<T>(WorkflowControlledObject associatedObject, ServerSideWorkflowButton<T> sender, string promptInput) where T : class, IProvidableObject {
            var buttons = new List<ActionButton>();
            buttons.AddRange(this.GetAdditionalViewFormButtons(associatedObject, sender.DataProvider));
            buttons.AddRange(this.GetAdditionalEditFormButtons(associatedObject, sender.DataProvider));
            foreach (var button in buttons) {
                if (button.Equals(sender)) {
                    sender.HandleClick(promptInput);
                    break;
                }
            }
            return;
        }

        /// <summary>
        /// Executes this workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        protected internal virtual void Execute(WorkflowControlledObject associatedObject) {
            return;
        }

    }

}