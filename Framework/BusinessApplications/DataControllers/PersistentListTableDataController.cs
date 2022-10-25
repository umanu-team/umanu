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
    /// Abstract base class for list table data controllers for
    /// persistent providable objects.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public abstract class PersistentListTableDataController<T> : ListTableDataController<T>, ISearchController<T> where T : PersistentObject, IProvidableObject, new() {

        /// <summary>
        /// Provider of persistent master/detail data.
        /// </summary>
        private readonly PersistentDataProvider<T> persistentDataProvider;

        /// <summary>
        /// Persistence mechanism to get data from.
        /// </summary>
        protected PersistenceMechanism PersistenceMechanism {
            get { return this.persistentDataProvider.PersistenceMechanism; }
        }

        /// <summary>
        /// Provider for search results.
        /// </summary>
        public ISearchProvider<T> SearchProvider {
            get { return this.persistentDataProvider; }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="dataProvider">provider of persistent
        /// master/detail data</param>
        public PersistentListTableDataController(PersistentDataProvider<T> dataProvider)
            : base(dataProvider) {
            this.persistentDataProvider = dataProvider;
        }

        /// <summary>
        /// Gets the view to use for faceted search.
        /// </summary>
        /// <returns>view to use for faceted search</returns>
        public virtual IFacetedSearchView GetFacetedSearchView() {
            return null;
        }

    }

}