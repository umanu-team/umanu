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

    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding to root requests of business
    /// applications.
    /// </summary>
    public class RootRedirectionController : IHttpController {

        /// <summary>
        /// Business application to process.
        /// </summary>
        public IBusinessApplication BusinessApplication { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        public RootRedirectionController(IBusinessApplication businessApplication)
            : base() {
            this.BusinessApplication = businessApplication;
        }

        /// <summary>
        /// Gets the target URL to redirect root requests to.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <returns>target URL to redirect root requests to</returns>
        public virtual string GetTargetUrlForRedirection(HttpRequest httpRequest) {
            var navigationItems = this.BusinessApplication.GetNavigationItems(httpRequest);
            string targetUrl = null;
            foreach (var navigationItem in navigationItems) {
                if (!navigationItem.IsHeadline && !navigationItem.Link.Contains("//")) {
                    if (null == targetUrl) {
                        targetUrl = navigationItem.Link;
                    } else if (navigationItem.IsDefault) {
                        targetUrl = navigationItem.Link;
                        break;
                    }
                }
            }
            return targetUrl;
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_PostAuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public virtual bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed = false;
            if (this.BusinessApplication.RootUrl == httpRequest.Url.AbsolutePath) {
                if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                    this.BusinessApplication.InitializeRoot();
                    string targetUrl = this.GetTargetUrlForRedirection(httpRequest);
                    if (!string.IsNullOrEmpty(targetUrl)) {
                        RedirectionController.RedirectRequest(httpResponse, targetUrl);
                        isProcessed = true;
                    }
                } else {
                    OptionsController.RejectRequest(httpResponse);
                    isProcessed = true;
                }
            }
            return isProcessed;
        }

    }

}