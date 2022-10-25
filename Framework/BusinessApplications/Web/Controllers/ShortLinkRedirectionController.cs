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
    /// HTTP controller for redirection of short links to their actual
    /// target.
    /// </summary>
    public class ShortLinkRedirectionController : IHttpController {

        /// <summary>
        /// Parent business application to check for possible
        /// redirection targets.
        /// </summary>
        public BusinessApplication BusinessApplication { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">parent business
        /// application to check for possible redirection targets</param>
        public ShortLinkRedirectionController(BusinessApplication businessApplication) {
            this.BusinessApplication = businessApplication;
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
            string shortLinkPrefix = this.BusinessApplication.RootUrl + "l/";
            if (httpRequest.Url.AbsolutePath.StartsWith(shortLinkPrefix, StringComparison.Ordinal)) {
                int elementIdLength = httpRequest.Url.AbsolutePath.IndexOf('/', shortLinkPrefix.Length) - shortLinkPrefix.Length;
                if (elementIdLength > 0 && Guid.TryParse(httpRequest.Url.AbsolutePath.Substring(shortLinkPrefix.Length, elementIdLength), out Guid elementId)) {
                    foreach (var httpController in this.BusinessApplication.GetHttpControllers()) {
                        if (httpController is IShortLinkableController shortLinkableController) {
                            string absoluteUrl = shortLinkableController.FindAbsoluteUrlOfListPageContaining(elementId);
                            if (!string.IsNullOrEmpty(absoluteUrl)) {
                                if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                                    string targetUrl = absoluteUrl + httpRequest.Url.AbsolutePath.Substring(shortLinkPrefix.Length) + httpRequest.Url.Query;
                                    RedirectionController.RedirectRequest(httpResponse, targetUrl);
                                } else {
                                    OptionsController.RejectRequest(httpResponse);
                                }
                                isProcessed = true;
                                break;
                            }
                        }
                    }
                }
            }
            return isProcessed;
        }

    }

}