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

    using Framework.BusinessApplications.Buttons;
    using Framework.BusinessApplications.Workflows.Steps;
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Sequence of workflow steps.
    /// </summary>
    public class WorkflowStepSequence : PersistentObject {

        /// <summary>
        /// Next scheduled date/time for the Excecute() method of
        /// this workflow to be called automatically.
        /// </summary>
        public DateTime AutoExecutionSchedule {
            get { return this.autoExecutionSchedule.Value; }
            private set { this.autoExecutionSchedule.Value = value; }
        }
        private readonly PersistentFieldForDateTime autoExecutionSchedule =
            new PersistentFieldForDateTime(nameof(AutoExecutionSchedule), UtcDateTime.Now.AddHours(1));

        /// <summary>
        /// Current workflow step.
        /// </summary>
        public WorkflowStep CurrentStep {
            get { return this.currentStep.Value; }
            protected set { this.currentStep.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<WorkflowStep> currentStep =
            new PersistentFieldForPersistentObject<WorkflowStep>(nameof(CurrentStep), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// First workflow step.
        /// </summary>
        public StartAction FirstStep {
            get {
                return this.firstStep.Value;
            }
            private set {
                this.CurrentStep = this.firstStep.Value = value;
            }
        }
        private readonly PersistentFieldForPersistentObject<StartAction> firstStep =
            new PersistentFieldForPersistentObject<StartAction>(nameof(FirstStep), CascadedRemovalBehavior.RemoveValuesForcibly);

        /// <summary>
        /// History of all passes of this workflow step instance.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<HistoryItem> History { get; private set; }

        /// <summary>
        /// True if this workflow instance was canceled, false otherwise.
        /// </summary>
        public bool IsCanceled {
            get { return this.isCanceled.Value; }
            private set { this.isCanceled.Value = value; }
        }
        private readonly PersistentFieldForBool isCanceled =
            new PersistentFieldForBool(nameof(IsCanceled), false);

        /// <summary>
        /// True if the current step of this workflow instance is a
        /// workflow end step, false otherwise.
        /// </summary>
        public bool IsCompleted {
            get {
                return null == this.CurrentStep;
            }
        }

        /// <summary>
        /// Last step of workflow to be executed when the workflow ends or
        /// is canceled.
        /// </summary>
        public LastStep LastStep {
            get { return this.lastStep.Value; }
            set { this.lastStep.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<LastStep> lastStep =
            new PersistentFieldForPersistentObject<LastStep>(nameof(LastStep), CascadedRemovalBehavior.RemoveValuesForcibly);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public WorkflowStepSequence()
            : base() {
            this.RegisterPersistentField(this.autoExecutionSchedule);
            this.RegisterPersistentField(this.currentStep);
            this.FirstStep = new StartAction();
            this.RegisterPersistentField(this.firstStep);
            this.History = new PersistentFieldForPersistentObjectCollection<HistoryItem>(nameof(this.History), CascadedRemovalBehavior.RemoveValuesForcibly);
            this.RegisterPersistentField(this.History);
            this.RegisterPersistentField(this.isCanceled);
            this.RegisterPersistentField(this.lastStep);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="workflowStep">workflow step to initialize
        /// workflow step sequence with</param>
        public WorkflowStepSequence(WorkflowStep workflowStep)
            : this() {
            this.AddStepAfterLastSteps(workflowStep);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="workflowSteps">workflow steps to initialize
        /// workflow step sequence with</param>
        public WorkflowStepSequence(IEnumerable<WorkflowStep> workflowSteps)
            : this() {
            this.AddStepsAfterLastSteps(workflowSteps);
        }

        /// <summary>
        /// Adds a workflow step after all last workflow steps.
        /// </summary>
        /// <param name="workflowStep">workflow step to add after all
        /// all last workflow steps</param>
        public void AddStepAfterLastSteps(WorkflowStep workflowStep) {
            this.FirstStep.AddStepAfterLastSteps(workflowStep);
            return;
        }

        /// <summary>
        /// Adds multiple workflow steps after all last workflow
        /// steps.
        /// </summary>
        /// <param name="workflowSteps">workflow steps to add after
        /// all last workflow steps</param>
        public void AddStepsAfterLastSteps(IEnumerable<WorkflowStep> workflowSteps) {
            this.FirstStep.AddStepsAfterLastSteps(workflowSteps);
            return;
        }

        /// <summary>
        /// Applies a workflow step result to this workflow step
        /// sequence.
        /// </summary>
        /// <param name="result">workflow step result to be applied</param>
        private void ApplyWorkflowStepResult(WorkflowStepResult result) {
            if (result.IsExecutionCompleted) {
                if (this.CurrentStep.IsDirectPredecessorOf(result.NextStep)) {
                    this.History.Add(new HistoryItem(this.CurrentStep, this.ParentPersistentContainer.ParentPersistenceMechanism.UserDirectory.CurrentUser, this.GetPreviousStepPassedAt()));
                    this.CurrentStep = result.NextStep;
                    if (null != this.CurrentStep) {
                        this.CurrentStep.Reset();
                    }
                    this.AutoExecutionSchedule = UtcDateTime.MaxValue;
                } else {
                    throw new WorkflowException("Workflow step to be set as next step is not a possible next step of current step.");
                }
            } else {
                this.AutoExecutionSchedule = result.AutoExecutionSchedule;
            }
            return;
        }

        /// <summary>
        /// Gets called on click of a button of this workflow.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        /// <param name="sender">button that was clicked</param>
        /// <param name="promptInput">prompt input of user</param>
        /// <typeparam name="T">type of objects server side workflow
        /// button is for</typeparam>
        internal void ButtonClick<T>(WorkflowControlledObject associatedObject, ServerSideWorkflowButton<T> sender, string promptInput) where T : class, IProvidableObject {
            if (this.IsCompleted) {
                if (null != this.LastStep) {
                    this.LastStep.ButtonClick(associatedObject, sender, promptInput);
                }
            } else {
                var result = this.CurrentStep.ButtonClick(associatedObject, sender, promptInput);
                if (result.IsExecutionCompleted || new WorkflowStepResult().AutoExecutionSchedule != result.AutoExecutionSchedule) {
                    this.ApplyWorkflowStepResult(result);
                    this.UpdateCascadedly();
                }
            }
            return;
        }

        /// <summary>
        /// Cancels this workflow step sequence.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        internal virtual void Cancel(WorkflowControlledObject associatedObject) {
            if (!this.IsCompleted) {
                this.CurrentStep.Cancel(associatedObject);
                this.CurrentStep = null;
                this.AutoExecutionSchedule = UtcDateTime.MaxValue;
                this.IsCanceled = true;
                if (null != this.LastStep) {
                    this.LastStep.Execute(associatedObject);
                }
                this.UpdateCascadedly();
            }
            return;
        }

        /// <summary>
        /// Executes this sequence of workflow steps if it is not
        /// completed yet. At the end it is going to be updated in
        /// persistence mechanism.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        /// <returns>workflow step pass if this workflow step is
        /// completed, null otherwise</returns>
        internal virtual void Execute(WorkflowControlledObject associatedObject) {
            try {
                WorkflowStepResult result;
                do {
                    if (this.IsCompleted) {
                        result = new WorkflowStepResult();
                        if (null != this.LastStep) {
                            this.LastStep.Execute(associatedObject);
                        }
                    } else {
                        result = this.CurrentStep.Execute(associatedObject);
                        this.ApplyWorkflowStepResult(result);
                    }
                } while (result.IsExecutionCompleted);
            } finally {
                this.UpdateCascadedly();
            }
            return;
        }

        /// <summary>
        /// Finds all active workflow steps of workflow step
        /// sequence.
        /// </summary>
        /// <returns>all active workflow steps of workflow step
        /// sequence</returns>
        public IEnumerable<WorkflowStep> FindActiveWorkflowSteps() {
            if (!this.IsCompleted) {
                yield return this.CurrentStep;
                if (this.CurrentStep is ParallelismAction currentStepAsParallelismAction) {
                    foreach (var parallelWorkflowStepSequence in currentStepAsParallelismAction.ParallelWorkflowStepSequences) {
                        if (null != parallelWorkflowStepSequence) {
                            foreach (var activeStep in parallelWorkflowStepSequence.FindActiveWorkflowSteps()) {
                                yield return activeStep;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds all completed preceding workflow steps of workflow
        /// step sequence.
        /// </summary>
        /// <returns>all completed preceding workflow steps of
        /// workflow step sequence</returns>
        public IEnumerable<WorkflowStep> FindCompletedPrecedingWorkflowSteps() {
            if (this.CurrentStep is ParallelismAction currentStepAsParallelismAction) {
                foreach (var parallelWorkflowStepSequence in currentStepAsParallelismAction.ParallelWorkflowStepSequences) {
                    if (null != parallelWorkflowStepSequence) {
                        foreach (var completedStep in parallelWorkflowStepSequence.FindCompletedPrecedingWorkflowSteps()) {
                            yield return completedStep;
                        }
                    }
                }
            }
            var historyItems = new List<HistoryItem>(this.History);
            historyItems.Reverse();
            foreach (var historyItem in historyItems) {
                if (false != this.CurrentStep?.IsSuccessorOf(historyItem.WorkflowStep)) {
                    if (historyItem.WorkflowStep is ParallelismAction parallelismAction) {
                        foreach (var parallelWorkflowStepSequence in parallelismAction.ParallelWorkflowStepSequences) {
                            if (null != parallelWorkflowStepSequence) {
                                foreach (var completedStep in parallelWorkflowStepSequence.FindCompletedPrecedingWorkflowSteps()) {
                                    yield return completedStep;
                                }
                            }
                        }
                    } else if (null != historyItem.WorkflowStep) {
                        yield return historyItem.WorkflowStep;
                    }
                }
            }
        }

        /// <summary>
        /// Finds all instances of history items for a specific
        /// workflow step.
        /// </summary>
        /// <param name="workflowStep">workflow step to find history
        /// items for</param>
        /// <returns>all instances of history items for specific
        /// workflow step</returns>
        public IEnumerable<HistoryItem> FindHistoryItemsFor(WorkflowStep workflowStep) {
            foreach (var historyItem in this.History) {
                if (null != historyItem.WorkflowStep) {
                    if (historyItem.WorkflowStep.Id == workflowStep.Id) {
                        yield return historyItem;
                    } else {
                        foreach (var historyItemOfWorkflowStep in historyItem.WorkflowStep.FindHistoryItemsFor(workflowStep)) {
                            yield return historyItemOfWorkflowStep;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the last history item of last instance of a
        /// workflow step of a specific type which is closest to the
        /// end of the workflow step sequence.
        /// </summary>
        /// <param name="type">type of workflow step to find</param>
        /// <returns>last history item of last instance of workflow
        /// step of specific type which is closest to the end of the
        /// workflow step sequence null if it could not be found</returns>
        public HistoryItem FindLastHistoryItemForLastWorkflowStepOfType(Type type) {
            HistoryItem lastHistoryItem = null;
            var lastWorfklowStepOfType = this.FindLastWorkflowStepOfType(type);
            if (null != lastWorfklowStepOfType) {
                foreach (var historyItem in this.FindHistoryItemsFor(lastWorfklowStepOfType)) {
                    lastHistoryItem = historyItem;
                }
            }
            return lastHistoryItem;
        }

        /// <summary>
        /// Finds the instance of a workflow step of a specific type
        /// which is closest to the end of the workflow step
        /// sequence.
        /// </summary>
        /// <param name="type">type of workflow step to find</param>
        /// <returns>instance of workflow step of specific type which
        /// is closest to the end of the workflow step sequence or
        /// null if no instance could be found</returns>
        public WorkflowStep FindLastWorkflowStepOfType(Type type) {
            WorkflowStep foundWorkflowStep = null;
            foreach (var workflowStep in this.FindWorkflowStepsOfType(type)) {
                foundWorkflowStep = workflowStep;
            }
            return foundWorkflowStep;
        }

        /// <summary>
        /// Finds all instances of workflow steps of a specific type.
        /// </summary>
        /// <param name="type">type of workflow steps to find</param>
        /// <returns>instances of workflow steps of specific type</returns>
        public IEnumerable<WorkflowStep> FindWorkflowStepsOfType(Type type) {
            return this.FindWorkflowStepsOfType(type, true);
        }

        /// <summary>
        /// Finds all instances of workflow steps of a specific type.
        /// </summary>
        /// <param name="type">type of workflow steps to find</param>
        /// <param name="isCascadedSearchEnabled">indicates whether
        /// cascaded search in parallel workflow steps is enabled</param>
        /// <returns>instances of workflow steps of specific type</returns>
        private IEnumerable<WorkflowStep> FindWorkflowStepsOfType(Type type, bool isCascadedSearchEnabled) {
            return this.FindWorkflowStepsOfType(type, this.FirstStep, isCascadedSearchEnabled, new List<Guid>());
        }

        /// <summary>
        /// Finds all instances of workflow steps of a specific type.
        /// </summary>
        /// <param name="type">type of workflow steps to find</param>
        /// <param name="workflowStep">workflow step to start search
        /// at</param>
        /// <param name="isCascadedSearchEnabled">indicates whether
        /// cascaded search in parallel workflow steps is enabled</param>
        /// <param name="idsOfFoundWorkflowSteps">list of IDs of
        /// found workflow steps</param>
        /// <returns>instances of workflow steps of specific type</returns>
        private IEnumerable<WorkflowStep> FindWorkflowStepsOfType(Type type, WorkflowStep workflowStep, bool isCascadedSearchEnabled, IList<Guid> idsOfFoundWorkflowSteps) {
            if (!idsOfFoundWorkflowSteps.Contains(workflowStep.Id)) {
                idsOfFoundWorkflowSteps.Add(workflowStep.Id);
                if (workflowStep.Type == type || workflowStep.Type.IsSubclassOf(type)) {
                    yield return workflowStep;
                }
                if (isCascadedSearchEnabled) {
                    foreach (var foundWorkflowStepOfChildStep in workflowStep.FindWorkflowStepsOfType(type)) {
                        yield return foundWorkflowStepOfChildStep;
                    }
                }
                foreach (var possibleNextStep in workflowStep.GetPossibleNextSteps()) {
                    if (null != possibleNextStep) {
                        foreach (var foundWorkflowStepOfPossibleNextStep in this.FindWorkflowStepsOfType(type, possibleNextStep, isCascadedSearchEnabled, idsOfFoundWorkflowSteps)) {
                            yield return foundWorkflowStepOfPossibleNextStep;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the leaf workflow step sequence of a button.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="sender">button to find leaf workflow step
        /// sequence of</param>
        /// <returns>leaf workflow step sequence of button or null</returns>
        internal WorkflowStepSequence FindLeafWorkflowStepSequenceOf(WorkflowControlledObject associatedObject, AdHocWorkflowButton sender) {
            WorkflowStepSequence leafWorkflowStepSequence;
            if (this.IsCompleted) {
                leafWorkflowStepSequence = null;
            } else {
                if (this.CurrentStep.HasDirectAssociationTo(associatedObject, sender)) {
                    leafWorkflowStepSequence = this;
                } else {
                    leafWorkflowStepSequence = this.CurrentStep.FindLeafWorkflowStepSequenceOf(associatedObject, sender);
                }
            }
            return leafWorkflowStepSequence;
        }

        /// <summary>
        /// Gets the additional buttons to be shown on edit form of
        /// items.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>additional buttons to be shown on edit form of
        /// items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        internal IEnumerable<ActionButton> GetAdditionalEditFormButtons<T>(WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            var viewFormButtons = new List<ActionButton>();
            this.ProcessEditFormButtons(viewFormButtons, associatedObject, dataProvider);
            return viewFormButtons;
        }

        /// <summary>
        /// Gets the additional buttons to be shown on view form of
        /// items.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>additional buttons to be shown on view form of
        /// items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        internal IEnumerable<ActionButton> GetAdditionalViewFormButtons<T>(WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            var viewFormButtons = new List<ActionButton>();
            this.ProcessViewFormButtons(viewFormButtons, associatedObject, dataProvider);
            return viewFormButtons;
        }

        /// <summary>
        /// Gets the object which is associated to this workflow to
        /// be used in forms.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        internal void GetAssociatedObjectForForms(WorkflowControlledObject associatedObject) {
            if (this.IsCompleted) {
                if (null != this.LastStep) {
                    this.LastStep.GetAssociatedObjectForForms(associatedObject);
                }
            } else {
                this.CurrentStep.GetAssociatedObjectForForms(associatedObject);
            }
            return;
        }

        /// <summary>
        /// Gets the URL of icon to display for current workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        internal string GetIconUrl(WorkflowControlledObject associatedObject) {
            string iconUrl;
            if (this.IsCompleted) {
                iconUrl = null;
            } else {
                iconUrl = this.CurrentStep.GetIconUrl(associatedObject);
            }
            return iconUrl;
        }

        /// <summary>
        /// Gets the date/time previous step in history was passed
        /// at.
        /// </summary>
        /// <returns>date/time previous step in history was passed at</returns>
        private DateTime GetPreviousStepPassedAt() {
            DateTime previousStepPassedAt;
            if (this.History.Count > 0) {
                previousStepPassedAt = this.History[this.History.Count - 1].PassedAt;
            } else {
                previousStepPassedAt = UtcDateTime.Now;
            }
            return previousStepPassedAt;
        }

        /// <summary>
        /// Gets the status of a workflow step.
        /// </summary>
        /// <param name="workflowStep">workflow step to get status of</param>
        /// <returns>status of workflow step</returns>
        public WorkflowStepStatus GetStatusOf(WorkflowStep workflowStep) {
            var workflowStepStatus = WorkflowStepStatus.Initial;
            foreach (var activeWorkflowStep in this.FindActiveWorkflowSteps()) {
                if (workflowStep.Id == activeWorkflowStep.Id) {
                    workflowStepStatus = WorkflowStepStatus.Active;
                    break;
                }
            }
            if (WorkflowStepStatus.Initial == workflowStepStatus) {
                foreach (var completedWorkflowStep in this.FindCompletedPrecedingWorkflowSteps()) {
                    if (workflowStep.Id == completedWorkflowStep.Id) {
                        workflowStepStatus = WorkflowStepStatus.Completed;
                        break;
                    }
                }
            }
            return workflowStepStatus;
        }

        /// <summary>
        /// Gets the status of the instance of a workflow step of a
        /// specific type which is closest to the end of the workflow
        /// step sequence.
        /// </summary>
        /// <param name="type">type of workflow step to get status of</param>
        /// <returns>status of the instance of workflow step of
        /// specific type which is closest to the end of the workflow
        /// step sequence or null if no instance could be found</returns>
        public WorkflowStepStatus? GetStatusOfLastWorkflowStepOfType(Type type) {
            WorkflowStepStatus? workflowStepStatus;
            var workflowStep = this.FindLastWorkflowStepOfType(type);
            if (null == workflowStep) {
                workflowStepStatus = null;
            } else {
                workflowStepStatus = this.GetStatusOf(workflowStep);
            }
            return workflowStepStatus;
        }

        /// <summary>
        /// Gets the display title of current workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        internal string GetTitle(WorkflowControlledObject associatedObject) {
            string title;
            if (this.IsCompleted) {
                title = null;
            } else {
                title = this.CurrentStep.GetTitle(associatedObject);
            }
            return title;
        }

        /// <summary>
        /// Gets the current undo buttons of workflow step sequence.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>current undo buttons of workflow step sequence</returns>
        internal IEnumerable<UndoWorkflowStepButton> GetUndoButtonsWithIdsOfWorkflowStepsToBeUndone(WorkflowControlledObject associatedObject) {
            var idsOfWorkflowStepsToBeUndone = new List<Guid>();
            if (this.CurrentStep is ParallelismAction) {
                foreach (var undoButton in this.CurrentStep.GetUndoButtonsWithIdsOfWorkflowStepsToBeUndone(associatedObject)) {
                    if (!idsOfWorkflowStepsToBeUndone.Contains(undoButton.IdOfWorkflowStepToBeUndone)) {
                        idsOfWorkflowStepsToBeUndone.Add(undoButton.IdOfWorkflowStepToBeUndone);
                        yield return undoButton;
                    }
                }
            }
            if (this.History.Count > 0) {
                var historyItems = new List<HistoryItem>(this.History);
                historyItems[0].ParentPersistentContainer.Preload(historyItems, new string[][] {
                    new string[] { nameof(HistoryItem.WorkflowStep), nameof(WorkflowStep.Id) }
                });
                historyItems.Reverse();
                foreach (var historyItem in historyItems) {
                    if (null != historyItem.WorkflowStep && false != this.CurrentStep?.IsSuccessorOf(historyItem.WorkflowStep)) {
                        foreach (var undoButton in historyItem.WorkflowStep.GetUndoButtonsWithIdsOfWorkflowStepsToBeUndone(associatedObject)) {
                            if (!idsOfWorkflowStepsToBeUndone.Contains(undoButton.IdOfWorkflowStepToBeUndone)) {
                                idsOfWorkflowStepsToBeUndone.Add(undoButton.IdOfWorkflowStepToBeUndone);
                                yield return undoButton;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the current step of workflow to another step of
        /// workflow.
        /// </summary>
        /// <param name="workflowStep">new current step</param>
        /// <returns>true if current step could be changed to new
        /// step, false otherwise</returns>
        public virtual bool GoTo(WorkflowStep workflowStep) {
            var success = false;
            if (workflowStep == this.CurrentStep) {
                success = true;
            } else if (workflowStep.Id == this.FirstStep.Id || workflowStep.IsSuccessorOf(this.FirstStep)) {
                this.CurrentStep?.Reset();
                this.CurrentStep = workflowStep;
                this.AutoExecutionSchedule = UtcDateTime.MaxValue;
                success = true;
            } else {
                foreach (ParallelismAction parallelismAction in this.FindWorkflowStepsOfType(typeof(ParallelismAction), false)) {
                    foreach (var parallelWorkflowStepSequence in parallelismAction.ParallelWorkflowStepSequences) {
                        success = parallelWorkflowStepSequence.GoTo(workflowStep);
                        if (success) {
                            if (parallelismAction != this.CurrentStep) {
                                this.CurrentStep?.Reset();
                                this.CurrentStep = parallelismAction;
                                this.AutoExecutionSchedule = UtcDateTime.MaxValue;
                            }
                            break;
                        }
                    }
                    if (success) {
                        break;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Inserts an ad-hoc workflow after current step of workflow
        /// step sequence.
        /// </summary>
        /// <param name="adHocWorkflow">ad-hoc workflow to be
        /// inserted</param>
        internal void InsertAdHocWorkflowAfterCurrentStep(WorkflowStepSequence adHocWorkflow) {
            using (var enumerator = this.CurrentStep.GetPossibleNextSteps().GetEnumerator()) {
                if (enumerator.MoveNext()) {
                    var nextStep = enumerator.Current;
                    if (enumerator.MoveNext()) {
                        throw new WorkflowException("Ad-hoc workflow cannot be inserted after current step because current step has multiple possible next steps.");
                    } else {
                        this.CurrentStep.ReplacePossibleNextStep(nextStep, adHocWorkflow.FirstStep.NextStep);
                        adHocWorkflow.AddStepAfterLastSteps(nextStep);
                        if (this.CurrentStep is ParallelismAction) {
                            this.History.Add(new HistoryItem(this.CurrentStep, this.ParentPersistentContainer.ParentPersistenceMechanism.UserDirectory.CurrentUser, this.GetPreviousStepPassedAt()));
                        }
                        this.CurrentStep = adHocWorkflow.FirstStep.NextStep;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Inserts an ad-hoc workflow before current step of
        /// workflow step sequence.
        /// </summary>
        /// <param name="adHocWorkflow">ad-hoc workflow to be
        /// inserted</param>
        internal void InsertAdHocWorkflowBeforeCurrentStep(WorkflowStepSequence adHocWorkflow) {
            WorkflowStep previousStep = null;
            for (var i = this.History.Count - 1; i > -1; i--) {
                var historyStep = this.History[i].WorkflowStep;
                if (null != historyStep) {
                    foreach (var possibleNextStep in historyStep.GetPossibleNextSteps()) {
                        if (possibleNextStep == this.CurrentStep) {
                            previousStep = historyStep;
                            break;
                        }
                    }
                }
            }
            if (!previousStep.ReplacePossibleNextStep(this.CurrentStep, adHocWorkflow.FirstStep.NextStep)) {
                throw new WorkflowException("Previous step of \"" + this.CurrentStep.Type + "\" could not be found. Please make sure that method\"ReplacePossibleNextStep(WorkflowStep, WorkflowStep)\" is implemented correctly in class \"" + previousStep.Type + "\".");
            }
            adHocWorkflow.AddStepAfterLastSteps(this.CurrentStep);
            this.CurrentStep = adHocWorkflow.FirstStep.NextStep;
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the buttons to show on edit form of
        /// items.
        /// </summary>
        /// <param name="editFormButtons">buttons for edit form to be
        /// processed</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>buttons to show on edit form of items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        internal void ProcessEditFormButtons<T>(IList<ActionButton> editFormButtons, WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            if (this.IsCompleted) {
                this.LastStep?.ProcessEditFormButtons(editFormButtons, associatedObject, dataProvider);
            } else {
                this.CurrentStep.ProcessEditFormButtons(editFormButtons, associatedObject, dataProvider);
            }
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the edit form for existing items for
        /// current worflow state.
        /// </summary>
        /// <param name="editFormView">edit form for existing items
        /// to be processed</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        internal void ProcessEditFormView(ref FormView editFormView, WorkflowControlledObject associatedObject) {
            if (this.IsCompleted) {
                this.LastStep?.ProcessEditFormView(ref editFormView, associatedObject);
            } else {
                this.CurrentStep.ProcessEditFormView(ref editFormView, associatedObject);
            }
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the buttons to show on view form of
        /// items.
        /// </summary>
        /// <param name="viewFormButtons">buttons for view form to be
        /// processed</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>buttons to show on view form of items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        internal void ProcessViewFormButtons<T>(IList<ActionButton> viewFormButtons, WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            if (this.IsCompleted) {
                this.LastStep?.ProcessViewFormButtons(viewFormButtons, associatedObject, dataProvider);
            } else {
                this.CurrentStep.ProcessViewFormButtons(viewFormButtons, associatedObject, dataProvider);
            }
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the view form for items for current
        /// worflow state.
        /// </summary>
        /// <param name="viewFormView">view form for items to be
        /// processed</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        internal void ProcessViewFormView(ref FormView viewFormView, WorkflowControlledObject associatedObject) {
            if (this.IsCompleted) {
                this.LastStep?.ProcessViewFormView(ref viewFormView, associatedObject);
            } else {
                this.CurrentStep.ProcessViewFormView(ref viewFormView, associatedObject);
            }
            return;
        }

        /// <summary>
        /// Resets this workflow step sequence to first step. Please
        /// use GoTo(FirstStep) instead if you like to reset this
        /// workflow step sequence from a business context.
        /// </summary>
        internal void Reset() {
            this.CurrentStep?.Reset();
            this.CurrentStep = this.FirstStep;
            return;
        }

    }

}