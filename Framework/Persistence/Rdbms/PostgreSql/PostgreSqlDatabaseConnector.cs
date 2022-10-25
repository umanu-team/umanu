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

    using Framework.Diagnostics;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using Npgsql;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Text;
    using System.Transactions;

    /// <summary>
    /// Connector to PostgreSQL database.
    /// </summary>
    internal sealed class PostgreSqlDatabaseConnector : RelationalDatabaseConnector<NpgsqlConnection> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="connectionString">connection settings for
        /// accessing the SQL database</param>
        public PostgreSqlDatabaseConnector(string connectionString)
            : base(connectionString, new PostgreSqlFieldNameConverter(), new PostgreSqlDataTypeMapper(), new DbCommandLogger()) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="connectionString">connection settings for
        /// accessing the SQL database</param>
        /// <param name="log">log to use</param>
        public PostgreSqlDatabaseConnector(string connectionString, ILog log)
            : base(connectionString, new PostgreSqlFieldNameConverter(), new PostgreSqlDataTypeMapper(), new DbCommandLogger(log)) {
            // nothing to do
        }

        /// <summary>
        /// Removes a constraint from a database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="constraintName">name of constraint to remove
        /// from database table</param>
        private void AlterTableDropConstraint(string tableName, string constraintName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("ALTER TABLE ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" DROP CONSTRAINT ");
            commandTextBuilder.Append(constraintName);
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Removes the foreign keys of a field from a database
        /// table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="fieldName">name of field to remove foreign
        /// keys for</param>
        public override void AlterTableDropForeignKeys(string tableName, string fieldName) {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                var commandTextBuilder = new StringBuilder();
                commandTextBuilder.Append("SELECT con.conname FROM pg_constraint con JOIN LATERAL UNNEST(con.conkey) WITH ORDINALITY AS u(attnum, attposition) ON TRUE JOIN pg_class tbl ON tbl.oid=con.conrelid JOIN pg_attribute col ON(col.attrelid=tbl.oid AND col.attnum=u.attnum) WHERE con.contype='f' AND tbl.relname=@p0 AND col.attname=@p1");
                var parameters = new DbParameter[2];
                parameters[0] = this.dataTypeMapper.CreateDbParameter("@p0", tableName.ToLowerInvariant(), TypeOf.IndexableString);
                parameters[1] = this.dataTypeMapper.CreateDbParameter("@p1", fieldName.ToLowerInvariant(), TypeOf.IndexableString);
                var foreignKeyNames = new List<string>();
                this.ExecuteCommandReader(commandTextBuilder.ToString(), parameters, delegate (DbDataReader dataReader) {
                    while (dataReader.Read()) {
                        foreignKeyNames.Add(dataReader.GetString(0));
                    }
                    return;
                });
                foreach (var foreignKeyName in foreignKeyNames) {
                    this.AlterTableDropConstraint(tableName, foreignKeyName);
                    var indexName = "i_" + foreignKeyName.Substring(0, foreignKeyName.Length - 5);
                    this.DropIndex(indexName);
                }
                transactionScope.Complete();
            }
            return;
        }

        /// <summary>
        /// Updates a field type in database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="fieldType">type of field to update type for
        /// in database table</param>
        public override void AlterTableUpdateFieldType(string tableName, PersistentFieldType fieldType) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("ALTER TABLE ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" ALTER COLUMN ");
            commandTextBuilder.Append(this.fieldNameConverter.Escape(fieldType.Key));
            commandTextBuilder.Append(" TYPE ");
            commandTextBuilder.Append(this.dataTypeMapper[fieldType.ContentBaseType]);
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Copies data from a column of a source table into
        /// relations table.
        /// </summary>
        /// <param name="sourceTableName">name of database table to
        /// copy data from</param>
        /// <param name="sourceColumnName">name of source column to
        /// copy data for</param>
        /// <param name="targetColumnName">name of target column to
        /// copy data for</param>
        internal override void CopyDataToRelationsTable(string sourceTableName, string sourceColumnName, string targetColumnName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("INSERT INTO ");
            commandTextBuilder.Append(RelationalDatabase.CollectionRelationsTableName);
            commandTextBuilder.Append(" (");
            commandTextBuilder.Append(nameof(PersistentObject.Id));
            commandTextBuilder.Append(", ParentTable, ParentID, ParentField, ParentIndex, ChildType, ChildID) SELECT md5(random()::text || clock_timestamp()::text)::uuid, '");
            commandTextBuilder.Append(sourceTableName);
            commandTextBuilder.Append("', ");
            commandTextBuilder.Append(nameof(PersistentObject.Id));
            commandTextBuilder.Append(", '");
            commandTextBuilder.Append(targetColumnName);
            commandTextBuilder.Append("', 0, ");
            commandTextBuilder.Append(sourceColumnName);
            commandTextBuilder.Append("_, ");
            commandTextBuilder.Append(sourceColumnName);
            commandTextBuilder.Append(" FROM ");
            commandTextBuilder.Append(sourceTableName);
            commandTextBuilder.Append(" WHERE ");
            commandTextBuilder.Append(sourceColumnName);
            commandTextBuilder.Append(" IS NOT NULL AND ");
            commandTextBuilder.Append(sourceColumnName);
            commandTextBuilder.Append("_ IS NOT NULL");
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Creates a full text index for a table.
        /// </summary>
        /// <param name="catalogueName">name of full text catalogue</param>
        /// <param name="tableName">name of database table</param>
        /// <param name="indexName">name of index</param>
        /// <param name="textColumn">name of text column</param>
        /// <param name="languageCode">code of language of contents</param>
        public override int CreateFullTextIndex(string catalogueName, string tableName, string indexName, string textColumn, ushort languageCode) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a database view.
        /// </summary>
        /// <param name="name">name of view</param>
        /// <param name="unions">relational unions to be part of
        /// relational view</param>
        /// <returns>number of rows affected</returns>
        public override int CreateView(string name, IEnumerable<RelationalViewUnion> unions) {
            return base.CreateView(name, unions, true);
        }

        /// <summary>
        /// Drops the full text index of a table.
        /// </summary>
        /// <param name="catalogueName">name of full text catalogue</param>
        /// <param name="tableName">name of database table</param>
        /// <returns>number of rows affected</returns>
        public override int DropFullTextIndex(string catalogueName, string tableName) {
            // throw new NotImplementedException();
            return 0; // HACK
        }

        /// <summary>
        /// Removes an index.
        /// </summary>
        /// <param name="indexName">name of index to remove</param>
        private void DropIndex(string indexName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("DROP INDEX ");
            commandTextBuilder.Append(indexName);
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Deletes specific rows from a database table.
        /// <param name="tableName">name of database table</param>
        /// <param name="filterCriteria">filter criteria to delete
        /// rows for</param>
        /// <param name="relationalJoins">relational joins to apply</param>
        /// <param name="relationalSubqueries">relational subqueries
        /// to apply</param>
        /// <returns>number of rows affected</returns>
        /// </summary>
        public override int DeleteRowsFromTable(string tableName, FilterCriteria filterCriteria, ICollection<RelationalJoin> relationalJoins, RelationalSubqueryCollection relationalSubqueries) {
            var joinBuilder = new DbJoinBuilder(relationalJoins, this.fieldNameConverter);
            IEnumerable<DbParameter> parameters = new DbParameter[0];
            // build command text
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("DELETE FROM ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" AS ");
            commandTextBuilder.Append(RelationalJoin.RootTableAlias);
            commandTextBuilder.Append(' ');
            var hasRelationalJoins = relationalJoins.Count > 0;
            if (hasRelationalJoins) {
                // sub query
                commandTextBuilder.Append("WHERE ");
                commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                commandTextBuilder.Append('.');
                commandTextBuilder.Append(nameof(PersistentObject.Id));
                commandTextBuilder.Append(" IN (SELECT ");
                commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                commandTextBuilder.Append('.');
                commandTextBuilder.Append(nameof(PersistentObject.Id));
                commandTextBuilder.Append(" FROM ");
                commandTextBuilder.Append(tableName);
                commandTextBuilder.Append(" AS ");
                commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                commandTextBuilder.Append(' ');
                // add join clause
                var joinType = this.GetJoinTypeFor(filterCriteria);
                commandTextBuilder.Append(joinBuilder.ToJoin(joinType));
            }
            // apply filter
            if (!filterCriteria.IsEmpty) {
                commandTextBuilder.Append("WHERE ");
                var dbFilter = new DbFilterBuilder(filterCriteria, this.fieldNameConverter, this.dataTypeMapper, joinBuilder, relationalSubqueries).ToFilter();
                commandTextBuilder.Append(dbFilter.Filter);
                parameters = dbFilter.Parameters;
            }
            if (hasRelationalJoins) {
                commandTextBuilder.Append(')');
            }
            // execute command
            return this.ExecuteCommandNonQuery(commandTextBuilder.ToString(), parameters);
        }

        /// <summary>
        /// Gets the SQL command text for detection of table changes.
        /// </summary>
        /// <param name="tableName">name of database table to detect
        /// changes for</param>
        /// <returns>SQL command text for detection of table changes</returns>
        protected override string GetTableSchemaCommandText(string tableName) {
            return "SELECT * FROM " + tableName + " LIMIT 1;";
        }

        /// <summary>
        /// Handles a database exception.
        /// </summary>
        /// <param name="exception">database exception to handle</param>
        /// <param name="tableName">name of database table affected
        /// by exception</param>
        protected override void HandleDbException(DbException exception, string tableName) {
            var sqlException = exception as NpgsqlException;
            if (null != sqlException) {
                if (-2147467259 == sqlException.ErrorCode && RelationalDatabase.ContainersTableName == tableName) {
                    throw new PersistenceMechanismNotInitializedException("Persistence mechanism was not initialized yet.", exception);
                } else if (-2147467259 == sqlException.ErrorCode && RelationalDatabase.FullTextTableName == tableName) {
                    throw new FullTextIndexNotInitializedException("Full-text table was not initialized yet. Run RelationalDatabase.RecreateFullTextTable() to fix this.", exception);
                }
            }
            return;
        }

        /// <summary>
        /// Selects fields of specific rows from a database table.
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
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <param name="selectedRowsAction">delagate to be called
        /// with data reader of results</param>
        /// </summary>
        public override void SelectRowsFromTable(string tableName, IEnumerable<string> columnNames, SelectionMode selectionMode, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ICollection<RelationalJoin> relationalJoins, ulong startPosition, ulong maxResults, SelectedRowsAction selectedRowsAction) {
            var joinType = this.GetJoinTypeFor(filterCriteria);
            var commandText = new PostgreSqlSelectBuilder(tableName, columnNames, selectionMode, fullTextQuery, filterCriteria, sortCriteria, relationalJoins, joinType, startPosition, maxResults, this.fieldNameConverter, this.dataTypeMapper).ToString(out var parameters);
            try {
                this.ExecuteCommandReader(commandText, parameters, selectedRowsAction);
            } catch (DbException exception) {
                this.HandleDbException(exception, tableName);
                throw;
            }
            return;
        }

        /// <summary>
        /// Updates a specific row in a database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="filterCriteria">filter criteria to update
        /// rows for</param>
        /// <param name="relationalJoins">relational joins to apply</param>
        /// <param name="relationalSubqueries">relational subqueries
        /// to apply</param>
        /// <param name="fields">fields to update</param>
        /// <returns>number of rows affected</returns>
        public override int UpdateTableRow(string tableName, FilterCriteria filterCriteria, ICollection<RelationalJoin> relationalJoins, RelationalSubqueryCollection relationalSubqueries, IEnumerable<PersistentFieldForElement> fields) {
            int affectedRows;
            var parameters = new List<DbParameter>();
            // build command text
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("UPDATE ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" AS ");
            commandTextBuilder.Append(RelationalJoin.RootTableAlias);
            // set columns
            commandTextBuilder.Append(" SET ");
            var fieldCount = 0;
            foreach (var field in fields) {
                if (fieldCount > 0) {
                    commandTextBuilder.Append(", ");
                }
                commandTextBuilder.Append(this.fieldNameConverter.Escape(field.Key));
                commandTextBuilder.Append("=@v");
                commandTextBuilder.Append(fieldCount);
                var parameter = this.dataTypeMapper.CreateDbParameter("@v" + fieldCount, field.ValueAsObject, field.ContentBaseType);
                parameters.Add(parameter);
                fieldCount++;
            }
            if (fieldCount > 0) {
                var joinBuilder = new DbJoinBuilder(relationalJoins, this.fieldNameConverter);
                var hasRelationalJoins = relationalJoins.Count > 0;
                if (hasRelationalJoins) {
                    // sub query
                    commandTextBuilder.Append(" WHERE ");
                    commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                    commandTextBuilder.Append('.');
                    commandTextBuilder.Append(nameof(PersistentObject.Id));
                    commandTextBuilder.Append(" IN (SELECT ");
                    commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                    commandTextBuilder.Append('.');
                    commandTextBuilder.Append(nameof(PersistentObject.Id));
                    commandTextBuilder.Append(" FROM ");
                    commandTextBuilder.Append(tableName);
                    commandTextBuilder.Append(" AS ");
                    commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                    commandTextBuilder.Append(' ');
                    // add join clause
                    var joinType = this.GetJoinTypeFor(filterCriteria);
                    commandTextBuilder.Append(joinBuilder.ToJoin(joinType));
                } else {
                    commandTextBuilder.Append(' ');
                }
                // apply filter
                if (!filterCriteria.IsEmpty) {
                    commandTextBuilder.Append("WHERE ");
                    var dbFilter = new DbFilterBuilder(filterCriteria, this.fieldNameConverter, this.dataTypeMapper, joinBuilder, relationalSubqueries).ToFilter();
                    commandTextBuilder.Append(dbFilter.Filter);
                    parameters.AddRange(dbFilter.Parameters);
                }
                if (hasRelationalJoins) {
                    commandTextBuilder.Append(')');
                }
                // execute query
                affectedRows = this.ExecuteCommandNonQuery(commandTextBuilder.ToString(), parameters);
            } else {
                affectedRows = 0;
            }
            return affectedRows;
        }

    }

}