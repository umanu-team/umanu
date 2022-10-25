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

namespace Framework.Persistence.Rdbms {

    using System.Collections.Generic;

    /// <summary>
    /// Represents a relational subquery.
    /// </summary>
    public sealed class RelationalSubquery {

        /// <summary>
        /// List of relational child queries.
        /// </summary>
        public IList<RelationalSubqueryChildQuery> ChildQueries { get; private set; }

        /// <summary>
        /// Full name of field subquery is for.
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Indicates whether relational sub query is for sub table.
        /// </summary>
        public bool IsForSubTable {
            get { return !string.IsNullOrEmpty(this.SubTableView); }
        }

        /// <summary>
        /// Internal name of sub table view to be queried.
        /// </summary>
        public string SubTableView { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">full name of field subquery is
        /// for</param>
        public RelationalSubquery(string fieldName)
            : base() {
            this.ChildQueries = new List<RelationalSubqueryChildQuery>();
            this.FieldName = fieldName;
        }

    }

}