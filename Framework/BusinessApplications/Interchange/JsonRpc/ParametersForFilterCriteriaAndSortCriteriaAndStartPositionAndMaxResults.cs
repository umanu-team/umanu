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

namespace Framework.BusinessApplications.Interchange.JsonRpc {

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;

    /// <summary>
    /// JSON-RPC parameters for filter criteria, sort criteria, start
    /// position and maximum results.
    /// </summary>
    internal sealed class ParametersForFilterCriteriaAndSortCriteriaAndStartPositionAndMaxResults : ParametersForFilterCriteriaAndSortCriteria {

        /// <summary>
        /// Maximum number of results.
        /// </summary>
        public ulong MaxResults {
            get { return this.maxResults.Value; }
            set { this.maxResults.Value = value; }
        }
        private readonly PersistentFieldForULong maxResults =
            new PersistentFieldForULong(nameof(MaxResults));

        /// <summary>
        /// Start position.
        /// </summary>
        public ulong StartPosition {
            get { return this.startPosition.Value; }
            set { this.startPosition.Value = value; }
        }
        private readonly PersistentFieldForULong startPosition =
            new PersistentFieldForULong(nameof(StartPosition));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForFilterCriteriaAndSortCriteriaAndStartPositionAndMaxResults()
            : base() {
            this.RegisterPersistentField(this.maxResults);
            this.RegisterPersistentField(this.startPosition);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="filterCriteria">filter criteria</param>
        /// <param name="sortCriteria">sort criteria</param>
        /// <param name="startPosition">start position</param>
        /// <param name="maxResults">maximum number of results</param>
        public ParametersForFilterCriteriaAndSortCriteriaAndStartPositionAndMaxResults(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults)
            : this() {
            this.FilterCriteria = filterCriteria;
            this.SortCriteria = sortCriteria;
            this.StartPosition = startPosition;
            this.MaxResults = maxResults;
        }

    }

}