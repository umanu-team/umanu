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
    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using Framework.Presentation.Converters;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Web;

    /// <summary>
    /// Lightweight controller for requesting a JSON-file of 
    /// the active workflow steps for the current user.
    /// </summary>
    internal class JsonActiveWorkflowStepsController : FileController, IHttpController {

        /// <summary>
        /// Business application to process.
        /// </summary>
        private readonly BusinessApplication businessApplication;

        /// <summary>
        /// File name for JSON-file.
        /// </summary>
        private const string jsonFileName = "active-workflowsteps.json";

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        public JsonActiveWorkflowStepsController(BusinessApplication businessApplication)
            : base(CacheControl.NoStore, false) {
            this.businessApplication = businessApplication;
        }

        /// <summary>
        /// Appends the information of a WorkflowControlledObject to
        /// the JsonBuilder.
        /// </summary>
        /// <param name="jsonBuilder">the JsonBuilder to append the 
        /// information to</param>
        /// <param name="workflowControlledObject">the 
        /// WorkflowControlledObject that is appended to the 
        /// JsonBuilder</param>
        private static void AppendWorkflowControlldObjectTo(JsonBuilder jsonBuilder, WorkflowControlledObject workflowControlledObject) {
            jsonBuilder.AppendObjectStart();
            jsonBuilder.AppendKey("ShortUrl");
            jsonBuilder.AppendValue(BusinessApplication.Url + "l/" + workflowControlledObject.Id.ToString("N"), true);
            jsonBuilder.AppendSeparator();
            jsonBuilder.AppendKey("Title");
            jsonBuilder.AppendValue(workflowControlledObject.GetTitle(), true);
            jsonBuilder.AppendSeparator();
            jsonBuilder.AppendKey("LastModified");
            jsonBuilder.AppendValue(workflowControlledObject.ModifiedAt.ToString("o"), true);
            jsonBuilder.AppendObjectEnd();
            return;
        }

        /// <summary>
        /// Returns a JSON-file containing active workflow steps
        /// of the current user. 
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <returns>JSON-file containing active workflow steps of
        /// the current user</returns>
        protected override File FindFile(Uri url) {
            var persistenceMechanism = this.businessApplication.PersistenceMechanism;
            var workflowControlledObjectContainer = persistenceMechanism.FindContainer<WorkflowControlledObject>();
            var filterCriteria = new FilterCriteria(new string[] { nameof(WorkflowControlledObject.Workflow), nameof(Workflow.CurrentStep) }, RelationalOperator.IsNotEqualTo, null);
            var sortCriteria = new SortCriterionCollection(nameof(WorkflowControlledObject.ModifiedAt), SortDirection.Ascending);
            var activeWorkflowControlledObjects = workflowControlledObjectContainer.Find(filterCriteria, sortCriteria);
            var jsonBuilder = new JsonBuilder();
            jsonBuilder.AppendObjectStart();
            jsonBuilder.AppendKey("WorkflowControlledObjects");
            jsonBuilder.AppendArrayStart();
            var isFirstIteration = true;
            foreach (var workflowControlledObject in activeWorkflowControlledObjects) {
                var viewFormButtons = workflowControlledObject.Workflow.GetAdditionalViewFormButtons(new DummyDataProvider<WorkflowControlledObject>());
                var isWorkflowControlledObjectRelevant = false;
                foreach (var viewFormButton in viewFormButtons) {
                    // TODO: Try to use Workflow.GetCurrentlyInvolvedUsers() OR better filter criteria instead if possible.
                    var isButtonReadableByCurrentUser = viewFormButton.AllowedGroupsForReading.ContainsPermissionsFor(persistenceMechanism.UserDirectory.CurrentUser);
                    if (isButtonReadableByCurrentUser && !(viewFormButton is UndoWorkflowStepButton) && (viewFormButton is IWorkflowDiagramCapableButton)) {
                        isWorkflowControlledObjectRelevant = true;
                        break;
                    }
                }
                if (isWorkflowControlledObjectRelevant) {
                    if (!isFirstIteration) {
                        jsonBuilder.AppendSeparator();
                    }
                    JsonActiveWorkflowStepsController.AppendWorkflowControlldObjectTo(jsonBuilder, workflowControlledObject);
                    isFirstIteration = false;
                }
            }
            jsonBuilder.AppendArrayEnd();
            jsonBuilder.AppendObjectEnd();
            var jsonFile = jsonBuilder.ToFile(jsonFileName);
            return jsonFile;
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
            bool isProcessed;
            if (httpRequest.RawUrl.Equals("/" + jsonFileName)) {
                var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                if ("GET" == httpMethod) {
                    var jsonFile = this.FindFile(httpRequest.Url);
                    var origin = httpRequest.Headers.Get("Origin");
                    if (!string.IsNullOrEmpty(origin)) {
                        httpResponse.AppendHeader("Access-Control-Allow-Origin", origin); // allows every origin as it is automatically set to the origin of the request
                        httpResponse.AppendHeader("Access-Control-Allow-Credentials", "true");
                    }
                    this.RespondFile(httpRequest, httpResponse, jsonFile);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            } else {
                isProcessed = false;
            }
            return isProcessed;
        }

        /// <summary>
        /// Updates the parent persistent object of a file for a
        /// specific URL.
        /// </summary>
        /// <param name="url">URL of file to update parent persistent
        /// object for</param>
        /// <returns>true on success, false otherwise</returns>
        protected override bool UpdateParentPersistentObjectOfFile(Uri url) {
            return false; //file is built in-memory, therefore no persistent object needs to be updated
        }

    }

}