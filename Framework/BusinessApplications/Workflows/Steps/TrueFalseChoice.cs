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
    /// Goes to one of two other workflow steps. Please consider
    /// using a TrueFalseButtonChoice instead if you need a
    /// TrueFalseChoice dependent on button clicks.
    /// </summary>
    public abstract class TrueFalseChoice : ChoiceStep {

        /// <summary>
        /// Next step in case "false" button was pressed.
        /// </summary>
        public WorkflowStep NextStepIfFalse {
            get { return this.nextStepIfFalse.Value; }
            set { this.nextStepIfFalse.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<WorkflowStep> nextStepIfFalse =
            new PersistentFieldForPersistentObject<WorkflowStep>(nameof(NextStepIfFalse), CascadedRemovalBehavior.RemoveValuesForcibly);

        /// <summary>
        /// Next step in case "true" button was pressed.
        /// </summary>
        public WorkflowStep NextStepIfTrue {
            get { return this.nextStepIfTrue.Value; }
            set { this.nextStepIfTrue.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<WorkflowStep> nextStepIfTrue =
            new PersistentFieldForPersistentObject<WorkflowStep>(nameof(NextStepIfTrue), CascadedRemovalBehavior.RemoveValuesForcibly);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public TrueFalseChoice()
            : base() {
            this.RegisterPersistentField(this.nextStepIfFalse);
            this.RegisterPersistentField(this.nextStepIfTrue);
        }

        /// <summary>
        /// Gets the possible next workflow steps to be executed as
        /// soon as this workflow step is completed.
        /// </summary>
        /// <returns>possible next workflow steps</returns>
        public override IEnumerable<WorkflowStep> GetPossibleNextSteps() {
            yield return this.NextStepIfTrue;
            yield return this.NextStepIfFalse;
        }

        /// <summary>
        /// Replaces a specific possible next step by a new one.
        /// </summary>
        /// <param name="oldValue">old workflow step to be replaced</param>
        /// <param name="newValue">new workflow step to replace old
        /// workflow step by</param>
        /// <returns>true on success, false otherwise</returns>
        protected internal override bool ReplacePossibleNextStep(WorkflowStep oldValue, WorkflowStep newValue) {
            bool success = false;
            if (this.NextStepIfTrue == oldValue) {
                this.NextStepIfTrue = newValue;
                success = true;
            }
            if (this.NextStepIfFalse == oldValue) {
                this.NextStepIfFalse = newValue;
                success = true;
            }
            return success;
        }

    }

}