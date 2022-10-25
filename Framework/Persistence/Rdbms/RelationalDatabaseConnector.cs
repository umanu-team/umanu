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

    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Text;
    using System.Transactions;

    /// <summary>
    /// Connector to relational database.
    /// </summary>
    /// <typeparam name="TConnection">type of connections</typeparam>
    public abstract class RelationalDatabaseConnector<TConnection>
        where TConnection : DbConnection, new() {

        /// <summary>
        /// Command timeout in seconds or null to use default command
        /// timeout.
        /// </summary>
        public int? CommandTimeoutInSeconds { get; set; }

        /// <summary>
        /// Connection settings for accessing the relational
        /// database.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Database command logger to use.
        /// </summary>
        public DbCommandLogger DbCommandLogger { get; private set; }

        /// <summary>
        /// Mapper for mapping .NET data types to data types of the
        /// RDBMS.
        /// </summary>
        protected DataTypeMapper dataTypeMapper;

        /// <summary>
        /// Converter for field names to be used in relational queries.
        /// </summary>
        protected IRelationalFieldNameConverter fieldNameConverter;

        /// <summary>
        /// Delegate to be called for selected rows.
        /// </summary>
        /// <param name="dataReader">data reader pointing to the
        /// first selected row</param>
        public delegate void SelectedRowsAction(DbDataReader dataReader);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="connectionString">connection settings for
        /// accessing the relational database</param>
        /// <param name="fieldNameConverter">converter for field
        /// names to be used in relational queries</param>
        /// <param name="dataTypeMapper">mapper for mapping .NET data
        /// types to RDBMS data types</param>
        /// <param name="dbCommandLogger">database command logger to
        /// use</param>
        protected RelationalDatabaseConnector(string connectionString, IRelationalFieldNameConverter fieldNameConverter, DataTypeMapper dataTypeMapper, DbCommandLogger dbCommandLogger) {
            this.ConnectionString = connectionString;
            this.dataTypeMapper = dataTypeMapper;
            this.DbCommandLogger = dbCommandLogger;
            this.fieldNameConverter = fieldNameConverter;
        }

        /// <summary>
        /// Adds a field to a database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="fieldType">simple element field type to add
        /// to database table</param>
        public virtual void AlterTableAddField(string tableName, PersistentFieldType fieldType) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("ALTER TABLE ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" ADD ");
            commandTextBuilder.Append(this.fieldNameConverter.Escape(fieldType.Key));
            commandTextBuilder.Append(' ');
            commandTextBuilder.Append(this.dataTypeMapper[fieldType.ContentBaseType]);
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Adds a foreign key to a database table.
        /// </summary>
        /// <param name="foreignKey">foreign key to add</param>
        public virtual void AlterTableAddForeignKey(RelationalForeignKey foreignKey) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("ALTER TABLE ");
            commandTextBuilder.Append(foreignKey.ForeignKeyTable);
            commandTextBuilder.Append(" ADD FOREIGN KEY (");
            commandTextBuilder.Append(this.fieldNameConverter.Escape(foreignKey.ForeignKeyColumn));
            commandTextBuilder.Append(") REFERENCES ");
            commandTextBuilder.Append(foreignKey.PrimaryKeyTable);
            commandTextBuilder.Append('(');
            commandTextBuilder.Append(this.fieldNameConverter.Escape(foreignKey.PrimaryKeyColumn));
            commandTextBuilder.Append(')');
            if (RelationalAlterAction.NoAction != foreignKey.OnDelete) {
                commandTextBuilder.Append(" ON DELETE ");
                if (RelationalAlterAction.Cascade == foreignKey.OnDelete) {
                    commandTextBuilder.Append("CASCADE");
                } else if (RelationalAlterAction.SetNull == foreignKey.OnDelete) {
                    commandTextBuilder.Append("SET NULL");
                } else if (RelationalAlterAction.SetDefault == foreignKey.OnDelete) {
                    commandTextBuilder.Append("SET DEFAULT");
                } else {
                    throw new PersistenceException("Relational alter action of type " + foreignKey.OnDelete.ToString() + " is not supported.");
                }
            }
            if (RelationalAlterAction.NoAction != foreignKey.OnUpdate) {
                commandTextBuilder.Append(" ON UPDATE ");
                if (RelationalAlterAction.Cascade == foreignKey.OnUpdate) {
                    commandTextBuilder.Append("CASCADE");
                } else if (RelationalAlterAction.SetNull == foreignKey.OnUpdate) {
                    commandTextBuilder.Append("SET NULL");
                } else if (RelationalAlterAction.SetDefault == foreignKey.OnUpdate) {
                    commandTextBuilder.Append("SET DEFAULT");
                } else {
                    throw new PersistenceException("Relational alter action of type " + foreignKey.OnUpdate.ToString() + " is not supported.");
                }
            }
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            if (foreignKey.IsIndexed) {
                this.CreateIndex(foreignKey.ForeignKeyTable, foreignKey.ForeignKeyColumn);
            }
            return;
        }

        /// <summary>
        /// Removes a field and its constraints from a database
        /// table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="fieldName">name of field to remove from
        /// database table</param>
        public virtual void AlterTableDropField(string tableName, string fieldName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("ALTER TABLE ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" DROP COLUMN ");
            commandTextBuilder.Append(this.fieldNameConverter.Escape(fieldName));
            commandTextBuilder.Append(" CASCADE"); // to delete constraints cascadedly
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
        public abstract void AlterTableDropForeignKeys(string tableName, string fieldName);

        /// <summary>
        /// Renames a database table.
        /// </summary>
        /// <param name="oldTableName">old name of database table</param>
        /// <param name="newTableName">new name of database table</param>
        public virtual void AlterTableRename(string oldTableName, string newTableName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("ALTER TABLE ");
            commandTextBuilder.Append(oldTableName);
            commandTextBuilder.Append(" RENAME TO ");
            commandTextBuilder.Append(newTableName);
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Renames a field of a database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="oldFieldName">old name of field to rename</param>
        /// <param name="newFieldName">new name of field to rename</param>
        public virtual void AlterTableRenameField(string tableName, string oldFieldName, string newFieldName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("ALTER TABLE ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" RENAME COLUMN ");
            commandTextBuilder.Append(this.fieldNameConverter.Escape(oldFieldName));
            commandTextBuilder.Append(" TO ");
            commandTextBuilder.Append(this.fieldNameConverter.Escape(newFieldName));
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Reanmes a key of a database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="oldKeyName">old name of key to rename</param>
        /// <param name="newKeyName">new name of key to rename</param>
        public virtual void AlterTableRenameKey(string tableName, string oldKeyName, string newKeyName) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("ALTER TABLE ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" RENAME CONSTRAINT ");
            commandTextBuilder.Append(oldKeyName);
            commandTextBuilder.Append(" TO ");
            commandTextBuilder.Append(newKeyName);
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Updates a field type in database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="fieldType">type of field to update type for
        /// in database table</param>
        public virtual void AlterTableUpdateFieldType(string tableName, PersistentFieldType fieldType) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("ALTER TABLE ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" ALTER COLUMN ");
            commandTextBuilder.Append(this.fieldNameConverter.Escape(fieldType.Key));
            commandTextBuilder.Append(' ');
            commandTextBuilder.Append(this.dataTypeMapper[fieldType.ContentBaseType]);
            this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            return;
        }

        /// <summary>
        /// Indicates whether two data types are equal.
        /// </summary>
        /// <param name="dataType1">first data type to compare</param>
        /// <param name="dataType2">second data type to compare</param>
        /// <returns>true if the two data types are equal, false
        /// otherwise</returns>
        public bool AreDataTypesEqual(string dataType1, string dataType2) {
            return this.dataTypeMapper.AreDataTypesEqual(dataType1, dataType2);
        }

        /// <summary>
        /// Copies data from relations table into a column of a
        /// target table.
        /// </summary>
        /// <param name="sourceColumnName">name of source column to
        /// copy data for</param>
        /// <param name="targetTableName">name of database table to
        /// copy data into</param>
        /// <param name="targetColumnName">name of target column to
        /// copy data for</param>
        /// <returns>number of rows affected</returns>
        internal int CopyDataFromRelationsTable(string sourceColumnName, string targetTableName, string targetColumnName) {
            int affectedRows;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                var commandTextBuilder = new StringBuilder();
                commandTextBuilder.Append("UPDATE ");
                commandTextBuilder.Append(targetTableName);
                commandTextBuilder.Append(" SET ");
                commandTextBuilder.Append(this.fieldNameConverter.Escape(targetColumnName));
                commandTextBuilder.Append("=r0.ChildID, ");
                commandTextBuilder.Append(this.fieldNameConverter.Escape(targetColumnName + '_'));
                commandTextBuilder.Append("=r0.ChildType FROM ");
                commandTextBuilder.Append(RelationalDatabase.CollectionRelationsTableName);
                commandTextBuilder.Append(" AS r0 WHERE ");
                commandTextBuilder.Append(targetTableName);
                commandTextBuilder.Append(".");
                commandTextBuilder.Append(nameof(PersistentObject.Id));
                commandTextBuilder.Append("=r0.ParentID AND r0.ParentField='");
                commandTextBuilder.Append(sourceColumnName);
                commandTextBuilder.Append("'");
                affectedRows = this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
                transactionScope.Complete();
            }
            return affectedRows;
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
        internal abstract void CopyDataToRelationsTable(string sourceTableName, string sourceColumnName, string targetColumnName);

        /// <summary>
        /// Counts the number of rows in a database table.
        /// </summary>
        /// <param name="tableName">name of database table to count
        /// rows for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// rows for</param>
        /// <param name="relationalJoins">relational joins to apply</param>
        /// <returns>number of rows in database table</returns>
        public int CountTableRows(string tableName, FilterCriteria filterCriteria, ICollection<RelationalJoin> relationalJoins) {
            int count = -1;
            foreach (var keyValuePair in this.CountTableRows(tableName, filterCriteria, relationalJoins, new string[0])) {
                count = keyValuePair.Value;
                break;
            }
            return count;
        }

        /// <summary>
        /// Counts the number of rows in groups in a database table.
        /// </summary>
        /// <param name="tableName">name of database table to count
        /// rows for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// rows for</param>
        /// <param name="relationalJoins">relational joins to apply</param>
        /// <param name="groupBy">column names to group table rows by</param>
        /// <returns>pairs of group and number of rows of group</returns>
        public IDictionary<string[], int> CountTableRows(string tableName, FilterCriteria filterCriteria, ICollection<RelationalJoin> relationalJoins, string[] groupBy) {
            var joinBuilder = new DbJoinBuilder(relationalJoins, this.fieldNameConverter);
            IList<DbParameter> parameters = new List<DbParameter>();
            // build command text
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("SELECT ");
            // distinct results
            if (relationalJoins.Count > 0) {
                commandTextBuilder.Append("COUNT(DISTINCT ");
                commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                commandTextBuilder.Append('.');
                commandTextBuilder.Append(nameof(PersistentObject.Id));
                commandTextBuilder.Append(')');
            } else {
                commandTextBuilder.Append("COUNT(*)");
            }
            // select columns to group by
            bool isGrouped = false;
            foreach (var groupByColumn in groupBy) {
                commandTextBuilder.Append(", ");
                commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                commandTextBuilder.Append('.');
                commandTextBuilder.Append(groupByColumn);
                isGrouped = true;
            }
            commandTextBuilder.Append(" FROM ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" AS ");
            commandTextBuilder.Append(RelationalJoin.RootTableAlias);
            commandTextBuilder.Append(' ');
            // add join clause
            var joinType = this.GetJoinTypeFor(filterCriteria);
            commandTextBuilder.Append(joinBuilder.ToJoin(joinType));
            // apply filter
            if (!filterCriteria.IsEmpty) {
                var dbFilter = new DbFilterBuilder(filterCriteria, this.fieldNameConverter, this.dataTypeMapper, joinBuilder, new RelationalSubqueryCollection()).ToFilter();
                commandTextBuilder.Append("WHERE ");
                commandTextBuilder.Append(dbFilter.Filter);
                parameters = dbFilter.Parameters;
            }
            // apply grouping
            if (isGrouped) {
                commandTextBuilder.Append(" GROUP BY ");
                bool isFirstGroupByColumn = true;
                foreach (var groupByColumn in groupBy) {
                    if (isFirstGroupByColumn) {
                        isFirstGroupByColumn = false;
                    } else {
                        commandTextBuilder.Append(", ");
                    }
                    commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                    commandTextBuilder.Append('.');
                    commandTextBuilder.Append(groupByColumn);
                }
            }
            // execute query
            var results = new Dictionary<string[], int>();
            try {
                string commandText = commandTextBuilder.ToString();
                this.ExecuteCommandReader(commandText, parameters, delegate (DbDataReader dataReader) {
                    while (dataReader.Read()) {
                        var value = dataReader.GetInt32(0);
                        var key = new string[groupBy.Length];
                        for (int i = 0; i < groupBy.Length; i++) {
                            key[i] = dataReader.GetValue(i + 1)?.ToString(); // ...instead of dataReader.GetString(i + 1) to allow grouping by numbers
                        }
                        results.Add(key, value);
                    }
                });
            } catch (DbException exception) {
                this.HandleDbException(exception, tableName);
                throw;
            }
            return results;
        }

        /// <summary>
        /// Creates a full text index for a table.
        /// </summary>
        /// <param name="catalogueName">name of full text catalogue</param>
        /// <param name="tableName">name of database table</param>
        /// <param name="indexName">name of index</param>
        /// <param name="textColumn">name of text column</param>
        /// <param name="languageCode">code of language of contents</param>
        /// <returns>number of rows affected</returns>
        public abstract int CreateFullTextIndex(string catalogueName, string tableName, string indexName, string textColumn, ushort languageCode);

        /// <summary>
        /// Creates an index for columns of a database table/view.
        /// </summary>
        /// <param name="tableName">name of database table/view</param>
        /// <param name="columnNames">names of database columns</param>
        /// <returns>number of rows affected</returns>
        public virtual int CreateIndex(string tableName, params string[] columnNames) {
            if (columnNames.LongLength < 1) {
                throw new ArgumentException("You have to specify at least one column name.", nameof(columnNames));
            }
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("CREATE INDEX I_");
            commandTextBuilder.Append(tableName);
            foreach (string columnName in columnNames) {
                commandTextBuilder.Append('_');
                commandTextBuilder.Append(columnName);
            }
            commandTextBuilder.Append(" ON ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append('(');
            bool firstColumnName = true;
            foreach (string columnName in columnNames) {
                if (firstColumnName) {
                    firstColumnName = false;
                } else {
                    commandTextBuilder.Append(',');
                }
                commandTextBuilder.Append(this.fieldNameConverter.Escape(columnName));
            }
            commandTextBuilder.Append(')');
            return this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
        }

        /// <summary>
        /// Creates a database table with specific fields.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="fieldTypes">array of simple element field
        /// types of table</param>
        /// <param name="primaryKey">names of columns containing
        /// primary key</param>
        /// <returns>number of rows affected</returns>
        public virtual int CreateTable(string tableName, IEnumerable<PersistentFieldType> fieldTypes, params string[] primaryKey) {
            // build RDMBS command
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("CREATE TABLE ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" (");
            foreach (var fieldType in fieldTypes) {
                commandTextBuilder.Append(this.fieldNameConverter.Escape(fieldType.Key));
                commandTextBuilder.Append(' ');
                commandTextBuilder.Append(this.dataTypeMapper[fieldType.ContentBaseType]);
                if (null != primaryKey) {
                    foreach (string primaryKeyField in primaryKey) {
                        if (fieldType.Key == primaryKeyField) {
                            commandTextBuilder.Append(" NOT NULL");
                            break;
                        }
                    }
                }
                commandTextBuilder.Append(", ");
            }
            if (null != primaryKey && primaryKey.LongLength > 0) {
                commandTextBuilder.Append("CONSTRAINT PK_");
                commandTextBuilder.Append(tableName);
                commandTextBuilder.Append(" PRIMARY KEY (");
                for (long i = 0; i < primaryKey.LongLength; i++) {
                    if (i > 0) {
                        commandTextBuilder.Append(',');
                    }
                    commandTextBuilder.Append(this.fieldNameConverter.Escape(primaryKey[i]));
                }
                commandTextBuilder.Append(')');
            }
            commandTextBuilder.Append(')');
            // execute RDBMS command
            int rowsAffected;
            try {
                rowsAffected = this.ExecuteCommandNonQuery(commandTextBuilder.ToString());
            } catch (DbException exception) {
                this.HandleDbException(exception, tableName);
                throw;
            }
            return rowsAffected;
        }

        /// <summary>
        /// Creates a database view.
        /// </summary>
        /// <param name="name">name of view</param>
        /// <param name="unions">relational unions to be part of
        /// relational view</param>
        /// <returns>number of rows affected</returns>
        public abstract int CreateView(string name, IEnumerable<RelationalViewUnion> unions);

        /// <summary>
        /// Creates a database view.
        /// </summary>
        /// <param name="name">name of view</param>
        /// <param name="unions">relational unions to be part of
        /// relational view</param>
        /// <param name="isCastEnforcedForStaticColumns">true to
        /// enforce cast of static columns in view, false otherwise</param>
        /// <returns>number of rows affected</returns>
        protected int CreateView(string name, IEnumerable<RelationalViewUnion> unions, bool isCastEnforcedForStaticColumns) {
            var commandTextBuilder = new StringBuilder();
            IEnumerable<DbParameter> parameters = new DbParameter[0];
            commandTextBuilder.Append("CREATE VIEW ");
            commandTextBuilder.Append(name);
            commandTextBuilder.Append(" AS");
            bool isFirstTable = true;
            foreach (var union in unions) {
                if (isFirstTable) {
                    isFirstTable = false;
                } else {
                    commandTextBuilder.Append(" UNION ALL");
                }
                commandTextBuilder.Append(" SELECT ");
                bool isFirstColumn = true;
                foreach (var dynamicColumn in union.DynamicColumns) {
                    if (isFirstColumn) {
                        isFirstColumn = false;
                    } else {
                        commandTextBuilder.Append(", ");
                    }
                    commandTextBuilder.Append(this.fieldNameConverter.Escape(dynamicColumn.Value));
                    if (dynamicColumn.Key != dynamicColumn.Value) {
                        commandTextBuilder.Append(" AS ");
                        commandTextBuilder.Append(dynamicColumn.Key);
                    }
                }
                foreach (var staticColumn in union.StaticColumns) {
                    if (isFirstColumn) {
                        isFirstColumn = false;
                    } else {
                        commandTextBuilder.Append(", ");
                    }
                    if (isCastEnforcedForStaticColumns || null == staticColumn.Item2 || (TypeOf.String != staticColumn.Item3 && TypeOf.IndexableString != staticColumn.Item3)) {
                        commandTextBuilder.Append("CAST(");
                        if (null == staticColumn.Item2) {
                            commandTextBuilder.Append("NULL");
                        } else {
                            commandTextBuilder.Append("'");
                            commandTextBuilder.Append(staticColumn.Item2);
                            commandTextBuilder.Append("'");
                        }
                        commandTextBuilder.Append(" AS ");
                        commandTextBuilder.Append(this.dataTypeMapper.GetMappingForType(staticColumn.Item3));
                        commandTextBuilder.Append(')');
                    } else {
                        commandTextBuilder.Append("'");
                        commandTextBuilder.Append(staticColumn.Item2);
                        commandTextBuilder.Append("'");
                    }
                    commandTextBuilder.Append(" AS ");
                    commandTextBuilder.Append(staticColumn.Item1);
                }
                commandTextBuilder.Append(" FROM ");
                commandTextBuilder.Append(union.TableName);
                if (!union.FilterCriteria.IsEmpty) {
                    commandTextBuilder.Append(" AS ");
                    commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                    commandTextBuilder.Append(" WHERE ");
                    var dbFilter = new DbFilterBuilder(union.FilterCriteria, this.fieldNameConverter, this.dataTypeMapper, new DbJoinBuilder(new RelationalJoin[0], this.fieldNameConverter), new RelationalSubqueryCollection()).ToFilter();
                    commandTextBuilder.Append(dbFilter.Filter);
                    parameters = dbFilter.Parameters;
                }
            }
            return this.ExecuteCommandNonQuery(DbFilterBuilder.ApplyParameters(commandTextBuilder.ToString(), parameters));
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
        public virtual int DeleteRowsFromTable(string tableName, FilterCriteria filterCriteria, ICollection<RelationalJoin> relationalJoins, RelationalSubqueryCollection relationalSubqueries) {
            var joinBuilder = new DbJoinBuilder(relationalJoins, this.fieldNameConverter);
            IEnumerable<DbParameter> parameters = new DbParameter[0];
            // build command text
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("DELETE ");
            commandTextBuilder.Append(RelationalJoin.RootTableAlias);
            commandTextBuilder.Append(" FROM ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" AS ");
            commandTextBuilder.Append(RelationalJoin.RootTableAlias);
            commandTextBuilder.Append(' ');
            // add join clause
            var joinType = this.GetJoinTypeFor(filterCriteria);
            commandTextBuilder.Append(joinBuilder.ToJoin(joinType));
            // filtering
            if (!filterCriteria.IsEmpty) {
                commandTextBuilder.Append("WHERE ");
                var dbFilter = new DbFilterBuilder(filterCriteria, this.fieldNameConverter, this.dataTypeMapper, joinBuilder, relationalSubqueries).ToFilter();
                commandTextBuilder.Append(dbFilter.Filter);
                parameters = dbFilter.Parameters;
            }
            // execute command
            return this.ExecuteCommandNonQuery(commandTextBuilder.ToString(), parameters);
        }

        /// <summary>
        /// Drops the full text index of a table.
        /// </summary>
        /// <param name="catalogueName">name of full text catalogue</param>
        /// <param name="tableName">name of database table</param>
        /// <returns>number of rows affected</returns>
        public abstract int DropFullTextIndex(string catalogueName, string tableName);

        /// <summary>
        /// Drops a database table.
        /// </summary>
        /// <param name="tableName">name of database table to drop</param>
        /// <returns>number of rows affected</returns>
        public virtual int DropTable(string tableName) {
            string commandText = "DROP TABLE " + tableName;
            return this.ExecuteCommandNonQuery(commandText);
        }

        /// <summary>
        /// Drops a database view, if it exists.
        /// </summary>
        /// <param name="viewName">name of database view to drop</param>
        /// <returns>number of rows affected</returns>
        public virtual int DropView(string viewName) {
            string commandText = "DROP VIEW " + viewName;
            int rowsAffected = 0;
            try {
                rowsAffected = this.ExecuteCommandNonQuery(commandText);
            } catch (DbException) {
                // ignore SQL exceptions, e.g. non-existing views
            }
            return rowsAffected;
        }

        /// <summary>
        /// Executes a RDBMS statement against the connection and
        /// returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">RDBMS statement to execute</param>
        /// <returns>number of rows affected</returns>
        protected int ExecuteCommandNonQuery(string commandText) {
            return this.ExecuteCommandNonQuery(commandText, null);
        }

        /// <summary>
        /// Executes a RDBMS statement against the connection and
        /// returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">RDBMS statement to execute</param>
        /// <param name="parameters">enumerable of RDBMS parameters
        /// to apply</param>
        /// <returns>number of rows affected</returns>
        protected virtual int ExecuteCommandNonQuery(string commandText, IEnumerable<DbParameter> parameters) {
            int rowsAffected;
            using (var connection = new TConnection()) {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand()) {
                    this.InitializeCommand(command, commandText, parameters);
                    rowsAffected = command.ExecuteNonQuery();
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
        protected virtual void ExecuteCommandReader(string commandText, IEnumerable<DbParameter> parameters, SelectedRowsAction selectedRowsAction) {
            using (var connection = new TConnection()) {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand()) {
                    this.InitializeCommand(command, commandText, parameters);
                    using (var dataReader = command.ExecuteReader()) {
                        selectedRowsAction(dataReader);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Gets the database data type as string for a .NET data
        /// type.
        /// </summary>
        /// <param name="type">.NET data type</param>
        /// <returns>database data type as string</returns>
        public string GetDataTypeForType(Type type) {
            return this.dataTypeMapper[type];
        }

        /// <summary>
        /// Determines the type of join to use for specific filter
        /// criteria.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to determine
        /// type of join to use for</param>
        /// <returns>type of join to use for filter criteria</returns>
        protected virtual RelationalJoinType GetJoinTypeFor(FilterCriteria filterCriteria) {
            RelationalJoinType joinType;
            if (filterCriteria.HasNullValues) {
                joinType = RelationalJoinType.LeftOuterJoin;
            } else {
                joinType = RelationalJoinType.InnerJoin;
            }
            return joinType;
        }

        /// <summary>
        /// Gets all field and their actual types of a table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <returns>all field and their actual types of table</returns>
        public IDictionary<string, string> GetTableSchema(string tableName) {
            var schema = new Dictionary<string, string>();
            try {
                string commandText = this.GetTableSchemaCommandText(tableName);
                this.ExecuteCommandReader(commandText, new DbParameter[0], delegate (DbDataReader dataReader) {
                    for (int i = 0; i < dataReader.FieldCount; i++) {
                        string actualDataTypeName = dataReader.GetDataTypeName(i);
                        string columnName = dataReader.GetName(i);
                        schema.Add(columnName, actualDataTypeName);
                    }
                });
            } catch (DbException exception) {
                this.HandleDbException(exception, tableName);
                throw;
            }
            return schema;
        }

        /// <summary>
        /// Gets the SQL command text for detection of table schema.
        /// </summary>
        /// <param name="tableName">name of database table to detect
        /// schema for</param>
        /// <returns>SQL command text for detection of table schema</returns>
        protected abstract string GetTableSchemaCommandText(string tableName);

        /// <summary>
        /// Gets the upper-case names of all GUID fields of a table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <returns>upper-case names of all GUID fields of a table</returns>
        internal IList<string> GetUpperCaseGuidFieldNamesOf(string tableName) {
            var upperCaseGuidFieldNames = new List<string>();
            try {
                string commandText = this.GetTableSchemaCommandText(tableName);
                this.ExecuteCommandReader(commandText, new DbParameter[0], delegate (DbDataReader dataReader) {
                    for (int i = 0; i < dataReader.FieldCount; i++) {
                        if (this.dataTypeMapper.DataTypeForGuid.ToUpperInvariant() == dataReader.GetDataTypeName(i).ToUpperInvariant()) {
                            upperCaseGuidFieldNames.Add(dataReader.GetName(i).ToUpperInvariant());
                        }
                    }
                });
            } catch (DbException exception) {
                this.HandleDbException(exception, tableName);
                throw;
            }
            return upperCaseGuidFieldNames;
        }

        /// <summary>
        /// Handles a database exception.
        /// </summary>
        /// <param name="exception">database exception to handle</param>
        /// <param name="tableName">name of database table affected
        /// by exception</param>
        protected abstract void HandleDbException(DbException exception, string tableName);

        /// <summary>
        /// Initializes an database command.
        /// </summary>
        /// <param name="command">command to initialize</param>
        /// <param name="commandText">command text to set</param>
        /// <param name="parameters">parameters to apply</param>
        protected void InitializeCommand(DbCommand command, string commandText, IEnumerable<DbParameter> parameters) {
            this.DbCommandLogger.LogCommand(commandText, parameters);
            command.CommandText = commandText;
            if (null != parameters) {
                foreach (var parameter in parameters) {
                    command.Parameters.Add(parameter);
                }
            }
            if (this.CommandTimeoutInSeconds.HasValue) {
                command.CommandTimeout = this.CommandTimeoutInSeconds.Value;
            }
            return;
        }

        /// <summary>
        /// Inserts a new row into a database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="fields">values to insert into new row</param>
        /// <returns>number of rows affected</returns>
        public int InsertRowIntoTable(string tableName, IEnumerable<PersistentFieldForElement> fields) {
            return this.InsertRowIntoTableIfNotDeleted(tableName, null, fields);
        }

        /// <summary>
        /// Inserts a new row into a database table if a row with a
        /// specific ID was not deleted before.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="id">ID to check previous deletion of as
        /// condition for insertion</param>
        /// <param name="fields">values to insert into new row</param>
        /// <returns>number of rows affected</returns>
        public int InsertRowIntoTableIfNotDeleted(string tableName, Guid id, IEnumerable<PersistentFieldForElement> fields) {
            return this.InsertRowIntoTableIfNotDeleted(tableName, new Guid?(id), fields);
        }

        /// <summary>
        /// Inserts a new row into a database table if a row with a
        /// specific ID was not deleted before.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="id">ID to check previous deletion of as
        /// condition for insertion or null if no condition is
        /// required</param>
        /// <param name="fields">values to insert into new row</param>
        /// <returns>number of rows affected</returns>
        public int InsertRowIntoTableIfNotDeleted(string tableName, Guid? id, IEnumerable<PersistentFieldForElement> fields) {
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("INSERT INTO ");
            commandTextBuilder.Append(tableName);
            commandTextBuilder.Append(" (");
            bool firstField = true;
            foreach (var field in fields) {
                if (firstField) {
                    firstField = false;
                } else {
                    commandTextBuilder.Append(", ");
                }
                commandTextBuilder.Append(this.fieldNameConverter.Escape(field.Key));
            }
            commandTextBuilder.Append(") ");
            if (id.HasValue) {
                commandTextBuilder.Append("SELECT ");
            } else {
                commandTextBuilder.Append("VALUES (");
            }
            var parameters = new List<DbParameter>();
            int fieldCount = 0;
            foreach (var field in fields) {
                if (fieldCount > 0) {
                    commandTextBuilder.Append(", ");
                }
                commandTextBuilder.Append("@v");
                commandTextBuilder.Append(fieldCount);
                parameters.Add(this.dataTypeMapper.CreateDbParameter("@v" + fieldCount, field.ValueAsObject, field.ContentBaseType));
                fieldCount++;
            }
            if (id.HasValue) {
                commandTextBuilder.Append(" WHERE NOT EXISTS (SELECT ");
                commandTextBuilder.Append(nameof(PersistentObject.Id));
                commandTextBuilder.Append(" FROM ");
                commandTextBuilder.Append(RelationalDatabase.DeletedIDsTableName);
                commandTextBuilder.Append(" WHERE ID=@v");
                commandTextBuilder.Append(fieldCount);
                parameters.Add(this.dataTypeMapper.CreateDbParameter("@v" + fieldCount, id.Value, TypeOf.Guid));
            }
            commandTextBuilder.Append(")");
            return this.ExecuteCommandNonQuery(commandTextBuilder.ToString(), parameters);
        }

        /// <summary>
        /// Selects a field of specific rows from a database table.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="columnName">name of database column to
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
        public void SelectRowsFromTable(string tableName, string columnName, SelectionMode selectionMode, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ICollection<RelationalJoin> relationalJoins, ulong startPosition, ulong maxResults, SelectedRowsAction selectedRowsAction) {
            this.SelectRowsFromTable(tableName, new string[] { columnName }, selectionMode, fullTextQuery, filterCriteria, sortCriteria, relationalJoins, startPosition, maxResults, selectedRowsAction);
            return;
        }

        /// <summary>
        /// Selects fields of specific rows from a database table.
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
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <param name="selectedRowsAction">delagate to be called
        /// with data reader of results</param>
        public abstract void SelectRowsFromTable(string tableName, IEnumerable<string> columnNames, SelectionMode selectionMode, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ICollection<RelationalJoin> relationalJoins, ulong startPosition, ulong maxResults, SelectedRowsAction selectedRowsAction);

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
        public virtual int UpdateTableRow(string tableName, FilterCriteria filterCriteria, ICollection<RelationalJoin> relationalJoins, RelationalSubqueryCollection relationalSubqueries, IEnumerable<PersistentFieldForElement> fields) {
            int affectedRows;
            var joinBuilder = new DbJoinBuilder(relationalJoins, this.fieldNameConverter);
            var parameters = new List<DbParameter>();
            // build command text
            var commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("UPDATE ");
            commandTextBuilder.Append(RelationalJoin.RootTableAlias);
            // set columns
            commandTextBuilder.Append(" SET ");
            int fieldCount = 0;
            foreach (var field in fields) {
                if (fieldCount > 0) {
                    commandTextBuilder.Append(", ");
                }
                commandTextBuilder.Append(this.fieldNameConverter.Escape(joinBuilder.GetInternalFieldNameOf(field.Key)));
                commandTextBuilder.Append("=@v");
                commandTextBuilder.Append(fieldCount);
                var parameter = this.dataTypeMapper.CreateDbParameter("@v" + fieldCount, field.ValueAsObject, field.ContentBaseType);
                parameters.Add(parameter);
                fieldCount++;
            }
            if (fieldCount > 0) {
                // add table alias
                commandTextBuilder.Append(" FROM ");
                commandTextBuilder.Append(tableName);
                commandTextBuilder.Append(" AS ");
                commandTextBuilder.Append(RelationalJoin.RootTableAlias);
                commandTextBuilder.Append(' ');
                // add join clause
                var joinType = this.GetJoinTypeFor(filterCriteria);
                commandTextBuilder.Append(joinBuilder.ToJoin(joinType));
                // apply filter
                if (!filterCriteria.IsEmpty) {
                    commandTextBuilder.Append("WHERE ");
                    var dbFilter = new DbFilterBuilder(filterCriteria, this.fieldNameConverter, this.dataTypeMapper, joinBuilder, relationalSubqueries).ToFilter();
                    commandTextBuilder.Append(dbFilter.Filter);
                    parameters.AddRange(dbFilter.Parameters);
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