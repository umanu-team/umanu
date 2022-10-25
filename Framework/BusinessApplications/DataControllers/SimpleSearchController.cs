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

namespace Framework.BusinessApplications.DataControllers {

    using Framework.BusinessApplications.DataProviders;
    using Framework.Persistence;
    using Framework.Presentation;
    using Framework.Presentation.Forms;

    /// <summary>
    /// Simple search provider for persistent objects.
    /// </summary>
    /// <typeparam name="T">type of objects to control</typeparam>
    public sealed class SimpleSearchController<T> : ISearchController<T> where T : PersistentObject, new() {

        /// <summary>
        /// View to use for faceted search.
        /// </summary>
        private readonly IFacetedSearchView facetedSearchView;

        /// <summary>
        /// Provider for search results.
        /// </summary>
        public ISearchProvider<T> SearchProvider { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        /// <param name="facetedSearchView">view to use for faceted
        /// search</param>
        public SimpleSearchController(PersistenceMechanism persistenceMechanism, IFacetedSearchView facetedSearchView)
            : base() {
            this.facetedSearchView = facetedSearchView;
            this.SearchProvider = new SimpleSearchProvider<T>(persistenceMechanism);
        }

        /// <summary>
        /// Gets the view to use for faceted search.
        /// </summary>
        /// <returns>view to use for faceted search</returns>
        public IFacetedSearchView GetFacetedSearchView() {
            return this.facetedSearchView;
        }

    }

}