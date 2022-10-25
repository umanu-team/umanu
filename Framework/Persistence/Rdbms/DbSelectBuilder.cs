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

    using Framework.Persistence.Filters;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Text;

    /// <summary>
    /// Builder for relational SELECT statements.
    /// </summary>
    public abstract class DbSelectBuilder {

        /// <summary>
        /// Names of database columns to select.
        /// </summary>
        protected IEnumerable<string> ColumnNames { get; private set; }

        /// <summary>
        /// Mapper for mapping .NET data types to data types of the
        /// RDBMS.
        /// </summary>
        protected DataTypeMapper DataTypeMapper { get; private set; }

        /// <summary>
        /// Converter for field names to be used in relational queries.
        /// </summary>
        protected IRelationalFieldNameConverter FieldNameConverter { get; private set; }

        /// <summary>
        /// Filter criteria to select rows for.
        /// </summary>
        protected FilterCriteria FilterCriteria { get; private set; }

        /// <summary>
        /// Full-text query to select rows for.
        /// </summary>
        protected string FullTextQuery { get; private set; }

        /// <summary>
        /// Join builder for resolving sub objects of filters.
        /// </summary>
        protected DbJoinBuilder JoinBuilder { get; private set; }

        /// <summary>
        /// Type of join to use for filter criteria.
        /// </summary>
        protected RelationalJoinType JoinType { get; private set; }

        /// <summary>
        /// Maximum number of results to return.
        /// </summary>
        protected ulong MaxResults { get; private set; }

        /// <summary>
        /// Selection mode to apply.
        /// </summary>
        protected SelectionMode SelectionMode { get; private set; }

        /// <summary>
        /// Criteria to sort objects by.
        /// </summary>
        protected SortCriterionCollection SortCriteria { get; private set; }

        /// <summary>
        /// Index of first position in results to return - "0" is the
        /// lowest index: "0" would return all results, whereas "5"
        /// would skip the five first results (this is useful for
        /// paging).
        /// </summary>
        protected ulong StartPosition { get; private set; }

        /// <summary>
        /// Name of database table.
        /// </summary>
        private protected string TableName { get; private set; }

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
        public DbSelectBuilder(string tableName, IEnumerable<string> columnNames, SelectionMode selectionMode, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ICollection<RelationalJoin> relationalJoins, RelationalJoinType joinType, ulong startPosition, ulong maxResults, IRelationalFieldNameConverter fieldNameConverter, DataTypeMapper dataTypeMapper)
            : base() {
            this.ColumnNames = columnNames;
            this.DataTypeMapper = dataTypeMapper;
            this.FieldNameConverter = fieldNameConverter;
            this.FilterCriteria = filterCriteria;
            this.FullTextQuery = fullTextQuery;
            this.JoinBuilder = new DbJoinBuilder(relationalJoins, fieldNameConverter);
            this.JoinType = joinType;
            this.MaxResults = maxResults;
            this.SelectionMode = selectionMode;
            this.SortCriteria = sortCriteria;
            this.StartPosition = startPosition;
            this.TableName = tableName;
        }

        /// <summary>
        /// Appends SQL for aggregate or distinct selection mode to
        /// command text builder.
        /// </summary>
        /// <param name="commandTextBuilder">command text builder to
        /// append SQL to</param>
        /// <returns>true if SELECT statement is for aggregate
        /// function, false otherwise</returns>
        protected bool AppendAggregateOrDistinctSelectionModeTo(StringBuilder commandTextBuilder) {
            bool isAggregateFunction = SelectionMode.SelectAverageResults == this.SelectionMode || SelectionMode.SelectSumsOfResults == this.SelectionMode;
            if (isAggregateFunction) {
                bool isFirstAggregate = true;
                foreach (string columnName in this.ColumnNames) {
                    if (isFirstAggregate) {
                        isFirstAggregate = false;
                    } else {
                        commandTextBuilder.Append(", ");
                    }
                    if (SelectionMode.SelectAverageResults == this.SelectionMode) {
                        commandTextBuilder.Append("AVG(");
                    } else if (SelectionMode.SelectSumsOfResults == this.SelectionMode) {
                        commandTextBuilder.Append("SUM(");
                    }
                    var indexOfLastDot = columnName.LastIndexOf('.');
                    commandTextBuilder.Append(this.FieldNameConverter.Escape("r." + columnName.Substring(indexOfLastDot + 1)));
                    commandTextBuilder.Append(")");
                }
                commandTextBuilder.Append(" FROM (SELECT ");
            }
            if (SelectionMode.SelectDistinctResults == this.SelectionMode || this.JoinBuilder.RelationalJoins.Count > 0) {
                commandTextBuilder.Append("DISTINCT ");
            }
            return isAggregateFunction;
        }

        /// <summary>
        /// Appends SQL for column names to command text builder.
        /// </summary>
        /// <param name="commandTextBuilder">command text builder to
        /// append SQL to</param>
        /// <param name="isAggregateFunction">true if SELECT
        /// statement is for aggregate function, false otherwise</param>
        protected void AppendColumnNamesTo(StringBuilder commandTextBuilder, bool isAggregateFunction) {
            if (isAggregateFunction) {
                commandTextBuilder.Append(this.JoinBuilder.GetInternalFieldNameOf(nameof(PersistentObject.Id)));
            }
            bool isFirstColumnName = !isAggregateFunction;
            foreach (string columnName in this.ColumnNames) {
                if (isFirstColumnName) {
                    isFirstColumnName = false;
                } else {
                    commandTextBuilder.Append(", ");
                }
                commandTextBuilder.Append(this.FieldNameConverter.Escape(this.JoinBuilder.GetInternalFieldNameOf(columnName)));
            }
            return;
        }

        /// <summary>
        /// Appends SQL for complement selection mode to command text
        /// builder.
        /// </summary>
        /// <param name="commandTextBuilder">command text builder to
        /// append SQL to</param>
        protected void AppendComplementSelectionModeTo(StringBuilder commandTextBuilder) {
            commandTextBuilder.Append(this.JoinBuilder.ToJoin(RelationalJoinType.LeftOuterJoin));
            commandTextBuilder.Append("WHERE ");
            commandTextBuilder.Append(this.JoinBuilder.GetInternalFieldNameOf(nameof(PersistentObject.Id)));
            commandTextBuilder.Append(" NOT IN (SELECT ");
            commandTextBuilder.Append(this.JoinBuilder.GetInternalFieldNameOf(nameof(PersistentObject.Id)));
            commandTextBuilder.Append(" FROM ");
            commandTextBuilder.Append(this.TableName);
            commandTextBuilder.Append(" AS ");
            commandTextBuilder.Append(RelationalJoin.RootTableAlias);
            commandTextBuilder.Append(' ');
            return;
        }

        /// <summary>
        /// Appends SQL for filters to command text builder.
        /// </summary>
        /// <param name="commandTextBuilder">command text builder to
        /// append SQL to</param>
        /// <param name="parameters">parameters which are contained
        /// in SQL command</param>
        protected void AppendFilterTo(StringBuilder commandTextBuilder, List<DbParameter> parameters) {
            if (!this.FilterCriteria.IsEmpty) {
                var dbFilter = new DbFilterBuilder(this.FilterCriteria, this.FieldNameConverter, this.DataTypeMapper, this.JoinBuilder, new RelationalSubqueryCollection()).ToFilter();
                commandTextBuilder.Append("WHERE ");
                commandTextBuilder.Append(dbFilter.Filter);
                parameters.AddRange(dbFilter.Parameters);
            }
            return;
        }

        /// <summary>
        /// Builds the SELECT command as string.
        /// </summary>
        /// <param name="dbParameters">database parameters which are
        /// contained in SELECT command</param>
        /// <returns>SELECT command as string</returns>
        public abstract string ToString(out List<DbParameter> dbParameters);

    }

}