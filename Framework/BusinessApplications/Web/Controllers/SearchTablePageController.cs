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
    using Framework.BusinessApplications.Web;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding business web pages for search
    /// tables.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class SearchTablePageController<T> : ListTablePageController<T> where T : class, IProvidableObject {

        /// <summary>
        /// True to show help pane if no results are present, false
        /// othwewise.
        /// </summary>
        public bool IsShowingHelpPaneIfNoResults { get; set; }

        /// <summary>
        /// Optional lookup provider for search suggestions.
        /// </summary>
        public SearchSuggestionProvider LookupProvider { get; set; }

        /// <summary>
        /// Controller for search.
        /// </summary>
        public ISearchController<T> SearchController { get; private set; }

        /// <summary>
        /// Factory for building web controls.
        /// </summary>
        protected WebFactory WebFactory { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of list table page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="listTableDataController">data controller for
        /// list table and forms</param>
        /// <param name="searchProvider">provider for search results</param>
        public SearchTablePageController(IBusinessApplication businessApplication, string absoluteUrl, ListTableDataController<T> listTableDataController, ISearchProvider<T> searchProvider)
            : this(businessApplication, absoluteUrl, listTableDataController, new DummySearchController<T>(searchProvider)) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of list table page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="listTableDataController">data controller for
        /// list table and forms</param>
        /// <param name="searchController">controller for search</param>
        public SearchTablePageController(IBusinessApplication businessApplication, string absoluteUrl, ListTableDataController<T> listTableDataController, ISearchController<T> searchController)
            : base(businessApplication, absoluteUrl, listTableDataController) {
            this.IsShowingHelpPaneIfNoResults = false;
            this.SearchController = searchController;
            this.WebFactory = this.BusinessApplication.GetWebFactory(FieldRenderMode.Form);
        }

        /// <summary>
        /// Fills the page with a representation of all relevant
        /// business objects.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        protected override void CreatePageForBusinessObjects(HttpRequest httpRequest) {
            var queryResolver = this.GetSearchQueryResolver(httpRequest);
            var searchFieldPane = new SearchFieldPane<T>(queryResolver, this.SearchController.GetFacetedSearchView(), this.BusinessApplication.FileBaseDirectory, this.WebFactory) {
                LookupProvider = this.LookupProvider
            };
            searchFieldPane.CssClasses.Add("fullwidth");
            this.Page.ContentSection.AddChildControl(searchFieldPane);
            if ("GET" == httpRequest.HttpMethod.ToUpperInvariant()) {
                var queryResult = queryResolver.Execute();
                var actionBar = new ActionBar<T>();
                actionBar.AddButtonRange(this.FilterButtonsForCurrentUser(this.ListTableDataController.GetListTableButtons()));
                actionBar.AddButtonRange(queryResult.PagingButtons);
                this.AddActionBarToPage(actionBar);
                if (queryResult.ProvidableObjects.Count > 0) {
                    this.ListTableDataController.DataProvider.Preload(queryResult.ProvidableObjects, this.GetKeyChainsToPreload());
                }
                var controlsForBusinessObjects = this.GetControlsForBusinessObjects(queryResult.ProvidableObjects, queryResult.IsSubset, queryResult.Description, out string pageTitle);
                this.Page.ContentSection.AddChildControls(controlsForBusinessObjects);
                this.Page.Title = pageTitle;
                this.Page.AddOpenSearchDescription(this.AbsoluteUrl + "opensearchdescription.xml", this.BusinessApplication.Title + ": " + this.Page.Title);
            }
            return;
        }

        /// <summary>
        /// Gets the child controllers for handling sub pages of list
        /// page.
        /// </summary>
        /// <returns>child controllers for handling sub pages of list
        /// page</returns>
        internal override IEnumerable<IHttpController> GetChildControllers() {
            foreach (var childController in base.GetChildControllers()) {
                yield return childController;
            }
            var title = this.ListTableDataController.GetListTableView().Title;
            var openSearchDescriptionController = new OpenSearchDescriptionController(this.AbsoluteUrl + "opensearchdescription.xml", TextUtility.Truncate(title, 16), TextUtility.Truncate(this.BusinessApplication.Title + ": " + title, 1024)) {
                HtmlTemplateUrl = BusinessApplications.BusinessApplication.Url + this.AbsoluteUrl.Substring(1) + "?search={searchTerms}&amp;top={count?}&amp;skip={startIndex?}",
                RssTemplateUrl = BusinessApplications.BusinessApplication.Url + this.AbsoluteUrl.Substring(1) + "feed.rss?search={searchTerms}&amp;top={count?}&amp;skip={startIndex?}"
            };
            if (!string.IsNullOrEmpty(this.BusinessApplication.PageSettings.FaviconUrl)) {
                openSearchDescriptionController.ImageUrl = BusinessApplications.BusinessApplication.Url + this.BusinessApplication.PageSettings.FaviconUrl.Substring(1);
            }
            yield return openSearchDescriptionController;
            var searchFacetsWithReplacedKeys = this.SearchController.GetFacetedSearchView()?.SearchFacets.WithReplacedKeys;
            if (null != searchFacetsWithReplacedKeys) {
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
        /// Gets the control to be rendered for business objects.
        /// </summary>
        /// <param name="businessObjects">business objects to be
        /// displayed</param>
        /// <param name="isSubset">true if business objects are a
        /// subset, false if they are total</param>
        /// <param name="description">description text to be
        /// displayed</param>
        /// <param name="pageTitle">title of list page to be set</param>
        /// <returns>new control to be rendered</returns>
        protected override ICollection<Control> GetControlsForBusinessObjects(ICollection<T> businessObjects, bool isSubset, string description, out string pageTitle) {
            var controls = base.GetControlsForBusinessObjects(businessObjects, isSubset, description, out pageTitle);
            ListTable listTable = null;
            foreach (var control in controls) {
                if (control is ListTable listTableControl) {
                    listTable = listTableControl;
                    break;
                }
            }
            if (null != listTable) {
                if (this.IsShowingHelpPaneIfNoResults && businessObjects.Count < 1) {
                    controls = new List<Control>(2);
                    if (!string.IsNullOrEmpty(description)) {
                        var infoPane = new InfoPane(description);
                        infoPane.CssClasses.Add(listTable.CssClassForDescriptionPane);
                        controls.Add(infoPane);
                    }
                    controls.Add(new SearchHelpPane());
                } else {
                    listTable.CssClasses.Add("results");
                }
            }
            return controls;
        }

        /// <summary>
        /// Get the resolver for providable objects based on query
        /// string.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <returns>resolver for providable objects based on query
        /// string</returns>
        protected sealed override QueryResolver<T> GetQueryResolver(HttpRequest httpRequest) {
            return this.GetSearchQueryResolver(httpRequest);
        }

        /// <summary>
        /// Get the resolver for providable objects based on query
        /// string.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <returns>resolver for providable objects based on query
        /// string</returns>
        protected virtual SearchQueryResolver<T> GetSearchQueryResolver(HttpRequest httpRequest) {
            var dataProvider = this.ListTableDataController.DataProvider;
            var searchProvider = this.SearchController.SearchProvider;
            var facettedSearchView = this.SearchController.GetFacetedSearchView();
            var optionDataProvider = this.WebFactory.OptionDataProvider;
            return new SearchQueryResolver<T>(httpRequest, dataProvider, searchProvider, this.DefaultMaximumSubsetSize, facettedSearchView, optionDataProvider);
        }

    }

}