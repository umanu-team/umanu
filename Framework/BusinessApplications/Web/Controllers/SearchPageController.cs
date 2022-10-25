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
    using Framework.Persistence;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Properties;
    using Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding faceted search pages.
    /// </summary>
    /// <typeparam name="T">type of serach results</typeparam>
    public class SearchPageController<T> : BusinessPageController where T : PersistentObject, IProvidableObject, new() {

        /// <summary>
        /// Absolute URL of search page - it may not be empty, not
        /// contain any special charaters except for dashes and has
        /// to start with a slash.
        /// </summary>
        public string AbsoluteUrl {
            get {
                return this.absoluteUrl;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of search page does not start with a slash.");
                }
                this.absoluteUrl = value;
            }
        }
        private string absoluteUrl;

        /// <summary>
        /// Default maximum number of providable objects per subset
        /// if top parameter is not present.
        /// </summary>
        public ulong DefaultMaximumSubsetSize { get; set; } = 100ul;

        /// <summary>
        /// Optional lookup provider for search suggestions.
        /// </summary>
        public SearchSuggestionProvider LookupProvider { get; set; }

        /// <summary>
        /// Controller for search.
        /// </summary>
        public ISearchController<T> SearchController { get; private set; }

        /// <summary>
        /// View to apply on search results.
        /// </summary>
        public SearchResultView SearchResultsView { get; private set; }

        /// <summary>
        /// Factory for building web controls.
        /// </summary>
        protected WebFactory WebFactory { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of search page -
        /// it may not be empty, not contain any special charaters
        /// except for dashes and has to start with a slash</param>
        /// <param name="searchResultsView">view to apply on search
        /// results</param>
        public SearchPageController(IBusinessApplication businessApplication, string absoluteUrl, SearchResultView searchResultsView)
            : this(businessApplication, absoluteUrl, searchResultsView, new SimpleSearchController<T>(businessApplication.PersistenceMechanism, null)) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of search page -
        /// it may not be empty, not contain any special charaters
        /// except for dashes and has to start with a slash</param>
        /// <param name="searchResultsView">view to apply on search
        /// results</param>
        /// <param name="searchController">controller for search</param>
        public SearchPageController(IBusinessApplication businessApplication, string absoluteUrl, SearchResultView searchResultsView, ISearchController<T> searchController)
            : base(businessApplication) {
            this.AbsoluteUrl = absoluteUrl;
            this.SearchController = searchController;
            this.SearchResultsView = searchResultsView;
            this.TitleLabel.Text = Resources.Search;
            this.WebFactory = businessApplication.GetWebFactory(FieldRenderMode.Form);
        }

        /// <summary>
        /// Fills the page with a search page.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        protected void CreateSearchPage(HttpRequest httpRequest) {
            this.Page.Title = Resources.Search;
            var queryResolver = new SearchQueryResolver<T>(httpRequest, null, this.SearchController.SearchProvider, this.DefaultMaximumSubsetSize, this.SearchController.GetFacetedSearchView(), this.WebFactory.OptionDataProvider);
            var searchFieldPane = new SearchFieldPane<T>(queryResolver, this.SearchController.GetFacetedSearchView(), this.BusinessApplication.FileBaseDirectory, this.WebFactory) {
                LookupProvider = this.LookupProvider
            };
            this.Page.ContentSection.AddChildControl(searchFieldPane);
            if ("GET" == httpRequest.HttpMethod.ToUpperInvariant()) {
                var search = queryResolver.GetSearchParameter();
                if (string.IsNullOrEmpty(search)) {
                    this.Page.ContentSection.AddChildControl(new SearchHelpPane());
                } else {
                    var queryResult = queryResolver.Execute();
                    var actionBar = new ActionBar<T>();
                    actionBar.AddButtonRange(queryResult.PagingButtons);
                    this.AddActionBarToPage(actionBar);
                    this.Page.ContentSection.AddChildControl(new SearchResultsPane<T>(search, queryResult.ProvidableObjects, queryResult.Description, this.SearchResultsView, this.BusinessApplication.RootUrl + "l/", "/view.html"));
                }
                this.Page.AddOpenSearchDescription(this.AbsoluteUrl + "opensearchdescription.xml", this.BusinessApplication.Title);
            }
            return;
        }

        /// <summary>
        /// Gets the child controllers for handling sub pages of list
        /// page.
        /// </summary>
        /// <returns>child controllers for handling sub pages of list
        /// page</returns>
        private IEnumerable<IHttpController> GetChildControllers() {
            var openSearchDescriptionController = new OpenSearchDescriptionController(this.AbsoluteUrl + "opensearchdescription.xml", TextUtility.Truncate(this.BusinessApplication.Title, 16), TextUtility.Truncate(this.BusinessApplication.Title, 1024)) {
                HtmlTemplateUrl = BusinessApplications.BusinessApplication.Url + this.AbsoluteUrl.Substring(1) + "?search={searchTerms}&amp;top={count?}&amp;skip={startIndex?}",
                RssTemplateUrl = BusinessApplications.BusinessApplication.Url + this.AbsoluteUrl.Substring(1) + "feed.rss?search={searchTerms}&amp;top={count?}&amp;skip={startIndex?}"
            };
            if (!string.IsNullOrEmpty(this.BusinessApplication.PageSettings.FaviconUrl)) {
                openSearchDescriptionController.ImageUrl = BusinessApplications.BusinessApplication.Url + this.BusinessApplication.PageSettings.FaviconUrl.Substring(1);
            }
            yield return openSearchDescriptionController;
            var searchFacetsWithReplacedKeys = this.SearchController.GetFacetedSearchView()?.SearchFacets.WithReplacedKeys;
            if (null != searchFacetsWithReplacedKeys) {
                var fileSearchDataController = new JsonSearchDataController<T>(this.BusinessApplication.PersistenceMechanism, this.SearchResultsView);
                yield return new JsonSearchController<T>(this.BusinessApplication, this.AbsoluteUrl + "list.json", fileSearchDataController, this.SearchController);
                yield return new RssSearchController<T>(this.BusinessApplication, this.AbsoluteUrl + "feed.rss", fileSearchDataController, this.SearchController);
                var queryValues = new PresentableObject();
                var viewPane = new ViewPaneForFields();
                foreach (var searchFacetWithReplacedKey in searchFacetsWithReplacedKeys) {
                    var presentableField = searchFacetWithReplacedKey.CreatePresentableField(queryValues);
                    queryValues.AddPresentableField(presentableField);
                    viewPane.ViewFields.Add(searchFacetWithReplacedKey);
                }
                var formView = new FormView();
                formView.ViewPanes.Add(viewPane);
                yield return new LookupController(this.AbsoluteUrl, queryValues, formView, this.WebFactory.OptionDataProvider);
            }
            if (null != this.LookupProvider) {
                yield return new SearchSuggestionsController(this.AbsoluteUrl, SearchFieldPane<T>.SuggestionsEndpointName, this.LookupProvider, this.WebFactory.OptionDataProvider);
            }
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
            bool isProcessed;
            if (httpRequest.Url.AbsolutePath == this.AbsoluteUrl) {
                var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                if ("GET" == httpMethod || "POST" == httpMethod) {
                    this.CreateSearchPage(httpRequest);
                    this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            } else if (httpRequest.Url.AbsolutePath.StartsWith(this.AbsoluteUrl, StringComparison.Ordinal)) {
                isProcessed = this.ProcessPotentialSubPageRequest(httpRequest, httpResponse);
            } else {
                isProcessed = false;
            }
            return isProcessed;
        }

    }

}