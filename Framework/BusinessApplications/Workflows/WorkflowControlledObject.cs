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
    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Presentation;
    using System;

    /// <summary>
    /// Base class of object controlled by workflow.
    /// </summary>
    public class WorkflowControlledObject : PersistentObject, IProvidableObject {

        /// <summary>
        /// Workflow instance associated with this object.
        /// </summary>
        public Workflow Workflow {
            get { return this.workflow.Value; }
            set { this.workflow.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<Workflow> workflow =
            new PersistentFieldForPersistentObject<Workflow>(nameof(Workflow), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Name of persistent field
        /// "WorkflowField.CurrentStepField.TypeField".
        /// </summary>
        public static readonly string[] WorkflowCurrentStepTypeNameField = new string[] { nameof(WorkflowControlledObject.Workflow), nameof(Workflows.Workflow.CurrentStep), nameof(WorkflowStep.Type) };

        /// <summary>
        ///  Current workflow instance if this object has elevated
        ///  permissions already, a copy of this workflow with the
        ///  security model set to ignore permissions otherwise.
        /// </summary>
        public Workflow WorkflowWithElevatedPrivileges {
            get {
                var elevatedPersistenceMechanism = this.ParentPersistentContainer.ParentPersistenceMechanism.CopyWithElevatedPrivileges();
                var elevatedWorkflowControlledObjects = elevatedPersistenceMechanism.FindContainer<WorkflowControlledObject>();
                var elevatedWorkflowControlledObject = elevatedWorkflowControlledObjects.FindOne(this.Id);
                Workflow workflowWithElevatedPrivileges;
                if (null == elevatedWorkflowControlledObject) {
                    workflowWithElevatedPrivileges = null;
                } else {
                    workflowWithElevatedPrivileges = elevatedWorkflowControlledObject.Workflow;
                }
                return workflowWithElevatedPrivileges;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public WorkflowControlledObject()
            : base() {
            this.RegisterPersistentField(this.workflow);
            this.IsFullTextQueryable = true;
        }

        /// <summary>
        /// Copies the state of another instance of this type into
        /// this instance except for the workflow.
        /// </summary>
        /// <param name="source">source instance to copy state from</param>
        /// <param name="copyBehaviorForAllowedGroups">determines how
        /// to copy allowed groups</param>
        /// <param name="copyBehaviorForAggregations">determines how
        /// to copy child objects that are not "owned" by their
        /// parent object</param>
        /// <param name="copyBehaviorForCompositions">determines how
        /// to copy child objects that are "owned" by their parent
        /// object</param>
        public void CopyWithoutWorkflowFrom(WorkflowControlledObject source, CopyBehaviorForAllowedGroups copyBehaviorForAllowedGroups, CopyBehaviorForAggregations copyBehaviorForAggregations, CopyBehaviorForCompositions copyBehaviorForCompositions) {
            var sourceWorkflow = source.Workflow;
            var thisWorkflow = this.Workflow;
            try {
                source.Workflow = null;
                this.CopyFrom(source, copyBehaviorForAllowedGroups, copyBehaviorForAggregations, copyBehaviorForCompositions);
                this.Workflow = thisWorkflow;
            } finally {
                source.Workflow = sourceWorkflow;
            }
            return;
        }

        /// <summary>
        /// Gets the title of providable object.
        /// </summary>
        /// <returns>title of providable object</returns>
        public virtual string GetTitle() {
            throw new NotImplementedException("Method " + nameof(GetTitle) + "() has to be implemented in each derived class of " + nameof(WorkflowControlledObject) + ".");
        }

    }

}