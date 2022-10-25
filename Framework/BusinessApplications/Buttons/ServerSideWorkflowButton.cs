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

namespace Framework.BusinessApplications.Buttons {

    using Framework.BusinessApplications.Workflows;
    using Framework.Presentation;
    using Framework.Presentation.Forms;

    /// <summary>
    /// Click handler for server side workflow buttons.
    /// </summary>
    /// <returns>result of execution of click handler</returns>
    public delegate WorkflowStepResult ServerSideWorkflowButtonClickHandler(string promptInput);

    /// <summary>
    /// Server side action button of action bar for workflow actions.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public sealed class ServerSideWorkflowButton<T> : ServerSideButton, IWorkflowDiagramCapableButton where T : class, IProvidableObject {

        /// <summary>
        /// Data provider for current providable object.
        /// </summary>
        internal DataProvider<T> DataProvider { get; private set; }

        /// <summary>
        /// Indicates whether workflow step is supposed to be visible
        /// on forms pages.
        /// </summary>
        public bool IsVisibleOnFormPages { get; set; }

        /// <summary>
        /// Indicates whether workflow step is supposed to be visible
        /// on workflow diagram pages.
        /// </summary>
        public bool IsVisibleOnWorkflowDiagramPages { get; set; }

        /// <summary>
        /// Event handler to be called on button click.
        /// </summary>
        public ServerSideWorkflowButtonClickHandler OnButtonClick { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="buttonTemplate">button template to apply</param>
        /// <param name="dataProvider">data provider for current
        /// providable object</param>
        public ServerSideWorkflowButton(ServerSideButtonTemplate buttonTemplate, DataProvider<T> dataProvider)
            : this(buttonTemplate.Title, buttonTemplate.ConfirmationMessage, dataProvider) {
            this.AllowedGroupsForReading.AddRange(buttonTemplate.AllowedGroupsForReading);
            this.HashSalt = buttonTemplate.HashSalt;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        /// <param name="dataProvider">data provider for current
        /// providable object</param>
        public ServerSideWorkflowButton(string title, DataProvider<T> dataProvider)
            : base(title) {
            this.DataProvider = dataProvider;
            this.IsVisibleOnFormPages = true;
            this.IsVisibleOnWorkflowDiagramPages = true;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        /// <param name="confirmationMessage">confirmation message to
        /// display on click</param>
        /// <param name="dataProvider">data provider for current
        /// providable object</param>
        public ServerSideWorkflowButton(string title, string confirmationMessage, DataProvider<T> dataProvider)
            : this(title, dataProvider) {
            this.ConfirmationMessage = confirmationMessage;
        }

        /// <summary>
        /// Handels the button click.
        /// </summary>
        /// <param name="promptInput">prompt input of user</param>
        /// <returns>result of execution of click handler</returns>
        internal WorkflowStepResult HandleClick(string promptInput) {
            return this.OnButtonClick(promptInput);
        }

        /// <summary>
        /// Server-side action to execute on click.
        /// </summary>
        /// <param name="form">current form or null</param>
        /// <param name="promptInput">prompt input of user</param>
        public override void OnClick(IForm form, string promptInput) {
            var workflowControlledObject = (WorkflowControlledObject)form.PresentableObject;
            workflowControlledObject.Workflow.ButtonClick(this, promptInput);
            return;
        }

    }

}