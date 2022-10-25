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

    using Presentation;
    using System;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding business web pages related to
    /// specific providable objects.
    /// </summary>
    public abstract class ProvidableObjectPageController : BusinessPageController {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        internal ProvidableObjectPageController(IBusinessApplication businessApplication)
            : base(businessApplication) {
            // nothing to do
        }

        /// <summary>
        /// Gets the segments of the requested URL relative to the
        /// URL of the parent list page.
        /// </summary>
        /// <param name="absoluteUrlPath">absolute URL path of http
        /// request to process</param>
        /// <param name="absoluteListPageUrl">absolute URL of parent
        /// list page</param>
        /// <returns>segments of the requested URL relative to the
        /// URL of the parent list page</returns>
        public static string[] GetListPageRelativeUrlSegments(string absoluteUrlPath, string absoluteListPageUrl) {
            string[] urlSegments;
            if (absoluteUrlPath.StartsWith(absoluteListPageUrl, StringComparison.Ordinal)) {
                string listTablePageRelativeFormUrl = absoluteUrlPath.Substring(absoluteListPageUrl.Length);
                urlSegments = listTablePageRelativeFormUrl.Split(new char[] { '/' }, StringSplitOptions.None);
                if (urlSegments.LongLength > 0 && string.IsNullOrEmpty(urlSegments[urlSegments.LongLength - 1])) {
                    var trimmedUrlSegments = new string[urlSegments.LongLength - 1];
                    Array.Copy(urlSegments, trimmedUrlSegments, urlSegments.LongLength - 1);
                    urlSegments = trimmedUrlSegments;
                }
            } else {
                urlSegments = new string[0];
            }
            return urlSegments;
        }

    }

    /// <summary>
    /// HTTP controller for responding business web pages related to
    /// specific providable objects.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public abstract class ProvidableObjectPageController<T> : ProvidableObjectPageController where T : class, IProvidableObject {

        /// <summary>
        /// Absolute URL of parent list page - it may not be empty,
        /// not contain any special charaters except for dashes and
        /// has to start and end with a slash.
        /// </summary>
        public string AbsoluteListPageUrl {
            get {
                return this.absoluteListPageUrl;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of list page does not start with a slash.");
                }
                if (!value.EndsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of list page does not end with a slash.");
                }
                this.absoluteListPageUrl = value;
            }
        }
        private string absoluteListPageUrl;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteListPageUrl">absolute URL of parent
        /// list page - it may not be empty, not contain any special
        /// charaters except for dashes and has to start and end with
        /// a slash</param>
        public ProvidableObjectPageController(IBusinessApplication businessApplication, string absoluteListPageUrl)
            : base(businessApplication) {
            this.AbsoluteListPageUrl = absoluteListPageUrl;
        }

        /// <summary>
        /// Gets the segments of the requested URL relative to the
        /// URL of the parent list page.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <returns>segments of the requested URL relative to the
        /// URL of the parent list page</returns>
        protected string[] GetListPageRelativeUrlSegments(HttpRequest httpRequest) {
            return ProvidableObjectPageController.GetListPageRelativeUrlSegments(httpRequest.Url.AbsolutePath, this.AbsoluteListPageUrl);
        }

    }

}