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

    using Framework.BusinessApplications.DataControllers;
    using Framework.Presentation;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding business web pages for
    /// wizard forms.
    /// </summary>
    public class WizardPageController<T> : ProvidableObjectPageController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Url file name of page.
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// Controller to use for wizard.
        /// </summary>
        public WizardController<T> WizardController { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteListPageUrl">absolute URL of parent
        /// list page - it may not be empty, not contain any special
        /// charaters except for dashes and has to start and end with
        /// a slash</param>
        /// <param name="wizardController">controller to use for
        /// wizard</param>
        /// <param name="pageName">url file name of page</param>
        public WizardPageController(IBusinessApplication businessApplication, string absoluteListPageUrl, WizardController<T> wizardController, string pageName)
            : base(businessApplication, absoluteListPageUrl) {
            this.PageName = pageName;
            this.WizardController = wizardController;
        }

        /// <summary>
        /// Fills the page with a form.
        /// </summary>
        /// <param name="id">ID from request URL or null</param>
        protected bool CreateFormPage(Guid? id) {
            bool success = false;
            var formData = this.WizardController.CreateElement(id);
            if (null != formData) {
                var formView = this.WizardController.GetFormView(formData);
                if (null != formView) {
                    this.Page.Title = formData.GetTitle();
                    var form = new Form(formData, formView, this.BusinessApplication.FileBaseDirectory, this.BusinessApplication.GetWebFactory(Presentation.Forms.FieldRenderMode.Form));
                    var actionBar = new ActionBar<T>(form);
                    actionBar.AddButtonRange(this.FilterButtonsForCurrentUser(this.WizardController.GetFormButtons(formData)));
                    this.AddActionBarToPage(actionBar);
                    this.Page.ContentSection.AddChildControl(form);
                    success = true;
                }
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
            string pageName = null;
            Guid? id = null;
            if (1L == urlSegments.LongLength) {
                pageName = urlSegments[0];
            } else if (2L == urlSegments.LongLength) {
                pageName = urlSegments[1];
                if (Guid.TryParse(urlSegments[0], out Guid parsedId)) {
                    id = parsedId;
                }
            }
            if (pageName == this.PageName && !string.IsNullOrEmpty(this.PageName)) {
                isProcessed = this.CreateFormPage(id);
                if (isProcessed) {
                    var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                    if ("GET" == httpMethod) {
                        this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                    } else if ("POST" == httpMethod) {
                        this.Page.CreateChildControls(httpRequest);
                        this.Page.HandleEvents(httpRequest, httpResponse);
                        if (null == this.WizardController.UpdatedFormData) {
                            this.SetPageProperties(httpRequest);
                            if (!httpResponse.IsRequestBeingRedirected) {
                                this.Page.Render(httpResponse);
                            }
                        } else {
                            isProcessed = false;
                            if (!httpResponse.IsRequestBeingRedirected) {
                                foreach (var httpController in this.WizardController.ProcessFormDataOf(this.WizardController.UpdatedFormData, pageName)) {
                                    isProcessed = httpController.ProcessRequest(httpRequest, httpResponse);
                                    if (isProcessed) {
                                        break;
                                    }
                                }
                            }
                        }
                    } else {
                        OptionsController.RejectRequest(httpResponse);
                    }
                }
            }
            return isProcessed;
        }

    }

}