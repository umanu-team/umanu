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

    using Buttons;
    using Framework.BusinessApplications.DataControllers;
    using Framework.BusinessApplications.Web;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Widgets;

    /// <summary>
    /// HTTP controller for responding business web pages for
    /// new/edit/view forms.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public class FormPageController<T> : ProvidableObjectPageController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Data controller to use for form.
        /// </summary>
        public FormDataController<T> FormDataController { get; private set; }

        /// <summary>
        /// Indicates what kind of form is to be created.
        /// </summary>
        public FormType FormType { get; private set; }

        /// <summary>
        /// True to redirect to correct path of page automatically if
        /// data provider for current path dores not contain element
        /// ID, false otherwise. This should be used with care to
        /// avoid infinite redirection loops.
        /// </summary>
        internal bool IsRedirectingToCorrectPathAutomatically { get; set; }

        /// <summary>
        /// Url file name of page.
        /// </summary>
        public string PageName { get; private set; }

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
        /// <param name="pageName">url file name of page</param>
        /// <param name="formType">indicates what kind of form is to
        /// be created</param>
        public FormPageController(IBusinessApplication businessApplication, string absoluteListPageUrl, FormDataController<T> formDataController, string pageName, FormType formType)
            : base(businessApplication, absoluteListPageUrl) {
            this.FormDataController = formDataController;
            this.FormType = formType;
            this.IsRedirectingToCorrectPathAutomatically = false;
            this.PageName = pageName;
        }

        /// <summary>
        /// Adds an action bar to page.
        /// </summary>
        /// <param name="form">parent form of action bar</param>
        /// <param name="viewWidgetForForm">view widget for form</param>
        /// <param name="additionalViewWidgets">additional view widgets</param>
        private void AddActionBarToPage(Form form, ViewWidgetForForm<T> viewWidgetForForm, IEnumerable<ViewWidget> additionalViewWidgets) {
            var actionBar = new ActionBar<T>(form);
            actionBar.AddButtonRange(this.FilterButtonsForCurrentUser(viewWidgetForForm.GetButtons()));
            if (null != additionalViewWidgets) {
                ulong positionId = 0;
                foreach (var additionalViewWidget in additionalViewWidgets) {
                    actionBar.AddButtonRange(this.FilterButtonsForCurrentUser(additionalViewWidget.GetButtons()), positionId);
                    positionId++;
                }
            }
            this.AddActionBarToPage(actionBar);
            return;
        }

        /// <summary>
        /// Fills the page with a form.
        /// </summary>
        /// <param name="elementId">ID of element as string</param>
        /// <returns>true if page exists and could be created, false
        /// otherwise</returns>
        protected bool CreateFormPage(string elementId) {
            bool success;
            var viewWidgetForForm = this.GetViewWidgetForForm(elementId);
            if (null == viewWidgetForForm?.Element) {
                success = false;
            } else {
                IEnumerable<ViewWidget> additionalViewWidgets = null;
                if (FormType.View == this.FormType) {
                    additionalViewWidgets = this.FormDataController.GetViewFormWidgets(viewWidgetForForm.Element);
                }
                this.Page.Title = viewWidgetForForm.Title;
                var form = new Form(viewWidgetForForm.Element, viewWidgetForForm.FormView, this.BusinessApplication.FileBaseDirectory, this.BusinessApplication.GetWebFactory(FieldRenderMode.Form));
                this.AddActionBarToPage(form, viewWidgetForForm, additionalViewWidgets);
                this.Page.ContentSection.AddChildControl(form);
                bool hasAdditionalFormWidgets = false;
                if (null != additionalViewWidgets) {
                    ulong positionId = 0;
                    foreach (var additionalViewWidget in additionalViewWidgets) {
                        this.Page.ContentSection.AddChildControl(additionalViewWidget.GetControl(this.BusinessApplication, positionId));
                        positionId++;
                        hasAdditionalFormWidgets = true;
                    }
                }
                if (hasAdditionalFormWidgets) {
                    var modificationInfoControl = form.DecoupleModificationInfoControl();
                    if (null != modificationInfoControl) {
                        this.Page.ContentSection.AddChildControl(modificationInfoControl);
                    }
                }
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Gets the view widget to be used for form.
        /// </summary>
        /// <param name="elementId">ID of element to get view widget for</param>
        /// <returns>view widget to be used for form</returns>
        private ViewWidgetForForm<T> GetViewWidgetForForm(string elementId) {
            ViewWidgetForForm<T> viewWidgetForForm;
            bool isHashedType = NewButton<T>.IsHashedType(elementId);
            if (FormType.New == this.FormType && isHashedType || FormType.New != this.FormType && !isHashedType) {
                viewWidgetForForm = new ViewWidgetForForm<T>(elementId, this.FormDataController, this.FormType);
            } else {
                viewWidgetForForm = null;
            }
            return viewWidgetForForm;
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
            var viewWidget = this.GetViewWidgetForForm(elementId);
            if (null == viewWidget?.Element) {
                if (this.IsRedirectingToCorrectPathAutomatically && FormType.New != this.FormType && Guid.TryParse(elementId, out _)) {
                    var targetUrl = this.BusinessApplication.RootUrl + "l/" + httpRequest.Url.PathAndQuery.Substring(this.AbsoluteListPageUrl.Length);
                    RedirectionController.RedirectRequest(httpResponse, targetUrl);
                    isProcessed = true;
                }
            } else {
                var childControllers = viewWidget.GetChildControllers(this.BusinessApplication, this.AbsoluteListPageUrl, null);
                foreach (var childController in childControllers) {
                    if (!this.IsNavigationSectionVisible && childController is BusinessPageController businessPageController) {
                        businessPageController.HideNavigationSection();
                    }
                    isProcessed = childController.ProcessRequest(httpRequest, httpResponse);
                    if (isProcessed) {
                        break;
                    }
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
                if (this.PageName == urlSegments[1] && !string.IsNullOrEmpty(this.PageName)) {
                    isProcessed = this.CreateFormPage(urlSegments[0]);
                    if (isProcessed) {
                        var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                        if ("GET" == httpMethod || "POST" == httpMethod) {
                            this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                        } else {
                            OptionsController.RejectRequest(httpResponse);
                        }
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