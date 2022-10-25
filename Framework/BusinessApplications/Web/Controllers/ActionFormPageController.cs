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
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding business web pages for
    /// action forms.
    /// </summary>
    /// <typeparam name="T">type of providable object to be processed
    /// on action form</typeparam>
    internal class ActionFormPageController<T> : BusinessPageController where T : class, IProvidableObject {

        /// <summary>
        /// Absolute path of page.
        /// </summary>
        public string AbsolutePath { get; private set; }

        /// <summary>
        /// Controller to be used for action form.
        /// </summary>
        public ActionFormButton<T> ActionFormButton { get; private set; }

        /// <summary>
        /// File name of page.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="actionFormButton">controller to use for
        /// action form</param>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absolutePath">absolute path of page</param>
        /// <param name="fileName">file name of page</param>
        public ActionFormPageController(ActionFormButton<T> actionFormButton, IBusinessApplication businessApplication, string absolutePath, string fileName)
            : base(businessApplication) {
            this.AbsolutePath = absolutePath;
            this.ActionFormButton = actionFormButton;
            this.FileName = fileName;
        }

        /// <summary>
        /// Fills the page with a form.
        /// </summary>
        private bool CreateFormPage() {
            bool success = false;
            var formData = this.ActionFormButton.GetElement();
            if (null != formData) {
                var formView = this.ActionFormButton.GetFormView(formData);
                if (null != formView) {
                    this.Page.Title = formData.GetTitle();
                    var form = new Form(formData, formView, this.BusinessApplication.FileBaseDirectory, this.BusinessApplication.GetWebFactory(Presentation.Forms.FieldRenderMode.Form));
                    var actionBar = new ActionBar<T>(form);
                    actionBar.AddButtonRange(this.FilterButtonsForCurrentUser(this.ActionFormButton.GetFormButtons(formData)));
                    this.AddActionBarToPage(actionBar);
                    this.Page.ContentSection.AddChildControl(form);
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Processes a potential lookup request.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if lookup request could be processed, false
        /// otherwise</returns>
        private bool ProcessPotentialLookupRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool success = false;
            if (httpRequest.Url.AbsolutePath.StartsWith(this.AbsolutePath)) {
                var formData = this.ActionFormButton.GetElement();
                var formView = this.ActionFormButton.GetFormView(formData);
                var lookupController = new LookupController(this.AbsolutePath, formData, formView, this.BusinessApplication.GetWebFactory(FieldRenderMode.Form).OptionDataProvider);
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
            bool isProcessed = false;
            if (httpRequest.Url.AbsolutePath == this.AbsolutePath + this.FileName) {
                isProcessed = this.CreateFormPage();
                if (isProcessed) {
                    var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                    if ("GET" == httpMethod || "POST" == httpMethod) {
                        this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                    } else {
                        OptionsController.RejectRequest(httpResponse);
                    }
                }
            } else {
                isProcessed = this.ProcessPotentialLookupRequest(httpRequest, httpResponse);
            }
            return isProcessed;
        }

    }

}