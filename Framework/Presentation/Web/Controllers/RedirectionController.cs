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

namespace Framework.Presentation.Web.Controllers {

    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Lightweight controller class for processing http page
    /// redirections.
    /// </summary>
    public class RedirectionController : IHttpController {

        /// <summary>
        /// List of redirection rules to apply.
        /// </summary>
        public IList<RedirectionRule> RedirectionRules {
            get {
                return this.redirectionRules;
            }
        }
        private List<RedirectionRule> redirectionRules;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public RedirectionController() {
            this.redirectionRules = new List<RedirectionRule>();
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_AuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public virtual bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed = false;
            string targetUrl = httpRequest.Url.AbsolutePath;
            foreach (var redirectionRule in redirectionRules) {
                targetUrl = redirectionRule.Match(targetUrl);
                if (redirectionRule.IsLastRule && targetUrl != httpRequest.Url.AbsolutePath) {
                    break;
                }
            }
            if (targetUrl != httpRequest.Url.AbsolutePath) {
                if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                    RedirectionController.RedirectRequest(httpResponse, targetUrl);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            }
            return isProcessed;
        }

        /// <summary>
        /// Redirects an HTTP request to another URL of the same web
        /// site.
        /// </summary>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="targetUrl">target URL to redirect request to</param>
        public static void RedirectRequest(HttpResponse httpResponse, string targetUrl) {
            if (string.IsNullOrEmpty(targetUrl) || targetUrl.Contains("//")) { // avoid redirection to external URLs for security reasons, to avoid fishing attacks
                httpResponse.Clear();
                httpResponse.StatusCode = 400;
                httpResponse.Write("Bad Request.");
            } else {
                RedirectionController.RedirectRequestUnsafe(httpResponse, targetUrl);
            }
            return;
        }

        /// <summary>
        /// Redirects an HTTP request to another URL of the same web
        /// site or of an external web site.
        /// </summary>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="targetUrl">target URL to redirect request to</param>
        public static void RedirectRequestUnsafe(HttpResponse httpResponse, string targetUrl) {
            httpResponse.Clear();
            httpResponse.Redirect(targetUrl, false); // always redirects via status code 302 (Found / Moved Temporarily)
            return;
        }

    }

}