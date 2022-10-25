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
    using Framework.BusinessApplications.DataControllers;
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Properties;
    using Presentation;
    using Presentation.Web.Controllers;
    using System;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding history pages.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public class HistoryPageController<T> : ProvidableObjectPageController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Data controller to use for form.
        /// </summary>
        public FormDataController<T> FormDataController { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteListPageUrl">absolute URL of parent
        /// list page - it may not be empty, not contain any special
        /// charaters except for dashes and has to start and end with
        /// a slash</param>
        /// <param name="formDataController">data controller to use
        /// for form</param>
        public HistoryPageController(IBusinessApplication businessApplication, string absoluteListPageUrl, FormDataController<T> formDataController)
            : base(businessApplication, absoluteListPageUrl) {
            this.FormDataController = formDataController;
        }

        /// <summary>
        /// Fills the page with a form.
        /// </summary>
        /// <param name="elementId">ID of element as string</param>
        /// <param name="versionId">ID of version as string</param>
        /// <returns>true if page exists and could be created, false
        /// otherwise</returns>
        protected bool CreateHistoryItemFormPage(string elementId, Guid versionId) {
            bool success;
            var element = this.FormDataController.DataProvider.FindOne(elementId);
            if (null != element && !element.IsNew) {
                this.Page.Title = element.GetTitle();
                var actionBar = new ActionBar<T>();
                var closeButton = new Presentation.Buttons.LinkButton(Resources.Close, "history.html");
                var allUsers = new Group("All users");
                allUsers.Members.Add(UserDirectory.AnonymousUser);
                closeButton.AllowedGroupsForReading.Add(allUsers);
                actionBar.AddButton(closeButton);
                this.AddActionBarToPage(actionBar);
                var formView = this.FormDataController.GetViewFormView(element);
                if (null != formView) {
                    formView.DescriptionForEditMode = null;
                    formView.DescriptionForViewMode = null;
                    formView.HasModificationInfo = false;
                    foreach (var version in element.Versions) {
                        if (version.Id == versionId) {
                            var form = new Form(element, formView, this.BusinessApplication.FileBaseDirectory, this.BusinessApplication.GetWebFactory(FieldRenderMode.Form)) {
                                ComparisonDate = version.CreatedAt
                            };
                            this.Page.ContentSection.AddChildControl(form);
                            break;
                        }
                    }
                }
                success = true;
            } else {
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Fills the page with a history list table.
        /// </summary>
        /// <param name="elementId">ID of element as string</param>
        /// <returns>true if page exists and could be created, false
        /// otherwise</returns>
        protected bool CreateHistoryListTablePage(string elementId) {
            bool success;
            var element = this.FormDataController.DataProvider.FindOne(elementId);
            if (null != element && !element.IsNew) {
                this.Page.Title = element.GetTitle();
                var actionBar = new ActionBar<T>();
                var closeButton = new CancelButton(Resources.Close);
                var allUsers = new Group("All users");
                allUsers.Members.Add(UserDirectory.AnonymousUser);
                closeButton.AllowedGroupsForReading.Add(allUsers);
                actionBar.AddButton(closeButton);
                this.AddActionBarToPage(actionBar);
                var listTableView = new ListTableView();
                listTableView.ViewFields.Add(new ViewFieldForDateTime(Resources.ModifiedAt, nameof(Model.Version.ModifiedAt), Mandatoriness.Optional, TimeSpan.Zero, DateTimeType.DateAndTime));
                listTableView.ViewFields.Add(new ViewFieldForPersonLookup(Resources.ModifiedBy, nameof(Model.Version.ModifiedBy), Mandatoriness.Optional));
                var listTable = new ListTable(element.Versions, listTableView, this.BusinessApplication.FileBaseDirectory, this.BusinessApplication.GetWebFactory(FieldRenderMode.ListTable)) {
                    OnClickUrlDelegate = delegate (IPresentableObject clickedObject) {
                        return "history." + clickedObject.Id.ToString("N") + ".html";
                    }
                };
                this.Page.ContentSection.AddChildControl(listTable);
                success = true;
            } else {
                success = false;
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
            bool isProcessed = false;
            var urlSegments = this.GetListPageRelativeUrlSegments(httpRequest);
            if (2L == urlSegments.LongLength) {
                string fileName = urlSegments[1];
                if (fileName.StartsWith("history") && fileName.EndsWith(".html")) {
                    if (12 == fileName.Length) {
                        isProcessed = this.CreateHistoryListTablePage(urlSegments[0]);
                    } else if ('.' == fileName[7]) {
                        if (Guid.TryParse(fileName.Substring(8, fileName.Length - 13), out Guid versionId)) {
                            isProcessed = this.CreateHistoryItemFormPage(urlSegments[0], versionId);
                        }
                    }
                    if (isProcessed) {
                        if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                            this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                        } else {
                            OptionsController.RejectRequest(httpResponse);
                        }
                    }
                }
            }
            return isProcessed;
        }

    }

}