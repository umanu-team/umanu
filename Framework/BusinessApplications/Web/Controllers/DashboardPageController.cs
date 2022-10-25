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

    using Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using Widgets;

    /// <summary>
    /// Type of delegate for resolval of dashboard view.
    /// </summary>
    /// <returns>delegate for resolval of dashboard properties</returns>
    public delegate DashboardView GetDashboardViewDelegate();

    /// <summary>
    /// HTTP controller for responding business web pages for
    /// dashboards of widgets.
    /// </summary>
    public class DashboardPageController : BusinessPageController, IShortLinkableController {

        /// <summary>
        /// Absolute URL of dashboard page - it may not be empty, not
        /// contain any special charaters except for dashes and has
        /// to start with a slash.
        /// </summary>
        public string AbsoluteUrl {
            get {
                return this.absoluteUrl;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of dashboard page does not start with a slash.");
                }
                this.absoluteUrl = value;
            }
        }
        private string absoluteUrl;

        /// <summary>
        /// View of dashboard to be rendered.
        /// </summary>
        public DashboardView DashboardView {
            get {
                if (null == this.dashboardView && null != this.getDashboardViewDelegate) {
                    this.dashboardView = this.getDashboardViewDelegate();
                }
                return this.dashboardView;
            }
        }
        private DashboardView dashboardView;

        /// <summary>
        /// Delegate to get view of dashboard to be rendered.
        /// </summary>
        private readonly GetDashboardViewDelegate getDashboardViewDelegate;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of dashboard page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start with a slash</param>
        /// <param name="getDashboardViewDelegate">delegate to get
        /// view of dashboard to be rendered</param>
        public DashboardPageController(IBusinessApplication businessApplication, string absoluteUrl, GetDashboardViewDelegate getDashboardViewDelegate)
            : base(businessApplication) {
            this.AbsoluteUrl = absoluteUrl;
            this.getDashboardViewDelegate = getDashboardViewDelegate;
        }

        /// <summary>
        /// Fills the page with a dashboard.
        /// </summary>
        protected void CreateDashboardPage() {
            this.Page.Title = this.DashboardView.Title;
            var actionBar = new ActionBar();
            ulong positionId = 0;
            foreach (var viewWidget in this.DashboardView.ViewWidgets) {
                actionBar.AddButtonRange(this.FilterButtonsForCurrentUser(viewWidget.GetButtons()), positionId);
                positionId++;
            }
            this.AddActionBarToPage(actionBar);
            var dashboard = new Dashboard(this.BusinessApplication, this.DashboardView);
            dashboard.CssClasses.Add("dashboard");
            this.Page.ContentSection.AddChildControl(dashboard);
            return;
        }

        /// <summary>
        /// Finds the absolute URL of a list page containing an
        /// object with a specific ID.
        /// </summary>
        /// <param name="id">specific ID to find</param>
        /// <returns>absolute URL of a list page containing the
        /// object with the specific ID or null if no such list page
        /// can be found</returns>
        public string FindAbsoluteUrlOfListPageContaining(Guid id) {
            string absoluteUrl = null;
            ulong positionId = 0;
            foreach (var viewWidget in this.DashboardView.ViewWidgets) {
                if (viewWidget is IShortLinkableWidget shortLinkableWidget && shortLinkableWidget.Contains(id)) {
                    absoluteUrl = this.AbsoluteUrl + positionId.ToString(CultureInfo.InvariantCulture) + '/';
                    break;
                }
                positionId++;
            }
            return absoluteUrl;
        }

        /// <summary>
        /// Gets the child controllers for handling sub pages of list
        /// table page.
        /// </summary>
        /// <returns>child controllers for handling sub pages of list
        /// table page</returns>
        private IEnumerable<IHttpController> GetChildControllers() {
            ulong positionId = 0;
            foreach (var viewWidget in this.DashboardView.ViewWidgets) {
                foreach (var childController in viewWidget.GetChildControllers(this.BusinessApplication, this.absoluteUrl, positionId)) {
                    yield return childController;
                }
                positionId++;
            }
        }

        /// <summary>
        /// Processes a potential sub page request.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        private bool ProcessPotentialSubPageRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
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
            if (httpRequest.Url.AbsolutePath == this.AbsoluteUrl) {
                var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                if ("GET" == httpMethod || "POST" == httpMethod) {
                    this.CreateDashboardPage();
                    this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            } else if (httpRequest.Url.AbsolutePath.StartsWith(this.absoluteUrl, StringComparison.Ordinal)) {
                isProcessed = this.ProcessPotentialSubPageRequest(httpRequest, httpResponse);
            }
            return isProcessed;
        }

    }

}