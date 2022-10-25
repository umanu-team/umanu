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

    using Framework.Persistence;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Base class of HTTP controllers for responding business web
    /// pages.
    /// </summary>
    public abstract class BusinessPageController : PageController<Page> {

        /// <summary>
        /// Business application to process.
        /// </summary>
        public IBusinessApplication BusinessApplication { get; private set; }

        /// <summary>
        /// Indicates whether navigation section is visible on page.
        /// </summary>
        public bool IsNavigationSectionVisible {
            get { return !this.Page.CssClasses.Contains("nonav"); }
        }

        /// <summary>
        /// Web control for title to be displayed.
        /// </summary>
        protected Label TitleLabel { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        public BusinessPageController(IBusinessApplication businessApplication)
            : base(new Page(businessApplication.PageSettings)) {
            this.BusinessApplication = businessApplication;
            if (!string.IsNullOrEmpty(businessApplication.WebAppManifestUrl)) {
                this.Page.AddWebAppManifest(businessApplication.WebAppManifestUrl);
            }
            this.TitleLabel = BusinessPageController.InitializePage(this.Page);
            var menuIcon = new MenuIcon();
            menuIcon.CssClasses.Add("menuicon");
            this.Page.HeaderSection.AddChildControl(menuIcon);
        }

        /// <summary>
        /// Adds an action bar to page.
        /// </summary>
        /// <param name="actionBar">action bar to add to page</param>
        protected void AddActionBarToPage<T>(ActionBar<T> actionBar) where T : class, IProvidableObject {
            actionBar.GlobalActions.CssClasses.Add("global");
            var globalActionButtonTemplates = BusinessPageController.FilterButtonsForCurrentUser(this.BusinessApplication.GetGlobalActionButtons(), this.BusinessApplication.PersistenceMechanism.UserDirectory.CurrentUser);
            foreach (var globalActionButtonTemplate in globalActionButtonTemplates) {
                var globalActionButton = new ClientSideWebButton(globalActionButtonTemplate, null);
                actionBar.GlobalActions.AddButton(globalActionButton);
            }
            this.Page.HeaderSection.AddChildControl(actionBar);
            return;
        }

        /// <summary>
        /// Filters an enumerable of buttons for current user. Only 
        /// buttons the current user is allowed to see are returned.
        /// </summary>
        /// <param name="buttons">enumerable of buttons to filter for
        /// current user</param>
        /// <returns>enumerable of buttons for current user is
        /// allowed to see</returns>
        protected IEnumerable<ActionButton> FilterButtonsForCurrentUser(IEnumerable<ActionButton> buttons) {
            return BusinessPageController.FilterButtonsForCurrentUser(buttons, this.BusinessApplication.PersistenceMechanism.UserDirectory.CurrentUser);
        }

        /// <summary>
        /// Filters an enumerable of buttons for current user. Only 
        /// buttons the current user is allowed to see are returned.
        /// </summary>
        /// <param name="buttons">enumerable of buttons to filter for
        /// current user</param>
        /// <param name="currentUser">current user to filter buttons
        /// for</param>
        /// <returns>enumerable of buttons for current user is
        /// allowed to see</returns>
        /// <typeparam name="T">type of buttons to be filtered</typeparam>
        public static IEnumerable<T> FilterButtonsForCurrentUser<T>(IEnumerable<T> buttons, IUser currentUser) where T : ActionButton {
            foreach (var button in buttons) {
                if (button.AllowedGroupsForReading.ContainsPermissionsFor(currentUser)) {
                    yield return button;
                }
            }
        }

        /// <summary>
        /// Hides the navigation section on page.
        /// </summary>
        public void HideNavigationSection() {
            if (this.IsNavigationSectionVisible) {
                this.Page.CssClasses.Add("nonav");
            }
            return;
        }

        /// <summary>
        /// Initializes the default page elements of a business
        /// application.
        /// </summary>
        /// <param name="page">page to be initialized</param>
        /// <returns>title label control</returns>
        public static Label InitializePage(Page page) {
            var navIcon = new Label("span");
            navIcon.CssClasses.Add("navicon");
            navIcon.Text = "menu";
            page.HeaderSection.AddChildControl(navIcon);
            var titleLabel = new Label("span");
            titleLabel.CssClasses.Add("navtitle");
            page.HeaderSection.AddChildControl(titleLabel);
            return titleLabel;
        }

        /// <summary>
        /// Processes a pre-processed web request.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false otherwise</returns>
        protected bool ProcessPreProcessedRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            this.Page.CreateChildControls(httpRequest);
            this.Page.HandleEvents(httpRequest, httpResponse);
            if (!httpResponse.IsRequestBeingRedirected) {
                this.SetPageProperties(httpRequest);
                this.Page.Render(httpResponse);
            }
            return true;
        }

        /// <summary>
        /// Sets default properties to page.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        protected void SetPageProperties(HttpRequest httpRequest) {
            var navTitle = new Label("span");
            navTitle.CssClasses.Add("navtitle");
            navTitle.Text = this.BusinessApplication.Title;
            this.Page.NavigationSection.TitleControl = navTitle;
            this.Page.NavigationSection.NavigationItems.AddRange(this.BusinessApplication.GetNavigationItems(httpRequest));
            this.Page.NavigationSection.AutoSelectItems(httpRequest);
            this.TitleLabel.Text = this.Page.Title;
            this.Page.ContentSection.AddChildControl(new OfflineIndicator(this.BusinessApplication.OnlineStatusIndicators));
            return;
        }

    }

}