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

    using Framework.Presentation.Web.Buttons;
    using Model;
    using Persistence;
    using Persistence.Directories;
    using Presentation;
    using Presentation.Forms;
    using Presentation.Web;
    using Presentation.Web.Controllers;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Base class of HTTP controllers for authentication.
    /// </summary>
    public abstract class LoginController : IHttpController {

        /// <summary>
        /// Business application to get navigation items from.
        /// </summary>
        private readonly BusinessApplication businessApplication;

        /// <summary>
        /// URL of imprint.
        /// </summary>
        protected string ImprintUrl { get; private set; }

        /// <summary>
        /// Root URL login is necessary for.
        /// </summary>
        protected string LoginRootUrl { get; private set; }

        /// <summary>
        /// Settings to apply to web pages.
        /// </summary>
        protected BusinessPageSettings PageSettings {
            get {
                BusinessPageSettings pageSettings;
                if (null == this.businessApplication) {
                    pageSettings = this.pageSettings;
                } else {
                    pageSettings = this.businessApplication.PageSettings;
                }
                return pageSettings;
            }
        }
        private readonly BusinessPageSettings pageSettings;

        /// <summary>
        /// Persistence mechanism to use.
        /// </summary>
        protected PersistenceMechanism PersistenceMechanism { get; private set; }

        /// <summary>
        /// URL of privacy notice.
        /// </summary>
        protected string PrivacyNoticeUrl { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="loginRootUrl">root URL login is necessary
        /// for</param>
        /// <param name="imprintUrl">URL of imprint</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        /// <param name="businessApplication">name of business application</param>
        public LoginController(string loginRootUrl, string imprintUrl, string privacyNoticeUrl, BusinessApplication businessApplication) {
            this.businessApplication = businessApplication;
            this.ImprintUrl = imprintUrl;
            this.LoginRootUrl = loginRootUrl;
            this.PersistenceMechanism = businessApplication.PersistenceMechanism;
            this.PrivacyNoticeUrl = privacyNoticeUrl;
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
        public LoginController(string loginRootUrl, string imprintUrl, string privacyNoticeUrl, string styleSheetUrl, PersistenceMechanism persistenceMechanism) {
            this.ImprintUrl = imprintUrl;
            this.LoginRootUrl = loginRootUrl;
            this.PersistenceMechanism = persistenceMechanism;
            this.PrivacyNoticeUrl = privacyNoticeUrl;
            this.pageSettings = new BusinessPageSettings {
                StyleSheetUrl = styleSheetUrl,
            };
        }

        /// <summary>
        /// Creates a web page for changing password.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="text">text to be displayed</param>
        /// <param name="passwordResetToken">password reset token</param>
        protected virtual void CreateChangePasswordPage(HttpRequest httpRequest, HttpResponse httpResponse, string text, PasswordResetToken passwordResetToken) {
            var valueWrapper = new PresentableObject();
            var valueField = new PresentableFieldForString(valueWrapper, "value");
            valueWrapper.AddPresentableField(valueField);
            var viewPane = new ViewPaneForFields();
            viewPane.ViewFields.Add(LoginController.CreateViewFieldForPassword(Resources.Password, valueField.Key, Mandatoriness.Required));
            var loginPage = new LoginFormPage(Resources.ChangePassword, text, this.ImprintUrl, this.PrivacyNoticeUrl, valueWrapper, viewPane, this.PersistenceMechanism.UserDirectory);
            this.InitializePage(httpRequest, loginPage);
            loginPage.CreateChildControls(httpRequest);
            loginPage.HandleEvents(httpRequest, httpResponse);
            if (true == loginPage.HasValidValue && null != passwordResetToken.AssociatedUser) {
                var user = passwordResetToken.AssociatedUser as PersistentUser;
                if (!user.HasPassword(valueField.Value)) {
                    user.PasswordExpirationDate = UtcDateTime.Now.Add(UserDirectory.PasswordExpirationTimeSpan);
                }
                user.SetPassword(valueField.Value);
                user.Update();
                if (!passwordResetToken.IsNew) {
                    var elevatedPaswordResetTokens = this.PersistenceMechanism.CopyWithElevatedPrivileges().FindContainer<PasswordResetToken>();
                    elevatedPaswordResetTokens.FindOne(passwordResetToken.Id)?.RemoveCascadedly();
                }
                RedirectionController.RedirectRequest(httpResponse, "./");
            }
            if (!httpResponse.IsRequestBeingRedirected) {
                loginPage.Render(httpResponse);
            }
            return;
        }

        /// <summary>
        /// Creates a web page for forgotten password.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        private void CreateForgotPasswordPage(HttpRequest httpRequest, HttpResponse httpResponse) {
            var valueWrapper = new PresentableObject();
            var valueField = new PresentableFieldForString(valueWrapper, "value");
            valueWrapper.AddPresentableField(valueField);
            var viewPane = new ViewPaneForFields();
            if (PersistentUserDirectory.HasEmailAddressAsUserName) {
                viewPane.ViewFields.Add(new ViewFieldForEmailAddress(Resources.EmailAddress, valueField.Key, Mandatoriness.Required));
            } else {
                viewPane.ViewFields.Add(new ViewFieldForSingleLineText(Resources.UserName, valueField.Key, Mandatoriness.Required));
            }
            string infoText;
            if (PersistentUserDirectory.HasEmailAddressAsUserName) {
                infoText = Resources.PleaseEnterYourEmailAddressBelowAndClickTheSubmitButton;
            } else {
                infoText = Resources.PleaseEnterYourUserNameBelowAndClickTheSubmitButton;
            }
            var loginPage = new LoginFormPage(Resources.ForgotPassword, infoText, this.ImprintUrl, this.PrivacyNoticeUrl, valueWrapper, viewPane, this.PersistenceMechanism.UserDirectory);
            this.InitializePage(httpRequest, loginPage);
            loginPage.CreateChildControls(httpRequest);
            loginPage.HandleEvents(httpRequest, httpResponse);
            if (true == loginPage.HasValidValue) {
                var passwordResetToken = PasswordResetToken.Create(valueField.Value, this.PersistenceMechanism);
                if (null != passwordResetToken?.AssociatedUser) {
                    var passwordResetUrl = BusinessApplication.Url + this.LoginRootUrl.Substring(1) + "login.html?user=" + HttpUtility.UrlEncode(passwordResetToken.AssociatedUser.UserName) + "&prt=" + HttpUtility.UrlEncode(passwordResetToken.Identifier);
                    var placeholders = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("[link]", "<a href=\"" + passwordResetUrl + "\">" + passwordResetUrl +  "</a>") };
                    var htmlEmail = new HtmlEmail();
                    htmlEmail.IsEmbeddingLinksAndTimesIntoHtmlTags = false;
                    htmlEmail.To.Add(passwordResetToken.AssociatedUser);
                    htmlEmail.Subject = Resources.ForgotPassword;
                    htmlEmail.BodyText = "<p>" + Resources.ForgotPasswordMailText.Replace(Environment.NewLine, "</p><p>").Replace("<p></p>", string.Empty) + "</p>";
                    htmlEmail.ReplacePlaceholders(placeholders);
                    htmlEmail.SendAsync();
                }
                RedirectionController.RedirectRequest(httpResponse, "./");
            }
            if (!httpResponse.IsRequestBeingRedirected) {
                loginPage.Render(httpResponse);
            }
            return;
        }

        /// <summary>
        /// Creates a web page for delay info.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="logOnInfo">log-on info of affected user</param>
        /// <param name="now">current date/time</param>
        protected void CreateLoginDelayPage(HttpRequest httpRequest, HttpResponse httpResponse, LogOnInfo logOnInfo, DateTime now) {
            string delayInSeconds = Math.Ceiling((logOnInfo.LockedUntil - now).TotalSeconds).ToString(CultureInfo.InvariantCulture);
            httpResponse.AppendHeader("Cache-Control", "no-store");
            httpResponse.AppendHeader("Retry-After", delayInSeconds);
            httpResponse.StatusCode = 429;
            var links = new LinkButton[] { new LinkButton(Resources.Retry, httpRequest.Url.AbsoluteUri) };
            var loginDelayPage = new LoginInfoPage(Resources.Login, string.Format(Resources.TooManyRequestsPleaseTryAgainIn0Seconds, delayInSeconds), this.ImprintUrl, this.PrivacyNoticeUrl, links);
            this.InitializePage(httpRequest, loginDelayPage);
            loginDelayPage.CreateChildControls(httpRequest);
            loginDelayPage.HandleEvents(httpRequest, httpResponse);
            if (!httpResponse.IsRequestBeingRedirected) {
                loginDelayPage.Render(httpResponse);
            }
            return;
        }

        /// <summary>
        /// Creates a web page for login.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        protected abstract void CreateLoginPage(HttpRequest httpRequest, HttpResponse httpResponse);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in field is required</param>
        public static ViewFieldForPassword CreateViewFieldForPassword(string title, string key, Mandatoriness mandatoriness) {
            var viewFieldForPassword = new ViewFieldForPassword(title, key, mandatoriness) {
                IsCharacterVarianceRequired = true,
                MinLength = 8,
                MaxLength = 4096
            };
            return viewFieldForPassword;
        }

        /// <summary>
        /// Initializes a login related page.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="page">login related page to be initialized</param>
        protected void InitializePage(HttpRequest httpRequest, Page page) {
            if (null == this.businessApplication) {
                page.AddStyleSheet("/" + this.PageSettings.StyleSheetUrl);
            } else {
                page.SetPageSettings(this.businessApplication.PageSettings);
                var navTitle = BusinessPageController.InitializePage(page);
                navTitle.Text = this.businessApplication.Title;
                page.NavigationSection.TitleControl = navTitle;
                page.NavigationSection.NavigationItems.AddRange(this.businessApplication.GetNavigationItems(httpRequest));
                page.NavigationSection.AutoSelectItems(httpRequest);
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
        public virtual bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed = new LocalFileController("/" + this.PageSettings.StyleSheetUrl, CacheControl.MustRevalidate).ProcessRequest(httpRequest, httpResponse);
            if (!isProcessed) {
                string loginPageUrl = this.LoginRootUrl + "login.html";
                bool isLoginPageRequested = loginPageUrl == httpRequest.Url.AbsolutePath;
                var userDirectory = this.PersistenceMechanism.UserDirectory as PersistentUserDirectory;
                if (null != userDirectory.ExpiredUser) {
                    if (isLoginPageRequested) {
                        var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                        if ("GET" == httpMethod || "POST" == httpMethod) {
                            string text;
                            if (userDirectory.ExpiredUser.CreatedAt.Date == userDirectory.ExpiredUser.PasswordExpirationDate.Date) {
                                text = Resources.YourPasswordHasToBeChangedAtFirstLogonPleaseEnterANewPasswordBelowAndClickTheSubmitButton;
                            } else {
                                text = Resources.YourPasswordHasExpiredPleaseEnterANewPasswordBelowAndClickTheSubmitButton;
                            }
                            var passwordResetToken = new PasswordResetToken(userDirectory.ExpiredUser);
                            this.CreateChangePasswordPage(httpRequest, httpResponse, text, passwordResetToken);
                        } else {
                            OptionsController.RejectRequest(httpResponse);
                        }
                    } else {
                        RedirectionController.RedirectRequest(httpResponse, loginPageUrl);
                    }
                    isProcessed = true;
                } else if (isLoginPageRequested) {
                    this.ProcessRequestOfLoginPage(httpRequest, httpResponse);
                    isProcessed = true;
                } else if (userDirectory.IsCurrentUserAnonymous) {
                    if (this.LoginRootUrl == httpRequest.Url.AbsolutePath && string.IsNullOrEmpty(httpRequest.QueryString["login"])) {
                        RedirectionController.RedirectRequest(httpResponse, loginPageUrl);
                        isProcessed = true;
                    } else if (httpRequest.Url.AbsolutePath.StartsWith(this.LoginRootUrl)) {
                        this.ProcessRequestWhichRequiresAuthentication(httpRequest, httpResponse);
                        isProcessed = true;
                    }
                }
            }
            return isProcessed;
        }

        /// <summary>
        /// Processes a web request of login page.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        private void ProcessRequestOfLoginPage(HttpRequest httpRequest, HttpResponse httpResponse) {
            var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
            if ("GET" == httpMethod || "POST" == httpMethod) {
                string passwordResetTokenIdentifier = httpRequest.QueryString["prt"];
                if (string.IsNullOrEmpty(passwordResetTokenIdentifier)) {
                    this.CreateLoginPage(httpRequest, httpResponse);
                } else {
                    string userName = httpRequest.QueryString["user"];
                    if (!string.IsNullOrEmpty(userName)) {
                        userName = HttpUtility.UrlDecode(userName);
                    }
                    passwordResetTokenIdentifier = HttpUtility.UrlDecode(passwordResetTokenIdentifier);
                    var passwordResetToken = PasswordResetToken.FindOne(userName, passwordResetTokenIdentifier, this.PersistenceMechanism);
                    if (null == passwordResetToken) {
                        this.CreateForgotPasswordPage(httpRequest, httpResponse);
                    } else {
                        this.CreateChangePasswordPage(httpRequest, httpResponse, Resources.PleaseEnterYourNewPasswordBelowAndClickTheSubmitButton, passwordResetToken);
                    }
                }
            } else {
                OptionsController.RejectRequest(httpResponse);
            }
            return;
        }

        /// <summary>
        /// Processes a web request which requires authentication.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        protected abstract void ProcessRequestWhichRequiresAuthentication(HttpRequest httpRequest, HttpResponse httpResponse);

    }

}