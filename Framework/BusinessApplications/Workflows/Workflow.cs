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
    using Framework.BusinessApplications.DataProviders;
    using Framework.BusinessApplications.Workflows.Steps;
    using Framework.Diagnostics;
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;
    using System.Transactions;

    /// <summary>
    /// Instance of a workflow.
    /// </summary>
    public sealed class Workflow : WorkflowStepSequence {

        /// <summary>
        /// Object which is associated to this workflow.
        /// </summary>
        public WorkflowControlledObject AssociatedObject {
            private get { return this.associatedObject.Value; }
            set { this.associatedObject.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<WorkflowControlledObject> associatedObject =
            new PersistentFieldForPersistentObject<WorkflowControlledObject>(nameof(AssociatedObject), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Indicates whether associated object is null.
        /// </summary>
        internal bool IsAssociatedObjectNull {
            get { return null == this.AssociatedObject; }
        }

        /// <summary>
        /// Display title of workflow.
        /// </summary>
        public string Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PersistentFieldForString title =
            new PersistentFieldForString(nameof(Title));

        /// <summary>
        /// List of workflow variables.
        /// </summary>
        private readonly PersistentFieldForPersistentObjectCollection<KeyValuePair> variables;

        /// <summary>
        /// Name of persistent field "Variables".
        /// </summary>
        public const string VariablesField = "Variables";

        /// <summary>
        /// Gets or sets the variable with a specific key.
        /// </summary>
        /// <param name="key">key of variable to get or set value for</param>
        /// <returns>value of variable with specific key</returns>
        public string this[string key] {
            get {
                string value = null;
                foreach (var variable in this.variables) {
                    if (null != variable && key == variable.KeyField) {
                        value = variable.Value;
                        break;
                    }
                }
                return value;
            }
            set {
                var isVariableExisting = false;
                foreach (var variable in this.variables) {
                    if (null != variable && key == variable.KeyField) {
                        isVariableExisting = true;
                        variable.Value = value;
                    }
                }
                if (!isVariableExisting) {
                    this.variables.Add(new KeyValuePair(key, value));
                }
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Workflow()
            : base() {
            this.RegisterPersistentField(this.associatedObject);
            this.title.IsFullTextIndexed = false;
            this.RegisterPersistentField(this.title);
            this.variables = new PersistentFieldForPersistentObjectCollection<KeyValuePair>(VariablesField, CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.variables);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        public Workflow(WorkflowControlledObject associatedObject)
            : this() {
            this.AssociatedObject = associatedObject;
        }

        /// <summary>
        /// Gets called on click of a button of this workflow.
        /// </summary>
        /// <param name="sender">button that was clicked</param>
        /// <param name="promptInput">prompt input of user</param>
        /// <typeparam name="T">type of objects server side workflow
        /// button is for</typeparam>
        internal void ButtonClick<T>(ServerSideWorkflowButton<T> sender, string promptInput) where T : class, IProvidableObject {
            var workflowWithElevatedPrivileges = this.AssociatedObject.WorkflowWithElevatedPrivileges;
            workflowWithElevatedPrivileges.ButtonClick(workflowWithElevatedPrivileges.AssociatedObject, sender, promptInput);
            workflowWithElevatedPrivileges.Execute();
            return;
        }

        /// <summary>
        /// Cancels this workflow.
        /// </summary>
        public void Cancel() {
            this.Cancel(this.AssociatedObject);
            return;
        }

        /// <summary>
        /// Executes this workflow if it is not completed yet. At the
        /// end it is going to be updated in peristence mechanism.
        /// </summary>
        public void Execute() {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                this.Execute(this.AssociatedObject);
                transactionScope.Complete();
            }
            return;
        }

        /// <summary>
        /// Executes all scheduled workflow steps and clean-up
        /// activities.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to execute workflow instances in</param>
        public static void ExecuteAllIn(PersistenceMechanism persistenceMechanism) {
            JobQueue.Log?.WriteEntry("Begin of processing scheduled workflows...", LogLevel.Information);
            var workflows = persistenceMechanism.FindContainer<Workflow>();
            var filterCriteria = new FilterCriteria(nameof(Workflow.AutoExecutionSchedule), RelationalOperator.IsLessThanOrEqualTo, UtcDateTime.Now);
            var sortCriteria = new SortCriterionCollection {
                { nameof(Workflow.AutoExecutionSchedule), SortDirection.Ascending }
            };
            var scheduledWorkflows = workflows.Find(filterCriteria, sortCriteria);
            foreach (var scheduledWorkflow in scheduledWorkflows) {
                try {
                    scheduledWorkflow.Execute();
                } catch (Exception exception) {
                    JobQueue.Log?.WriteEntry(exception, LogLevel.Error);
                }
            }
            JobQueue.Log?.WriteEntry("...end of processing scheduled workflows.", LogLevel.Information);
            return;
        }

        /// <summary>
        /// Gets the additional buttons to be shown on edit form of
        /// items.
        /// </summary>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>additional buttons to be shown on edit form of
        /// items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        public IEnumerable<ActionButton> GetAdditionalEditFormButtons<T>(DataProvider<T> dataProvider) where T : class, IProvidableObject {
            var editFormButtons = new List<ActionButton>();
            this.ProcessEditFormButtons(editFormButtons, dataProvider);
            return editFormButtons;
        }

        /// <summary>
        /// Gets the additional buttons to be shown on view form of
        /// items.
        /// </summary>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>additional buttons to be shown on view form of
        /// items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        public IEnumerable<ActionButton> GetAdditionalViewFormButtons<T>(DataProvider<T> dataProvider) where T : class, IProvidableObject {
            var viewFormButtons = new List<ActionButton>();
            this.ProcessViewFormButtons(viewFormButtons, dataProvider);
            return viewFormButtons;
        }

        /// <summary>
        /// Gets the object which is associated to this workflow to
        /// be used in forms.
        /// </summary>
        /// <returns>object which is associated to this workflow</returns>
        public WorkflowControlledObject GetAssociatedObjectForForms() {
            this.GetAssociatedObjectForForms(this.AssociatedObject);
            return this.AssociatedObject;
        }

        /// <summary>
        /// Gets the URL of icon to display for current workflow step.
        /// </summary>
        public string GetIconUrl() {
            return this.GetIconUrl(this.AssociatedObject);
        }

        /// <summary>
        /// Gets all users who are currently allowed to interact with
        /// the workflow currently.
        /// </summary>
        /// <returns>all users who are allowed to interact with the
        /// workflow currently</returns>
        public IEnumerable<IUser> GetCurrentlyInvolvedUsers<T>() where T : class, IProvidableObject {
            var involvedUsers = new HashSet<IUser>();
            var dummyDataProvider = new DummyDataProvider<T>();
            foreach (var additionalViewFormButton in this.GetAdditionalViewFormButtons(dummyDataProvider)) {
                involvedUsers.UnionWith(additionalViewFormButton.AllowedGroupsForReading.Members);
            }
            foreach (var additionalEditFormButton in this.GetAdditionalEditFormButtons(dummyDataProvider)) {
                involvedUsers.UnionWith(additionalEditFormButton.AllowedGroupsForReading.Members);
            }
            return involvedUsers;
        }

        /// <summary>
        /// Gets all users who interacted with the workflow
        /// previously or who are allowed to interact currently.
        /// </summary>
        /// <returns>all users who interacted with the workflow 
        /// previously or who are allowed to interact currently</returns>
        public IEnumerable<IUser> GetPreviouslyAndCurrentlyInvolvedUsers<T>() where T : class, IProvidableObject {
            var involvedUsers = new HashSet<IUser>();
            foreach (var historyItem in this.History) {
                involvedUsers.Add(historyItem.PassedBy);
            }
            involvedUsers.UnionWith(this.GetCurrentlyInvolvedUsers<T>());
            return involvedUsers;
        }

        /// <summary>
        /// Gets the display title of current workflow step.
        /// </summary>
        public string GetTitle() {
            return this.GetTitle(this.AssociatedObject);
        }

        /// <summary>
        /// Gets the current undo buttons of workflow.
        /// </summary>
        /// <returns>current undo buttons of workflow</returns>
        internal IEnumerable<UndoWorkflowStepButton> GetUndoButtons() {
            return ((Workflow)this.GetWithElevatedPrivileges()).GetUndoButtonsWithIdsOfWorkflowStepsToBeUndone(this.AssociatedObject); // elevated privileges are used for performance reasons
        }

        /// <summary>
        /// Sets the current step of workflow to another step of
        /// workflow.
        /// </summary>
        /// <param name="workflowStep">new current step</param>
        /// <returns>true if current step could be changed, false
        /// otherwise</returns>
        public override bool GoTo(WorkflowStep workflowStep) {
            if (!base.GoTo(workflowStep)) {
                throw new WorkflowException("Workflow step to be set as next step is not part of workflow step sequence.");
            }
            this.UpdateCascadedly();
            this.Execute();
            return true;
        }

        /// <summary>
        /// Inserts an ad-hoc workflow into the running one.
        /// </summary>
        /// <param name="sender">sender button who caused the
        /// insertion of ad-hoc workflow</param>
        internal void InsertAdHocWorkflow(AdHocWorkflowButton sender) {
            if (InsertionPoint.BeforeCurrentStep == sender.InsertionPoint) {
                this.InsertAdHocWorkflowBeforeCurrentStep(sender.AdHocWorkflow);
            } else if (InsertionPoint.BeforeCurrentLeafStep == sender.InsertionPoint) {
                var leafWorkflowStepSequence = this.FindLeafWorkflowStepSequenceOf(this.AssociatedObject, sender);
                leafWorkflowStepSequence.InsertAdHocWorkflowBeforeCurrentStep(sender.AdHocWorkflow);
            } else if (InsertionPoint.AfterCurrentStep == sender.InsertionPoint) {
                this.InsertAdHocWorkflowAfterCurrentStep(sender.AdHocWorkflow);
            } else if (InsertionPoint.AfterCurrentLeafStep == sender.InsertionPoint) {
                var leafWorkflowStepSequence = this.FindLeafWorkflowStepSequenceOf(this.AssociatedObject, sender);
                leafWorkflowStepSequence.InsertAdHocWorkflowAfterCurrentStep(sender.AdHocWorkflow);
            } else if (InsertionPoint.ParallelToCurrentStep == sender.InsertionPoint) {
                var parallelismAction = this.CurrentStep as ParallelismAction;
                if (null == parallelismAction) {
                    throw new WorkflowException("Ad-hoc workflow cannot be inserted parallel to current step because current step is not a parallelism action.");
                } else {
                    parallelismAction.ParallelWorkflowStepSequences.Add(sender.AdHocWorkflow);
                }
            } else {
                throw new ArgumentException("Insertion point \"" + sender.InsertionPoint.ToString() + "\" is not valid.");
            }
            this.UpdateCascadedly();
            this.Execute();
            return;
        }

        /// <summary>
        /// Inserts an ad-hoc workflow into the running one.
        /// </summary>
        /// <param name="adHocWorkflow">ad-hoc workflow step sequence
        /// to insert into the running workflow</param>
        /// <param name="insertionPoint">insertion point</param>
        public void InsertAdHocWorkflow(WorkflowStepSequence adHocWorkflow, InsertionPoint insertionPoint) {
            if (InsertionPoint.BeforeCurrentStep == insertionPoint) {
                this.InsertAdHocWorkflowBeforeCurrentStep(adHocWorkflow);
            } else if (InsertionPoint.AfterCurrentStep == insertionPoint) {
                this.InsertAdHocWorkflowAfterCurrentStep(adHocWorkflow);
            } else if (InsertionPoint.ParallelToCurrentStep == insertionPoint) {
                var parallelismAction = this.CurrentStep as ParallelismAction;
                if (null == parallelismAction) {
                    throw new WorkflowException("Ad-hoc workflow cannot be inserted parallel to current step because current step is not a parallelism action.");
                } else {
                    parallelismAction.ParallelWorkflowStepSequences.Add(adHocWorkflow);
                }
            } else if (InsertionPoint.BeforeCurrentLeafStep == insertionPoint || InsertionPoint.AfterCurrentLeafStep == insertionPoint) {
                throw new ArgumentException("Insertion point \"" + insertionPoint.ToString() + "\" is not valid in this context.", nameof(insertionPoint));
            } else {
                throw new ArgumentException("Insertion point \"" + insertionPoint.ToString() + "\" is not valid.", nameof(insertionPoint));
            }
            this.UpdateCascadedly();
            this.Execute();
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the buttons to show on edit form of
        /// items.
        /// </summary>
        /// <param name="editFormButtons">buttons for edit form to be
        /// processed</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>buttons to show on edit form of items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        public void ProcessEditFormButtons<T>(IList<ActionButton> editFormButtons, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            this.ProcessEditFormButtons<T>(editFormButtons, this.AssociatedObject, dataProvider);
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the edit form for existing items for
        /// current worflow state.
        /// </summary>
        /// <param name="editFormView">edit form for existing items
        /// to be processed</param>
        public void ProcessEditFormView(ref FormView editFormView) {
            this.ProcessEditFormView(ref editFormView, this.AssociatedObject);
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the buttons to show on view form of
        /// items. Duplicate buttons that are also visible on
        /// workflow diagram are replaced by a workflow diagram
        /// button automatically.
        /// </summary>
        /// <param name="viewFormButtons">buttons for view form to be
        /// processed</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>buttons to show on view form of items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        public void ProcessViewFormButtons<T>(IList<ActionButton> viewFormButtons, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            this.ProcessViewFormButtons<T>(viewFormButtons, this.AssociatedObject, dataProvider);
            foreach (var undoButton in this.GetUndoButtons()) {
                viewFormButtons.Add(undoButton);
            }
            var persistenceMechanism = this.AssociatedObject.ParentPersistentContainer.ParentPersistenceMechanism;
            var workflowDiagramCapableButtons = new Dictionary<string, List<ActionButton>>();
            for (var i = viewFormButtons.Count - 1; i > -1; i--) {
                var viewFormButton = viewFormButtons[i];
                if (viewFormButton is IWorkflowDiagramCapableButton workflowDiagramCapableButton) {
                    if (!workflowDiagramCapableButton.IsVisibleOnFormPages) {
                        viewFormButtons.RemoveAt(i);
                    } else if (workflowDiagramCapableButton.IsVisibleOnWorkflowDiagramPages && viewFormButton.AllowedGroupsForReading.ContainsPermissionsFor(persistenceMechanism.UserDirectory.CurrentUser)) { // the filter for permissions will be applied again later, but there is no other way to find duplicates for current user
                        if (!workflowDiagramCapableButtons.ContainsKey(viewFormButton.Title)) {
                            workflowDiagramCapableButtons[viewFormButton.Title] = new List<ActionButton>();
                        }
                        workflowDiagramCapableButtons[viewFormButton.Title].Add(viewFormButton);
                    }
                }
            }
            foreach (var keyValuePair in workflowDiagramCapableButtons) {
                var workflowDiagramCapableButtonGroup = keyValuePair.Value;
                if (workflowDiagramCapableButtonGroup.Count > 1) {
                    var replacementButton = new LinkButton(keyValuePair.Key, "workflow.html") {
                        ConfirmationMessage = string.Format(Resources.TheActionFor0CannotBeExecutedBecauseItIsAmbiguous, keyValuePair.Key)
                    };
                    replacementButton.AllowedGroupsForReading.Add(persistenceMechanism.AllUsers);
                    var replacementIndex = viewFormButtons.IndexOf(workflowDiagramCapableButtonGroup[workflowDiagramCapableButtonGroup.Count - 1]);
                    viewFormButtons.Insert(replacementIndex, replacementButton);
                    foreach (var workflowDiagramCapableButton in workflowDiagramCapableButtonGroup) {
                        viewFormButtons.Remove(workflowDiagramCapableButton);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the view form for items for current
        /// worflow state.
        /// </summary>
        /// <param name="viewFormView">view form for items to be
        /// processed</param>
        public void ProcessViewFormView(ref FormView viewFormView) {
            this.ProcessViewFormView(ref viewFormView, this.AssociatedObject);
            return;
        }

    }

}