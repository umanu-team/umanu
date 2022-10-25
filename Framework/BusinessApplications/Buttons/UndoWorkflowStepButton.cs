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

    using Framework.BusinessApplications.Workflows.Steps;
    using System;

    /// <summary>
    /// Server side action button of action bar for undoing a step of
    /// the workflow.
    /// </summary>
    public class UndoWorkflowStepButton : GoToWorkflowStepButton, IWorkflowDiagramCapableButton {

        /// <summary>
        /// ID of workflow step to be undone.
        /// </summary>
        internal Guid IdOfWorkflowStepToBeUndone { get; set; }

        /// <summary>
        /// Indicates whether workflow step is supposed to be visible
        /// on forms pages.
        /// </summary>
        public bool IsVisibleOnFormPages { get; set; }

        /// <summary>
        /// Indicates whether workflow step is supposed to be visible
        /// on workflow diagram pages.
        /// </summary>
        public bool IsVisibleOnWorkflowDiagramPages { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        /// <param name="workflowStep">workflow step to go to</param>
        /// <param name="confirmationMessage">confirmation message to
        /// display on click</param>
        public UndoWorkflowStepButton(string title, WorkflowStep workflowStep, string confirmationMessage)
            : base(title, workflowStep, confirmationMessage) {
            this.IsVisibleOnFormPages = true;
            this.IsVisibleOnWorkflowDiagramPages = true;
        }

    }

}