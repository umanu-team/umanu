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

    using Framework.BusinessApplications;
    using System;
    using System.Net;
    using System.Web;

    /// <summary>
    /// HTTP controller acting as lightweight reverse proxy.
    /// </summary>
    public class ReverseProxyController : IHttpController {

        /// <summary>
        /// Absolute URL of controller - it may not be empty, not
        /// contain any special charaters except for dashes and has
        /// to start and end with a slash.
        /// </summary>
        public string AbsoluteUrl {
            get {
                return this.absoluteUrl;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of controller does not start with a slash.");
                }
                if (!value.EndsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of controller does not end with a slash.");
                }
                this.absoluteUrl = value;
            }
        }
        private string absoluteUrl;

        /// <summary>
        /// HTTP client to be used for web requests.
        /// </summary>
        protected HttpClient HttpClient { get; private set; }

        /// <summary>
        /// Credentials to be used for reverse proxy request.
        /// </summary>
        public ICredentials TargetCredentials {
            get { return this.HttpClient.Credentials; }
            set { this.HttpClient.Credentials = value; }
        }

        /// <summary>
        /// Ttarget URL to deliver as reverse proxy - it may not be
        /// empty, not contain any special charaters except for
        /// dashes and has to end with a slash.
        /// </summary>
        public string TargetUrl {
            get {
                return this.targetUrl;
            }
            private set {
                if (!value.EndsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Target URL \"" + value + "\" does not end with a slash.");
                }
                this.targetUrl = value;
            }
        }
        private string targetUrl;

        /// <summary>
        /// Proxy to be used for reverse proxy request.
        /// </summary>
        public IWebProxy TargetWebProxy {
            get { return this.HttpClient.WebProxy; }
            set { this.HttpClient.WebProxy = value; }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="absoluteUrl">absolute URL of list controller
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="targetUrl">target URL to deliver as reverse
        /// proxy - it may not be empty, not contain any special
        /// charaters except for dashes and has to end with a slash</param>

        public ReverseProxyController(string absoluteUrl, string targetUrl)
            : base() {
            this.AbsoluteUrl = absoluteUrl;
            this.HttpClient = new HttpClient();
            this.TargetUrl = targetUrl;
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
            if (httpRequest.Url.AbsolutePath.StartsWith(this.absoluteUrl)) {
                string requestUrl = this.TargetUrl + httpRequest.Url.AbsolutePath.Substring(this.absoluteUrl.Length) + httpRequest.Url.Query;
                byte[] responseBytes = this.HttpClient.SendReceiveByteArray(httpRequest.HttpMethod, requestUrl, null, httpRequest.BinaryRead(httpRequest.ContentLength), httpRequest.ContentType, out var responseContentEncoding, out string responseContentType, out var responseStatusCode);
                if (null != responseContentEncoding) {
                    httpResponse.ContentEncoding = responseContentEncoding;
                }
                httpResponse.ContentType = responseContentType;
                httpResponse.StatusCode = (int)responseStatusCode;
                httpResponse.BinaryWrite(responseBytes);
                return true;
            }
            return isProcessed;
        }

    }

}