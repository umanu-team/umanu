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

namespace Framework.Persistence.Rdbms.MSSql {

    using Framework.Diagnostics;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using System.Transactions;

    /// <summary>
    /// Connector to MS SQL database.
    /// </summary>
    internal sealed class MSSqlDatabaseConnector : RelationalDatabaseConnector<SqlConnection> {

        /// <summary>
        /// Maximum number of attempts to execute SQL.
        /// </summary>
        private const byte maxAttemptsToExecuteSql = 50;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="connectionString">connection settings for
        /// accessing the SQL database</param>
        public MSSqlDatabaseConnector(string connectionString)
            : base(connectionString, new MSSqlFieldNameConverter(), new MSSqlDataTypeMapper(), new DbCommandLogger()) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="connectionString">connection settings for
        /// accessing the SQL database</param>
        /// <param name="log">log to use</param>
        public MSSqlDatabaseConnector(string connectionString, ILog log)
            : base(connectionString, new MSSqlFieldNameConverter(), new MSSqlDataTypeMapper(), new DbCommandLogger(log)) {
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
        /// Removes a field and its constraints from a database
        /// table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="fieldName">name of field to remove from
        /// database table</param>
        public override void AlterTableDropField(string tableName, string fieldName) {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                this.AlterTableDropForeignKeys(tableName, fieldName);
                var commandTextBuilder = new StringBuilder();
                commandTextBuilder.Append("ALTER TABLE ");
                commandTextBuilder.Append(tableName);
                commandTextBuilder.Append(" DROP COLUMN ");
                commandTextBuilder.Append(this.fieldNameConverter.Escape(fieldName));
                this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
                transactionScope.Complete();
            }
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
                commandTextBuilder.Append("EXEC sp_fkeys @fktable_name=");
                commandTextBuilder.Append(tableName);
                var foreignKeyNames = new List<string>();
                this.ExecuteCommandReader(commandTextBuilder.ToString(), new DbParameter[0], delegate (DbDataReader dataReader) {
                    while (dataReader.Read()) {
                        if (dataReader.GetString(dataReader.GetOrdinal("FKCOLUMN_NAME")).ToUpperInvariant() == fieldName.ToUpperInvariant()) {
                            foreignKeyNames.Add(dataReader.GetString(dataReader.GetOrdinal("FK_NAME")));
                        }
                    }
                    return;
                });
                commandTextBuilder.Clear();
                foreach (var foreignKeyName in foreignKeyNames) {
                    this.AlterTableDropConstraint(tableName, foreignKeyName);
                }
                commandTextBuilder.Append("EXEC sp_helpindex @objname=");
                commandTextBuilder.Append(tableName);
                var indexNames = new List<string>();
                this.ExecuteCommandReader(commandTextBuilder.ToString(), new DbParameter[0], delegate (DbDataReader dataReader) {
                    while (dataReader.Read()) {
                        if (dataReader.GetString(dataReader.GetOrdinal("index_keys")).ToUpperInvariant() == fieldName.ToUpperInvariant()) {
                            indexNames.Add(dataReader.GetString(dataReader.GetOrdinal("index_name")));
                        }
                    }
                    return;
                });
                foreach (var indexName in indexNames) {
                    this.AlterTableDropIndex(tableName, indexName);
                }
                transactionScope.Complete();
            }
            return;
        }

