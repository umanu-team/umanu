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

namespace Framework.BusinessApplications.DataProviders {

    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a simple search provider for persistent objects.
    /// </summary>
    /// <typeparam name="T">type of objects to provide</typeparam>
    public sealed class SimpleSearchProvider<T> : ISearchProvider<T> where T : PersistentObject, new() {

        /// <summary>
        /// Persistence mechanism to get data from.
        /// </summary>
        private readonly PersistenceMechanism persistenceMechanism;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        public SimpleSearchProvider(PersistenceMechanism persistenceMechanism)
            : base() {
            this.persistenceMechanism = persistenceMechanism;
        }

        /// <summary>
        /// Finds all matching results for query.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to find
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to return</param>
        /// <returns>search results matching the query</returns>
        public ICollection<T> FindFullText(string fullTextQuery, FilterCriteria filterCriteria, ulong startPosition, ulong maxResults) {
            var container = this.persistenceMechanism.FindContainer<T>();
            return container.FindFullText(fullTextQuery, filterCriteria, startPosition, maxResults);
        }

    }

}