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
    using Framework.BusinessApplications.Workflows;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Presentation;
    using Framework.Properties;
    using Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding workflow diagram pages.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public class WorkflowDiagramPageController<T> : ProvidableObjectPageController<T> where T : WorkflowControlledObject {

        /// <summary>
        /// Data provider containing element to get worfklow diagram
        /// for.
        /// </summary>
        protected DataProvider<T> DataProvider { get; private set; }

        /// <summary>
        /// Element to get workflow diagram for.
        /// </summary>
        public T Element { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="element">element to get workflow diagram for</param>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteListPageUrl">absolute URL of parent
        /// list page - it may not be empty, not contain any special
        /// charaters except for dashes and has to start and end with
        /// a slash</param>
        /// <param name="dataProvider">data provider containing
        /// element to get workflow diagram for</param>
        public WorkflowDiagramPageController(T element, IBusinessApplication businessApplication, string absoluteListPageUrl, DataProvider<T> dataProvider)
            : base(businessApplication, absoluteListPageUrl) {
            this.DataProvider = dataProvider;
            this.Element = element;
        }

        /// <summary>
        /// Fills the page with a workflow diagram.
        /// </summary>
        /// <returns>true if page exists and could be created, false
        /// otherwise</returns>
        protected bool CreateWorkflowDiagramPage() {
            bool success;
            if (null != this.Element && !this.Element.IsNew && null != this.Element.Workflow) {
                this.Page.Title = this.Element.GetTitle();
                var actionBar = new ActionBar();
                var closeButton = new CancelButton(Resources.Close);
                var allUsers = new Group("All users");
                allUsers.Members.Add(UserDirectory.AnonymousUser);
                closeButton.AllowedGroupsForReading.Add(allUsers);
                actionBar.AddButton(closeButton);
                this.AddActionBarToPage(actionBar);
                var workflowDiagram = new WorkflowDiagram<T>(this.Element.Workflow, this.DataProvider);
                this.Page.ContentSection.AddChildControl(workflowDiagram);
                success = true;
            } else {
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Gets the child controllers for handling sub pages of list
        /// page.
        /// </summary>
        /// <returns>child controllers for handling sub pages of list
        /// page</returns>
        internal virtual IEnumerable<IHttpController> GetChildControllers() {
            if (null != this.Element && !this.Element.IsNew && null != this.Element.Workflow) {
                var currentUser = this.Element.ParentPersistentContainer.ParentPersistenceMechanism.UserDirectory.CurrentUser;
                foreach (var activeWorkflowStep in this.Element.Workflow.FindActiveWorkflowSteps()) {
                    foreach (var additionalViewFormButton in activeWorkflowStep.GetAdditionalViewFormButtons<T>(this.Element.Workflow.GetAssociatedObjectForForms(), this.DataProvider)) {
                        if (additionalViewFormButton.AllowedGroupsForReading.ContainsPermissionsFor(currentUser) && additionalViewFormButton is IWorkflowDiagramCapableButton workflowDiagramCapableButton && workflowDiagramCapableButton.IsVisibleOnWorkflowDiagramPages) {
                            foreach (var childController in additionalViewFormButton.GetChildControllers(this.Element, this.BusinessApplication, this.AbsoluteListPageUrl)) {
                                if (!this.IsNavigationSectionVisible && childController is BusinessPageController businessPageController) {
                                    businessPageController.HideNavigationSection();
                                }
                                yield return childController;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Processes a potential sub page request.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="elementId">ID of element as string</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        protected bool ProcessPotentialSubPageRequest(HttpRequest httpRequest, HttpResponse httpResponse, string elementId) {
            bool isProcessed = false;
            foreach (var childController in this.GetChildControllers()) {
                isProcessed = childController.ProcessRequest(httpRequest, httpResponse);
                if (isProcessed) {
                    break;
                }
            }
            return isProcessed;
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
            bool isProcessed = false;
            var urlSegments = this.GetListPageRelativeUrlSegments(httpRequest);
            if (2L == urlSegments.LongLength) {
                string elementId = urlSegments[0];
                string fileName = urlSegments[1];
                if (this.Element.Id.ToString("N") == elementId && "workflow.html" == fileName) {
                    isProcessed = this.CreateWorkflowDiagramPage();
                }
                if (isProcessed) {
                    var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                    if ("GET" == httpMethod || "POST" == httpMethod) {
                        this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                    } else {
                        OptionsController.RejectRequest(httpResponse);
                    }
                }
            }
            if (!isProcessed && httpRequest.Url.AbsolutePath.StartsWith(this.AbsoluteListPageUrl, StringComparison.Ordinal) && urlSegments.LongLength > 0) {
                isProcessed = this.ProcessPotentialSubPageRequest(httpRequest, httpResponse, urlSegments[0]);
            }
            return isProcessed;
        }

    }

}