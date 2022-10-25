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

    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// HTTP controller for filling in missing redirections for
    /// virtual links in navigation sections of business
    /// applications.
    /// </summary>
    public class NavigationRedirectionController : IHttpController {

        /// <summary>
        /// Business application to process.
        /// </summary>
        public IBusinessApplication BusinessApplication { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        public NavigationRedirectionController(IBusinessApplication businessApplication)
            : base() {
            this.BusinessApplication = businessApplication;
        }

        /// <summary>
        /// Gets the target URL to redirect request to based on
        /// potentially missing tailing slash.
        /// </summary>
        /// <param name="requestedUrl">requested url</param>
        /// <returns>target URL to redirect request to</returns>
        protected string GetTargetUrlForMissingSlashRedirection(string requestedUrl) {
            string targetUrl;
            var lastIndexOfSlash = requestedUrl.LastIndexOf('/');
            var fileName = requestedUrl.Substring(lastIndexOfSlash + 1);
            if (!fileName.Contains(".") && !System.IO.File.Exists(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + requestedUrl.Substring(1).Replace('/', System.IO.Path.DirectorySeparatorChar))) {
                targetUrl = requestedUrl + "/";
            } else {
                targetUrl = null;
            }
            return targetUrl;
        }

        /// <summary>
        /// Gets the target URL to redirect request to based on
        /// navigation items.
        /// </summary>
        /// <param name="requestedUrl">requested url</param>
        /// <param name="navigationItems">navigation items to browse
        /// for matching target URLs</param>
        /// <returns>target URL to redirect request to</returns>
        protected string GetTargetUrlForNavigationRedirection(string requestedUrl, IEnumerable<NavigationItem> navigationItems) {
            string targetUrl = null;
            foreach (var navigationItem in navigationItems) {
                string navigationItemAbsolutePath;
                if (Uri.TryCreate(navigationItem.Link, UriKind.Absolute, out Uri uri)) {
                    if (System.Web.HttpContext.Current.Request.Url.Authority == uri.Authority) {
                        navigationItemAbsolutePath = uri.AbsolutePath;
                    } else {
                        navigationItemAbsolutePath = null; // skip external URLs
                    }
                } else {
                    navigationItemAbsolutePath = navigationItem.Link;
                }
                if (requestedUrl == navigationItemAbsolutePath) {
                    foreach (var childNavigationItem in navigationItem.NavigationItems) {
                        if (!childNavigationItem.IsHeadline) {
                            if (null == targetUrl) {
                                targetUrl = childNavigationItem.Link;
                            } else if (childNavigationItem.IsDefault) {
                                targetUrl = childNavigationItem.Link;
                                break;
                            }
                        }
                    }
                } else {
                    targetUrl = this.GetTargetUrlForNavigationRedirection(requestedUrl, navigationItem.NavigationItems);
                }
                if (null != targetUrl) {
                    break;
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
            string targetUrl;
            if (httpRequest.Url.AbsolutePath.EndsWith("/", StringComparison.Ordinal)) {
                targetUrl = this.GetTargetUrlForNavigationRedirection(httpRequest.Url.AbsolutePath, this.BusinessApplication.GetNavigationItems(httpRequest));
            } else {
                targetUrl = this.GetTargetUrlForMissingSlashRedirection(httpRequest.Url.AbsolutePath);
            }
            if (null != targetUrl) {
                if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                    RedirectionController.RedirectRequest(httpResponse, targetUrl);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            }
            return isProcessed;
        }

    }

}
