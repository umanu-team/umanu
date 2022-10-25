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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// HTTP controller for responding dynamic CSV files based on
    /// list table views and search queries.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class CsvSearchController<T> : CsvListController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Controller for search.
        /// </summary>
        public ISearchController<T> SearchController { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of dynamic CSV
        /// file - it may not be empty, not contain any special
        /// charaters except for dashes and has to start with a slash</param>
        /// <param name="listTableDataController">data controller for
        /// dynamic CSV file</param>
        /// <param name="searchController">controller for search</param>
        public CsvSearchController(IBusinessApplication businessApplication, string absoluteUrl, ListTableDataController<T> listTableDataController, ISearchController<T> searchController)
            : base(businessApplication, absoluteUrl, listTableDataController) {
            this.SearchController = searchController;
        }

        /// <summary>
        /// Gets the business objects to be provided as CSV file
        /// </summary>
        /// <param name="url">URL of requested CSV file</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>business objects to be provided as CSV file</returns>
        protected override ICollection<T> GetProvidableObjects(Uri url, OptionDataProvider optionDataProvider) {
            var facetedSearchView = this.SearchController.GetFacetedSearchView();
            return new SearchQueryResolver<T>(url, this.ListTableDataController.DataProvider, this.SearchController.SearchProvider, ulong.MaxValue, facetedSearchView, optionDataProvider).FindProvidableObjects();
        }

    }

}