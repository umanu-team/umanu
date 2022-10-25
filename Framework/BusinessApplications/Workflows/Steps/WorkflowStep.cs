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

    using Framework.BusinessApplications.Buttons;
    using Framework.BusinessApplications.DataProviders;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Instance of a workflow step. Usually there it should not be
    /// necessary to derive from this step. Please derive from
    /// ActionStep or ChoiceStep instead.
    /// </summary>
    public class WorkflowStep : WorkflowState {

        /// <summary>
        /// True if workflow step is supposed to show the average
        /// duration in graphical representations of workflows, false
        /// otherwise.
        /// </summary>
        public virtual bool IsDisplayingAverageDuration {
            get { return false; }
        }

        /// <summary>
        /// True if workflow step is supposed to be shown in
        /// graphical representations of workflow, false if it is a
        /// hidden workflow step.
        /// </summary>
        public virtual bool IsVisible {
            get { return true; }
        }

        /// <summary>
        /// Type.Name of workflow step.
        /// </summary>
        private readonly PersistentFieldForString type =
            new PersistentFieldForString(nameof(Type));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public WorkflowStep()
            : base() {
            this.type.Value = this.Type.Name;
            this.type.IsFullTextIndexed = false;
            this.RegisterPersistentField(this.type);
        }

        /// <summary>
        /// Adds a workflow step after all last workflow steps.
        /// </summary>
        /// <param name="workflowStep">workflow step to add after all
        /// all last workflow steps</param>
        internal void AddStepAfterLastSteps(WorkflowStep workflowStep) {
            this.AddStepAfterLastSteps(workflowStep, new List<Guid>());
            return;
        }

        /// <summary>
        /// Adds a workflow step after all last workflow steps.
        /// </summary>
        /// <param name="workflowStep">workflow step to add after
        /// all last workflow steps</param>
        /// <param name="checkedStepIDs">IDs of the steps that were
        /// already checked</param>
        internal void AddStepAfterLastSteps(WorkflowStep workflowStep, IList<Guid> checkedStepIDs) {
            bool hasPossibleNextSteps = false;
            foreach (var nextStep in this.GetPossibleNextSteps()) {
                if (null != nextStep) {
                    hasPossibleNextSteps = true;
                    if (!checkedStepIDs.Contains(nextStep.Id)) {
                        checkedStepIDs.Add(nextStep.Id);
                        nextStep.AddStepAfterLastSteps(workflowStep, checkedStepIDs);
                    }
                }
            }
            if (!hasPossibleNextSteps) {
                if (this is ActionStep actionStep) {
                    actionStep.NextStep = workflowStep;
                } else {
                    throw new WorkflowException("A new workflow step cannot be added to a workflow step of type " + this.Type + " automatically.");
                }
            }
            return;
        }

        /// <summary>
        /// Adds multiple workflow steps after all last workflow
        /// steps.
        /// </summary>
        /// <param name="workflowSteps">workflow steps to add after
        /// all last workflow steps</param>
        public void AddStepsAfterLastSteps(IEnumerable<WorkflowStep> workflowSteps) {
            foreach (var workflowStep in workflowSteps) {
                this.AddStepAfterLastSteps(workflowStep);
            }
            return;
        }

        /// <summary>
        /// Gets called on click of a button of the workflow.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="sender">button that was clicked</param>
        /// <param name="promptInput">prompt input of user</param>
        /// <returns>result of execution of button click</returns>
        /// <typeparam name="T">type of objects server side workflow
        /// button is for</typeparam>
        internal virtual WorkflowStepResult ButtonClick<T>(WorkflowControlledObject associatedObject, ServerSideWorkflowButton<T> sender, string promptInput) where T : class, IProvidableObject {
            WorkflowStepResult result;
            var button = this.FindButton(associatedObject, sender.DataProvider, sender) as ServerSideWorkflowButton<T>;
            if (null == button) {
                result = new WorkflowStepResult();
            } else {
                result = button.HandleClick(promptInput);
            }
            return result;
        }

        /// <summary>
        /// Cancels this workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        internal virtual void Cancel(WorkflowControlledObject associatedObject) {
            return;
        }

        /// <summary>
        /// Executes this workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>result of execution of workflow step</returns>
        protected internal virtual WorkflowStepResult Execute(WorkflowControlledObject associatedObject) {
            return new WorkflowStepResult();
        }

        /// <summary>
        /// Finds a button.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <param name="sender">button to find</param>
        /// <returns>button to find or null</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        private ActionButton FindButton<T>(WorkflowControlledObject associatedObject, DataProvider<T> dataProvider, ActionButton sender) where T : class, IProvidableObject {
            ActionButton result = null;
            foreach (var button in this.GetAdditionalViewFormButtons(associatedObject, dataProvider)) {
                if (button.Equals(sender)) {
                    result = button;
                    break;
                }
            }
            if (null == result) {
                foreach (var button in this.GetAdditionalEditFormButtons(associatedObject, dataProvider)) {
                    if (button.Equals(sender)) {
                        result = button;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the closest common next step with another workflow
        /// step.
        /// </summary>
        /// <param name="workflowStep">workflow step to find common
        /// closest next step for</param>
        /// <returns>closest common next step with other workflow
        /// step as key, distance to it as value</returns>
        internal KeyValuePair<WorkflowStep, long> FindClosestCommonNextStepWith(WorkflowStep workflowStep) {
            var closestCommonNextStep = new KeyValuePair<WorkflowStep, long>(null, long.MaxValue);
            if (null != workflowStep) {
                if (this == workflowStep || this.IsSuccessorOf(workflowStep)) {
                    closestCommonNextStep = new KeyValuePair<WorkflowStep, long>(this, 0);
                } else {
                    foreach (var possibleNextStep in this.GetPossibleNextSteps()) {
                        if (null != possibleNextStep) {
                            var closestNextStep = possibleNextStep.FindClosestCommonNextStepWith(workflowStep);
                            if (closestNextStep.Value < closestCommonNextStep.Value) {
                                closestCommonNextStep = new KeyValuePair<WorkflowStep, long>(closestNextStep.Key, closestNextStep.Value + 1);
                            }
                        }
                    }
                }
            }
            return closestCommonNextStep;
        }

        /// <summary>
        /// Finds all instances of history items for a specific
        /// workflow step.
        /// </summary>
        /// <param name="workflowStep">workflow step to find history
        /// items for</param>
        /// <returns>all instances of history items for specific
        /// workflow step</returns>
        internal virtual IEnumerable<HistoryItem> FindHistoryItemsFor(WorkflowStep workflowStep) {
            yield break;
        }

        /// <summary>
        /// Finds the leaf workflow step sequence of a button.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="sender">button to find leaf workflow step
        /// sequence of</param>
        /// <returns>leaf workflow step sequence of button or null</returns>
        internal virtual WorkflowStepSequence FindLeafWorkflowStepSequenceOf(WorkflowControlledObject associatedObject, AdHocWorkflowButton sender) {
            return null;
        }

        /// <summary>
        /// Finds all instances of workflow steps of a specific type.
        /// </summary>
        /// <param name="type">type of workflow steps to find</param>
        /// <returns>instances of workflow steps of specific type</returns>
        internal virtual IEnumerable<WorkflowStep> FindWorkflowStepsOfType(Type type) {
            yield break;
        }

        /// <summary>
        /// Gets the average processing duration of workflow steps of
        /// same type. Please be aware that durations might look
        /// strange whenever releases are undone.
        /// </summary>
        /// <returns>average processing duration of workflow steps of
        /// same type or null if no historical data is present</returns>
        public TimeSpan? GetAverageDuration() {
            TimeSpan? averageDuration = null;
            if (this.IsDisplayingAverageDuration && !this.IsNew) {
                var filterCriteria = new FilterCriteria(nameof(HistoryItem.WorkflowStep) + '_', RelationalOperator.IsEqualTo, this.Type.AssemblyQualifiedName, FilterTarget.IsOtherTextValue);
                var historyItems = this.ParentPersistentContainer.ParentPersistenceMechanism.FindContainer<HistoryItem>();
                var averageDurationTicks = historyItems.FindAverageValue(filterCriteria, new string[] { nameof(HistoryItem.Duration) });
                if (averageDurationTicks[0] is long ticks) {
                    averageDuration = TimeSpan.FromTicks(ticks);
                }
            }
            return averageDuration;
        }

        /// <summary>
        /// Gets the URL of icon to display for workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        protected internal virtual string GetIconUrl(WorkflowControlledObject associatedObject) {
            return string.Empty;
        }

        /// <summary>
        /// Gets the possible next workflow steps to be executed as
        /// soon as this workflow step is completed.
        /// </summary>
        /// <returns>possible next workflow steps</returns>
        public virtual IEnumerable<WorkflowStep> GetPossibleNextSteps() {
            throw new WorkflowException("Please implement method \"GetPossibleNextSteps()\" in every derived class.");
        }

        /// <summary>
        /// Gets the display title of workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        protected internal virtual string GetTitle(WorkflowControlledObject associatedObject) {
            throw new NotSupportedException("Please override method \"GetTitle(WorkflowControlledObject)\" in every derived class.");
        }

        /// <summary>
        /// Gets the undo buttons of workflow step. They are provided
        /// as additional view form buttons for all steps succeeding
        /// this one.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>undo buttons of workflow step to be shown as
        /// additional view form buttons in all succeeding steps</returns>
        protected virtual IEnumerable<UndoWorkflowStepButton> GetUndoButtons(WorkflowControlledObject associatedObject) {
            yield break;
        }

        /// <summary>
        /// Gets the undo buttons of workflow step. They are provided
        /// as additional view form buttons for all steps succeeding
        /// this one. The IDs of workflow steps to be undone are
        /// assigned to all undo buttons by this method.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>undo buttons of workflow step to be shown as
        /// additional view form buttons in all succeeding steps</returns>
        internal virtual IEnumerable<UndoWorkflowStepButton> GetUndoButtonsWithIdsOfWorkflowStepsToBeUndone(WorkflowControlledObject associatedObject) {
            foreach (var undoButton in this.GetUndoButtons(associatedObject)) {
                undoButton.IdOfWorkflowStepToBeUndone = this.Id;
                yield return undoButton;
            }
        }

        /// <summary>
        /// Indicates whether workflow step has a direct association
        /// to a specified button.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="sender">button to find</param>
        /// <returns>true if workflow step has direct association to
        /// button, false otherwise</returns>
        protected internal virtual bool HasDirectAssociationTo(WorkflowControlledObject associatedObject, AdHocWorkflowButton sender) {
            return null != this.FindButton(associatedObject, new DummyDataProvider<IProvidableObject>(), sender);
        }

        /// <summary>
        /// Determines whether a specific step is a possible direct
        /// next step of this step.
        /// </summary>
        /// <param name="workflowStep">specific workflow step to
        /// check</param>
        /// <returns>true if specific step is a possible direct next
        /// step of this step, false otherwise</returns>
        internal bool IsDirectPredecessorOf(WorkflowStep workflowStep) {
            bool isThisDirectPredecessorOfWorkflowStep = null == workflowStep;
            if (!isThisDirectPredecessorOfWorkflowStep) {
                foreach (var possibleNextStep in this.GetPossibleNextSteps()) {
                    if (null != possibleNextStep) {
                        if (possibleNextStep.Id == workflowStep.Id) {
                            isThisDirectPredecessorOfWorkflowStep = true;
                            break;
                        }
                    }
                }
            }
            return isThisDirectPredecessorOfWorkflowStep;
        }

        /// <summary>
        /// Determines whether this step is a successor of a specific
        /// step.
        /// </summary>
        /// <param name="workflowStep">specific workflow step to
        /// check</param>
        /// <returns>true if this step is a successor of the
        /// specified step, false otherwise</returns>
        internal bool IsSuccessorOf(WorkflowStep workflowStep) {
            return this.IsSuccessorOf(workflowStep, new List<Guid>());
        }

        /// <summary>
        /// Determines whether this step is a successor of a specific
        /// step.
        /// </summary>
        /// <param name="workflowStep">specific workflow step to
        /// check</param>
        /// <param name="checkedStepIDs">IDs of the steps that were
        /// already checked</param>
        /// <returns>true if this step is a successor of the
        /// specified step, false otherwise</returns>
        private bool IsSuccessorOf(WorkflowStep workflowStep, IList<Guid> checkedStepIDs) {
            bool isThisSuccessorOfWorkflowStep = false;
            if (null != workflowStep && this != workflowStep && !checkedStepIDs.Contains(workflowStep.Id)) {
                checkedStepIDs.Add(workflowStep.Id);
                foreach (var possibleNextStep in workflowStep.GetPossibleNextSteps()) {
                    if (null != possibleNextStep) {
                        if (possibleNextStep.Id == this.Id) {
                            isThisSuccessorOfWorkflowStep = true;
                        } else {
                            isThisSuccessorOfWorkflowStep = this.IsSuccessorOf(possibleNextStep, checkedStepIDs);
                        }
                        if (isThisSuccessorOfWorkflowStep) {
                            break;
                        }
                    }
                }
            }
            return isThisSuccessorOfWorkflowStep;
        }

        /// <summary>
        /// Adjusts/exchanges the view form for items for current
        /// worflow step.
        /// </summary>
        /// <param name="viewFormView">view form for items to be
        /// processed</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        protected internal override void ProcessViewFormView(ref FormView viewFormView, WorkflowControlledObject associatedObject) {
            base.ProcessViewFormView(ref viewFormView, associatedObject);
            if (null != viewFormView && string.IsNullOrEmpty(viewFormView.DescriptionForViewMode)) {
                string title = this.GetTitle(associatedObject);
                if (!string.IsNullOrEmpty(title)) {
                    viewFormView.DescriptionForViewMode = string.Format(Resources.TheWorkflowWillGoOnAsSoonAsTheStep0IsFinished, title);
                }
            }
            return;
        }

        /// <summary>
        /// Replaces a specific possible next step by a new one.
        /// </summary>
        /// <param name="oldValue">old workflow step to be replaced</param>
        /// <param name="newValue">new workflow step to replace old
        /// workflow step by</param>
        /// <returns>true on success, false otherwise</returns>
        protected internal virtual bool ReplacePossibleNextStep(WorkflowStep oldValue, WorkflowStep newValue) {
            throw new WorkflowException("Please implement method \"ReplacePossibleNextStep(WorkflowStep, WorkflowStep)\" in every derived class.");
        }

        /// <summary>
        /// Resets the workflow step.
        /// </summary>
        public virtual void Reset() {
            return;
        }

    }

}