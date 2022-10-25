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

    using Framework.Persistence.Directories;
    using Model;
    using Properties;
    using System;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Lightweight HTTP conroller for http authentication.
    /// </summary>
    public sealed class LoginControllerForHttpAuthentication : IHttpController {

        /// <summary>
        /// URL of imprint page.
        /// </summary>
        private readonly string imprintUrl;

        /// <summary>
        /// Root URL login is necessary for.
        /// </summary>
        private readonly string loginRootUrl;

        /// <summary>
        /// URL of privacy notice.
        /// </summary>
        private readonly string privacyNoticeUrl;

        /// <summary>
        /// Name of CSS file.
        /// </summary>
        private readonly string styleSheetFile;

        /// <summary>
        /// User directory to use.
        /// </summary>
        private readonly PersistentUserDirectoryWithHttpAuthentication userDirectory;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="loginRootUrl">root URL login is necessary
        /// for</param>
        /// <param name="imprintUrl">URL of imprint page</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        /// <param name="styleSheetFile">name of CSS file</param>
        /// <param name="userDirectory">user directory to use</param>
        public LoginControllerForHttpAuthentication(string loginRootUrl, string imprintUrl, string privacyNoticeUrl, string styleSheetFile, PersistentUserDirectoryWithHttpAuthentication userDirectory) {
            this.loginRootUrl = loginRootUrl;
            this.imprintUrl = imprintUrl;
            this.privacyNoticeUrl = privacyNoticeUrl;
            this.styleSheetFile = styleSheetFile;
            this.userDirectory = userDirectory;
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_PostAuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed = false;
            if ("/" == httpRequest.Url.AbsolutePath) {
                string loginUrl = httpRequest.QueryString["login"];
                if (!string.IsNullOrEmpty(loginUrl)) {
                    if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                        loginUrl = HttpUtility.UrlDecode(loginUrl);
                        if (this.userDirectory.IsCurrentUserAnonymous) {
                            httpResponse.Clear();
                            var logOnInfo = this.userDirectory.GetLogOnInfo();
                            var now = UtcDateTime.Now;
                            if (null != logOnInfo && logOnInfo.LockedUntil > now.AddSeconds(LogOnInfo.MinimumDelayInSeconds)) {
                                string delayInSeconds = Math.Ceiling((logOnInfo.LockedUntil - now).TotalSeconds).ToString(CultureInfo.InvariantCulture);
                                httpResponse.AppendHeader("Retry-After", delayInSeconds);
                                httpResponse.StatusCode = 429;
                                var links = new Link[] { new Link(Resources.Retry, loginUrl) };
                                var signInPage = new LoginInfoPage(Resources.Login, string.Format(Resources.TooManyRequestsPleaseTryAgainIn0Seconds, delayInSeconds), this.imprintUrl, this.privacyNoticeUrl, links);
                                signInPage.AddStyleSheet(this.styleSheetFile);
                                signInPage.CreateChildControls(httpRequest);
                                signInPage.HandleEvents(httpRequest, httpResponse);
                                if (!httpResponse.IsRequestBeingRedirected) {
                                    signInPage.Render(httpResponse);
                                }
                            } else {
                                httpResponse.AppendHeader("WWW-Authenticate", "Basic realm=\"Login\"");
                                httpResponse.StatusCode = 401;
                                httpResponse.Write("<html><head><meta http-equiv=\"refresh\" content=\"0; url=" + loginUrl + "\"></head></html>");
                            }
                        } else {
                            RedirectionController.RedirectRequest(httpResponse, loginUrl);
                        }
                    } else {
                        OptionsController.RejectRequest(httpResponse);
                    }
                    isProcessed = true;
                }
            }
            if (!isProcessed && this.userDirectory.IsCurrentUserAnonymous && httpRequest.Url.AbsolutePath != this.loginRootUrl && httpRequest.Url.AbsolutePath.StartsWith(this.loginRootUrl)) {
                RedirectionController.RedirectRequest(httpResponse, "/?login=" + HttpUtility.UrlEncode(httpRequest.Url.PathAndQuery));
                isProcessed = true;
            }
            return isProcessed;
        }

    }

}