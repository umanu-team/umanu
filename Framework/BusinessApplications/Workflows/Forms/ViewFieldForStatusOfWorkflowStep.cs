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

    using Framework.Presentation.Forms;

    /// <summary>
    /// Field for status of the instance of a workflow step of a
    /// specific type which is closest to the end of the workflow.
    /// </summary>
    public abstract class ViewFieldForStatusOfWorkflowStep : ViewField {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForStatusOfWorkflowStep()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets the status to be displayed for an instance of a
        /// workflow step.
        /// </summary>
        /// <param name="workflow">workflow to get status of instance
        /// of workflow step for</param>
        /// <returns>status to be displayed for instance of workflow
        /// step</returns>
        public abstract WorkflowStepStatus? GetStatusOfWorkflowStep(Workflow workflow);

    }

}