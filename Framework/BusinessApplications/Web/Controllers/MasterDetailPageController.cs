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

    using Framework.BusinessApplications.DataControllers;
    using Framework.Presentation.Web;
    using Presentation;
    using Presentation.Forms;
    using Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Widgets;

    /// <summary>
    /// Base class of HTTP controllers for responding business web
    /// pages for master/detail.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public abstract class MasterDetailPageController<T> : BusinessPageController, IShortLinkableController where T : class, IProvidableObject {

        /// <summary>
        /// Absolute URL of list page - it may not be empty, not
        /// contain any special charaters except for dashes and has
        /// to start and end with a slash.
        /// </summary>
        public string AbsoluteUrl {
            get {
                return this.absoluteUrl;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of list page does not start with a slash.");
                }
                if (!value.EndsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of list page does not end with a slash.");
                }
                this.absoluteUrl = value;
            }
        }
        private string absoluteUrl;

        /// <summary>
        /// Default maximum number of providable objects per &quot;list&quot;
        /// page if top parameter is not present.
        /// </summary>
        public virtual ulong DefaultMaximumSubsetSize { get; set; } = 50ul;

        /// <summary>
        /// Data controller for master/detail.
        /// </summary>
        private readonly MasterDetailDataController<T> masterDetailDataController;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// be processed</param>
        /// <param name="absoluteUrl">absolute URL of list page - it
        /// may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="masterDetailDataController">data controller
        /// for master/detail</param>
        public MasterDetailPageController(IBusinessApplication businessApplication, string absoluteUrl, MasterDetailDataController<T> masterDetailDataController)
            : base(businessApplication) {
            this.AbsoluteUrl = absoluteUrl;
            this.masterDetailDataController = masterDetailDataController;
        }

        /// <summary>
        /// Adds a link to RSS feed to page.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        private void AddRssLinkToPage(HttpRequest httpRequest) {
            var queryString = SearchQueryResolver<T>.BuildQueryString(httpRequest);
            this.Page.AddRssFeed(this.AbsoluteUrl + "feed.rss" + queryString);
            return;
        }

        /// <summary>
        /// Fills the page with a representation of all relevant
        /// business objects.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        protected virtual void CreatePageForBusinessObjects(HttpRequest httpRequest) {
            var actionBar = new ActionBar<T>();
            actionBar.AddButtonRange(this.FilterButtonsForCurrentUser(this.masterDetailDataController.GetListTableButtons()));
            var queryResolver = this.GetQueryResolver(httpRequest);
            var queryResult = queryResolver.Execute();
            actionBar.AddButtonRange(queryResult.PagingButtons);
            this.AddActionBarToPage(actionBar);
            if (queryResult.ProvidableObjects.Count > 0) {
                this.masterDetailDataController.DataProvider.Preload(queryResult.ProvidableObjects, this.GetKeyChainsToPreload());
            }
            var controlsForBusinessObjects = this.GetControlsForBusinessObjects(queryResult.ProvidableObjects, queryResult.IsSubset, queryResult.Description, out string pageTitle);
            this.Page.ContentSection.AddChildControls(controlsForBusinessObjects);
            this.Page.Title = pageTitle;
            return;
        }

        /// <summary>
        /// Finds the absolute URL of a list page containing an
        /// object with a specific ID.
        /// </summary>
        /// <param name="id">specific ID to find</param>
        /// <returns>absolute URL of a list page containing the
        /// object with the specific ID or null if no such list page
        /// can be found</returns>
        public virtual string FindAbsoluteUrlOfListPageContaining(Guid id) {
            string absoluteUrl;
            if (null == this.masterDetailDataController.DataProvider.FindOne(id.ToString("N"))) {
                absoluteUrl = null;
            } else {
                absoluteUrl = this.AbsoluteUrl;
            }
            return absoluteUrl;
        }

        /// <summary>
        /// Gets the child controllers for handling sub pages of list
        /// page.
        /// </summary>
        /// <returns>child controllers for handling sub pages of list
        /// page</returns>
        internal virtual IEnumerable<IHttpController> GetChildControllers() {
            var viewWidget = new ViewWidgetForListTable<T>(this.masterDetailDataController);
            var childControllers = viewWidget.GetChildControllers(this.BusinessApplication, this.AbsoluteUrl, null);
            foreach (var childController in childControllers) {
                if (!this.IsNavigationSectionVisible && childController is BusinessPageController businessPageController) {
                    businessPageController.HideNavigationSection();
                }
                yield return childController;
            }
        }

        /// <summary>
        /// Gets the controls to be rendered for business objects.
        /// </summary>
        /// <param name="businessObjects">business objects to be
        /// displayed</param>
        /// <param name="isSubset">true if business objects are a
        /// subset, false if they are total</param>
        /// <param name="description">description text to be
        /// displayed</param>
        /// <param name="pageTitle">title of list page to be set</param>
        /// <returns>new controls to be rendered</returns>
        protected abstract ICollection<Control> GetControlsForBusinessObjects(ICollection<T> businessObjects, bool isSubset, string description, out string pageTitle);

        /// <summary>
        /// Gets key chains to be preloaded.
        /// </summary>
        /// <returns>key chains to be preloaded</returns>
        protected virtual IList<string[]> GetKeyChainsToPreload() {
            return this.masterDetailDataController.GetKeyChainsToPreloadForListTableView();
        }

        /// <summary>
        /// Gets the delegate for resolval of on click URL for a
        /// presentable object.
        /// </summary>
        /// <param name="businessObjects">business objects to get
        /// delegate for</param>
        /// <returns>delegate for resolval of on click URL for a
        /// presentable object</returns>
        protected virtual OnClickUrlDelegate GetOnClickUrlDelegate(IEnumerable<T> businessObjects) {
            OnClickUrlDelegate onClickUrlDelegate = null;
#if !DEBUG
            try {
#endif
            foreach (var businessObject in businessObjects) {
                if (null != this.masterDetailDataController.GetViewFormView(businessObject)) {
                    onClickUrlDelegate = delegate (IPresentableObject clickedObject) {
                        string onClickUrl;
                        if (null != clickedObject) {
                            onClickUrl = "./" + clickedObject.Id.ToString("N") + "/view.html";
                        } else {
                            onClickUrl = null;
                        }
                        return onClickUrl;
                    };
                }
                break;
            }
#if !DEBUG
            } catch (Exception exception) {
                Model.JobQueue.Log?.WriteEntry(exception, Framework.Diagnostics.LogLevel.Warning);
            }
#endif
            return onClickUrlDelegate;
        }

        /// <summary>
        /// Get the resolver for providable objects based on query
        /// string.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <returns>resolver for providable objects based on query
        /// string</returns>
        protected virtual QueryResolver<T> GetQueryResolver(HttpRequest httpRequest) {
            return new SubsetQueryResolver<T>(httpRequest, this.masterDetailDataController.DataProvider, this.DefaultMaximumSubsetSize);
        }

        /// <summary>
        /// Processes a potential sub page request.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        protected bool ProcessPotentialSubPageRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed = false;
            foreach (var childController in this.GetChildControllers()) {
                isProcessed = childController.ProcessRequest(httpRequest, httpResponse);
                if (isProcessed) {
                    break;
                }
            }
            return isProcessed;
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
            bool isProcessed = false;
            if (httpRequest.Url.AbsolutePath == this.AbsoluteUrl) {
                var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                if ("GET" == httpMethod || "POST" == httpMethod) {
                    this.AddRssLinkToPage(httpRequest);
                    this.CreatePageForBusinessObjects(httpRequest);
                    this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            } else if (httpRequest.Url.AbsolutePath.StartsWith(this.AbsoluteUrl, StringComparison.Ordinal)) {
                isProcessed = this.ProcessPotentialSubPageRequest(httpRequest, httpResponse);
            }
            return isProcessed;
        }

    }

}