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

    using Steps;
    using System;

    /// <summary>
    /// Field for status of the instance of a workflow step of a
    /// specific type which is closest to the end of the workflow.
    /// </summary>
    /// <typeparam name="T">type of workflow step to display status
    /// of</typeparam>
    public sealed class ViewFieldForStatusOfLastWorkflowStepOfType<T> : ViewFieldForStatusOfWorkflowStep where T : WorkflowStep {

        /// <summary>
        /// Type of workflow step.
        /// </summary>
        public Type TypeOfWorkflowStep {
            get {
                if (null == this.typeOfWorkflowStep) {
                    this.typeOfWorkflowStep = typeof(T);
                }
                return this.typeOfWorkflowStep;
            }
        }
        private Type typeOfWorkflowStep;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForStatusOfLastWorkflowStepOfType()
        : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        public ViewFieldForStatusOfLastWorkflowStepOfType(string title)
            : this() {
            this.Title = title;
        }

        /// <summary>
        /// Gets the status to be displayed for an instance of a
        /// workflow step.
        /// </summary>
        /// <param name="workflow">workflow to get status of instance
        /// of workflow step for</param>
        /// <returns>status to be displayed for instance of workflow
        /// step</returns>
        public override WorkflowStepStatus? GetStatusOfWorkflowStep(Workflow workflow) {
            return ((Workflow)workflow.GetWithElevatedPrivileges()).GetStatusOfLastWorkflowStepOfType(this.TypeOfWorkflowStep); // elevated privileges are used for performance reasons
        }

    }

}