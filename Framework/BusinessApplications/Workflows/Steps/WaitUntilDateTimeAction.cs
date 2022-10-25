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

    using Framework.Model;
    using Framework.Persistence.Fields;
    using System;

    /// <summary>
    /// Waits until a specific date and time as a workflow step.
    /// </summary>
    public abstract class WaitUntilDateTimeAction : ActionStep {

        /// <summary>
        /// Date and time to wait until.
        /// </summary>
        public DateTime WaitUntil {
            get { return this.waitUntil.Value; }
            set { this.waitUntil.Value = value; }
        }
        private readonly PersistentFieldForDateTime waitUntil =
            new PersistentFieldForDateTime(nameof(WaitUntil), UtcDateTime.MaxValue);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public WaitUntilDateTimeAction()
            : base() {
            this.RegisterPersistentField(this.waitUntil);
        }

        /// <summary>
        /// Executes this workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        /// <returns>result of execution of workflow step</returns>
        protected internal override WorkflowStepResult Execute(WorkflowControlledObject associatedObject) {
            WorkflowStepResult result;
            if (UtcDateTime.Now >= this.WaitUntil) {
                result = new WorkflowStepResult(this.NextStep);
            } else {
                result = new WorkflowStepResult(this.WaitUntil);
            }
            return result;
        }

    }

}