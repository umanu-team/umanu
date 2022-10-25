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
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using System;

    /// <summary>
    /// Details of a pass of a workflow step instance.
    /// </summary>
    public class HistoryItem : PersistentObject {

        /// <summary>
        /// Timespan that elapsed from end of processing previous
        /// workflow step until end of processing associated workflow
        /// step. Please be aware that durations might look strange
        /// whenever releases are undone.
        /// </summary>
        public TimeSpan Duration {
            get { return TimeSpan.FromTicks(this.duration.Value); }
            set { this.duration.Value = value.Ticks; }
        }
        private readonly PersistentFieldForLong duration =
            new PersistentFieldForLong(nameof(Duration));

        /// <summary>
        /// Time of pass of associated workflow step instance.
        /// </summary>
        public DateTime PassedAt {
            get { return this.passedAt.Value; }
            set { this.passedAt.Value = value; }
        }
        private readonly PersistentFieldForDateTime passedAt =
            new PersistentFieldForDateTime(nameof(PassedAt));

        /// <summary>
        /// User who passed associated workflow step instance.
        /// </summary>
        public IUser PassedBy {
            get { return this.passedBy.Value; }
            set { this.passedBy.Value = value; }
        }
        private readonly PersistentFieldForIUser passedBy =
            new PersistentFieldForIUser(nameof(PassedBy));

        /// <summary>
        /// Workflow step that was executed.
        /// </summary>
        public WorkflowStep WorkflowStep {
            get { return this.workflowStep.Value; }
            set { this.workflowStep.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<WorkflowStep> workflowStep =
            new PersistentFieldForPersistentObject<WorkflowStep>(nameof(WorkflowStep), CascadedRemovalBehavior.DoNotRemoveValues);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public HistoryItem()
            : base() {
            this.RegisterPersistentField(this.duration);
            this.RegisterPersistentField(this.passedAt);
            this.RegisterPersistentField(this.passedBy);
            this.RegisterPersistentField(this.workflowStep);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="workflowStep">workflow step that was
        /// executed</param>
        /// <param name="passedBy">user who passed workflow step</param>
        /// <param name="previousStepPassedAt">time of pass of
        /// previous workflow step instance</param>
        public HistoryItem(WorkflowStep workflowStep, IUser passedBy, DateTime previousStepPassedAt)
            : this() {
            this.PassedAt = UtcDateTime.Now;
            this.Duration = this.PassedAt - previousStepPassedAt;
            this.PassedBy = passedBy;
            this.WorkflowStep = workflowStep;
        }

    }

}