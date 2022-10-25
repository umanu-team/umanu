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

namespace Framework.BusinessApplications.Workflows {

    using Framework.BusinessApplications.Workflows.Steps;
    using System;

    /// <summary>
    /// Execution result of a workflow step.
    /// </summary>
    public class WorkflowStepResult {

        /// <summary>
        /// Next scheduled date/time for the Excecute() method of
        /// workflow step to be called automatically.
        /// </summary>
        public DateTime AutoExecutionSchedule { get; private set; }

        /// <summary>
        /// True is execution of workflow step is completed, false
        /// otherwise.
        /// </summary>
        public bool IsExecutionCompleted { get; private set; }

        /// <summary>
        /// Next workflow step to be executed or null if workflow
        /// step sequence is finished.
        /// </summary>
        public WorkflowStep NextStep { get; private set; }

        /// <summary>
        /// Instantiates a new instance - use this constructor to
        /// indicate that the execution of a workflow step is not
        /// finished yet.
        /// </summary>
        public WorkflowStepResult() {
            this.IsExecutionCompleted = false;
        }

        /// <summary>
        /// Instantiates a new instance - use this constructor to
        /// indicate that the execution of a workflow step is not
        /// finished yet.
        /// </summary>
        /// <param name="autoExecutionSchedule">next scheduled
        /// date/time for the Excecute() method of workflow step to
        /// be called automatically</param>
        public WorkflowStepResult(DateTime autoExecutionSchedule)
            : this() {
            this.AutoExecutionSchedule = autoExecutionSchedule;
        }

        /// <summary>
        /// Instantiates a new instance - use this constructor to
        /// indicate that the execution of a workflow step is
        /// finished.
        /// </summary>
        /// <param name="nextStep">next workflow step to be executed
        /// or null if workflow step sequence is finished</param>
        public WorkflowStepResult(WorkflowStep nextStep) {
            this.IsExecutionCompleted = true;
            this.NextStep = nextStep;
        }

    }

}