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

    /// <summary>
    /// Placeholder workflow step, e.g. for visualization of manual
    /// steps.
    /// </summary>
    public abstract class PlaceholderAction : ActionStep {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PlaceholderAction()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Executes this workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        /// <returns>result of execution of workflow step</returns>
        protected internal override WorkflowStepResult Execute(WorkflowControlledObject associatedObject) {
            return new WorkflowStepResult(this.NextStep);
        }

    }

}