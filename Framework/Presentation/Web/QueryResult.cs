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

    using Framework.Presentation.Buttons;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the result of a query of providable objects
    /// whereas the result is a subset of a total.
    /// </summary>
    /// <typeparam name="T">type of providable objects of result</typeparam>
    public class QueryResult<T> where T : class, IProvidableObject {

        /// <summary>
        /// Description to be shown on top.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// True if result is subset, false if result is total.
        /// </summary>
        public bool IsSubset {
            get { return this.ResultCount != this.TotalCount; }
        }

        /// <summary>
        /// Maximum number of results per page.
        /// </summary>
        public ulong MaxResults { get; private set; }

        /// <summary>
        /// Buttons for navigation within result pages.
        /// </summary>
        public ICollection<LinkButton> PagingButtons { get; private set; }

        /// <summary>
        /// Matching subset of providable objects.
        /// </summary>
        public ICollection<T> ProvidableObjects { get; private set; }

        /// <summary>
        /// Number of results of query in subset.
        /// </summary>
        public ulong ResultCount {
            get { return (ulong)this.ProvidableObjects.Count; }
        }

        /// <summary>
        /// Zero based index of first position of subset in total
        /// results.
        /// </summary>
        public ulong StartPosition { get; private set; }

        /// <summary>
        /// Total number of results of query independent of subset.
        /// </summary>
        public ulong TotalCount { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="providableObjects">matching subset of
        /// providable objects</param>
        /// <param name="startPosition">zero based index of first
        /// position of subset in total results</param>
        /// <param name="maxResults">maximum number of results per
        /// page</param>
        /// <param name="totalCount">total number of results of query
        /// independent of subset</param>
        public QueryResult(ICollection<T> providableObjects, ulong startPosition, ulong maxResults, ulong totalCount)
            : base() {
            this.PagingButtons = new List<LinkButton>(2);
            this.ProvidableObjects = providableObjects;
            this.StartPosition = startPosition;
            this.MaxResults = maxResults;
            this.TotalCount = totalCount;
        }

    }

}
