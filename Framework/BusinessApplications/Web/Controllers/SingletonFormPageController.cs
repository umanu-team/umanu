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
    using Framework.Presentation.Exceptions;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding business web pages for
    /// new/edit/view forms of providable singleton objects.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public class SingletonFormPageController<T> : MasterDetailPageController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Data controller for master/detail.
        /// </summary>
        public MasterDetailDataController<T> MasterDetailDataController { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteFormPageUrl">absolute URL of form
        /// page - it may not be empty, not contain any special
        /// charaters except for dashes and has to start and end with
        /// a slash</param>
        /// <param name="masterDetailDataController">data controller
        /// for list and forms</param>
        public SingletonFormPageController(IBusinessApplication businessApplication, string absoluteFormPageUrl, MasterDetailDataController<T> masterDetailDataController)
            : base(businessApplication, absoluteFormPageUrl, masterDetailDataController) {
            this.MasterDetailDataController = masterDetailDataController;
        }

        /// <summary>
        /// Fills the page with a representation of all relevant
        /// business objects.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        protected override void CreatePageForBusinessObjects(HttpRequest httpRequest) {
            // nothing to do
            return;
        }

        /// <summary>
        /// Gets the controls to be rendered for business objects.
        /// </summary>
        /// <param name="businessObjects">business objects to be
        /// displayed</param>
        /// <param name="isSubset">true if business objects are a
        /// subset, false if they are total</param>
        /// <param name="description">description text to be
        /// displayed</param>
        /// <param name="pageTitle">title of list page to be set</param>
        /// <returns>new controls to be rendered</returns>
        protected override ICollection<Control> GetControlsForBusinessObjects(ICollection<T> businessObjects, bool isSubset, string description, out string pageTitle) {
            pageTitle = null;
            return new Control[0];
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
            if (httpRequest.Url.AbsolutePath == this.AbsoluteUrl) {
                var elements = this.MasterDetailDataController.DataProvider.GetAll();
                bool isFirstElement = true;
                foreach (var element in elements) {
                    if (isFirstElement) {
                        isFirstElement = false;
                        if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                            var formUrl = this.GetOnClickUrlDelegate(elements)(element);
                            RedirectionController.RedirectRequest(httpResponse, formUrl);
                        } else {
                            OptionsController.RejectRequest(httpResponse);
                        }
                        isProcessed = true;
                    } else {
                        throw new PresentationException("Element of type " + element.GetType().FullName + " cannot be handled as singleton object because multiple instances of this type exist in persistence mechanism.");
                    }
                }
            } else {
                isProcessed = base.ProcessRequest(httpRequest, httpResponse);
            }
            return isProcessed;
        }

    }

}