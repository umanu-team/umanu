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

namespace Framework.BusinessApplications.Web {

    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Finds providable objects based on query string.
    /// </summary>
    /// <typeparam name="T">type of providable objects to be resolved</typeparam>
    public class SearchQueryResolver<T> : SubsetQueryResolver<T> where T : class, IProvidableObject {

        /// <summary>
        /// Facetted search query to be processed.
        /// </summary>
        protected FacetedSearchQuery FacetedSearchQuery { get; private set; }

        /// <summary>
        /// Provider for search results.
        /// </summary>
        protected ISearchProvider<T> SearchProvider { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpRequest">HTTP request to get parameters
        /// from</param>
        /// <param name="dataProvider">data provider to be used for
        /// resolval of matching providable objects</param>
        /// <param name="searchProvider">provider for search results</param>
        /// <param name="defaultMaximumSubsetSize">default maximum
        /// number of providable objects per subset if top parameter
        /// is not present</param>
        public SearchQueryResolver(HttpRequest httpRequest, DataProvider<T> dataProvider, ISearchProvider<T> searchProvider, ulong defaultMaximumSubsetSize)
            : this(httpRequest, dataProvider, searchProvider, defaultMaximumSubsetSize, null, null) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="url">URL sent by client to get parameters
        /// from</param>
        /// <param name="dataProvider">data provider to be used for
        /// resolval of matching providable objects</param>
        /// <param name="searchProvider">provider for search results</param>
        /// <param name="defaultMaximumSubsetSize">default maximum
        /// number of providable objects per subset if top parameter
        /// is not present</param>
        public SearchQueryResolver(Uri url, DataProvider<T> dataProvider, ISearchProvider<T> searchProvider, ulong defaultMaximumSubsetSize)
            : this(url, dataProvider, searchProvider, defaultMaximumSubsetSize, null, null) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpRequest">HTTP request to get parameters
        /// from</param>
        /// <param name="dataProvider">data provider to be used for
        /// resolval of matching providable objects</param>
        /// <param name="searchProvider">provider for search results</param>
        /// <param name="defaultMaximumSubsetSize">default maximum
        /// number of providable objects per subset if top parameter
        /// is not present</param>
        /// <param name="facetedSearchView">view for faceted search</param>
        /// <param name="optionDataProvider">data provider for option
        /// providers</param>
        public SearchQueryResolver(HttpRequest httpRequest, DataProvider<T> dataProvider, ISearchProvider<T> searchProvider, ulong defaultMaximumSubsetSize, IFacetedSearchView facetedSearchView, IOptionDataProvider optionDataProvider)
            : base(httpRequest, dataProvider, defaultMaximumSubsetSize) {
            if (null != facetedSearchView) {
                this.FacetedSearchQuery = new FacetedSearchQuery(this.GetSearchParameter(), facetedSearchView.SearchFacets, optionDataProvider);
            }
            this.SearchProvider = searchProvider;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="url">URL sent by client to get parameters
        /// from</param>
        /// <param name="dataProvider">data provider to be used for
        /// resolval of matching providable objects</param>
        /// <param name="searchProvider">provider for search results</param>
        /// <param name="defaultMaximumSubsetSize">default maximum
        /// number of providable objects per subset if top parameter
        /// is not present</param>
        /// <param name="facetedSearchView">view for faceted search</param>
        /// <param name="optionDataProvider">data provider for option
        /// providers</param>
        public SearchQueryResolver(Uri url, DataProvider<T> dataProvider, ISearchProvider<T> searchProvider, ulong defaultMaximumSubsetSize, IFacetedSearchView facetedSearchView, IOptionDataProvider optionDataProvider)
            : base(url, dataProvider, defaultMaximumSubsetSize) {
            if (null != facetedSearchView) {
                this.FacetedSearchQuery = new FacetedSearchQuery(this.GetSearchParameter(), facetedSearchView.SearchFacets, optionDataProvider);
            }
            this.SearchProvider = searchProvider;
        }

        /// <summary>
        /// Adds buttons for navigation within result pages to query
        /// result.
        /// </summary>
        /// <param name="queryResult">query result to add paging
        /// buttons to</param>
        /// <returns>buttons for navigation within result pages</returns>
        protected override void AddPagingButtonsTo(QueryResult<T> queryResult) {
            base.AddPagingButtonsTo(queryResult);
            var search = this.GetSearchParameter();
            if (!string.IsNullOrEmpty(search)) {
                foreach (var pagingButton in queryResult.PagingButtons) {
                    pagingButton.TargetUrl += "&search=" + HttpUtility.UrlEncode(search);
                }
            }
            return;
        }

        /// <summary>
        /// Builds a query string with all relevant parameters.
        /// </summary>
        /// <returns>query string with all relevant parameters</returns>
        public sealed override string BuildQueryString() {
            var urlParameters = new List<KeyValuePair<string, string>>(3);
            if (null == this.FacetedSearchQuery) {
                var search = this.GetSearchParameter();
                if (!string.IsNullOrEmpty(search)) {
                    urlParameters.Add(new KeyValuePair<string, string>("search", HttpUtility.UrlEncode(search)));
                }
            } else {
                urlParameters.Add(new KeyValuePair<string, string>("search", HttpUtility.UrlEncode(this.FacetedSearchQuery.CombinedQuery)));
            }
            var skip = this.GetSkipParameter();
            if (skip > ulong.MinValue) {
                urlParameters.Add(new KeyValuePair<string, string>("skip", skip.ToString(CultureInfo.InvariantCulture)));
            }
            var top = this.GetTopParameter();
            if (top < ulong.MaxValue) {
                urlParameters.Add(new KeyValuePair<string, string>("top", top.ToString(CultureInfo.InvariantCulture)));
            }
            return UrlUtility.BuildQueryStringFor(urlParameters);
        }

        /// <summary>
        /// Builds a query string with all relevant parameters.
        /// </summary>
        /// <param name="httpRequest">HTTP request to get parameters
        /// from</param>
        /// <returns>query string with all relevant parameters</returns>
        public static string BuildQueryString(HttpRequest httpRequest) {
            return new SearchQueryResolver<T>(httpRequest, null, null, ulong.MaxValue).BuildQueryString();
        }

        /// <summary>
        /// Builds a query string with all relevant parameters.
        /// </summary>
        /// <param name="url">URL sent by client to get parameters
        /// from</param>
        /// <returns>query string with all relevant parameters</returns>
        public static string BuildQueryString(Uri url) {
            return new SearchQueryResolver<T>(url, null, null, ulong.MaxValue).BuildQueryString();
        }

        /// <summary>
        /// Builds a query string with search parameter only.
        /// </summary>
        /// <param name="httpRequest">HTTP request to get search
        /// parameter from</param>
        /// <returns>query string with search parameter only</returns>
        public static string BuildSearchOnlyQueryString(HttpRequest httpRequest) {
            return SearchQueryResolver<T>.BuildSearchOnlyQueryString(httpRequest.Url);
        }

        /// <summary>
        /// Builds a query string with search parameter only.
        /// </summary>
        /// <param name="url">URL sent by client to get search
        /// parameter from</param>
        /// <returns>query string with search parameter only</returns>
        public static string BuildSearchOnlyQueryString(Uri url) {
            var urlParameters = new List<KeyValuePair<string, string>>(1);
            var search = new SearchQueryResolver<T>(url, null, null, ulong.MaxValue).GetSearchParameter();
            if (!string.IsNullOrEmpty(search)) {
                urlParameters.Add(new KeyValuePair<string, string>("search", HttpUtility.UrlEncode(search)));
            }
            return UrlUtility.BuildQueryStringFor(urlParameters);
        }

        /// <summary>
        /// Finds the matching result for query.
        /// </summary>
        /// <returns>matching result for query</returns>
        public override QueryResult<T> Execute() {
            var search = this.GetSearchParameter();
            ICollection<T> providableObjects;
            if (string.IsNullOrEmpty(search)) {
                if (null == this.DataProvider) {
                    providableObjects = new T[0];
                } else {
                    providableObjects = this.DataProvider.GetAll();
                }
            } else if (null == this.FacetedSearchQuery) {
                providableObjects = this.SearchProvider.FindFullText(search, FilterCriteria.Empty, ulong.MinValue, ulong.MaxValue);
            } else {
                providableObjects = this.SearchProvider.FindFullText(this.FacetedSearchQuery.FullTextQuery, this.FacetedSearchQuery.GetFilterCriteria(), ulong.MinValue, ulong.MaxValue);
            }
            var queryResult = this.GetQueryResultForSubsetOf(providableObjects);
            if (!string.IsNullOrEmpty(search)) {
                queryResult.Description = SearchQueryResolver<T>.GetDescriptionTextForNumberOfResults(search, queryResult.TotalCount);
                if (queryResult.IsSubset) {
                    if (ulong.MinValue == queryResult.StartPosition) {
                        queryResult.Description += ' ' + string.Format(Resources.TheFirst0ResultsAreDisplayedBelow, queryResult.ResultCount);
                    } else {
                        queryResult.Description += ' ' + string.Format(Resources.Results0To1AreDisplayedBelow, queryResult.StartPosition + 1UL, queryResult.StartPosition + queryResult.ResultCount);
                    }
                }
            }
            return queryResult;
        }

        /// <summary>
        /// Finds the matching subset of providable objects for
        /// query.
        /// </summary>
        /// <returns>matching subset of providable objects for query</returns>
        public override ICollection<T> FindProvidableObjects() {
            ICollection<T> providableObjects;
            var search = this.GetSearchParameter();
            if (string.IsNullOrEmpty(search)) {
                if (null == this.DataProvider) {
                    providableObjects = new T[0];
                } else {
                    providableObjects = base.FindProvidableObjects();
                }
            } else {
                var skip = this.GetSkipParameter();
                var top = this.GetTopParameter();
                if (null == this.FacetedSearchQuery) {
                    providableObjects = this.SearchProvider.FindFullText(search, FilterCriteria.Empty, skip, top);
                } else {
                    providableObjects = this.SearchProvider.FindFullText(this.FacetedSearchQuery.FullTextQuery, this.FacetedSearchQuery.GetFilterCriteria(), skip, top);
                }
            }
            return providableObjects;
        }

        /// <summary>
        /// Gets the description text for number of results.
        /// </summary>
        /// <param name="query">query to get description text for</param>
        /// <param name="count">number of results to get description
        /// text for</param>
        /// <returns>description text for number of results</returns>
        private static string GetDescriptionTextForNumberOfResults(string query, ulong count) {
            string descriptionText;
            if (count < 1UL) {
                descriptionText = string.Format(Resources.NoResultsWereFoundFor0, query);
            } else if (1UL == count) {
                descriptionText = string.Format(Resources.OneResultWasFoundFor0, query);
            } else {
                descriptionText = string.Format(Resources.NResultsWereFoundFor0, query, count);
            }
            return descriptionText;
        }

        /// <summary>
        /// Gets the value of search parameter of query.
        /// </summary>
        /// <returns>value of search parameter of query</returns>
        public string GetSearchParameter() {
            return this.GetStringParameter("search");
        }

        /// <summary>
        /// Replaces the keys in the filter criteria.
        /// </summary>
        /// <param name="queryOperators">wrapper of all relational
        /// operators to be applied</param>
        /// <param name="queryValues">wrapper of all values to be
        /// filtered by</param>
        public void ReplaceKeysInFilterCriteria(PresentableObject queryOperators, PresentableObject queryValues) {
            this.FacetedSearchQuery?.ReplaceKeysInFilterCriteria(queryOperators, queryValues);
            return;
        }

    }

}