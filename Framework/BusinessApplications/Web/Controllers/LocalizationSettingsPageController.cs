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

    using Framework.BusinessApplications;
    using Framework.BusinessApplications.DataControllers;
    using Framework.BusinessApplications.Web;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Lightweight controller class for setting localization
    /// settings.
    /// </summary>
    public class LocalizationSettingsPageController : BusinessPageController {

        /// <summary>
        /// Indicates whether user is supposed to be redirected to
        /// settings page if no culture is set.
        /// </summary>
        public bool IsRedirectingToSettingsPageIfNoCultureIsSet { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">parent business
        /// application</param>
        public LocalizationSettingsPageController(IBusinessApplication businessApplication)          
            : base(businessApplication) {
            this.HideNavigationSection();
            this.IsRedirectingToSettingsPageIfNoCultureIsSet = true;
        }

        /// <summary>
        /// Fills the page with a form for localization settings.
        /// </summary>
        public virtual void CreateLocalizationSettingsPage() {
            var userDirectory = this.BusinessApplication.PersistenceMechanism.UserDirectory;
            var localizationSettings = new LocalizationSettings();
            localizationSettings.Culture = userDirectory.CurrentUser.Culture;
            LocalizationSettingsDataController localizationSettingsDataController;
            if (userDirectory is ActiveDirectory || userDirectory is MixedUserDirectory) {
                localizationSettingsDataController = new ActiveDirectoryLocalizationSettingsDataController(this.BusinessApplication.PersistenceMechanism);
            } else {
                localizationSettingsDataController = new LocalizationSettingsDataController(this.BusinessApplication.PersistenceMechanism);
            }
            var optionDataProvider = new OptionDataProvider(new DummyPersistenceMechanism(userDirectory, SecurityModel.ApplyPermissions));
            var form = new Form(localizationSettings, localizationSettingsDataController.GetEditFormView(localizationSettings), null, optionDataProvider, BuildingRule.ThrowExceptionsOnMissingFields, this.BusinessApplication.PageSettings);
            var actionBar = new ActionBar<LocalizationSettings>(form);
            actionBar.AddButtonRange(localizationSettingsDataController.GetEditFormButtons(localizationSettings));
            this.AddActionBarToPage(actionBar);
            this.Page.ContentSection.AddChildControl(form);
            this.Page.Title = userDirectory.CurrentUser.DisplayName;
            return;
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_AuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public override bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed = false;
            var currentUser = this.BusinessApplication.PersistenceMechanism.UserDirectory.CurrentUser;
            string userProfilePageUrl = this.BusinessApplication.RootUrl + "user-profile.html";
            if (httpRequest.Url.AbsolutePath == userProfilePageUrl) {
                var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                if ("GET" == httpMethod || "POST" == httpMethod) {
                    this.CreateLocalizationSettingsPage();
                    return this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            } else if (this.IsRedirectingToSettingsPageIfNoCultureIsSet && (httpRequest.Url.AbsolutePath.EndsWith(".html", StringComparison.Ordinal) || httpRequest.Url.AbsolutePath.EndsWith("/", StringComparison.Ordinal)) && (null == currentUser.Culture || CultureInfo.InvariantCulture == currentUser.Culture)) {
                RedirectionController.RedirectRequest(httpResponse, userProfilePageUrl);
                isProcessed = true;
            }
            return isProcessed;
        }

    }

}