        /// <summary>
        /// Removes an index from a database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="indexName">name of index to remove from
        /// database table</param>
        private void AlterTableDropIndex(string tableName, string indexName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("DROP INDEX ");
            commandTextBuilder.Append(indexName);
            commandTextBuilder.Append(" ON ");
            commandTextBuilder.Append(tableName);
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Renames a database table.
        /// </summary>
        /// <param name="oldTableName">old name of database table</param>
        /// <param name="newTableName">new name of database table</param>
        public override void AlterTableRename(string oldTableName, string newTableName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("EXEC sp_rename '");
            commandTextBuilder.Append(oldTableName);
            commandTextBuilder.Append("', '");
            commandTextBuilder.Append(newTableName);
            commandTextBuilder.Append("'");
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Renames a field of a database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="oldFieldName">old name of field to rename</param>
        /// <param name="newFieldName">new name of field to rename</param>
        public override void AlterTableRenameField(string tableName, string oldFieldName, string newFieldName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("EXEC sp_rename '");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(".");
            commandTextBuilder.Append(oldFieldName);
            commandTextBuilder.Append("', '");
            commandTextBuilder.Append(newFieldName);
            commandTextBuilder.Append("', 'COLUMN'");
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Reanmes a key of a database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="oldKeyName">old name of key to rename</param>
        /// <param name="newKeyName">new name of key to rename</param>
        public override void AlterTableRenameKey(string tableName, string oldKeyName, string newKeyName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("EXEC sp_rename '");
            commandTextBuilder.Append(oldKeyName);
            commandTextBuilder.Append("', '");
            commandTextBuilder.Append(newKeyName);
            commandTextBuilder.Append("'");
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
        /// <returns>number of rows affected</returns>
        internal override void CopyDataToRelationsTable(string sourceTableName, string sourceColumnName, string targetColumnName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("INSERT INTO ");
            commandTextBuilder.Append(RelationalDatabase.CollectionRelationsTableName);
            commandTextBuilder.Append(" (");
            commandTextBuilder.Append(nameof(PersistentObject.Id));
            commandTextBuilder.Append(", ParentTable, ParentID, ParentField, ParentIndex, ChildType, ChildID) SELECT NEWID(), '");
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
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("CREATE FULLTEXT CATALOG ");
            commandTextBuilder.Append(catalogueName);
            commandTextBuilder.Append(';');
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            commandTextBuilder.Clear();
            commandTextBuilder.Append("CREATE FULLTEXT INDEX ON ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append("( ");
            commandTextBuilder.Append(textColumn);
            commandTextBuilder.Append(" Language ");
            commandTextBuilder.Append(languageCode.ToString(CultureInfo.InvariantCulture));
            commandTextBuilder.Append(" ) KEY INDEX ");
            commandTextBuilder.Append(indexName);
            commandTextBuilder.Append(" ON ");
            commandTextBuilder.Append(catalogueName);
            commandTextBuilder.Append(" WITH STOPLIST = OFF;");
            return this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
        }

        /// <summary>
        /// Creates a database view.
        /// </summary>
        /// <param name="name">name of view</param>
        /// <param name="unions">relational unions to be part of
        /// relational view</param>
        /// <returns>number of rows affected</returns>
        public override int CreateView(string name, IEnumerable<RelationalViewUnion> unions) {
            return base.CreateView(name, unions, false);
        }

        /// <summary>
        /// Delays a retry.
        /// </summary>
        /// <param name="maxAttempts">maximum number of attempts</param>
        /// <param name="remainingAttempts">number of remaining attempts</param>
        private static void DelayRetry(byte maxAttempts, byte remainingAttempts) {
            Thread.Sleep(new Random().Next(0, 900) + (maxAttempts - remainingAttempts) * 100);
            return;
        }

        /// <summary>
        /// Drops the full text index of a table.
        /// </summary>
        /// <param name="catalogueName">name of full text catalogue</param>
        /// <param name="tableName">name of database table</param>
        /// <returns>number of rows affected</returns>
        public override int DropFullTextIndex(string catalogueName, string tableName) {
            int result;
            var commandTextBuilder = new StringBuilder();
            try {
                commandTextBuilder.Append("DROP FULLTEXT INDEX ON ");
                commandTextBuilder.Append(tableName);
                commandTextBuilder.Append(';');
                result = this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            } finally {
                commandTextBuilder.Clear();
                commandTextBuilder.Append("DROP FULLTEXT CATALOG ");
                commandTextBuilder.Append(catalogueName);
                commandTextBuilder.Append(';');
                result = this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            }
            return result;
        }

        /// <summary>
        /// Executes a RDBMS statement against the connection and
        /// returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">RDBMS statement to execute</param>
        /// <param name="parameters">enumerable of RDBMS parameters
        /// to apply</param>
        /// <returns>number of rows affected</returns>
        protected override int ExecuteCommandNonQuery(string commandText, IEnumerable<DbParameter> parameters) {
            var rowsAffected = 0;
            var remainingAttempts = MSSqlDatabaseConnector.maxAttemptsToExecuteSql;
            while (remainingAttempts > 0) {
                var isFailedAttempt = false;
                try {
                    rowsAffected = base.ExecuteCommandNonQuery(commandText, parameters);
                    break;
                } catch (SqlException sqlException) {
                    if (1205 == sqlException.Number && remainingAttempts > 1) { // handle deadlock
                        remainingAttempts--;
                        isFailedAttempt = true;
                    } else {
                        throw;
                    }
                }
                if (isFailedAttempt) {
                    MSSqlDatabaseConnector.DelayRetry(MSSqlDatabaseConnector.maxAttemptsToExecuteSql, remainingAttempts);
                    parameters = this.RecreateParameters(parameters);
                }
            }
            return rowsAffected;
        }

        /// <summary>
        /// Executes a RDBMS statement against the connection and
        /// returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">RDBMS statement to execute</param>
        /// <param name="parameters">SQL parameters to fill in to
        /// command text</param>
        /// <param name="selectedRowsAction">delagate to be called
        /// with data reader of results</param>
        /// <returns>number of rows affected</returns>
        protected override void ExecuteCommandReader(string commandText, IEnumerable<DbParameter> parameters, SelectedRowsAction selectedRowsAction) {
            var remainingAttempts = MSSqlDatabaseConnector.maxAttemptsToExecuteSql;
            while (remainingAttempts > 0) {
                var isFailedAttempt = false;
                try {
                    base.ExecuteCommandReader(commandText, parameters, selectedRowsAction);
                    break;
                } catch (SqlException sqlException) {
                    if (1205 == sqlException.Number && remainingAttempts > 1) { // handle deadlock
                        remainingAttempts--;
                        isFailedAttempt = true;
                    } else {
                        throw;
                    }
                }
                if (isFailedAttempt) {
                    MSSqlDatabaseConnector.DelayRetry(MSSqlDatabaseConnector.maxAttemptsToExecuteSql, remainingAttempts);
                    parameters = this.RecreateParameters(parameters);
                }
            }
            return;
        }

        /// <summary>
        /// Executes a RDBMS statement against the connection and
        /// returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">RDBMS statement to execute</param>
        /// <param name="parameters">SQL parameters to fill in to
        /// command text</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="selectedRowsAction">delagate to be called
        /// with data reader of results</param>
        /// <returns>number of rows affected</returns>
        private void ExecuteCommandReader(string commandText, IEnumerable<DbParameter> parameters, ulong startPosition, SelectedRowsAction selectedRowsAction) {
            var remainingAttempts = MSSqlDatabaseConnector.maxAttemptsToExecuteSql;
            while (remainingAttempts > 0) {
                var isFailedAttempt = false;
                try {
                    this.ExecuteCommandReaderUnsafe(commandText, parameters, startPosition, selectedRowsAction);
                    break;
                } catch (SqlException sqlException) {
                    if (1205 == sqlException.Number && remainingAttempts > 1) { // handle deadlock
                        remainingAttempts--;
                        isFailedAttempt = true;
                    } else {
                        throw;
                    }
                }
                if (isFailedAttempt) {
                    MSSqlDatabaseConnector.DelayRetry(MSSqlDatabaseConnector.maxAttemptsToExecuteSql, remainingAttempts);
                    parameters = this.RecreateParameters(parameters);
                }
            }
            return;
        }

        /// <summary>
        /// Executes a RDBMS statement against the connection and
        /// returns the number of rows affected. Deadlocks are not
        /// handled.
        /// </summary>
        /// <param name="commandText">RDBMS statement to execute</param>
        /// <param name="parameters">SQL parameters to fill in to
        /// command text</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="selectedRowsAction">delagate to be called
        /// with data reader of results</param>
        /// <returns>number of rows affected</returns>
        private void ExecuteCommandReaderUnsafe(string commandText, IEnumerable<DbParameter> parameters, ulong startPosition, SelectedRowsAction selectedRowsAction) {
            using (var connection = new SqlConnection()) {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand()) {
                    this.InitializeCommand(command, commandText, parameters);
                    using (var dataReader = command.ExecuteReader()) {
                        ulong count = 0;
                        while (count < startPosition && dataReader.Read()) { // HACK: Bad performance!
                            count++;
                        }
                        if (count == startPosition) {
                            selectedRowsAction(dataReader);
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Gets the SQL command text for detection of table changes.
        /// </summary>
        /// <param name="tableName">name of database table to detect
        /// changes for</param>
        /// <returns>SQL command text for detection of table changes</returns>
        protected override string GetTableSchemaCommandText(string tableName) {
            return "SELECT TOP 1 * FROM " + tableName + ";";
        }

        /// <summary>
        /// Handles a database exception.
        /// </summary>
        /// <param name="exception">database exception to handle</param>
        /// <param name="tableName">name of database table affected
        /// by exception</param>
        protected override void HandleDbException(DbException exception, string tableName) {
            if (exception is SqlException sqlException) {
                if (156 == sqlException.Number) {
                    throw new FieldNameException("Persistent container \"" + tableName + "\" cannot be created beacause one or more field names are reserved keywords in the persistence mechanism.", exception);
                } else if (208 == sqlException.Number && RelationalDatabase.ContainersTableName == tableName) {
                    throw new PersistenceMechanismNotInitializedException("Persistence mechanism was not initialized yet.", exception);
                } else if (208 == sqlException.Number && RelationalDatabase.FullTextTableName == tableName) {
                    throw new FullTextIndexNotInitializedException("Full text table was not initialized yet. Run RelationalDatabase.RecreateFullTextTable() to fix this.", exception);
                }
            }
            return;
        }

        /// <summary>
        /// Recreates an enumerable of SQL parameters. This is
        /// necessary to allow retries of SQL execution, sice SQL
        /// parameters may not be used in more than one SQL
        /// execution.
        /// </summary>
        /// <param name="parameters">enumerable of SQL parameters to
        /// be recreated</param>
        /// <returns>enumerable of recreated SQL parameters</returns>
        private IEnumerable<DbParameter> RecreateParameters(IEnumerable<DbParameter> parameters) {
            var recreatedParameters = new List<DbParameter>(); // yield return is not used for compatibility reasons
            foreach (var parameter in parameters) {
                recreatedParameters.Add(new SqlParameter(parameter.ParameterName, parameter.Value));
            }
            return recreatedParameters;
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
        public override void SelectRowsFromTable(string tableName, IEnumerable<string> columnNames, SelectionMode selectionMode, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ICollection<RelationalJoin> relationalJoins, ulong startPosition, ulong maxResults, RelationalDatabaseConnector<SqlConnection>.SelectedRowsAction selectedRowsAction) {
            var joinType = this.GetJoinTypeFor(filterCriteria);
            var commandText = new MSSqlSelectBuilder(tableName, columnNames, selectionMode, fullTextQuery, filterCriteria, sortCriteria, relationalJoins, joinType, startPosition, maxResults, this.fieldNameConverter, this.dataTypeMapper).ToString(out var parameters);
            try {
                this.ExecuteCommandReader(commandText, parameters, startPosition, selectedRowsAction);
            } catch (DbException exception) {
                if (!string.IsNullOrEmpty(fullTextQuery) && -2146232060 == exception.ErrorCode) {
                    // ignore syntax errors in full text queries
                } else {
                    this.HandleDbException(exception, tableName);
                    throw;
                }
            }
            return;
        }

    }

}