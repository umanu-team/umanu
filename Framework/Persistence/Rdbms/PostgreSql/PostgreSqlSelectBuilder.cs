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

namespace Framework.Persistence.Rdbms.PostgreSql {

    using Framework.Persistence.Filters;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Text;

    /// <summary>
    /// Builder for PostgreSQL SELECT statements.
    /// </summary>
    public sealed class PostgreSqlSelectBuilder : DbSelectBuilder {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="columnNames">names of database columns to
        /// select</param>
        /// <param name="selectionMode">selection mode to apply</param>
        /// <param name="fullTextQuery">full-text query to select
        /// rows for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// rows for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="relationalJoins">relational joins to apply</param>
        /// <param name="joinType">type of join to use for filter criteria</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <param name="fieldNameConverter">converter for field
        /// names to be used in relational queries</param>
        /// <param name="dataTypeMapper">mapper for mapping .NET data
        /// types to RDBMS data types</param>
        public PostgreSqlSelectBuilder(string tableName, IEnumerable<string> columnNames, SelectionMode selectionMode, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ICollection<RelationalJoin> relationalJoins, RelationalJoinType joinType, ulong startPosition, ulong maxResults, IRelationalFieldNameConverter fieldNameConverter, DataTypeMapper dataTypeMapper)
            : base(tableName, columnNames, selectionMode, fullTextQuery, filterCriteria, sortCriteria, relationalJoins, joinType, startPosition, maxResults, fieldNameConverter, dataTypeMapper) {
            // nothing to do
        }

        /// <summary>
        /// Builds the SELECT command as string.
        /// </summary>
        /// <param name="parameters">database parameters which are
        /// contained in SELECT command</param>
        /// <returns>SELECT command as string</returns>
        public override string ToString(out List<DbParameter> parameters) {
            // TODO: Implement full-text query!
            var isForComplementResults = SelectionMode.SelectComplementResults == this.SelectionMode;
            parameters = new List<DbParameter>();
            // build command text
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("SELECT ");
            // aggregate function, distinct results
            var isAggregateFunction = this.AppendAggregateOrDistinctSelectionModeTo(commandTextBuilder);
            // select columns
            this.AppendColumnNamesTo(commandTextBuilder, isAggregateFunction);
            commandTextBuilder.Append(" FROM ");
            commandTextBuilder.Append(this.TableName);
            commandTextBuilder.Append(" AS ");
            commandTextBuilder.Append(RelationalJoin.RootTableAlias);
            commandTextBuilder.Append(' ');
            // add complementing clause
            if (isForComplementResults) {
                this.AppendComplementSelectionModeTo(commandTextBuilder);
            }
            // add join clause
            commandTextBuilder.Append(this.JoinBuilder.ToJoin(this.JoinType));
            // apply filter
            this.AppendFilterTo(commandTextBuilder, parameters);
            // close parantheses
            if (isForComplementResults) {
                commandTextBuilder.Append(')');
            }
            // apply sort criteria
            if (this.SortCriteria.Count > 0) {
                commandTextBuilder.Append(" ORDER BY ");
                var isFirstSortCriterion = true;
                foreach (var sortCriterion in this.SortCriteria) {
                    if (isFirstSortCriterion) {
                        isFirstSortCriterion = false;
                    } else {
                        commandTextBuilder.Append(", ");
                    }
                    commandTextBuilder.Append(this.FieldNameConverter.Escape(this.JoinBuilder.GetInternalFieldNameOf(sortCriterion.FieldName)));
                    if (SortDirection.Ascending == sortCriterion.SortDirection) {
                        commandTextBuilder.Append(" ASC");
                    } else { // SortDirection.Descending == sortCriterion.SortDirection
                        commandTextBuilder.Append(" DESC");
                    }
                }
            }
            // limit results
            if (this.MaxResults < ulong.MaxValue) {
                commandTextBuilder.Append(" LIMIT ");
                commandTextBuilder.Append(this.MaxResults);
            }
            if (this.StartPosition > 0) {
                commandTextBuilder.Append(" OFFSET ");
                commandTextBuilder.Append(this.StartPosition);
            }
            // close parantheses
            if (isAggregateFunction) {
                commandTextBuilder.Append(") r");
            }
            return commandTextBuilder.ToString();
        }

    }

}