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

    using Buttons;
    using Presentation;
    using Presentation.Buttons;
    using Presentation.Forms;
    using Properties;
    using System.Collections.Generic;

    /// <summary>
    /// Waits for users to fire a release user task of a draft form.
    /// </summary>
    public abstract class WaitForDraftReleaseAction : FormAction {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public WaitForDraftReleaseAction()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Executes this workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>result of execution of workflow step</returns>
        protected internal override WorkflowStepResult Execute(WorkflowControlledObject associatedObject) {
            WorkflowStepResult result;
            if (null == this.UpdatedFormData) {
                result = new WorkflowStepResult();
            } else {
                (this.UpdatedFormData as WorkflowControlledObject).UpdateCascadedly();
                result = new WorkflowStepResult(this.NextStep);
            }
            return result;
        }

        /// <summary>
        /// Gets the template for the button for opening the form
        /// action form.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>template for button for opening the form action
        /// form</returns>
        public sealed override ClientSideButtonTemplate GetButtonTemplate(WorkflowControlledObject associatedObject) {
            var serverSideReleaseButtonTemplate = this.GetReleaseButtonTemplate(associatedObject);
            var clientSideReleaseButtonTemplate = new ClientSideButtonTemplate(serverSideReleaseButtonTemplate.Title);
            foreach (var allowedGroupForReading in serverSideReleaseButtonTemplate.AllowedGroupsForReading) {
                clientSideReleaseButtonTemplate.AllowedGroupsForReading.Add(allowedGroupForReading);
            }
            return clientSideReleaseButtonTemplate;
        }

        /// <summary>
        /// Gets the name of resource for description to show on form
        /// action form.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>name of resource for description to show on form
        /// action form</returns>
        public override string GetDescription(WorkflowControlledObject associatedObject) {
            return Resources.PleaseClickSaveToReleaseTheForm;
        }

        /// <summary>
        /// Gets the edit form view for existing items.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>edit form view for existing items</returns>
        protected abstract FormView GetEditFormView(WorkflowControlledObject associatedObject);

        /// <summary>
        /// Gets the initial data object of form action form.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>initial data object of form action form</returns>
        public override IProvidableObject GetInitialFormData(WorkflowControlledObject associatedObject) {
            return associatedObject;
        }

        /// <summary>
        /// Gets a template for the release button to show on view
        /// form of items.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>template for release button to show on view form
        /// of items</returns>
        protected abstract ServerSideButtonTemplate GetReleaseButtonTemplate(WorkflowControlledObject associatedObject);

        /// <summary>
        /// Gets the list of view panes to show in form action form.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>list of view panes to show in form action form</returns>
        public sealed override IEnumerable<ViewPane> GetViewPanes(WorkflowControlledObject associatedObject) {
            var formView = this.GetEditFormView(associatedObject);
            formView.SetMandatoriness(Mandatoriness.Required);
            return formView.ViewPanes;
        }

        /// <summary>
        /// Adjusts/exchanges the edit form for existing items for
        /// current worflow state.
        /// </summary>
        /// <param name="editFormView">edit form for existing items
        /// to be processed</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        protected internal sealed override void ProcessEditFormView(ref FormView editFormView, WorkflowControlledObject associatedObject) {
            editFormView = this.GetEditFormView(associatedObject);
            base.ProcessEditFormView(ref editFormView, associatedObject);
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
            var editFormView = this.GetEditFormView(associatedObject);
            if (this.IsValidValue(editFormView, associatedObject, ValidityCheck.Strict)) {
                var releaseButtonTemplate = this.GetReleaseButtonTemplate(associatedObject);
                if (string.IsNullOrEmpty(releaseButtonTemplate.HashSalt)) {
                    releaseButtonTemplate.HashSalt = this.Id.ToString("N"); // to allow multiple release buttons of same type with same label
                }
                var releaseButton = new ServerSideWorkflowButton<T>(releaseButtonTemplate, dataProvider);
                releaseButton.OnButtonClick += new ServerSideWorkflowButtonClickHandler(delegate (string promptInput) {
                    return new WorkflowStepResult(this.NextStep);
                });
                viewFormButtons.Add(releaseButton);
            } else {
                base.ProcessViewFormButtons<T>(viewFormButtons, associatedObject, dataProvider);
            }
            return;
        }

    }

}