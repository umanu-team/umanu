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
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Buttons;
    using Framework.Presentation.Web.Controllers;
    using Framework.Properties;
    using Persistence;
    using System.Net;
    using System.Web;

    /// <summary>
    /// Lightweight HTTP controller for cookie based authentication.
    /// </summary>
    public sealed class LoginControllerForCookieAuthentication : LoginController {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="loginRootUrl">root URL login is necessary
        /// for</param>
        /// <param name="imprintUrl">URL of imprint</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        /// <param name="businessApplication">name of business application</param>
        public LoginControllerForCookieAuthentication(string loginRootUrl, string imprintUrl, string privacyNoticeUrl, BusinessApplication businessApplication)
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
        /// <param name="styleSheetFile">name of CSS file</param>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to use</param>
        public LoginControllerForCookieAuthentication(string loginRootUrl, string imprintUrl, string privacyNoticeUrl, string styleSheetFile, PersistenceMechanism persistenceMechanism)
            : base(loginRootUrl, imprintUrl, privacyNoticeUrl, styleSheetFile, persistenceMechanism) {
            // nothing to do
        }

        /// <summary>
        /// Creates a web page for changing password.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="text">text to be displayed</param>
        /// <param name="passwordResetToken">password reset token</param>
        protected override void CreateChangePasswordPage(HttpRequest httpRequest, HttpResponse httpResponse, string text, PasswordResetToken passwordResetToken) {
            if (bool.TryParse(httpRequest.QueryString["expired"], out bool isPasswordExpired) && isPasswordExpired) {
                if (passwordResetToken.AssociatedUser.CreatedAt.Date == ((PersistentUser)passwordResetToken.AssociatedUser).PasswordExpirationDate.Date) {
                    text = Resources.YourPasswordHasToBeChangedAtFirstLogonPleaseEnterANewPasswordBelowAndClickTheSubmitButton;
                } else {
                    text = Resources.YourPasswordHasExpiredPleaseEnterANewPasswordBelowAndClickTheSubmitButton;
                }
            }
            base.CreateChangePasswordPage(httpRequest, httpResponse, text, passwordResetToken);
            return;
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
            string errorMessage;
            if (bool.TryParse(httpRequest.QueryString["invalid"], out bool isInvalid) && isInvalid) {
                errorMessage = Resources.TheEnteredCombinationOfUserNameAndPasswordIsInvalid;
            } else {
                errorMessage = null;
            }
            var valueWrapper = new PresentableObject();
            var userNameField = new PresentableFieldForString(valueWrapper, "username");
            valueWrapper.AddPresentableField(userNameField);
            var passwordField = new PresentableFieldForString(valueWrapper, "password");
            valueWrapper.AddPresentableField(passwordField);
            var viewPane = new ViewPaneForFields();
            viewPane.ViewFields.Add(new ViewFieldForSingleLineText(Resources.UserName, userNameField.Key, Mandatoriness.Required) { IsAutofocused = true });
            viewPane.ViewFields.Add(new ViewFieldForPassword(Resources.Password, passwordField.Key, Mandatoriness.Required) { OnClientKeyDown = "javascript:if('13'==((event.which)?event.which:event.keyCode))document.querySelectorAll('form')[0].submit();" });
            var userDirectory = this.PersistenceMechanism.UserDirectory as PersistentUserDirectoryWithCookieAuthentication;
            var loginPage = new LoginFormPage(Resources.Login, infoText, errorMessage, this.ImprintUrl, this.PrivacyNoticeUrl, valueWrapper, viewPane, userDirectory, new Control[] {
                new SubmitButton(Resources.Login),
                new LinkButton(Resources.ForgotPassword, "?prt=1")
            });
            this.InitializePage(httpRequest, loginPage);
            loginPage.CreateChildControls(httpRequest);
            loginPage.HandleEvents(httpRequest, httpResponse);
            if (true == loginPage.HasValidValue) {
                httpResponse.Clear();
                var logOnInfo = userDirectory.GetLogOnInfo(userNameField.Value);
                var now = UtcDateTime.Now;
                if (null != logOnInfo && logOnInfo.LockedUntil > now.AddSeconds(LogOnInfo.MinimumDelayInSeconds)) {
                    this.CreateLoginDelayPage(httpRequest, httpResponse, logOnInfo, now);
                } else {
                    userDirectory.LogOn(new NetworkCredential(userNameField.Value, passwordField.Value), httpResponse);
                    if (null != userDirectory.ExpiredUser) { // regular creation of password reset page in base.ProcessRequest(...) does not work because userDirectory.ExpiredUser is set too late when using cookie authentication
                        var passwordResetToken = PasswordResetToken.Create(userDirectory.ExpiredUser.UserName, this.PersistenceMechanism);
                        httpResponse.Redirect("./login.html?expired=true&user=" + HttpUtility.UrlEncode(passwordResetToken.AssociatedUser.UserName) + "&prt=" + HttpUtility.UrlEncode(passwordResetToken.Identifier), false); // always redirects via status code 302 (Found / Moved Temporarily)
                    } else if (userDirectory.IsCurrentUserAnonymous) {
                        httpResponse.Redirect("./login.html?invalid=true", false); // always redirects via status code 302 (Found / Moved Temporarily)
                    } else {
                        httpResponse.Redirect("./", false); // always redirects via status code 302 (Found / Moved Temporarily)
                    }
                }
            } else {
                loginPage.Render(httpResponse);
            }
            return;
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
            var isProcessed = base.ProcessRequest(httpRequest, httpResponse);
            if (!isProcessed && this.LoginRootUrl + "logout.html" == httpRequest.Url.AbsolutePath) {
                this.ProcessRequestOfLogoutPage(httpRequest, httpResponse);
                isProcessed = true;
            }
            return isProcessed;
        }

        /// <summary>
        /// Processes a web request of logout page.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        private void ProcessRequestOfLogoutPage(HttpRequest httpRequest, HttpResponse httpResponse) {
            httpResponse.Clear();
            var userDirectory = this.PersistenceMechanism.UserDirectory as PersistentUserDirectoryWithCookieAuthentication;
            userDirectory.LogOut(httpResponse);
            httpResponse.Redirect("./", false); // always redirects via status code 302 (Found / Moved Temporarily)
            return;
        }

        /// <summary>
        /// Processes a web request which requires authentication.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        protected override void ProcessRequestWhichRequiresAuthentication(HttpRequest httpRequest, HttpResponse httpResponse) {
            RedirectionController.RedirectRequest(httpResponse, this.LoginRootUrl + "login.html");
            return;
        }

    }

}