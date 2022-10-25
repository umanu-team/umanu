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

namespace Framework.BusinessApplications.Web.Controllers {

    using Framework.BusinessApplications.Buttons;
    using Framework.BusinessApplications.DataProviders;
    using Framework.BusinessApplications.Workflows;
    using Framework.BusinessApplications.Workflows.Steps;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Web;
    using Framework.Properties;
    using Presentation.Forms;
    using Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding business web pages for form
    /// action forms of workflows.
    /// </summary>
    public class WorkflowFormActionPageController : ProvidableObjectPageController<IProvidableObject> {

        /// <summary>
        /// Parent providable object of form action.
        /// </summary>
        private readonly WorkflowControlledObject element;

        /// <summary>
        /// Form action to be processed.
        /// </summary>
        private readonly FormAction formAction;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteListTablePageUrl">absolute URL of
        /// parent list table page - it may not be empty, not contain
        /// any special charaters except for dashes and has to start
        /// and end with a slash</param>
        /// <param name="element">parent providable object
        /// of form action</param>
        /// <param name="formAction">form action to be processed</param>
        public WorkflowFormActionPageController(WorkflowControlledObject element, FormAction formAction, IBusinessApplication businessApplication, string absoluteListTablePageUrl)
            : base(businessApplication, absoluteListTablePageUrl) {
            this.element = element;
            this.formAction = formAction;
        }

        /// <summary>
        /// Fills the page with a form.
        /// </summary>
        /// <returns>true if page exists and could be created, false
        /// otherwise</returns>
        protected bool CreateFormPage() {
            var success = false;
            if (null != this.element && !this.element.IsNew && null != this.element.Workflow && !this.element.Workflow.IsCompleted) {
                this.Page.Title = this.element.GetTitle();
                var formData = this.formAction.GetInitialFormData(this.element);
                var dataProvider = new FormActionDataProvider(this.element, this.formAction);
                var buttons = this.GetFormButtons(dataProvider);
                var formView = this.GetFormView();
                var form = new Form(formData, formView, this.BusinessApplication.FileBaseDirectory, this.BusinessApplication.GetWebFactory(FieldRenderMode.Form));
                if (null != buttons && null != form) {
                    var actionBar = new ActionBar<IProvidableObject>(form);
                    actionBar.AddButtonRange(this.FilterButtonsForCurrentUser(buttons));
                    this.AddActionBarToPage(actionBar);
                    this.Page.ContentSection.AddChildControl(form);
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Gets the buttons to show on form.
        /// </summary>
        /// <param name="dataProvider">data provider to be used for
        /// saving</param>
        private IEnumerable<ActionButton> GetFormButtons(DataProvider<IProvidableObject> dataProvider) {
            var saveButton = new SaveButton<IProvidableObject>(Resources.Save, dataProvider);
            saveButton.AllowedGroupsForReading.ReplaceBy(this.formAction.GetButtonTemplate(this.element).AllowedGroupsForReading);
            saveButton.RedirectionTarget = "../";
            yield return saveButton;
            var cancelButton = new CancelButton(Resources.Cancel);
            var allUsers = new Group("All users");
            allUsers.Members.Add(UserDirectory.AnonymousUser);
            cancelButton.AllowedGroupsForReading.Add(allUsers);
            yield return cancelButton;
        }

        /// <summary>
        /// Gets the form view.
        /// </summary>
        private FormView GetFormView() {
            var formView = new FormView();
            var description = this.formAction.GetDescription(this.element);
            formView.DescriptionForViewMode = description;
            formView.DescriptionForEditMode = description;
            foreach (var viewPane in this.formAction.GetViewPanes(this.element)) {
                formView.ViewPanes.Add(viewPane);
            }
            return formView;
        }

        /// <summary>
        /// Processes a potential lookup request.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if lookup request could be processed, false
        /// otherwise</returns>
        private bool ProcessPotentialLookupRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            var success = false;
            if (null != this.element && !this.element.IsNew && null != this.element.Workflow && !this.element.Workflow.IsCompleted) {
                var formData = this.formAction.GetInitialFormData(this.element);
                var formView = this.GetFormView();
                var lookupController = new LookupController(this.AbsoluteListPageUrl + this.element.Id.ToString("N") + '/', formData, formView, this.BusinessApplication.GetWebFactory(FieldRenderMode.Form).OptionDataProvider);
                success = lookupController.ProcessRequest(httpRequest, httpResponse);
            }
            return success;
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_PostAuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public override bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            var isProcessed = false;
            var urlSegments = this.GetListPageRelativeUrlSegments(httpRequest);
            if (2L == urlSegments.LongLength) {
                var elementId = urlSegments[0];
                var fileName = urlSegments[1];
                if (this.element.Id.ToString("N") == elementId && fileName.EndsWith(".html", StringComparison.Ordinal) && Guid.TryParse(fileName.Substring(0, fileName.Length - 5), out var formActionId) && formActionId == this.formAction.Id) {
                    isProcessed = this.CreateFormPage();
                }
                if (isProcessed) {
                    var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                    if ("GET" == httpMethod || "POST" == httpMethod) {
                        this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                    } else {
                        OptionsController.RejectRequest(httpResponse);
                    }
                }
                if (!isProcessed) {
                    isProcessed = this.ProcessPotentialLookupRequest(httpRequest, httpResponse);
                }
            }
            return isProcessed;
        }

    }

}