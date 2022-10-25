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
    /// Proxy data controller to be used for JSON data search.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    internal sealed class JsonSearchDataProvider<T> : PersistentReadOnlyDataProvider<T> where T : PersistentObject, IProvidableObject, new() {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data for preloading from</param>
        public JsonSearchDataProvider(PersistenceMechanism persistenceMechanism)
            : base(persistenceMechanism, FilterCriteria.Empty) {
            // nothing to do
        }

        /// <summary>
        /// Finds all matching results of this data provider.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>all matching objects from this container</returns>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <returns>matching objects from this data provider</returns>
        public override ICollection<T> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            return new T[0];
        }

    }

}