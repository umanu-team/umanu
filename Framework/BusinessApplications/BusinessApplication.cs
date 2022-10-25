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

namespace Framework.BusinessApplications {

    using Framework.BusinessApplications.Web;
    using Framework.BusinessApplications.Web.Controllers;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using Presentation.Forms;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Base class of business applications.
    /// </summary>
    public abstract class BusinessApplication : ApplicationController, IBusinessApplication {

        /// <summary>
        /// Rules for building controls of list tables and forms.
        /// </summary>
        public BuildingRule BuildingRule { get; private set; }

        /// <summary>
        /// URL of base directory for files.
        /// </summary>
        public string FileBaseDirectory {
            get { return "attachments/"; }
        }

        /// <summary>
        /// Indicates whether current user is allowed to view
        /// workflow diagram pages.
        /// </summary>
        public bool IsCurrentUserAllowedToViewWorkflowDiagramPages { get; set; } = true;

        /// <summary>
        /// List of indicators for online/offline status.
        /// </summary>
        public IList<IOfflineCapable> OnlineStatusIndicators { get; private set; }

        /// <summary>
        /// Settings to apply to web pages.
        /// </summary>
        public BusinessPageSettings PageSettings { get; private set; }

        /// <summary>
        /// Persistence mechanism to get data from.
        /// </summary>
        public PersistenceMechanism PersistenceMechanism { get; private set; }

        /// <summary>
        /// Primary color of application.
        /// </summary>
        public PrimaryColor PrimaryColor { get; set; }

        /// <summary>
        /// Root url of application.
        /// </summary>
        public string RootUrl {
            get {
                return this.rootUrl;
            }
            protected set {
                this.rootUrl = value;
                this.WebAppManifestUrl = value + "app.webmanifest";
            }
        }
        private string rootUrl;

        /// <summary>
        /// Secondary color of application.
        /// </summary>
        public SecondaryColor SecondaryColor { get; set; }

        /// <summary>
        /// Display title of application.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Url of business application resolved from app.config,
        /// web.config ("Url" in "appSettings") or current url. If no
        /// url can be resolved, this is null.
        /// </summary>
        public static string Url {
            get {
                string url = ConfigurationManager.AppSettings["Url"];
                if (string.IsNullOrEmpty(url) && null != System.Web.HttpContext.Current?.Request?.Url) {
                    url = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/";
                }
                return url;
            }
        }

        /// <summary>
        /// Absolute URL of web app manifest or null if no manifest
        /// is supposed to be served.
        /// </summary>
        public string WebAppManifestUrl { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="pageSettings">settings to apply to web pages</param>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        /// <param name="buildingRule">rules for building controls
        /// of list tables and forms</param>
        public BusinessApplication(BusinessPageSettings pageSettings, PersistenceMechanism persistenceMechanism, BuildingRule buildingRule) {
            UserDirectory.HasInvertedDisplayNameGeneration = true;
            this.BuildingRule = buildingRule;
            this.OnlineStatusIndicators = new List<IOfflineCapable>();
            this.PageSettings = pageSettings;
            this.PersistenceMechanism = persistenceMechanism;
            this.PrimaryColor = PrimaryColor.BlueGrey;
            this.RootUrl = "/";
            this.SecondaryColor = SecondaryColor.Cyan;
            var culture = this.PersistenceMechanism.UserDirectory.CurrentUser.Culture;
            if (null == culture || CultureInfo.InvariantCulture == culture) {
                culture = UserDirectory.AnonymousUser.Culture;
            }
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="pageSettings">settings to apply to web pages</param>
        /// <param name="userDirectory">directory to use for user
        /// resolval</param>
        /// <param name="securityModel">security model to use for
        /// accessing this persistence mechanism</param>
        /// <param name="buildingRule">rules for building controls
        /// of list tables and forms</param>
        public BusinessApplication(BusinessPageSettings pageSettings, UserDirectory userDirectory, SecurityModel securityModel, BuildingRule buildingRule)
            : this(pageSettings, new DummyPersistenceMechanism(userDirectory, securityModel), buildingRule) {
            // nothing to do
        }

        /// <summary>
        /// Determines whether current user is authorized to access
        /// this application.
        /// </summary>
        /// <returns>true if current user is authorized to access
        /// this application, false otherwise</returns>
        public abstract bool CurrentUserIsAuthorized();

        /// <summary>
        /// Gets the global action buttons to display.
        /// </summary>
        /// <returns>global action buttons to display</returns>
        public virtual IEnumerable<Presentation.Buttons.LinkButton> GetGlobalActionButtons() {
            if (!this.PersistenceMechanism.UserDirectory.IsCurrentUserAnonymous) {
                var userProfileButton = new Presentation.Buttons.LinkButton(this.PersistenceMechanism.UserDirectory.CurrentUser.DisplayName, this.RootUrl + "user-profile.html");
                var allUsers = new Group("All users");
                allUsers.Members.Add(UserDirectory.AnonymousUser);
                userProfileButton.AllowedGroupsForReading.Add(allUsers);
                yield return userProfileButton;
            }
        }

        /// <summary>
        /// Gets all HTTP controllers to process.
        /// </summary>
        /// <returns>HTTP controllers to process</returns>
        public override IEnumerable<IHttpController> GetHttpControllers() {
            yield return new OptionsController();
            yield return new RootRedirectionController(this);
            yield return new Web.Controllers.WebAppManifestController(this);
            yield return new AuthorizationController(this);
            yield return new LocalizationSettingsPageController(this);
            yield return new ShortLinkRedirectionController(this);
            var globalActionButtons = BusinessPageController.FilterButtonsForCurrentUser(this.GetGlobalActionButtons(), this.PersistenceMechanism.UserDirectory.CurrentUser);
            foreach (var globalActionButton in globalActionButtons) {
                foreach (var childController in globalActionButton.GetChildControllers(null, this, null)) {
                    yield return childController;
                }
            }
            yield return new MaterialDesignController("style.css", this.PrimaryColor, this.SecondaryColor);
            yield return new JsonActiveWorkflowStepsController(this);
        }

        /// <summary>
        /// Gets the navigation items to show in menu of business
        /// application.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <returns>navigation items to show in menu of business
        /// application</returns>
        public abstract IEnumerable<NavigationItem> GetNavigationItems(HttpRequest httpRequest);

        /// <summary>
        /// Gets the web factory to use.
        /// </summary>
        /// <param name="renderMode">render mode of fields, e.g. for
        /// forms or for list tables</param>
        /// <returns>web factory to use</returns>
        public virtual WebFactory GetWebFactory(FieldRenderMode renderMode) {
            var optionDataProvider = new OptionDataProvider(this.PersistenceMechanism);
            return new WorkflowWebFactory(renderMode, optionDataProvider, this.BuildingRule, this.PageSettings);
        }

        /// <summary>
        /// Initializes this business application on each access of
        /// root path - this can be used for initialization of
        /// permission groups in database.
        /// </summary>
        public abstract void InitializeRoot();

    }

}