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

    using Framework.Persistence.Fields;
    using System.Collections.Generic;

    /// <summary>
    /// Workflow action which is a single step of a workflow.
    /// </summary>
    public abstract class ActionStep : WorkflowStep {

        /// <summary>
        /// Next workflow step to be executed once this workflow step
        /// is completed.
        /// </summary>
        public WorkflowStep NextStep {
            get { return this.nextStep.Value; }
            set { this.nextStep.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<WorkflowStep> nextStep =
            new PersistentFieldForPersistentObject<WorkflowStep>(nameof(NextStep), CascadedRemovalBehavior.RemoveValuesForcibly);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ActionStep()
            : base() {
            this.RegisterPersistentField(this.nextStep);
        }

        /// <summary>
        /// Gets the possible next workflow steps to be executed as
        /// soon as this workflow step is completed.
        /// </summary>
        /// <returns>possible next workflow steps</returns>
        public override IEnumerable<WorkflowStep> GetPossibleNextSteps() {
            yield return this.NextStep;
        }

        /// <summary>
        /// Replaces a specific possible next step by a new one.
        /// </summary>
        /// <param name="oldValue">old workflow step to be replaced</param>
        /// <param name="newValue">new workflow step to replace old
        /// workflow step by</param>
        /// <returns>true on success, false otherwise</returns>
        protected internal override bool ReplacePossibleNextStep(WorkflowStep oldValue, WorkflowStep newValue) {
            bool success = this.NextStep == oldValue;
            if (success) {
                this.NextStep = newValue;
            }
            return success;
        }

    }

}