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
    using Framework.Model;
    using Framework.Persistence.Fields;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Executes a list of parallel workflow step sequences as a
    /// workflow step.
    /// </summary>
    public abstract class ParallelismAction : ActionStep {

        /// <summary>
        /// True if this workflow step is supposed to be shown in
        /// graphical representations of this workflow, false if this
        /// is a hidden workflow step.
        /// </summary>
        public sealed override bool IsVisible {
            get { return true; }
        }

        /// <summary>
        /// List of parallel workflow step sequences.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<WorkflowStepSequence> ParallelWorkflowStepSequences { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParallelismAction()
            : base() {
            this.ParallelWorkflowStepSequences = new PersistentFieldForPersistentObjectCollection<WorkflowStepSequence>(nameof(this.ParallelWorkflowStepSequences), CascadedRemovalBehavior.RemoveValuesForcibly);
            this.RegisterPersistentField(this.ParallelWorkflowStepSequences);
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
        internal sealed override WorkflowStepResult ButtonClick<T>(WorkflowControlledObject associatedObject, ServerSideWorkflowButton<T> sender, string promptInput) {
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                if (null != parallelWorkflowStepSequence) {
                    parallelWorkflowStepSequence.ButtonClick(associatedObject, sender, promptInput);
                }
            }
            return new WorkflowStepResult();
        }

        /// <summary>
        /// Cancels this workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        internal sealed override void Cancel(WorkflowControlledObject associatedObject) {
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                if (null != parallelWorkflowStepSequence) {
                    parallelWorkflowStepSequence.Cancel(associatedObject);
                }
            }
            this.Reset();
            return;
        }

        /// <summary>
        /// Executes this workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        /// <returns>result of execution of workflow step</returns>
        protected internal override WorkflowStepResult Execute(WorkflowControlledObject associatedObject) {
            var allParallelWorkflowStepSequencesAreCompleted = true;
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                if (null != parallelWorkflowStepSequence) {
                    parallelWorkflowStepSequence.Execute(associatedObject);
                    if (!parallelWorkflowStepSequence.IsCompleted) {
                        allParallelWorkflowStepSequencesAreCompleted = false;
                    }
                }
            }
            WorkflowStepResult result;
            if (allParallelWorkflowStepSequencesAreCompleted) {
                result = new WorkflowStepResult(this.NextStep);
            } else {
                var earliestSchedule = UtcDateTime.MaxValue;
                foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                    if (null != parallelWorkflowStepSequence && parallelWorkflowStepSequence.AutoExecutionSchedule < earliestSchedule) {
                        earliestSchedule = parallelWorkflowStepSequence.AutoExecutionSchedule;
                    }
                }
                result = new WorkflowStepResult(earliestSchedule);
            }
            return result;
        }

        /// <summary>
        /// Finds all instances of history items for a specific
        /// workflow step.
        /// </summary>
        /// <param name="workflowStep">workflow step to find history
        /// items for</param>
        /// <returns>all instances of history items for specific
        /// workflow step</returns>
        internal sealed override IEnumerable<HistoryItem> FindHistoryItemsFor(WorkflowStep workflowStep) {
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                if (null != parallelWorkflowStepSequence) {
                    foreach (var historyItem in parallelWorkflowStepSequence.FindHistoryItemsFor(workflowStep)) {
                        yield return historyItem;
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
        internal sealed override WorkflowStepSequence FindLeafWorkflowStepSequenceOf(WorkflowControlledObject associatedObject, AdHocWorkflowButton sender) {
            WorkflowStepSequence leafWorkflowStepSequence = null;
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                if (null != parallelWorkflowStepSequence) {
                    leafWorkflowStepSequence = parallelWorkflowStepSequence.FindLeafWorkflowStepSequenceOf(associatedObject, sender);
                    if (null != leafWorkflowStepSequence) {
                        break;
                    }
                }
            }
            return leafWorkflowStepSequence;
        }

        /// <summary>
        /// Finds all instances of workflow steps of a specific type.
        /// </summary>
        /// <param name="type">type of workflow steps to find</param>
        /// <returns>instances of workflow steps of specific type</returns>
        internal sealed override IEnumerable<WorkflowStep> FindWorkflowStepsOfType(Type type) {
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                if (null != parallelWorkflowStepSequence) {
                    foreach (var workflowStepOfParallelWorkflowStepSequence in parallelWorkflowStepSequence.FindWorkflowStepsOfType(type)) {
                        yield return workflowStepOfParallelWorkflowStepSequence;
                    }
                }
            }
        }

        /// <summary>
        /// Gets additional buttons to show on edit form of items of
        /// parallel workflow step sequences.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>buttons to show on edit form of items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        private IEnumerable<ActionButton> GetAdditionalEditFormButtonsOfParallelWorkflowStepSequences<T>(WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            var additionalEditFormButtons = new List<ActionButton>();
            this.ProcessEditFormButtonsOfParallelWorkflowStepSequences(additionalEditFormButtons, associatedObject, dataProvider);
            return additionalEditFormButtons;
        }

        /// <summary>
        /// Gets additional buttons to show on view form of items of
        /// parallel workflow step sequences.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>buttons to show on view form of items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        private IEnumerable<ActionButton> GetAdditionalViewFormButtonsOfParallelWorkflowStepSequences<T>(WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            var additionalViewFormButtons = new List<ActionButton>();
            this.ProcessViewFormButtonsOfParallelWorkflowStepSequences(additionalViewFormButtons, associatedObject, dataProvider);
            return additionalViewFormButtons;
        }

        /// <summary>
        /// Gets the object which is associated to this workflow.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        /// <returns>object which is associated to this workflow</returns>
        protected internal sealed override void GetAssociatedObjectForForms(WorkflowControlledObject associatedObject) {
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                if (null != parallelWorkflowStepSequence) {
                    parallelWorkflowStepSequence.GetAssociatedObjectForForms(associatedObject);
                }
            }
            return;
        }

        /// <summary>
        /// Gets the possible next workflow steps to be executed as
        /// soon as this workflow step is completed.
        /// </summary>
        /// <returns>possible next workflow steps</returns>
        public sealed override IEnumerable<WorkflowStep> GetPossibleNextSteps() {
            return base.GetPossibleNextSteps();
        }

        /// <summary>
        /// Gets the display title of workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        protected internal override string GetTitle(WorkflowControlledObject associatedObject) {
            return string.Empty;
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
        protected sealed override IEnumerable<UndoWorkflowStepButton> GetUndoButtons(WorkflowControlledObject associatedObject) {
            yield break;
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
        internal sealed override IEnumerable<UndoWorkflowStepButton> GetUndoButtonsWithIdsOfWorkflowStepsToBeUndone(WorkflowControlledObject associatedObject) {
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                if (null != parallelWorkflowStepSequence) {
                    foreach (var undoButton in parallelWorkflowStepSequence.GetUndoButtonsWithIdsOfWorkflowStepsToBeUndone(associatedObject)) {
                        yield return undoButton;
                    }
                }
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
        protected internal sealed override bool HasDirectAssociationTo(WorkflowControlledObject associatedObject, AdHocWorkflowButton sender) {
            var hasDirectAssociation = false;
            var dummyDataProvider = new DummyDataProvider<IProvidableObject>();
            foreach (var button in this.GetAdditionalViewFormButtons(associatedObject, dummyDataProvider)) {
                if (button.Equals(sender)) {
                    hasDirectAssociation = true;
                    foreach (var nonDirectButton in this.GetAdditionalViewFormButtonsOfParallelWorkflowStepSequences(associatedObject, dummyDataProvider)) {
                        if (button.Equals(nonDirectButton)) {
                            hasDirectAssociation = false;
                            break;
                        }
                    }
                    if (hasDirectAssociation) {
                        break;
                    }
                }
            }
            if (!hasDirectAssociation) {
                foreach (var button in this.GetAdditionalEditFormButtons(associatedObject, dummyDataProvider)) {
                    if (button.Equals(sender)) {
                        hasDirectAssociation = true;
                        foreach (var nonDirectButton in this.GetAdditionalEditFormButtonsOfParallelWorkflowStepSequences(associatedObject, dummyDataProvider)) {
                            if (button.Equals(nonDirectButton)) {
                                hasDirectAssociation = false;
                                break;
                            }
                        }
                        if (hasDirectAssociation) {
                            break;
                        }
                    }
                }
            }
            return hasDirectAssociation;
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
        protected internal override void ProcessEditFormButtons<T>(IList<ActionButton> editFormButtons, WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) {
            this.ProcessEditFormButtonsOfParallelWorkflowStepSequences(editFormButtons, associatedObject, dataProvider);
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the buttons of parallel workflow step
        /// sequences to show on edit form of items.
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
        private void ProcessEditFormButtonsOfParallelWorkflowStepSequences<T>(IList<ActionButton> editFormButtons, WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                parallelWorkflowStepSequence?.ProcessEditFormButtons(editFormButtons, associatedObject, dataProvider);
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
        protected internal override void ProcessViewFormButtons<T>(IList<ActionButton> viewFormButtons, WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) {
            this.ProcessViewFormButtonsOfParallelWorkflowStepSequences(viewFormButtons, associatedObject, dataProvider);
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the buttons of parallel workflow step
        /// sequences to show on view form of items.
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
        private void ProcessViewFormButtonsOfParallelWorkflowStepSequences<T>(IList<ActionButton> viewFormButtons, WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                parallelWorkflowStepSequence?.ProcessViewFormButtons(viewFormButtons, associatedObject, dataProvider);
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
        protected internal sealed override bool ReplacePossibleNextStep(WorkflowStep oldValue, WorkflowStep newValue) {
            return base.ReplacePossibleNextStep(oldValue, newValue);
        }

        /// <summary>
        /// Resets the workflow step sequences of parallelism action
        /// to first step.
        /// </summary>
        public sealed override void Reset() {
            foreach (var parallelWorkflowStepSequence in this.ParallelWorkflowStepSequences) {
                if (null != parallelWorkflowStepSequence) {
                    parallelWorkflowStepSequence.Reset();
                }
            }
            return;
        }

    }

}