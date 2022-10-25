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

    using Framework.Diagnostics;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Transactions;

    // Partial class for container operations of relational database.
    public partial class RelationalDatabase<TConnection> {

        /// <summary>
        /// Cache for names of internal containers and related
        /// assembly qualified types.
        /// </summary>
        protected ConcurrentDictionary<string, string> containerNameCache;

        /// <summary>
        /// Cache for container name caches with conncetion string as
        /// key.
        /// </summary>
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> containerNameCaches = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        /// <summary>
        /// True if this relational database is supposed to update
        /// views automatically, false otherwise.
        /// </summary>
        protected bool isUpdatingViewsAutomatically = true;

        /// <summary>
        /// Lock for this object.
        /// </summary>
        private readonly object thisLock = new object();

        /// <summary>
        /// Adds a container for storing persistent objects of a
        /// specific type to persistence mechanism.
        /// </summary>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to create container for</param>
        protected override void AddContainer(PersistentObject sampleInstance) {
            Type type = sampleInstance.Type;
            this.db.DbCommandLogger.Log?.WriteEntry("Adding container " + type.FullName + ".", LogLevel.Information);
            if (type.IsAbstract) {
                throw new TypeException("Persistent container for type \""
                    + type.AssemblyQualifiedName
                    + "\" cannot be created because it is an abstract type.");
            } else {
                try {
                    if (this.isUpdatingViewsAutomatically) {
                        this.RemoveViews();
                    }
                    using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                        string tableName = this.GetInternalNameOfContainer(type);
                        if (string.IsNullOrEmpty(tableName)) {
                            tableName = this.GenerateNewInternalTableNameFor(type);
                        }
                        this.AddContainerTableName(tableName, type.AssemblyQualifiedName);
                        var foreignKeys = this.AddContainerSubTables(tableName, sampleInstance.PersistentFieldsForCollectionsOfElements);
                        this.AddContainerParentTable(tableName, sampleInstance.PersistentFieldsForElements, sampleInstance.PersistentFieldsForPersistentObjects);
                        this.AddContainerForeignKeys(tableName, sampleInstance.PersistentFieldsForElements, foreignKeys);
                        transactionScope.Complete();
                    }
                } catch (PersistenceMechanismNotInitializedException) {
                    bool wasUpdatingViewsAutomatically = this.isUpdatingViewsAutomatically;
                    try {
                        this.isUpdatingViewsAutomatically = false;
                        this.InitializePersistenceMechanism();
                        this.AddContainer(sampleInstance);
                    } finally {
                        this.isUpdatingViewsAutomatically = wasUpdatingViewsAutomatically;
                    }
                } finally {
                    if (this.isUpdatingViewsAutomatically) {
                        this.AddViews();
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Adds the foreign keys for a container.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="persistentFieldsForElements">simple element
        /// fields</param>
        /// <param name="foreignKeys">foreign keys to add</param>
        private void AddContainerForeignKeys(string tableName, IEnumerable<PersistentFieldForElement> persistentFieldsForElements, IEnumerable<RelationalForeignKey> foreignKeys) {
            foreach (RelationalForeignKey foreignKey in foreignKeys) {
                this.db.AlterTableAddForeignKey(foreignKey);
            }
            foreach (var persistentFieldForElement in persistentFieldsForElements) {
                if (persistentFieldForElement.IsIndexed && nameof(PersistentObject.Id) != persistentFieldForElement.Key) {
                    bool isIndexedForeignKey = false;
                    foreach (var foreignKey in foreignKeys) {
                        if (foreignKey.ForeignKeyTable == tableName && foreignKey.ForeignKeyColumn == persistentFieldForElement.Key && foreignKey.IsIndexed) {
                            isIndexedForeignKey = true;
                            break;
                        }
                    }
                    if (!isIndexedForeignKey) {
                        this.db.CreateIndex(tableName, persistentFieldForElement.Key);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Add parent table containing simple element fields.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="fieldsForElements">fields for single simple
        /// elements</param>
        /// <param name="fieldsForPersistentObjects">fields for
        /// single persistent objects</param>
        private void AddContainerParentTable(string tableName, IEnumerable<PersistentFieldForElement> fieldsForElements, IEnumerable<PersistentFieldForPersistentObject> fieldsForPersistentObjects) {
            var tableFields = new List<PersistentFieldType>();
            foreach (var fieldForElement in fieldsForElements) {
                var fieldType = fieldForElement.FieldType;
                tableFields.Add(fieldType);
            }
            foreach (var fieldForPersistentObject in fieldsForPersistentObjects) {
                var fieldTypeForValue = new PersistentFieldType(fieldForPersistentObject.Key, true, TypeOf.NullableGuid);
                tableFields.Add(fieldTypeForValue);
                var fieldTypeForType = new PersistentFieldType(fieldForPersistentObject.Key + '_', true, TypeOf.IndexableString);
                tableFields.Add(fieldTypeForType);
            }
            this.db.CreateTable(tableName, tableFields, nameof(PersistentObject.Id));
        }

        /// <summary>
        /// Initializes the name of a sub table for a specific .NET
        /// type.
        /// </summary>
        /// <param name="parentTableName">name of parent table for .NET
        /// type</param>
        /// <param name="subTableName">name of sub table for .NET
        /// type</param>
        private void AddContainerSubTableName(string parentTableName, string subTableName) {
            var fields = new PersistentFieldForElement[3];
            fields[0] = new PersistentFieldForGuid(nameof(PersistentObject.Id), Guid.NewGuid());
            fields[1] = new PersistentFieldForString("ParentTable", parentTableName);
            fields[2] = new PersistentFieldForString("SubTableName", subTableName);
            this.db.InsertRowIntoTable(RelationalDatabase.SubTablesTableName, fields);
            return;
        }

        /// <summary>
        /// Add sub table for field of a container containing simple
        /// collections.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="field">fields to create sub table for</param>
        /// <returns>foreign key to be added after the parent table
        /// was added</returns>
        private RelationalForeignKey AddContainerSubTable(string tableName, PersistentFieldForCollection field) {
            RelationalForeignKey parentKey;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                string subTableName = RelationalDatabase.GetInternalNameOfSubTable(tableName, field.Key);
                var subTableFields = RelationalDatabase<TConnection>.GetContainerSubTableFieldsFor(field.ContentBaseType);
                this.db.CreateTable(subTableName, subTableFields, nameof(PersistentObject.Id));
                this.AddContainerSubTableName(tableName, subTableName);
                parentKey = new RelationalForeignKey(subTableName, "ParentID", tableName, nameof(PersistentObject.Id)) {
                    OnDelete = RelationalAlterAction.Cascade
                };
                transactionScope.Complete();
            }
            return parentKey;
        }

        /// <summary>
        /// Add sub tables for fields of a container containing
        /// simple collections.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="fields">fields to create sub tables for</param>
        /// <returns>foreign keys to be added after the parent table
        /// was added</returns>
        private IEnumerable<RelationalForeignKey> AddContainerSubTables(string tableName, IEnumerable<PersistentFieldForCollection> fields) {
            foreach (var field in fields) {
                yield return this.AddContainerSubTable(tableName, field);
            }
        }

        /// <summary>
        /// Initializes the name of the database table for a specific
        /// .NET type.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="assemblyQualifiedTypeName">assembly
        /// qualified name of .NET type</param>
        private void AddContainerTableName(string tableName, string assemblyQualifiedTypeName) {
            var fields = new PersistentFieldForElement[2];
            fields[0] = new PersistentFieldForString("TableName", tableName);
            fields[1] = new PersistentFieldForString("TypeName", assemblyQualifiedTypeName);
            this.db.InsertRowIntoTable(RelationalDatabase.ContainersTableName, fields);
            return;
        }

        /// <summary>
        /// Adds a table for storing full text data.
        /// </summary>
        private void AddFullTextTable() {
            this.db.DbCommandLogger.Log?.WriteEntry("Trying to add full-text table.", LogLevel.Information);
            var fields = new PersistentFieldType[2];
            fields[0] = new PersistentFieldType(nameof(PersistentObject.Id), true, TypeOf.Guid);
            fields[1] = new PersistentFieldType("T", true, TypeOf.String);
            this.db.CreateTable(RelationalDatabase.FullTextTableName, fields, nameof(PersistentObject.Id));
            this.db.CreateFullTextIndex("FC_" + RelationalDatabase.FullTextTableName, RelationalDatabase.FullTextTableName, "PK_" + RelationalDatabase.FullTextTableName, "T", 1031);
            return;
        }

        /// <summary>
        /// Creates all views in database.
        /// </summary>
        protected virtual void AddViews() {
            var containerInfos = this.GetContainerInfos();
            var viewsForTables = new List<RelationalViewForTable>(containerInfos.Count);
            var viewsForRelations = new List<RelationalViewForRelations>(containerInfos.Count);
            foreach (var containerInfo in containerInfos) {
                viewsForRelations.Add(new RelationalViewForRelations(this, containerInfo.Type));
            }
            foreach (var containerInfo in containerInfos) {
                foreach (var viewForRelations in viewsForRelations) {
                    viewForRelations.AddContainer(containerInfo);
                }
                var viewForTable = new RelationalViewForTable(this, containerInfo.InternalName, containerInfo.AssemblyQualifiedTypeName);
                if (null != viewForTable.Type && !viewForTable.Type.IsAbstract) {
                    foreach (var view in viewsForTables) {
                        if (view.Type.IsSubclassOf(viewForTable.Type)) {
                            viewForTable.TableNamesAndTypes[view.TableName] = view.Type;
                        } else if (viewForTable.Type.IsSubclassOf(view.Type)) {
                            view.TableNamesAndTypes[viewForTable.TableName] = viewForTable.Type;
                        }
                    }
                    viewsForTables.Add(viewForTable);
                }
            }
            foreach (var viewForTable in viewsForTables) {
                this.db.CreateView(viewForTable.Name, viewForTable.GetUnions());
                var viewsForSubTables = viewForTable.GetViewsForSubTables();
                foreach (var viewForSubTable in viewsForSubTables) {
                    this.db.CreateView(viewForSubTable.Name, viewForSubTable.GetUnions());
                }
            }
            foreach (var viewForRelations in viewsForRelations) {
                if (!string.IsNullOrEmpty(viewForRelations.Name)) {
                    if (viewForRelations.Unions.Count < 1) {
                        viewForRelations.AddDummyUnion();
                    }
                    this.db.CreateView(viewForRelations.Name, viewForRelations.GetUnions());
                }
            }
            return;
        }

        /// <summary>
        /// Copies data of 1:n relations to be converted to n:m relations.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to copy relations for</param>
        private void CopyContainerRelations(string tableName, PersistentObject sampleInstance) {
            var fieldsToBeAdded = new List<PersistentFieldForPersistentObjectCollection>(sampleInstance.PersistentFieldsForCollectionsOfPersistentObjects);
            var fieldKeysToBeRemoved = new List<string>();
            var filterCriteria = new FilterCriteria("ParentTable", RelationalOperator.IsEqualTo, tableName, FilterTarget.IsOtherTextValue);
            this.db.SelectRowsFromTable(RelationalDatabase.CollectionRelationsTableName, "ParentField", SelectionMode.SelectDistinctResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    string fieldKey = dataReader.GetString(0);
                    if (null != fieldKey) {
                        if (null == sampleInstance.GetPersistentFieldForCollectionOfPersistentObjects(fieldKey)) {
                            fieldKeysToBeRemoved.Add(fieldKey);
                        } else {
                            for (var i = fieldsToBeAdded.Count - 1; i > -1; i--) {
                                if (fieldsToBeAdded[i].Key.ToUpperInvariant() == fieldKey.ToUpperInvariant()) {
                                    fieldsToBeAdded.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    }
                }
                return;
            });
            foreach (string fieldKeyToBeRemoved in fieldKeysToBeRemoved) {
                for (var i = fieldsToBeAdded.Count - 1; i > -1; i--) {
                    if (fieldsToBeAdded[i].PreviousKeys.Contains(fieldKeyToBeRemoved)) {
                        fieldsToBeAdded.RemoveAt(i);
                        break;
                    }
                }
            }
            var tableSchema = this.db.GetTableSchema(tableName);
            foreach (var fieldToBeAdded in fieldsToBeAdded) {
                var keysToBeAdded = new List<string>(fieldToBeAdded.PreviousKeys.Count + 1) {
                    fieldToBeAdded.Key
                };
                keysToBeAdded.AddRange(fieldToBeAdded.PreviousKeys);
                for (int i = 0; i < keysToBeAdded.Count; i++) {
                    bool isKeyFoundInSchema = false;
                    var keyToBeAdded = keysToBeAdded[i];
                    var keyToBeAddedKeyToUpper = keyToBeAdded.ToUpperInvariant();
                    foreach (var column in tableSchema) {
                        if (keyToBeAddedKeyToUpper == column.Key.ToUpperInvariant()) {
                            if (i < 1) {
                                this.db.DbCommandLogger.Log?.WriteEntry("Copying data of n:1 fields \"" + fieldToBeAdded.Key + "\" and \"" + fieldToBeAdded.Key + "_\" of container " + tableName + " to n:m field.", LogLevel.Information);
                            } else {
                                this.db.DbCommandLogger.Log?.WriteEntry("Copying data of previous n:1 fields \"" + keyToBeAdded + "\" and \"" + keyToBeAdded + "_\" of container " + tableName + " to n:m field \"" + fieldToBeAdded.Key + "\".", LogLevel.Information);
                            }
                            this.db.CopyDataToRelationsTable(tableName, column.Key, fieldToBeAdded.Key);
                            isKeyFoundInSchema = true;
                            break;
                        }
                    }
                    if (isKeyFoundInSchema) {
                        break;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Generates a new internal table name for a type.
        /// </summary>
        /// <param name="type">type to generate internal table name
        /// for</param>
        /// <returns>generated internal table name for type</returns>
        protected virtual string GenerateNewInternalTableNameFor(Type type) {
            return "O_" + Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Gets the assembly qualified .NET type name of a container
        /// in this persistence mechanism.
        /// </summary>
        /// <param name="internalContainerName">internal name of a
        /// container in this persistence mechanism</param>
        /// <returns>assembly qualified .NET type name of a container
        /// in this persistence mechanism or an null if container
        /// does not exist</returns>
        protected internal override string GetAssemblyQualifiedTypeNameOfContainer(string internalContainerName) {
            if (!this.containerNameCache.TryGetValue(internalContainerName, out string assemblyQualifiedTypeName)) {
                this.RefreshContainerNameCache();
                this.containerNameCache.TryGetValue(internalContainerName, out assemblyQualifiedTypeName);
            }
            return assemblyQualifiedTypeName;
        }

        /// <summary>
        /// Gets info objects for all containers.
        /// </summary>
        /// <returns>info objects for all containers</returns>
        protected override ICollection<ContainerInfo> GetContainerInfos() {
            var containerInfos = new List<ContainerInfo>();
            this.db.SelectRowsFromTable(RelationalDatabase.ContainersTableName, new string[] { "TableName", "TypeName" }, SelectionMode.SelectAllResults, null, FilterCriteria.Empty, SortCriterionCollection.Empty, new RelationalJoin[0], 0, ulong.MaxValue, delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    containerInfos.Add(new ContainerInfo(dataReader.GetString(0), dataReader.GetString(1)));
                }
                return;
            });
            return containerInfos;
        }

        /// <summary>
        /// Gets the field types of a sub table for a specific content base type.
        /// </summary>
        /// <param name="contentBaseType">content base type to get
        /// sub table field types for</param>
        /// <returns>field types of sub table for content base type</returns>
        protected static IEnumerable<PersistentFieldType> GetContainerSubTableFieldsFor(Type contentBaseType) {
            yield return new PersistentFieldType(nameof(PersistentObject.Id), true, TypeOf.Guid);
            yield return new PersistentFieldType("ParentID", true, TypeOf.Guid);
            yield return new PersistentFieldType("ParentIndex", true, TypeOf.Int);
            yield return new PersistentFieldType("Value", true, contentBaseType);
        }

        /// <summary>
        /// Gets the names of all sub tables of a container.
        /// </summary>
        /// <param name="tableName">name of parent database table to
        /// get names of sub tables for</param>
        /// <returns>names of all sub tables of container</returns>
        private IList<string> GetContainerSubTableNamesOf(string tableName) {
            var subTableNames = new List<string>();
            var filterCriteria = new FilterCriteria("ParentTable", RelationalOperator.IsEqualTo, tableName, FilterTarget.IsOtherTextValue);
            this.db.SelectRowsFromTable(RelationalDatabase.SubTablesTableName, "SubTableName", SelectionMode.SelectDistinctResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    subTableNames.Add(dataReader.GetString(0));
                }
                return;
            });
            return subTableNames;
        }

        /// <summary>
        /// Gets the fields to be renamed or removed in relations.
        /// </summary>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to get fields to be renamed or removed
        /// for</param>
        /// <param name="filterCriteria">filter criteria to apply</param>
        /// <param name="fieldsToBeRenamed">dictionary of fields to
        /// be renamed</param>
        /// <param name="fieldKeysToBeRemoved">list of fields to be
        /// removed</param>
        private void GetFieldsToBeRenamedOrRemovedInRelations(PersistentObject sampleInstance, FilterCriteria filterCriteria, out Dictionary<string, PersistentField> fieldsToBeRenamed, out List<string> fieldKeysToBeRemoved) {
            var fieldsToBeAdded = new List<PersistentFieldForPersistentObjectCollection>(sampleInstance.PersistentFieldsForCollectionsOfPersistentObjects);
            var keysToBeRemoved = new List<string>();
            this.db.SelectRowsFromTable(RelationalDatabase.CollectionRelationsTableName, "ParentField", SelectionMode.SelectDistinctResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    string fieldKey = dataReader.GetString(0);
                    if (null != fieldKey) {
                        if (null == sampleInstance.GetPersistentFieldForCollectionOfPersistentObjects(fieldKey)) {
                            keysToBeRemoved.Add(fieldKey);
                        } else {
                            for (var i = fieldsToBeAdded.Count - 1; i > -1; i--) {
                                if (fieldsToBeAdded[i].Key.ToUpperInvariant() == fieldKey.ToUpperInvariant()) {
                                    fieldsToBeAdded.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    }
                }
                return;
            });
            fieldKeysToBeRemoved = keysToBeRemoved;
            fieldsToBeRenamed = new Dictionary<string, PersistentField>();
            foreach (string fieldKeyToBeRemoved in fieldKeysToBeRemoved) {
                bool isFieldToBeRenamedFound = false;
                for (var i = fieldsToBeAdded.Count - 1; i > -1; i--) {
                    var fieldToBeAdded = fieldsToBeAdded[i];
                    foreach (var previousKey in fieldToBeAdded.PreviousKeys) {
                        if (fieldKeyToBeRemoved == previousKey) {
                            fieldsToBeRenamed[previousKey] = fieldToBeAdded;
                            fieldsToBeAdded.RemoveAt(i);
                            isFieldToBeRenamedFound = true;
                            break;
                        }
                    }
                    if (isFieldToBeRenamedFound) {
                        break;
                    }
                }
            }
            foreach (var fieldToBeRenamed in fieldsToBeRenamed) {
                fieldKeysToBeRemoved.Remove(fieldToBeRenamed.Key);
            }
            return;
        }

        /// <summary>
        /// Gets the internal name of a container in this
        /// persistence mechanism (e.g. the name of the related
        /// database table).
        /// </summary>
        /// <param name="assemblyQualifiedTypeName">assembly qualified
        /// name of persistent object to get internal name for</param>
        /// <returns>internal name of a container in this persistence
        /// mechanism or null if container does not exist</returns>
        protected internal override string GetInternalNameOfContainer(string assemblyQualifiedTypeName) {
            string internalContainerName = this.GetInternalNameOfContainerFromCache(assemblyQualifiedTypeName);
            if (string.IsNullOrEmpty(internalContainerName) && TypeOf.PersistentObject.AssemblyQualifiedName != assemblyQualifiedTypeName) {
                this.RefreshContainerNameCache();
                internalContainerName = this.GetInternalNameOfContainerFromCache(assemblyQualifiedTypeName);
            }
            return internalContainerName;
        }

        /// <summary>
        /// Gets the internal name of a container in this
        /// persistence mechanism (e.g. the name of the related
        /// database table) from cache.
        /// </summary>
        /// <param name="assemblyQualifiedTypeName">assembly qualified
        /// name of persistent object to get internal name for</param>
        /// <returns>internal name of a container in this persistence
        /// mechanism or null if container does not exist in cache</returns>
        private string GetInternalNameOfContainerFromCache(string assemblyQualifiedTypeName) {
            string internalContainerName = null;
            foreach (var keyValuePair in this.containerNameCache) {
                if (keyValuePair.Value == assemblyQualifiedTypeName) {
                    internalContainerName = keyValuePair.Key;
                    break;
                }
            }
            return internalContainerName;
        }

        /// <summary>
        /// Gets a list of pairs of name of database table as key and
        /// name of table field as value for all combinations of
        /// table/field which contain n:1 relations.
        /// </summary>
        /// <returns>list if pairs of name of database table as key
        /// and name of table field as value for all combinations of
        /// table/field which contain n:1 relations</returns>
        private IEnumerable<KeyValuePair<string, string>> GetTableFieldPairsForSingleRelations() {
            var tableFieldPairs = new List<KeyValuePair<string, string>>();
            foreach (var containerInfo in this.GetContainerInfos()) {
                var tableSchema = this.db.GetTableSchema(containerInfo.InternalName);
                foreach (var tableColumn in tableSchema) {
                    if (tableColumn.Key.EndsWith("_", StringComparison.Ordinal)) {
                        var typeField = tableColumn.Key;
                        var valueField = typeField.Substring(0, typeField.Length - 1);
                        if (tableSchema.ContainsKey(valueField)) {
                            tableFieldPairs.Add(new KeyValuePair<string, string>(containerInfo.InternalName, valueField));
                        }
                    }
                }
            }
            return tableFieldPairs;
        }

        /// <summary>
        /// Creates a table for storing n:m relations between
        /// persistent .NET objects.
        /// </summary>
        private void InitializeCollectionRelationsTable() {
            var fields = new PersistentFieldType[7];
            fields[0] = new PersistentFieldType(nameof(PersistentObject.Id), true, TypeOf.Guid);
            fields[1] = new PersistentFieldType("ParentTable", true, TypeOf.IndexableString);
            fields[2] = new PersistentFieldType("ParentID", true, TypeOf.Guid);
            fields[3] = new PersistentFieldType("ParentField", true, TypeOf.IndexableString);
            fields[4] = new PersistentFieldType("ParentIndex", true, TypeOf.Int);
            fields[5] = new PersistentFieldType("ChildType", true, TypeOf.IndexableString);
            fields[6] = new PersistentFieldType("ChildID", true, TypeOf.Guid);
            this.db.CreateTable(RelationalDatabase.CollectionRelationsTableName, fields, nameof(PersistentObject.Id));
            this.db.CreateIndex(RelationalDatabase.CollectionRelationsTableName, "ParentID", "ParentField");
            this.db.CreateIndex(RelationalDatabase.CollectionRelationsTableName, "ParentID");
            this.db.CreateIndex(RelationalDatabase.CollectionRelationsTableName, "ChildID");
            var foreignKey = new RelationalForeignKey(RelationalDatabase.CollectionRelationsTableName, "ParentTable", RelationalDatabase.ContainersTableName, "TableName") {
                OnDelete = RelationalAlterAction.Cascade,
                OnUpdate = RelationalAlterAction.Cascade
            };
            this.db.AlterTableAddForeignKey(foreignKey);
            return;
        }

        /// <summary>
        /// Creates a table for storing table names of persistent
        /// .NET types.
        /// </summary>
        private void InitializeContainersTable() {
            var fields = new PersistentFieldType[2];
            fields[0] = new PersistentFieldType("TableName", true, TypeOf.IndexableString);
            fields[1] = new PersistentFieldType("TypeName", true, TypeOf.IndexableString);
            this.db.CreateTable(RelationalDatabase.ContainersTableName, fields, "TableName");
            this.db.CreateIndex(RelationalDatabase.ContainersTableName, "TypeName");
            return;
        }

        /// <summary>
        /// Creates a table for storing table names for persistent
        /// .NET types.
        /// </summary>
        private void InitializeDeletedIDsTable() {
            var fields = new PersistentFieldType[3];
            fields[0] = new PersistentFieldType(nameof(PersistentObject.Id), true, TypeOf.Guid);
            fields[1] = new PersistentFieldType("DeletedAt", true, TypeOf.DateTime);
            fields[2] = new PersistentFieldType("DeletedBy", true, TypeOf.IUser);
            this.db.CreateTable(RelationalDatabase.DeletedIDsTableName, fields, nameof(PersistentObject.Id));
            return;
        }

        /// <summary>
        /// Creates all required system containers in an empty
        /// persistence mechanism - this needs to be called from
        /// "AddContainer()", "MigrateAllContainers()",
        /// "RemoveAllContainers()" and "RenameAllContainers()" if
        /// necessary.
        /// </summary>
        protected override void InitializePersistenceMechanism() {
            this.db.DbCommandLogger.Log?.WriteEntry("Adding system tables.", LogLevel.Information);
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                this.InitializeContainersTable();
                this.InitializeSubTablesTable();
                this.InitializeDeletedIDsTable();
                this.InitializeCollectionRelationsTable();
                transactionScope.Complete();
            }
            if (this.HasFullTextSearchEnabled) {
                this.AddFullTextTable();
            }
            return;
        }

        /// <summary>
        /// Creates a table for storing sub table names of persistent
        /// .NET types.
        /// </summary>
        private void InitializeSubTablesTable() {
            var fields = new PersistentFieldType[3];
            fields[0] = new PersistentFieldType(nameof(PersistentObject.Id), true, TypeOf.Guid);
            fields[1] = new PersistentFieldType("ParentTable", true, TypeOf.IndexableString);
            fields[2] = new PersistentFieldType("SubTableName", true, TypeOf.IndexableString);
            this.db.CreateTable(RelationalDatabase.SubTablesTableName, fields, nameof(PersistentObject.Id));
            var foreignKey = new RelationalForeignKey(RelationalDatabase.SubTablesTableName, "ParentTable", RelationalDatabase.ContainersTableName, "TableName") {
                OnDelete = RelationalAlterAction.Cascade,
                OnUpdate = RelationalAlterAction.Cascade
            };
            this.db.AlterTableAddForeignKey(foreignKey);
            return;
        }

        /// <summary>
        /// Ensures that containers for storing all types of
        /// persistent objects contained in a given enumerable of
        /// types exist in persistence mechanism and that they are up
        /// to date. Orphaned containers are deleted.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// machnism!
        /// </summary>
        /// <param name="types">types to ensure containers for</param>
        public override void MigrateAllContainers(IEnumerable<Type> types) {
            this.MigrateAllContainers(types, true);
            return;
        }

        /// <summary>
        /// Ensures that containers for storing all types of
        /// persistent objects contained in a given enumerable of
        /// types exist in persistence mechanism and that they are up
        /// to date. Orphaned containers are deleted. Cleans up the
        /// persistence mechanism afterwards. Please use this method
        /// if you intent to start the migration from setup routines.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// mechanism!
        /// </summary>
        /// <param name="types">types to ensure containers for</param>
        /// <param name="isRemovingBrokenRelations">true to remove
        /// broken relations, false otherwise</param>
        public void MigrateAllContainers(IEnumerable<Type> types, bool isRemovingBrokenRelations) {
            this.MigrateAllContainersDirty(types);
            this.CleanUp(isRemovingBrokenRelations);
            return;
        }

        /// <summary>
        /// Ensures that containers for storing all types of
        /// persistent objects contained in a given enumerable of
        /// types exist in persistence mechanism and that they are up
        /// to date. Orphaned containers are deleted. Does not clean
        /// up the persistence mechanism afterwards. Please use this
        /// method if you intent to start the migration from within
        /// a performance critical context.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// machanism!
        /// </summary>
        /// <param name="types">types to ensure containers for</param>
        public override void MigrateAllContainersDirty(IEnumerable<Type> types) {
            this.db.DbCommandLogger.Log?.WriteEntry("Starting analysis of database schema.", LogLevel.Information);
            try {
                lock (this.thisLock) {
                    try {
                        this.isUpdatingViewsAutomatically = false;
                        this.RemoveViews();
                        base.MigrateAllContainersDirty(types);
                    } finally {
                        this.AddViews();
                        this.isUpdatingViewsAutomatically = true;
                    }
                }
                if (this.HasFullTextSearchEnabled) {
                    if (this.TryAddFullTextTable()) { // try to add a dummy full-text table just to check whether full-text table exists already - if not, in the if-condition the dummy full-text table will be removed again and the actual full-text table will be added
                        this.RecreateFullTextTable();
                    }
                } else {
                    try {
                        this.RemoveFullTextTable();
                    } catch (DbException) {
                        // ignore SQL exceptions                
                    }
                }
            } catch (PersistenceMechanismNotInitializedException) {
                this.InitializePersistenceMechanism();
                this.MigrateAllContainersDirty(types);
            }
            return;
        }

        /// <summary>
        /// Migrates all allowed groups by ensuring columns with type
        /// name.
        /// </summary>
        /// <param name="persistentTypes">types to migrate allowed groups for</param>
        // TODO: Remove this method after successfull migration of all framework applications.
        protected override void MigrateAllowedGroups(IEnumerable<Type> persistentTypes) {
            foreach (var persistentType in persistentTypes) {
                var persistentTypeContainerName = this.GetInternalNameOfContainer(persistentType);
                if (!string.IsNullOrEmpty(persistentTypeContainerName)) {
                    var tableSchema = this.db.GetTableSchema(persistentTypeContainerName);
                    foreach (var persistentFieldForPersistentObject in this.CreateInstance(persistentType).PersistentFieldsForPersistentObjects) {
                        if (persistentFieldForPersistentObject.GetContentType() == TypeOf.AllowedGroups) {
                            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                                if ((tableSchema.ContainsKey(persistentFieldForPersistentObject.Key) || tableSchema.ContainsKey(persistentFieldForPersistentObject.Key.ToLowerInvariant())) && !tableSchema.ContainsKey(persistentFieldForPersistentObject.Key + '_') && !tableSchema.ContainsKey(persistentFieldForPersistentObject.Key.ToLowerInvariant() + '_')) {
                                    bool isPreviousKeyContainedInSchema = false;
                                    foreach (var previousKey in persistentFieldForPersistentObject.PreviousKeys) {
                                        foreach (var columnName in tableSchema.Keys) {
                                            if (columnName.ToUpperInvariant() == previousKey.ToUpperInvariant() + '_') {
                                                isPreviousKeyContainedInSchema = true;
                                                break;
                                            }
                                        }
                                        if (isPreviousKeyContainedInSchema) {
                                            break;
                                        }
                                    }
                                    if (!isPreviousKeyContainedInSchema) {
                                        if (TypeOf.AllowedGroups != persistentType) {
                                            this.db.DbCommandLogger.Log?.WriteEntry("Removing foreign key for field \"" + persistentFieldForPersistentObject.Key + "\" from container table \"" + persistentTypeContainerName + "\".", LogLevel.Information);
                                            this.db.AlterTableDropForeignKeys(persistentTypeContainerName, persistentFieldForPersistentObject.Key);
                                        }
                                        this.db.DbCommandLogger.Log?.WriteEntry("Adding column \"" + persistentFieldForPersistentObject.Key + "_\" to container table \"" + persistentTypeContainerName + "\".", LogLevel.Information);
                                        this.db.AlterTableAddField(persistentTypeContainerName, new PersistentFieldType(persistentFieldForPersistentObject.Key + '_', true, TypeOf.IndexableString));
                                        var persistentFieldToBeUpdated = new PersistentFieldForString(persistentFieldForPersistentObject.Key + '_', TypeOf.AllowedGroups.AssemblyQualifiedName);
                                        this.db.UpdateTableRow(persistentTypeContainerName, FilterCriteria.Empty, new RelationalJoin[0], new RelationalSubqueryCollection(), new PersistentFieldForElement[] { persistentFieldToBeUpdated });
                                    }
                                }
                                transactionScope.Complete();
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Migrates all foreign keys.
        /// </summary>
        // TODO: Remove this method after successfull migration of all framework applications.
        protected override void MigrateForeignKeys() {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                this.db.AlterTableDropForeignKeys(RelationalDatabase.CollectionRelationsTableName, "ParentTable");
                var foreignKeyForRelationsTable = new RelationalForeignKey(RelationalDatabase.CollectionRelationsTableName, "ParentTable", RelationalDatabase.ContainersTableName, "TableName") {
                    OnDelete = RelationalAlterAction.Cascade,
                    OnUpdate = RelationalAlterAction.Cascade
                };
                this.db.AlterTableAddForeignKey(foreignKeyForRelationsTable);
                this.db.AlterTableDropForeignKeys(RelationalDatabase.SubTablesTableName, "ParentTable");
                var foreignKeyForSubtablesTable = new RelationalForeignKey(RelationalDatabase.SubTablesTableName, "ParentTable", RelationalDatabase.ContainersTableName, "TableName") {
                    OnDelete = RelationalAlterAction.Cascade,
                    OnUpdate = RelationalAlterAction.Cascade
                };
                this.db.AlterTableAddForeignKey(foreignKeyForSubtablesTable);
                transactionScope.Complete();
            }
            return;
        }

        /// <summary>
        /// Recreates the full text table.
        /// </summary>
        public void RecreateFullTextTable() {
            try {
                this.RemoveFullTextTable();
            } catch (DbException) {
                // ignore SQL exceptions
            }
            if (this.HasFullTextSearchEnabled) {
                this.AddFullTextTable();
                this.db.DbCommandLogger.Log?.WriteEntry("Rebuilding full text index. This may take a while. Cancelling this activity will cause the full text index to be incomplete. All objects will be processed, but only relevant ones will be added to full text index.", LogLevel.Information);
                ulong count = 0;
                var containerInfos = this.GetContainerInfos();
                foreach (var containerInfo in containerInfos) {
                    var container = this.GetType().GetMethod("FindContainer").MakeGenericMethod(containerInfo.Type).Invoke(this, null) as IEnumerable;
                    foreach (PersistentObject persistentObject in container) {
                        try {
                            this.AddOrUpdateObjectInFullTextTable(persistentObject);
                            count++;
                            if (0 == count % 50000) {
                                this.db.DbCommandLogger.Log?.WriteEntry("Processed " + count + " objects for full text index so far...", LogLevel.Information);
                            }
                        } catch (Exception) {
                            this.db.DbCommandLogger.Log?.WriteEntry("An error occured while processing object of type " + persistentObject.Type.FullName + " with ID " + persistentObject.Id + " after processing " + count + " object(s).", LogLevel.Error);
                            throw;
                        }
                    }
                }
                this.db.DbCommandLogger.Log?.WriteEntry("Finished rebuild of full text index - processed " + count + " object(s).", LogLevel.Information);
            }
            return;
        }

        /// <summary>
        /// Recreates all views in database.
        /// </summary>,
        public void RecreateViews() {
            this.RemoveViews();
            this.AddViews();
            return;
        }

        /// <summary>
        /// Reloads the container name cache from database.
        /// </summary>
        protected void RefreshContainerNameCache() {
            ConcurrentDictionary<string, string> GetRefreshedContainerNameCache() {
                var refreshedContainerNameCache = new ConcurrentDictionary<string, string>();
                this.db.SelectRowsFromTable(RelationalDatabase.ContainersTableName, new string[] { "TableName", "TypeName" }, SelectionMode.SelectAllResults, null, FilterCriteria.Empty, SortCriterionCollection.Empty, new RelationalJoin[0], 0, ulong.MaxValue, delegate (DbDataReader dataReader) {
                    while (dataReader.Read()) {
                        refreshedContainerNameCache.TryAdd(dataReader.GetString(0), dataReader.GetString(1));
                    }
                    return;
                });
                return refreshedContainerNameCache;
            }
            RelationalDatabase<TConnection>.containerNameCaches.AddOrUpdate(this.ConnectionString, delegate (string connectionString) {
                return this.containerNameCache = GetRefreshedContainerNameCache();
            }, delegate (string connectionString, ConcurrentDictionary<string, string> oldContainerNameCache) {
                return this.containerNameCache = GetRefreshedContainerNameCache();
            });
            return;
        }

        /// <summary>
        /// Deletes all containers for storing persistent objects.
        /// </summary>
        public override void RemoveAllContainers() {
            try {
                lock (this.thisLock) {
                    try {
                        this.isUpdatingViewsAutomatically = false;
                        this.RemoveViews();
                        base.RemoveAllContainers();
                        if (this.HasFullTextSearchEnabled) {
                            try {
                                this.RemoveFullTextTable();
                            } catch (DbException) {
                                // ignore SQL exceptions                
                            }
                        }
                    } finally {
                        this.containerNameCache.Clear();
                        this.AddViews();
                        this.isUpdatingViewsAutomatically = true;
                    }
                }
            } catch (PersistenceMechanismNotInitializedException) {
                this.InitializePersistenceMechanism();
                this.RemoveAllContainers();
            }
            return;
        }

        /// <summary>
        /// Removes all broken relations. Usually this should never
        /// be necessary.
        /// </summary>
        protected virtual void RemoveBrokenRelations() {
            var columnNames = new string[] { nameof(PersistentObject.Id), "ParentTable", "ParentID", "ChildType", "ChildID" };
            var potentialBrokenRelations = new List<PotentialBrokenRelation>();
            this.db.SelectRowsFromTable(RelationalDatabase.CollectionRelationsTableName, columnNames, SelectionMode.SelectDistinctResults, null, FilterCriteria.Empty, SortCriterionCollection.Empty, new RelationalJoin[0], 0, ulong.MaxValue, delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    potentialBrokenRelations.Add(new PotentialBrokenRelation(dataReader.GetGuid(0), dataReader.GetString(1), dataReader.GetGuid(2), dataReader.GetString(3), dataReader.GetGuid(4)));
                }
            });
            for (int i = 0; i < potentialBrokenRelations.Count; i++) {
                var potentialBrokenRelation = potentialBrokenRelations[i];
                var childTable = this.GetInternalNameOfContainer(potentialBrokenRelation.ChildType);
                if (string.IsNullOrEmpty(potentialBrokenRelation.ParentTable) || string.IsNullOrEmpty(childTable)
                    || !this.ContainsID(potentialBrokenRelation.ParentTable, potentialBrokenRelation.ParentId) || !this.ContainsID(childTable, potentialBrokenRelation.ChildId)) {
                    this.db.DbCommandLogger.Log?.WriteEntry("Broken relation detected - parent table \"" + potentialBrokenRelation.ParentTable + "\", parent ID \"" + potentialBrokenRelation.ParentId + "\", child type \"" + potentialBrokenRelation.ChildType + "\", child ID \"" + potentialBrokenRelation.ChildId + "\". The broken relation is going to be removed now.", LogLevel.Warning);
                    var filterCritera = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, potentialBrokenRelation.RelationId);
                    this.db.DeleteRowsFromTable(RelationalDatabase.CollectionRelationsTableName, filterCritera, new RelationalJoin[0], new RelationalSubqueryCollection());
                }
                if (0 == (i + 1) % 100000) {
                    this.db.DbCommandLogger.Log?.WriteEntry("Checked " + (i + 1) + " relations so far...", LogLevel.Information);
                }
            }
            return;
        }

        /// <summary>
        /// Deletes the container for storing persistent objects of
        /// a type and all its persistent objects in persistence
        /// mechanism. Containers of subclasses are not affected.
        /// </summary>
        /// <param name="internalContainerName">internal name of
        /// container in this persistence mechanism to remove</param>
        protected override void RemoveContainer(string internalContainerName) {
            if (null != this.db.DbCommandLogger.Log) {
                if (this.db.CountTableRows(internalContainerName, FilterCriteria.Empty, new RelationalJoin[0]) > 0) {
                    this.db.DbCommandLogger.Log.WriteEntry("Removing NONEMPTY container table \"" + internalContainerName + "\".", LogLevel.Warning);
                } else {
                    this.db.DbCommandLogger.Log.WriteEntry("Removing empty container table \"" + internalContainerName + "\".", LogLevel.Information);
                }
            }
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                try {
                    if (this.isUpdatingViewsAutomatically) {
                        this.RemoveViews();
                    }
                    this.RemoveContainerSingleRelations(internalContainerName);
                    this.RemoveContainerSubTables(internalContainerName);
                    this.RemoveContainerTableName(internalContainerName);
                    try {
                        this.db.DropTable(internalContainerName);
                    } catch (DbException ex) {
                        throw new DependencyException("Container table \""
                            + internalContainerName
                            + "\" cannot be deleted - maybe another container must be deleted first: "
                            + ex.Message, ex);
                    }
                } finally {
                    if (this.isUpdatingViewsAutomatically) {
                        this.AddViews();
                    }
                }
                if (this.ContainerCache.Contains(internalContainerName)) {
                    this.ContainerCache.Remove(internalContainerName);
                }
                transactionScope.Complete();
            }
            return;
        }

        /// <summary>
        /// Removes all n:1 relations to objects of a specific parent
        /// table.
        /// </summary>
        /// <param name="tableName">name of parent table to remove
        /// n:1 relations of</param>
        private void RemoveContainerSingleRelations(string tableName) {
            var assemblyQualifiedName = this.GetAssemblyQualifiedTypeNameOfContainer(tableName);
            var tableFieldPairs = this.GetTableFieldPairsForSingleRelations();
            foreach (var tableFieldPair in tableFieldPairs) {
                var setNullFilterCriteria = new FilterCriteria(tableFieldPair.Value + '_', RelationalOperator.IsEqualTo, assemblyQualifiedName, FilterTarget.IsOtherTextValue);
                var fieldsToBeUpdated = new PersistentFieldForElement[] {
                    new PersistentFieldForNullableGuid(tableFieldPair.Value, null),
                    new PersistentFieldForString(tableFieldPair.Value + '_', null)
                };
                this.db.UpdateTableRow(tableFieldPair.Key, setNullFilterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection(), fieldsToBeUpdated);
            }
            return;
        }

        /// <summary>
        /// Deletes the sub table of a container.
        /// </summary>
        /// <param name="tableName">name of parent table</param>
        private void RemoveContainerSubTables(string tableName) {
            var subTableNames = this.GetContainerSubTableNamesOf(tableName);
            foreach (string subTableName in subTableNames) {
                this.RemoveContainerSubTable(tableName, subTableName);
            }
            return;
        }

        /// <summary>
        /// Deletes a sub table for a specific .NET type.
        /// </summary>
        /// <param name="parentTableName">name of parent table</param>
        /// <param name="subTableName">name of sub table</param>
        private void RemoveContainerSubTable(string parentTableName, string subTableName) {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                this.RemoveContainerSubTableName(parentTableName, subTableName);
                this.db.DropTable(subTableName);
                transactionScope.Complete();
            }
            return;
        }

        /// <summary>
        /// Deletes the name of a sub table for a specific .NET type.
        /// </summary>
        /// <param name="parentTableName">name of parent table</param>
        /// <param name="subTableName">name of sub table</param>
        private void RemoveContainerSubTableName(string parentTableName, string subTableName) {
            var filterCriteria = new FilterCriteria("ParentTable", RelationalOperator.IsEqualTo, parentTableName, FilterTarget.IsOtherTextValue)
                .And("SubTableName", RelationalOperator.IsEqualTo, subTableName, FilterTarget.IsOtherTextValue);
            this.db.DeleteRowsFromTable(RelationalDatabase.SubTablesTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection());
            return;
        }

        /// <summary>
        /// Deletes the name of the database table for a specific
        /// .NET type.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        private void RemoveContainerTableName(string tableName) {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                string typeName = this.GetAssemblyQualifiedTypeNameOfContainer(tableName);
                var filterCriteria = new FilterCriteria("ChildType", RelationalOperator.IsEqualTo, typeName, FilterTarget.IsOtherTextValue);
                this.db.DeleteRowsFromTable(RelationalDatabase.CollectionRelationsTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection());
                filterCriteria = new FilterCriteria("TableName", RelationalOperator.IsEqualTo, tableName, FilterTarget.IsOtherTextValue);
                this.db.DeleteRowsFromTable(RelationalDatabase.ContainersTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection());
                this.containerNameCache.TryRemove(tableName, out typeName);
                transactionScope.Complete();
            }
            return;
        }

        /// <summary>
        /// Removes the table for storing full text data.
        /// </summary>
        private void RemoveFullTextTable() {
            this.db.DbCommandLogger.Log?.WriteEntry("Trying to remove full-text table.", LogLevel.Information);
            try {
                this.db.DropFullTextIndex("FC_" + RelationalDatabase.FullTextTableName, RelationalDatabase.FullTextTableName);
            } finally {
                this.db.DropTable(RelationalDatabase.FullTextTableName);
            }
            return;
        }

        /// <summary>
        /// Removes all views from database.
        /// </summary>
        protected virtual void RemoveViews() {
            var viewNames = new List<string>();
            var containerInfos = this.GetContainerInfos();
            foreach (var containerInfo in containerInfos) {
                var viewForTableName = RelationalDatabase.GetViewNameForContainer(containerInfo.InternalName);
                viewNames.Add(viewForTableName);
                var viewForRelationsName = RelationalDatabase.GetViewNameForSingleRelations(containerInfo.InternalName);
                viewNames.Add(viewForRelationsName);
            }
            this.db.SelectRowsFromTable(RelationalDatabase.SubTablesTableName, "SubTableName", SelectionMode.SelectAllResults, null, FilterCriteria.Empty, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    var view = new RelationalViewForSubTable(this);
                    view.TableNamesAndTypes.Add(dataReader.GetString(0), null);
                    viewNames.Add(view.Name);
                }
                return;
            });
            viewNames.Add("Q_" + RelationalDatabase.CollectionRelationsTableName); // TODO: Remove this after upgrading all databases!
            foreach (var viewName in viewNames) {
                this.db.DropView(viewName);
            }
            return;
        }

        /// <summary>
        /// Updates the names of all containers for storing
        /// persistent objects.
        /// </summary>
        public override void RenameAllContainers() {
            try {
                lock (this.thisLock) {
                    try {
                        this.isUpdatingViewsAutomatically = false;
                        this.RemoveViews();
                        base.RenameAllContainers();
                    } finally {
                        this.containerNameCache.Clear();
                        this.AddViews();
                        this.isUpdatingViewsAutomatically = true;
                    }
                }
            } catch (PersistenceMechanismNotInitializedException) {
                this.InitializePersistenceMechanism();
                this.RenameAllContainers();
            }
            return;
        }

        /// <summary>
        /// Renames a container for storing persistent objects of a
        /// type.
        /// </summary>
        /// <param name="oldName">start of old name of container</param>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to rename container for</param>
        /// <returns>true if container could be renamed successfully,
        /// false otherwise or if it did not exist in persistence
        /// mechanism</returns>
        protected override bool RenameContainer(string oldName, PersistentObject sampleInstance) {
            bool success = false;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                string oldTableName = null;
                try {
                    if (this.isUpdatingViewsAutomatically) {
                        this.RemoveViews();
                    }
                    var filterCriteria = new FilterCriteria("TypeName", RelationalOperator.StartsWith, oldName + ',', FilterTarget.IsOtherTextValue)
                        .Or("TypeName", RelationalOperator.IsEqualTo, oldName, FilterTarget.IsOtherTextValue);
                    this.db.SelectRowsFromTable(RelationalDatabase.ContainersTableName, "TableName", SelectionMode.SelectAllResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                        if (dataReader.Read()) {
                            oldTableName = dataReader.GetString(0);
                            if (dataReader.Read()) {
                                throw new TypeException("Container for old name \"" + oldName
                                    + "\" to be renamed is not unique in relational database.");
                            }
                        }
                        return;
                    });
                    if (!string.IsNullOrEmpty(oldTableName)) {
                        string oldAssemblyQualifiedTypeName = this.GetAssemblyQualifiedTypeNameOfContainer(oldTableName);
                        Type newType = sampleInstance.Type;
                        string newTableName = this.GenerateNewInternalTableNameFor(newType);
                        this.db.DbCommandLogger.Log?.WriteEntry("Renaming container \"" + oldName + "\" to \"" + newType.AssemblyQualifiedName + "\".", LogLevel.Information);
                        this.RenameContainerParentTable(oldTableName, oldAssemblyQualifiedTypeName, newTableName, newType.AssemblyQualifiedName);
                        this.RenameContainerSubTables(oldTableName, newTableName);
                        this.RenameContainerSingleRelations(oldAssemblyQualifiedTypeName, newType.AssemblyQualifiedName);
                        success = true;
                    }
                } finally {
                    if (this.isUpdatingViewsAutomatically) {
                        this.AddViews();
                    }
                }
                if (!string.IsNullOrEmpty(oldTableName) && this.ContainerCache.Contains(oldTableName)) {
                    this.ContainerCache.Remove(oldTableName);
                }
                transactionScope.Complete();
            }
            return success;
        }

        /// <summary>
        /// Renames the parent container of a container for storing
        /// persistent objects of a type.
        /// </summary>
        /// <param name="oldTableName">old name of parent table to be renamed</param>
        /// <param name="oldAssemblyQualifiedTypeName">old assembly qualified type name</param>
        /// <param name="newTableName">new name of parent table to be renamed</param>
        /// <param name="newAssemblyQualifiedTypeName">new assembly qualified type name</param>
        private void RenameContainerParentTable(string oldTableName, string oldAssemblyQualifiedTypeName, string newTableName, string newAssemblyQualifiedTypeName) {
            if (newTableName.ToUpperInvariant() != oldTableName.ToUpperInvariant()) {
                if (newAssemblyQualifiedTypeName.ToUpperInvariant() == oldAssemblyQualifiedTypeName.ToUpperInvariant()) {
                    this.RenameContainerParentTable(oldTableName, oldAssemblyQualifiedTypeName, "tempTableName", "tempAssemblyQualifiedTypeName");
                    this.RenameContainerParentTable("tempTableName", "tempAssemblyQualifiedTypeName", newTableName, newAssemblyQualifiedTypeName);
                } else {
                    this.db.AlterTableRename(oldTableName, newTableName);
                    var containersFilterCriteria = new FilterCriteria("TableName", RelationalOperator.IsEqualTo, oldTableName, FilterTarget.IsOtherTextValue);
                    var containersFieldsToBeUpdated = new PersistentFieldForElement[2];
                    containersFieldsToBeUpdated[0] = new PersistentFieldForString("TableName", newTableName);
                    containersFieldsToBeUpdated[1] = new PersistentFieldForString("TypeName", newAssemblyQualifiedTypeName);
                    this.db.UpdateTableRow(RelationalDatabase.ContainersTableName, containersFilterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection(), containersFieldsToBeUpdated);
                    var relationsFilterCriteria = new FilterCriteria("ChildType", RelationalOperator.IsEqualTo, oldAssemblyQualifiedTypeName, FilterTarget.IsOtherTextValue);
                    var relationsFieldsToBeUpdated = new PersistentFieldForElement[1];
                    relationsFieldsToBeUpdated[0] = new PersistentFieldForString("ChildType", newAssemblyQualifiedTypeName);
                    this.db.UpdateTableRow(RelationalDatabase.CollectionRelationsTableName, relationsFilterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection(), relationsFieldsToBeUpdated);
                }
            }
            return;
        }

        /// <summary>
        /// Renames all n:1 relations to objects of a specific parent
        /// table.
        /// </summary>
        /// <param name="oldAssemblyQualifiedTypeName">old assembly
        /// qualified type name</param>
        /// <param name="newAssemblyQualifiedTypeName">new assembly
        /// qualified type name</param>
        private void RenameContainerSingleRelations(string oldAssemblyQualifiedTypeName, string newAssemblyQualifiedTypeName) {
            var tableFieldPairs = this.GetTableFieldPairsForSingleRelations();
            foreach (var tableFieldPair in tableFieldPairs) {
                var updateTableNameFilterCriteria = new FilterCriteria(tableFieldPair.Value + '_', RelationalOperator.IsEqualTo, oldAssemblyQualifiedTypeName, FilterTarget.IsOtherTextValue);
                var fieldsToBeUpdated = new PersistentFieldForElement[] {
                    new PersistentFieldForString(tableFieldPair.Value + '_', newAssemblyQualifiedTypeName)
                };
                this.db.UpdateTableRow(tableFieldPair.Key, updateTableNameFilterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection(), fieldsToBeUpdated);
            }
            return;
        }

        /// <summary>
        /// Renames a sub container of a container for storing
        /// persistent objects of a type.
        /// </summary>
        /// <param name="oldSubTableName">old name of sub table to be
        /// renamed</param>
        /// <param name="newSubTableName">new name of sub table to be
        /// renamed</param>
        private void RenameContainerSubTable(string oldSubTableName, string newSubTableName) {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                this.db.AlterTableRename(oldSubTableName, newSubTableName);
                var filterCriteria = new FilterCriteria("SubTableName", RelationalOperator.IsEqualTo, oldSubTableName, FilterTarget.IsOtherTextValue);
                var fieldsToBeUpdated = new PersistentFieldForElement[1];
                fieldsToBeUpdated[0] = new PersistentFieldForString("SubTableName", newSubTableName);
                this.db.UpdateTableRow(RelationalDatabase.SubTablesTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection(), fieldsToBeUpdated);
                this.db.AlterTableRenameKey(newSubTableName, "PK_" + oldSubTableName, "PK_" + newSubTableName); // this was missing in old framework versions - you might need to rename keys manually in old databases to make them match the pattern again
                transactionScope.Complete();
            }
            return;
        }

        /// <summary>
        /// Renames all sub containers of a container for storing
        /// persistent objects of a type.
        /// </summary>
        /// <param name="oldTableName">old name of parent table to be
        /// renamed</param>
        /// <param name="newTableName">new name of parent table to be
        /// renamed</param>
        private void RenameContainerSubTables(string oldTableName, string newTableName) {
            var oldSubTableNames = this.GetContainerSubTableNamesOf(newTableName);
            foreach (string oldSubTableName in oldSubTableNames) {
                string newSubTableName = oldSubTableName.Replace(oldTableName.Substring(1), newTableName.Substring(1));
                this.RenameContainerSubTable(oldSubTableName, newSubTableName);
            }
            return;
        }

        /// <summary>
        /// Tries to add a table for storing full text data.
        /// </summary>
        /// <returns>true if full text table was created, false
        /// otherwise</returns>
        private bool TryAddFullTextTable() {
            bool success = false;
            try {
                this.AddFullTextTable();
                success = true;
            } catch (DbException) {
                // ignore SQL exceptions                
            }
            return success;
        }

        /// <summary>
        /// Updates the container for storing persistent objects of a
        /// specific type in persistence mechanism. This needs to be
        /// called whenever there is a change in the fields to
        /// persist of the related persistent objects.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// machnism!
        /// </summary>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to update container for</param>
        protected override void UpdateContainer(PersistentObject sampleInstance) {
            try {
                if (this.isUpdatingViewsAutomatically) {
                    this.RemoveViews();
                }
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                    string internalContainerName = this.GetInternalNameOfContainer(sampleInstance.Type);
                    this.CopyContainerRelations(internalContainerName, sampleInstance);
                    this.UpdateContainerParentTable(internalContainerName, sampleInstance);
                    this.UpdateContainerSubTables(internalContainerName, sampleInstance);
                    this.UpdateContainerRelations(internalContainerName, sampleInstance);
                    transactionScope.Complete();
                }
            } finally {
                if (this.isUpdatingViewsAutomatically) {
                    this.AddViews();
                }
            }
            return;
        }

        /// <summary>
        /// Update parent table containing fields for single simple
        /// elements and persistent objects.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to update parent table for</param>
        private void UpdateContainerParentTable(string tableName, PersistentObject sampleInstance) {
            var tableFields = new List<PersistentFieldTypeWithPreviousKeys>();
            foreach (var persistentFieldForElement in sampleInstance.PersistentFieldsForElements) {
                tableFields.Add(new PersistentFieldTypeWithPreviousKeys(persistentFieldForElement.FieldType, persistentFieldForElement.PreviousKeys));
            }
            foreach (var persistentFieldForPersistentObject in sampleInstance.PersistentFieldsForPersistentObjects) {
                var fieldTypeForValue = new PersistentFieldType(persistentFieldForPersistentObject.Key, true, TypeOf.NullableGuid);
                tableFields.Add(new PersistentFieldTypeWithPreviousKeys(fieldTypeForValue, persistentFieldForPersistentObject.PreviousKeys));
                var fieldTypeForType = new PersistentFieldType(persistentFieldForPersistentObject.Key + '_', true, TypeOf.IndexableString);
                var previousKeysForType = new List<string>(persistentFieldForPersistentObject.PreviousKeys.Count);
                foreach (var previousKey in persistentFieldForPersistentObject.PreviousKeys) {
                    previousKeysForType.Add(previousKey + '_');
                }
                tableFields.Add(new PersistentFieldTypeWithPreviousKeys(fieldTypeForType, previousKeysForType));
            }
            this.UpdateContainerParentTable(tableName, sampleInstance, sampleInstance.Type.FullName, tableFields);
            return;
        }

        /// <summary>
        /// Update parent table containing fields for single simple
        /// elements and persistent objects.
        /// </summary>
        /// <param name="tableName">name of database table</param>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to update parent table for</param>
        /// <param name="containerName">display name of container</param>
        /// <param name="fieldTypes">array of simple element field
        /// types of to-be table schema</param>
        private void UpdateContainerParentTable(string tableName, PersistentObject sampleInstance, string containerName, IEnumerable<PersistentFieldTypeWithPreviousKeys> fieldTypes) {
            var fieldsToBeAdded = new PersistentFieldTypeCollection<PersistentFieldTypeWithPreviousKeys>(fieldTypes);
            var columnsToBeRemoved = new List<string>();
            var fieldsToBeRenamed = new Dictionary<string, PersistentFieldTypeWithPreviousKeys>();
            var fieldsToBeUpdated = new List<PersistentFieldTypeWithPreviousKeys>();
            foreach (var column in this.db.GetTableSchema(tableName)) {
                string columnName = column.Key;
                string columnNameToUpper = columnName.ToUpperInvariant();
                bool isColumnMissingInFields = true;
                for (var i = fieldsToBeAdded.Count - 1; i > -1; i--) {
                    var field = fieldsToBeAdded[i];
                    if (field.Key.ToUpperInvariant() == columnNameToUpper) {
                        fieldsToBeAdded.RemoveAt(i);
                        isColumnMissingInFields = false;
                        var actualDataTypeName = column.Value;
                        var expectedDataTypeName = this.db.GetDataTypeForType(field.ContentBaseType);
                        if (!this.db.AreDataTypesEqual(actualDataTypeName, expectedDataTypeName)) {
                            fieldsToBeUpdated.Add(field);
                        }
                        break;
                    }
                }
                if (isColumnMissingInFields) {
                    bool isColumnToBeRemoved = true;
                    foreach (var field in fieldsToBeAdded) {
                        foreach (var previousKey in field.PreviousKeys) {
                            if (previousKey.ToUpperInvariant() == columnNameToUpper) {
                                fieldsToBeAdded.Remove(field);
                                fieldsToBeRenamed.Add(columnName, field);
                                isColumnToBeRemoved = false;
                                break;
                            }
                        }
                        if (!isColumnToBeRemoved) {
                            break;
                        }
                    }
                    if (isColumnToBeRemoved) {
                        columnsToBeRemoved.Add(columnName);
                    }
                }
            }
            foreach (var fieldToBeRenamed in fieldsToBeRenamed) {
                var previousFieldKey = fieldToBeRenamed.Key;
                var newFieldKey = fieldToBeRenamed.Value.Key;
                this.db.DbCommandLogger.Log?.WriteEntry("Renaming 1:1/n:1 field \"" + previousFieldKey + "\" to \"" + newFieldKey + "\" of container " + containerName + ".", LogLevel.Information);
                this.db.AlterTableRenameField(tableName, previousFieldKey, newFieldKey);
                this.db.AlterTableUpdateFieldType(tableName, fieldToBeRenamed.Value);
            }
            foreach (var fieldToBeUpdated in fieldsToBeUpdated) {
                this.db.DbCommandLogger.Log?.WriteEntry("Updating data type of 1:1 field \"" + fieldToBeUpdated.Key + "\" of container " + containerName + ".", LogLevel.Information);
                this.db.AlterTableUpdateFieldType(tableName, fieldToBeUpdated);
            }
            foreach (var fieldToBeAdded in fieldsToBeAdded) {
                var fieldKeyToBeConverted = fieldToBeAdded.Key.Substring(0, fieldToBeAdded.Key.Length - 1);
                if (fieldToBeAdded.Key.EndsWith("_", StringComparison.Ordinal) && fieldsToBeAdded.Contains(fieldKeyToBeConverted)) {
                    this.db.DbCommandLogger.Log?.WriteEntry("Copying data of n:m field \"" + fieldKeyToBeConverted + "\" of container " + containerName + " to n:1 field.", LogLevel.Information);
                    this.db.AlterTableAddField(tableName, fieldToBeAdded); // field to be converted without tailing underscore should exist in parent table already
                    if (this.db.CopyDataFromRelationsTable(fieldKeyToBeConverted, tableName, fieldKeyToBeConverted) < 1) {
                        foreach (var previousKey in fieldsToBeAdded[fieldKeyToBeConverted].PreviousKeys) {
                            this.db.DbCommandLogger.Log?.WriteEntry("Copying data of previous n:m field \"" + previousKey + "\" of container " + containerName + " to n:1 field \"" + fieldKeyToBeConverted + "\".", LogLevel.Information);
                            if (this.db.CopyDataFromRelationsTable(previousKey, tableName, fieldKeyToBeConverted) > 0) {
                                break;
                            }
                        }
                    }
                } else {
                    this.db.DbCommandLogger.Log?.WriteEntry("Adding 1:1/n:1 field \"" + fieldToBeAdded.Key + "\" of container " + containerName + ".", LogLevel.Information);
                    this.db.AlterTableAddField(tableName, fieldToBeAdded);
                }
            }
            foreach (var columnToBeRemoved in columnsToBeRemoved) {
                if (null != this.db.DbCommandLogger.Log) {
                    if (this.db.CountTableRows(tableName, FilterCriteria.Empty, new RelationalJoin[0]) > 0) {
                        if (null != sampleInstance.GetPersistentFieldForCollectionOfPersistentObjects(columnToBeRemoved)
                            || (columnToBeRemoved.EndsWith("_", StringComparison.Ordinal) && null != sampleInstance.GetPersistentFieldForCollectionOfPersistentObjects(columnToBeRemoved.Substring(0, columnToBeRemoved.Length - 1)))) {
                            this.db.DbCommandLogger.Log?.WriteEntry("Removing n:1 field \"" + columnToBeRemoved + "\" of container " + containerName + " which contains data that should have been copied to an n:m field before automatically.", LogLevel.Information);
                        } else {
                            this.db.DbCommandLogger.Log.WriteEntry("Removing NONEMPTY 1:1/n:1 field \"" + columnToBeRemoved + "\" of container " + containerName + ".", LogLevel.Warning);
                        }
                    } else {
                        this.db.DbCommandLogger.Log.WriteEntry("Removing empty 1:1/n:1 field \"" + columnToBeRemoved + "\" of container " + containerName + ".", LogLevel.Information);
                    }
                }
                this.db.AlterTableDropField(tableName, columnToBeRemoved);
            }
            return;
        }

        /// <summary>
        /// Updates relations of fields for persistent objects.
        /// </summary>
        /// <param name="tableName">name of parent database table</param>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to update relations for</param>
        private void UpdateContainerRelations(string tableName, PersistentObject sampleInstance) {
            var filterCriteria = new FilterCriteria("ParentTable", RelationalOperator.IsEqualTo, tableName, FilterTarget.IsOtherTextValue);
            this.GetFieldsToBeRenamedOrRemovedInRelations(sampleInstance, filterCriteria, out var fieldsToBeRenamed, out var fieldKeysToBeRemoved);
            foreach (var fieldToBeRenamed in fieldsToBeRenamed) {
                var previousFieldKey = fieldToBeRenamed.Key;
                var newFieldKey = fieldToBeRenamed.Value.Key;
                this.db.DbCommandLogger.Log?.WriteEntry("Renaming n:m field \"" + previousFieldKey + "\" to \"" + newFieldKey + "\" for container " + sampleInstance.Type.FullName + ".", LogLevel.Information);
                var previousKeyFilterCriteria = filterCriteria.And("ParentField", RelationalOperator.IsEqualTo, previousFieldKey, FilterTarget.IsOtherTextValue);
                var fieldsToBeUpdated = new PersistentFieldForElement[1];
                fieldsToBeUpdated[0] = new PersistentFieldForString("ParentField", newFieldKey);
                this.db.UpdateTableRow(RelationalDatabase.CollectionRelationsTableName, previousKeyFilterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection(), fieldsToBeUpdated);
            }
            foreach (string fieldKeyToBeRemoved in fieldKeysToBeRemoved) {
                if (null == sampleInstance.GetPersistentFieldForPersistentObject(fieldKeyToBeRemoved)) {
                    this.db.DbCommandLogger.Log?.WriteEntry("Removing NONEMPTY n:m field \"" + fieldKeyToBeRemoved + "\" of container " + sampleInstance.Type.FullName + ".", LogLevel.Warning);
                } else {
                    this.db.DbCommandLogger.Log?.WriteEntry("Removing n:m field \"" + fieldKeyToBeRemoved + "\" of container " + sampleInstance.Type.FullName + " which contains data that should have been copied to an n:1 field before automatically.", LogLevel.Information);
                }
                var subFilterCriteria = filterCriteria.And("ParentField", RelationalOperator.IsEqualTo, fieldKeyToBeRemoved, FilterTarget.IsOtherTextValue);
                this.db.DeleteRowsFromTable(RelationalDatabase.CollectionRelationsTableName, subFilterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection());
            }
            return;
        }

        /// <summary>
        /// Updates sub tables containing simple element fields.
        /// </summary>
        /// <param name="tableName">name of parent database table</param>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to update sub tables for</param>
        private void UpdateContainerSubTables(string tableName, PersistentObject sampleInstance) {
            var subTablesToBeRemoved = this.GetContainerSubTableNamesOf(tableName);
            var fieldsToBeAdded = new List<PersistentFieldForCollection>();
            foreach (var field in sampleInstance.PersistentFieldsForCollectionsOfElements) {
                string fieldSubTableName = RelationalDatabase.GetInternalNameOfSubTable(tableName, field.Key);
                if (subTablesToBeRemoved.Contains(fieldSubTableName)) {
                    subTablesToBeRemoved.Remove(fieldSubTableName);
                    var subTableFields = new List<PersistentFieldTypeWithPreviousKeys>();
                    foreach (var subTableField in RelationalDatabase<TConnection>.GetContainerSubTableFieldsFor(field.ContentBaseType)) {
                        subTableFields.Add(new PersistentFieldTypeWithPreviousKeys(subTableField, new string[0]));
                    }
                    this.UpdateContainerParentTable(fieldSubTableName, sampleInstance, sampleInstance.Type.FullName + '_' + field.Key, subTableFields);
                } else {
                    fieldsToBeAdded.Add(field);
                }
            }
            var fieldsToBeRenamed = new Dictionary<string, PersistentFieldForCollection>();
            foreach (var subTableToBeRemoved in subTablesToBeRemoved) {
                bool isFieldToBeRenamedFound = false;
                foreach (var fieldToBeAdded in fieldsToBeAdded) {
                    foreach (var previousKey in fieldToBeAdded.PreviousKeys) {
                        if (RelationalDatabase.GetInternalNameOfSubTable(tableName, previousKey) == subTableToBeRemoved) {
                            fieldsToBeRenamed[previousKey] = fieldToBeAdded;
                            isFieldToBeRenamedFound = true;
                            break;
                        }
                    }
                    if (isFieldToBeRenamedFound) {
                        break;
                    }
                }
            }
            foreach (var fieldToBeRenamed in fieldsToBeRenamed) {
                var previousFieldKey = fieldToBeRenamed.Key;
                var newFieldKey = fieldToBeRenamed.Value.Key;
                var previousNameOfSubTable = RelationalDatabase.GetInternalNameOfSubTable(tableName, previousFieldKey);
                subTablesToBeRemoved.Remove(previousNameOfSubTable);
                fieldsToBeAdded.Remove(fieldToBeRenamed.Value);
                this.db.DbCommandLogger.Log?.WriteEntry("Renaming 1:n field \"" + previousFieldKey + "\" to \"" + newFieldKey + "\" for container " + sampleInstance.Type.FullName + ".", LogLevel.Information);
                var newNameOfSubTable = RelationalDatabase.GetInternalNameOfSubTable(tableName, newFieldKey);
                this.RenameContainerSubTable(previousNameOfSubTable, newNameOfSubTable);
            }
            foreach (var fieldToBeAdded in fieldsToBeAdded) {
                this.db.DbCommandLogger.Log?.WriteEntry("Adding 1:n field \"" + fieldToBeAdded.Key + "\" to container " + sampleInstance.Type.FullName + ".", LogLevel.Information);
                var foreignKey = this.AddContainerSubTable(tableName, fieldToBeAdded);
                this.db.AlterTableAddForeignKey(foreignKey);
            }
            foreach (var subTableToBeRemoved in subTablesToBeRemoved) {
                if (null != this.db.DbCommandLogger.Log) {
                    LogLevel logLevel;
                    string action = "Removing ";
                    if (this.db.CountTableRows(tableName, FilterCriteria.Empty, new RelationalJoin[0]) > 0) {
                        action += "NONEMPTY";
                        logLevel = LogLevel.Warning;
                    } else {
                        action += "empty";
                        logLevel = LogLevel.Information;
                    }
                    this.db.DbCommandLogger.Log.WriteEntry(action + " table \"" + subTableToBeRemoved + "\" for 1:n field of container " + sampleInstance.Type.FullName + ".", logLevel);
                }
                this.RemoveContainerSubTable(tableName, subTableToBeRemoved);
            }
            return;
        }

    }

}