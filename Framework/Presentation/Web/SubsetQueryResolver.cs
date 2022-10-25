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

namespace Framework.Presentation.Web {

    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using Framework.Presentation.Buttons;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Resolves a subset of providable objects based on query
    /// string.
    /// </summary>
    /// <typeparam name="T">type of providable objects to be resolved</typeparam>
    public class SubsetQueryResolver<T> : QueryResolver<T> where T : class, IProvidableObject {

        /// <summary>
        /// Data provider to be used for resolval of matching
        /// providable objects.
        /// </summary>
        protected DataProvider<T> DataProvider { get; private set; }

        /// <summary>
        /// Default maximum number of providable objects per subset
        /// if top parameter is not present.
        /// </summary>
        public ulong DefaultMaximumSubsetSize { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpRequest">HTTP request to get parameters
        /// from</param>
        /// <param name="dataProvider">data provider to be used for
        /// resolval of matching providable objects</param>
        /// <param name="defaultMaximumSubsetSize">default maximum
        /// number of providable objects per subset if top parameter
        /// is not present</param>
        public SubsetQueryResolver(HttpRequest httpRequest, DataProvider<T> dataProvider, ulong defaultMaximumSubsetSize)
            : base(httpRequest) {
            this.DataProvider = dataProvider;
            this.DefaultMaximumSubsetSize = defaultMaximumSubsetSize;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="url">URL sent by client to get parameters
        /// from</param>
        /// <param name="dataProvider">data provider to be used for
        /// resolval of matching providable objects</param>
        /// <param name="defaultMaximumSubsetSize">default maximum
        /// number of providable objects per subset if top parameter
        /// is not present</param>
        public SubsetQueryResolver(Uri url, DataProvider<T> dataProvider, ulong defaultMaximumSubsetSize)
            : base(url) {
            this.DataProvider = dataProvider;
            this.DefaultMaximumSubsetSize = defaultMaximumSubsetSize;
        }

        /// <summary>
        /// Adds buttons for navigation within result pages to query
        /// result.
        /// </summary>
        /// <param name="queryResult">query result to add paging
        /// buttons to</param>
        /// <returns>buttons for navigation within result pages</returns>
        protected virtual void AddPagingButtonsTo(QueryResult<T> queryResult) {
            if (queryResult.IsSubset) {
                var skip = this.GetSkipParameter();
                if (queryResult.TotalCount > skip) {
                    var top = this.GetTopParameter();
                    if (skip > ulong.MinValue) {
                        var end = top;
                        if (end > skip) {
                            end = skip;
                        }
                        var titleFrom = skip - end + 1UL;
                        var titleTo = skip;
                        var title = titleFrom.ToString(CultureInfo.InvariantCulture) + " – " + titleTo.ToString(CultureInfo.InvariantCulture);
                        var urlParameters = new KeyValuePair<string, string>[] {
                            new KeyValuePair<string, string>("skip", (skip - end).ToString(CultureInfo.InvariantCulture)),
                            new KeyValuePair<string, string>("top", end.ToString(CultureInfo.InvariantCulture))
                        };
                        var previousButton = new LinkButton(title, "./" + UrlUtility.BuildQueryStringFor(urlParameters));
                        queryResult.PagingButtons.Add(previousButton);
                    }
                    if ((skip + top) < queryResult.TotalCount) {
                        var titleFrom = skip + top + 1UL;
                        var titleTo = skip + 2 * top;
                        if (titleTo > queryResult.TotalCount) {
                            titleTo = queryResult.TotalCount;
                        }
                        var title = titleFrom.ToString(CultureInfo.InvariantCulture);
                        if (titleTo > titleFrom) {
                            title += " – " + titleTo.ToString(CultureInfo.InvariantCulture);
                        }
                        var urlParameters = new KeyValuePair<string, string>[] {
                            new KeyValuePair<string, string>("skip", (skip + top).ToString(CultureInfo.InvariantCulture)),
                            new KeyValuePair<string, string>("top", top.ToString(CultureInfo.InvariantCulture))
                        };
                        var nextButton = new LinkButton(title, "./" + UrlUtility.BuildQueryStringFor(urlParameters));
                        queryResult.PagingButtons.Add(nextButton);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Builds a query string with all relevant parameters.
        /// </summary>
        /// <returns>query string with all relevant parameters</returns>
        public override string BuildQueryString() {
            var urlParameters = new List<KeyValuePair<string, string>>(2);
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
        /// Finds the matching result for query.
        /// </summary>
        /// <returns>matching result for query</returns>
        public override QueryResult<T> Execute() {
            var providableObjects = this.DataProvider.GetAll();
            return this.GetQueryResultForSubsetOf(providableObjects);
        }

        /// <summary>
        /// Finds the matching subset of providable objects for
        /// query.
        /// </summary>
        /// <returns>matching subset of providable objects for query</returns>
        public override ICollection<T> FindProvidableObjects() {
            var skip = this.GetSkipParameter();
            var top = this.GetTopParameter();
            return this.DataProvider.Find(FilterCriteria.Empty, SortCriterionCollection.Empty, skip, top);
        }

        /// <summary>
        /// Gets a query result for subset of providable object based
        /// on query parameters.
        /// </summary>
        /// <param name="providableObjects">providable objects to get
        /// subset of</param>
        /// <returns>query result for subset of providable object
        /// based on query parameters</returns>
        protected QueryResult<T> GetQueryResultForSubsetOf(ICollection<T> providableObjects) {
            QueryResult<T> queryResult;
            var skip = this.GetSkipParameter();
            var top = this.GetTopParameter();
            if (skip > ulong.MinValue || top < ulong.MaxValue) {
                providableObjects = new List<T>(providableObjects); // collection might be read-only
                var startPosition = skip;
                var maxResults = top;
                var totalCount = SubsetQueryResolver<T>.RoundToULong(providableObjects.Count);
                new Range(skip, top).ApplyTo(providableObjects);
                queryResult = new QueryResult<T>(providableObjects, startPosition, maxResults, totalCount);
                this.AddPagingButtonsTo(queryResult);
            } else {
                var startPosition = ulong.MinValue;
                var maxResults = ulong.MaxValue;
                var totalCount = SubsetQueryResolver<T>.RoundToULong(providableObjects.Count);
                queryResult = new QueryResult<T>(providableObjects, startPosition, maxResults, totalCount);
            }
            return queryResult;
        }

        /// <summary>
        /// Gets the value of skip parameter of query.
        /// </summary>
        /// <returns>value of skip parameter of query or
        /// ulong.MinValue if parameter has invalid value or is not
        /// present</returns>
        protected ulong GetSkipParameter() {
            var skip = this.GetULongParameter("skip");
            if (!skip.HasValue) {
                skip = ulong.MinValue;
            }
            return skip.Value;
        }

        /// <summary>
        /// Gets the value of top parameter of query.
        /// </summary>
        /// <returns>value of top parameter of query or
        /// ulong.MaxValue if parameter has invalid value or is not
        /// present</returns>
        protected ulong GetTopParameter() {
            var top = this.GetULongParameter("top");
            if (!top.HasValue) {
                top = this.DefaultMaximumSubsetSize;
            }
            return top.Value;
        }

    }

}