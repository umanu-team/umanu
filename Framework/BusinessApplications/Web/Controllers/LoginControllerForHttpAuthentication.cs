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

    using Framework.Model;
    using Framework.Persistence.Directories;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Buttons;
    using Framework.Presentation.Web.Controllers;
    using Framework.Properties;
    using Persistence;
    using System;
    using System.Web;

    /// <summary>
    /// Lightweight HTTP controller for http authentication.
    /// </summary>
    public sealed class LoginControllerForHttpAuthentication : LoginController {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="loginRootUrl">root URL login is necessary
        /// for</param>
        /// <param name="imprintUrl">URL of imprint</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        /// <param name="businessApplication">name of business application</param>
        public LoginControllerForHttpAuthentication(string loginRootUrl, string imprintUrl, string privacyNoticeUrl, BusinessApplication businessApplication)
            : base(loginRootUrl, imprintUrl, privacyNoticeUrl, businessApplication) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="loginRootUrl">root URL login is necessary
        /// for</param>
        /// <param name="imprintUrl">URL of imprint</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        /// <param name="styleSheetUrl">name of CSS file</param>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to use</param>
        public LoginControllerForHttpAuthentication(string loginRootUrl, string imprintUrl, string privacyNoticeUrl, string styleSheetUrl, PersistenceMechanism persistenceMechanism)
            : base(loginRootUrl, imprintUrl, privacyNoticeUrl, styleSheetUrl, persistenceMechanism) {
            // nothing to do
        }

        /// <summary>
        /// Creates a web page for login.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        protected override void CreateLoginPage(HttpRequest httpRequest, HttpResponse httpResponse) {
            string infoText;
            if (PersistentUserDirectory.HasEmailAddressAsUserName) {
                infoText = Resources.PleaseUseTheSignInButtonToLogOnWithYourEmailAddress;
            } else {
                infoText = Resources.PleaseUseTheSignInButtonToLogOnWithYourUserName;
            }
            var loginPage = new LoginInfoPage(Resources.Login, infoText, this.ImprintUrl, this.PrivacyNoticeUrl, new Control[] {
                new LinkButton(Resources.Login, this.LoginRootUrl + "?login=true"),
                new LinkButton(Resources.ForgotPassword, "?prt=1")
            });
            this.InitializePage(httpRequest, loginPage);
            loginPage.CreateChildControls(httpRequest);
            loginPage.HandleEvents(httpRequest, httpResponse);
            if (!httpResponse.IsRequestBeingRedirected) {
                loginPage.Render(httpResponse);
            }
            return;
        }

        /// <summary>
        /// Processes a web request which requires authentication.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        protected override void ProcessRequestWhichRequiresAuthentication(HttpRequest httpRequest, HttpResponse httpResponse) {
            if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                httpResponse.Clear();
                var logOnInfo = ((PersistentUserDirectoryWithHttpAuthentication)this.PersistenceMechanism.UserDirectory).GetLogOnInfo();
                var now = UtcDateTime.Now;
                if (null != logOnInfo && logOnInfo.LockedUntil > now.AddSeconds(LogOnInfo.MinimumDelayInSeconds)) {
                    this.CreateLoginDelayPage(httpRequest, httpResponse, logOnInfo, now);
                } else {
                    httpResponse.AppendHeader("WWW-Authenticate", "Basic realm=\"Login\"");
                    httpResponse.StatusCode = 401;
                    httpResponse.Write("<html><head><meta http-equiv=\"refresh\" content=\"0; url=" + httpRequest.Url.AbsolutePath + "?login=true\"></head></html>");
                }
            } else {
                OptionsController.RejectRequest(httpResponse);
            }
            return;
        }

    }

}