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

    using Framework.Model;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common;
    using System.Text;
    using System.Transactions;

    // Partial class for object operations of relational database.
    public partial class RelationalDatabase<TConnection> {

        /// <summary>
        /// Delegate for getting a specific persistent field of a
        /// persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to get
        /// persistent field for</param>
        /// <param name="key">key of persistent field to get</param>
        /// <returns>specific persistent field of persistent object</returns>
        private delegate PersistentField GetPersistentFieldForKeyDelegate(PersistentObject persistentObject, string key);

        /// <summary>
        /// Internal name of group members table.
        /// </summary>
        private string GroupMembersTableName {
            get {
                if (string.IsNullOrEmpty(RelationalDatabase<TConnection>.groupMembersTableName)) {
                    RelationalDatabase<TConnection>.groupMembersTableName = RelationalDatabase.GetInternalNameOfSubTable(this.GetInternalNameOfContainer(typeof(Group)), nameof(Group.Members));
                }
                return RelationalDatabase<TConnection>.groupMembersTableName;
            }
        }
        private static string groupMembersTableName;

        /// <summary>
        /// Adds filter criteria for applying read permissions to a
        /// filter chain.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to add
        /// additional filter criteria to</param>
        /// <returns>extended filter citeria</returns>
        private FilterCriteria AddFilterCriteriaForReadPermissions(FilterCriteria filterCriteria) {
            FilterCriteria newFilterCriteria;
            if (SecurityModel.ApplyPermissions == this.SecurityModel) {
                if (filterCriteria.IsEmpty) {
                    newFilterCriteria = FilterCriteria.Empty;
                } else {
                    newFilterCriteria = new FilterCriteria(filterCriteria);
                }
                newFilterCriteria = newFilterCriteria.And(new FilterCriteria(new string[] { nameof(PersistentObject.AllowedGroups), nameof(AllowedGroups.ForReading), nameof(Group.Members) }, RelationalOperator.IsEqualTo, this.UserDirectory.CurrentUser)
                    .Or(new string[] { nameof(PersistentObject.AllowedGroups), nameof(AllowedGroups.ForReading), nameof(Group.Members) }, RelationalOperator.IsEqualTo, Directories.UserDirectory.AnonymousUser));
            } else if (SecurityModel.IgnorePermissions == this.SecurityModel) {
                newFilterCriteria = filterCriteria;
            } else {
                throw new PersistenceException("Security model \"" + this.SecurityModel.ToString() + "\" is not supported.");
            }
            return newFilterCriteria;
        }

        /// <summary>
        /// Adds filter criteria for applying write permissions to a
        /// filter chain.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to add
        /// additional filter criteria to</param>
        /// <returns>extended filter citeria</returns>
        private FilterCriteria AddFilterCriteriaForWritePermissions(FilterCriteria filterCriteria) {
            FilterCriteria newFilterCriteria;
            if (SecurityModel.ApplyPermissions == this.SecurityModel) {
                if (filterCriteria.IsEmpty) {
                    newFilterCriteria = FilterCriteria.Empty;
                } else {
                    newFilterCriteria = new FilterCriteria(filterCriteria);
                }
                newFilterCriteria = newFilterCriteria.And(new FilterCriteria(new string[] { nameof(PersistentObject.AllowedGroups), nameof(AllowedGroups.ForWriting), nameof(Group.Members) }, RelationalOperator.IsEqualTo, this.UserDirectory.CurrentUser)
                    .Or(new string[] { nameof(PersistentObject.AllowedGroups), nameof(AllowedGroups.ForWriting), nameof(Group.Members) }, RelationalOperator.IsEqualTo, Directories.UserDirectory.AnonymousUser));
            } else if (SecurityModel.IgnorePermissions == this.SecurityModel) {
                newFilterCriteria = filterCriteria;
            } else {
                throw new PersistenceException("Security model \"" + this.SecurityModel.ToString() + "\" is not supported.");
            }
            return newFilterCriteria;
        }

        /// <summary>
        /// Adds an object to a container. 
        /// </summary>
        /// <param name="persistentObject">object to add to the
        /// container</param>
        /// <param name="persistentContainer">persistent container to
        /// add object to</param>
        /// <returns>true if object was added to container
        /// successfully, false otherwise</returns>
        internal override bool AddObject(PersistentObject persistentObject, PersistentContainer persistentContainer) {
            bool success;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                success = base.AddObject(persistentObject, persistentContainer);
                transactionScope.Complete();
            }
            return success;
        }

        /// <summary>
        /// Adds an object to the persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to add</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        /// <returns>true if object was added to persistence
        /// mechanism successfully, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        protected internal override bool AddObject(PersistentObject persistentObject, string internalContainerName, IList<PersistentObject> potentialBrokenReferences) {
            bool success;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                success = this.AddObjectToParentTable(persistentObject, internalContainerName, potentialBrokenReferences);
                if (success) {
                    this.AddObjectToSubTables(internalContainerName, persistentObject.Id, persistentObject.PersistentFieldsForCollectionsOfElements);
                    this.AddObjectFieldsForCollectionsOfPersistentObjects(internalContainerName, persistentObject.Id, persistentObject.PersistentFieldsForCollectionsOfPersistentObjects, potentialBrokenReferences);
                    this.AddOrUpdateObjectInFullTextTable(persistentObject);
                }
                transactionScope.Complete();
            }
            return success;
        }

        /// <summary>
        /// Adds an object to a container and adds all of its
        /// persistent child objects cascadedly. Changed persistent
        /// child objects are updated cascadedly.
        /// </summary>
        /// <param name="persistentObject">object to add to the
        /// container cascadedly</param>
        /// <param name="persistentContainer">persistent container to
        /// add object to cascadedly</param>
        internal override void AddObjectCascadedly(PersistentObject persistentObject, PersistentContainer persistentContainer) {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                base.AddObjectCascadedly(persistentObject, persistentContainer);
                transactionScope.Complete();
            }
            return;
        }

        /// <summary>
        /// Inserts n:m relations of a persistent object to other
        /// persistent objects into the relations table.
        /// </summary>
        /// <param name="tableName">name of parent database table</param>
        /// <param name="id">id of persistent object to insert n:m
        /// relations for</param>
        /// <param name="fields">fields to insert</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        private void AddObjectFieldsForCollectionsOfPersistentObjects(string tableName, Guid id, IEnumerable<PersistentFieldForPersistentObjectCollection> fields, IList<PersistentObject> potentialBrokenReferences) {
            foreach (var field in fields) {
                var index = 0;
                foreach (var childObject in field.GetValuesAsPersistentObject()) {
                    this.AddObjectFieldValueForCollectionOfPersistentObjects(tableName, id, field.Key, index, childObject, potentialBrokenReferences);
                    index++;
                }
            }
            return;
        }

        /// <summary>
        /// Inserts an n:m relation of a persistent object to another
        /// persistent object into the relations table.
        /// </summary>
        /// <param name="parentTable">name of parent database table</param>
        /// <param name="parentId">id of persistent object to insert
        /// n:m relation for</param>
        /// <param name="parentField">name of field of persistent
        /// object to insert n:m relation for</param>
        /// <param name="parentIndex">index position of child object
        /// in enumerable of parent object</param>
        /// <param name="childObject">persistent object to insert n:m
        /// relation to</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        private void AddObjectFieldValueForCollectionOfPersistentObjects(string parentTable, Guid parentId, string parentField, int parentIndex, PersistentObject childObject, IList<PersistentObject> potentialBrokenReferences) {
            if (null == childObject) {
                throw new PersistenceException("Field with key \"" + parentField + "\" of object with ID " + parentId.ToString() + " contains a null value which cannot be stored in persistence mechanism.");
            }
            var subTableFields = new PersistentFieldForElement[] {
                    new PersistentFieldForGuid(nameof(PersistentObject.Id), Guid.NewGuid()),
                    new PersistentFieldForString("ParentTable", parentTable),
                    new PersistentFieldForGuid("ParentID", parentId),
                    new PersistentFieldForString("ParentField", parentField),
                    new PersistentFieldForInt("ParentIndex", parentIndex),
                    new PersistentFieldForString("ChildType", childObject.Type.AssemblyQualifiedName),
                    new PersistentFieldForGuid("ChildID", childObject.Id)
                };
            if (this.db.InsertRowIntoTableIfNotDeleted(RelationalDatabase.CollectionRelationsTableName, childObject.Id, subTableFields) > 0 && childObject.IsNew) {
                potentialBrokenReferences.Add(childObject);
            }
            return;
        }

        /// <summary>
        /// Inserts fields for single elements of simple types and
        /// persistent objects into parent database table.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// insert fields for elements and fields for persistent
        /// objects for</param>
        /// <param name="tableName">name of parent database table</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        /// <returns>true if object was added to persistence
        /// mechanism successfully, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        private bool AddObjectToParentTable(PersistentObject persistentObject, string tableName, IList<PersistentObject> potentialBrokenReferences) {
            bool success;
            try {
                this.RemoveFromDeletedIDsTable(persistentObject.Id);
                var fieldsToBeInserted = new List<PersistentFieldForElement>(persistentObject.PersistentFieldsForElements);
                foreach (var persistentFieldForPersistentObject in persistentObject.PersistentFieldsForPersistentObjects) {
                    if (null != persistentFieldForPersistentObject.ValueAsObject) {
                        var childObject = persistentFieldForPersistentObject.ValueAsPersistentObject;
                        if (!this.IsIdDeleted(childObject.Id) || childObject.IsNew) {
                            if (childObject.IsNew) {
                                potentialBrokenReferences.Add(childObject);
                            }
                            fieldsToBeInserted.Add(new PersistentFieldForNullableGuid(persistentFieldForPersistentObject.Key, childObject.Id));
                            fieldsToBeInserted.Add(new PersistentFieldForString(persistentFieldForPersistentObject.Key + '_', childObject.Type.AssemblyQualifiedName));
                        }
                    }
                }
                success = this.db.InsertRowIntoTable(tableName, fieldsToBeInserted) > 0;
            } catch (DbException exception) {
                var container = this.FindContainer(persistentObject.Type);
                if (container.Contains(persistentObject.Id)) {
                    throw new ObjectNotUniqueException("Object cannot be added to container for type \""
                        + persistentObject.Type.AssemblyQualifiedName + "\", because an object with ID \""
                        + persistentObject.Id
                        + "\" is already contained in that container.", exception);
                } else if (this.GetInternalNameOfContainer(persistentObject.Type.AssemblyQualifiedName) != tableName) {
                    throw new TypeException("Object cannot be added to container for type \""
                        + this.GetAssemblyQualifiedTypeNameOfContainer(tableName)
                        + "\". Try to add object to container for type \""
                        + persistentObject.Type.AssemblyQualifiedName + "\" instead.", exception);
                } else {
                    throw;
                }
            }
            return success;
        }

        /// <summary>
        /// Inserts simple element collections into sub tables.
        /// </summary>
        /// <param name="tableName">name of parent database table</param>
        /// <param name="id">id of persistent object to insert fields
        /// for</param>
        /// <param name="fields">fields to insert into table</param>
        private void AddObjectToSubTables(string tableName, Guid id, IEnumerable<PersistentFieldForCollection> fields) {
            foreach (var field in fields) {
                var subTableFields = new PersistentFieldForElement[4];
                subTableFields[1] = new PersistentFieldForGuid("ParentID", id);
                var subTableName = RelationalDatabase.GetInternalNameOfSubTable(tableName, field.Key);
                var index = 0;
                foreach (var value in field.GetValuesAsObject()) {
                    subTableFields[0] = new PersistentFieldForGuid(nameof(PersistentObject.Id), Guid.NewGuid());
                    subTableFields[2] = new PersistentFieldForInt("ParentIndex", index);
                    subTableFields[3] = new PersistentFieldForObject("Value", field.ContentBaseType, value);
                    this.db.InsertRowIntoTable(subTableName, subTableFields);
                    index++;
                }
            }
            return;
        }

        /// <summary>
        /// Adds or updates an object in full-text table.
        /// </summary>
        /// <param name="persistentObject">persistent object ot add
        /// or update in full-text table</param>
        protected abstract void AddOrUpdateObjectInFullTextTable(PersistentObject persistentObject);

        /// <summary>
        /// Adds or updates an object in full-text table.
        /// </summary>
        /// <param name="persistentObject">persistent object ot add
        /// or update in full-text table</param>
        /// <param name="isReversedCopyRequired">indicates whether it
        /// is necessary to store a reversed copy of full-text in index</param>
        protected void AddOrUpdateObjectInFullTextTable(PersistentObject persistentObject, bool isReversedCopyRequired) {
            if (this.HasFullTextSearchEnabled && persistentObject.IsFullTextQueryable) {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                    var isFullTextRecordExistent = this.TableOrViewContainsId(RelationalDatabase.FullTextTableName, persistentObject.Id);
                    if (!isFullTextRecordExistent || persistentObject.IsChanged) {
                        var fullTextFields = new PersistentFieldForElement[2];
                        fullTextFields[0] = new PersistentFieldForGuid(nameof(PersistentObject.Id), persistentObject.Id);
                        var fullText = persistentObject.GetFullTextCascadedly();
                        if (isReversedCopyRequired && string.Empty != fullText) {
                            var fullTextBuilder = new StringBuilder(fullText.Length * 2 + 1);
                            fullTextBuilder.Append(fullText);
                            fullTextBuilder.Append(' ');
                            for (var i = fullText.Length - 1; i > -1; i--) { // append reversed full-text
                                fullTextBuilder.Append(fullText[i]);
                            }
                            fullText = fullTextBuilder.ToString();
                        }
                        fullTextFields[1] = new PersistentFieldForString("T", fullText);
                        if (isFullTextRecordExistent) {
                            var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, persistentObject.Id);
                            this.db.UpdateTableRow(RelationalDatabase.FullTextTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection(), fullTextFields);
                        } else {
                            this.db.InsertRowIntoTable(RelationalDatabase.FullTextTableName, fullTextFields);
                        }

                    }
                    transactionScope.Complete();
                }
            }
            return;
        }

        /// <summary>
        /// Adds an ID to the table of deleted IDs.
        /// </summary>
        /// <param name="id">ID to add to the table of deleted IDs</param>
        private void AddToDeletedIDsTable(Guid id) {
            var fields = new PersistentFieldForElement[] {
                new PersistentFieldForGuid(nameof(PersistentObject.Id), id),
                new PersistentFieldForDateTime("DeletedAt", UtcDateTime.Now),
                new PersistentFieldForIUser("DeletedBy", this.UserDirectory.CurrentUser)
            };
            this.db.InsertRowIntoTableIfNotDeleted(RelationalDatabase.DeletedIDsTableName, id, fields);
            return;
        }

        /// <summary>
        /// Determines whether a container contains a specific
        /// object - including containers of sub types. Permissions
        /// are ignored.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="id">specific ID to look for</param>
        /// <returns>true if specific ID is contained, false
        /// otherwise</returns>
        internal override bool ContainsID(string internalContainerName, Guid id) {
            var viewName = RelationalDatabase.GetViewNameForContainer(internalContainerName);
            return this.TableOrViewContainsId(viewName, id);
        }

        /// <summary>
        /// Checks the existence of references to a persistent object
        /// regardless of permissions.
        /// </summary>
        /// <param name="persistentObject">persistent object to check
        /// existence of references to for</param>
        /// <returns>true if references to persistent object exist,
        /// false otherwise</returns>
        internal override bool ContainsReferencingPersistentObjectsTo(PersistentObject persistentObject) {
            var hasReferencingPersistentObjects = false;
            var filterCriteria = new FilterCriteria("ChildID", RelationalOperator.IsEqualTo, persistentObject.Id)
                .And("ParentID", RelationalOperator.IsNotEqualTo, persistentObject.Id);
            RelationalDatabaseConnector<TConnection>.SelectedRowsAction selectedRowsAction = delegate (DbDataReader dataReader) {
                if (dataReader.Read()) {
                    hasReferencingPersistentObjects = true;
                }
                return;
            };
            this.db.SelectRowsFromTable(RelationalDatabase.CollectionRelationsTableName, "ParentID", SelectionMode.SelectAllResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, 1L, selectedRowsAction);
            if (!hasReferencingPersistentObjects) {
                if (persistentObject is AllowedGroups allowedGroups) { // ignore circular references of allowed groups
                    var groups = new List<Group>(allowedGroups.ForReading.Count + allowedGroups.ForWriting.Count);
                    groups.AddRange(allowedGroups.ForReading);
                    groups.AddRange(allowedGroups.ForWriting);
                    var circularIds = new List<Guid>(groups.Count);
                    foreach (var group in groups) {
                        if (null != group && !circularIds.Contains(group.Id)) {
                            var isGroupCircular = true;
                            foreach (var referencingIdContainerPairToGroup in this.GetReferencingIdContainerPairsTo(group)) {
                                if (referencingIdContainerPairToGroup.Key != allowedGroups.Id) {
                                    isGroupCircular = false;
                                    break;
                                }
                            }
                            if (isGroupCircular) {
                                circularIds.Add(group.Id);
                            }
                        }
                    }
                    selectedRowsAction = delegate (DbDataReader dataReader) {
                        while (dataReader.Read()) {
                            var parentId = dataReader.GetGuid(0);
                            if (!circularIds.Contains(parentId)) {
                                hasReferencingPersistentObjects = true;
                                break;
                            }
                        }
                        return;
                    };
                }
                var viewName = RelationalDatabase.GetViewNameForSingleRelations(persistentObject.ParentPersistentContainer.InternalName);
                this.db.SelectRowsFromTable(viewName, "ParentID", SelectionMode.SelectAllResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, ulong.MaxValue, selectedRowsAction);
            }
            return hasReferencingPersistentObjects;
        }

        /// <summary>
        /// Counts objects of a certain type in persistence mechanism
        /// - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <returns>pairs of group and number of objects of group</returns>
        /// <typeparam name="T">type of persistent objects to count</typeparam>
        internal override int CountObjects<T>(string internalContainerName, FilterCriteria filterCriteria) {
            var viewName = RelationalDatabase.GetViewNameForContainer(internalContainerName);
            filterCriteria = this.AddFilterCriteriaForReadPermissions(filterCriteria);
            var sampleObject = this.CreateInstance<T>();
            var relationalJoins = new RelationalJoinBuilder(filterCriteria, SortCriterionCollection.Empty, this.GetInternalNameOfContainer, sampleObject).ToRelationalJoins();
            return this.db.CountTableRows(viewName, filterCriteria, relationalJoins);
        }

        /// <summary>
        /// Counts objects of a certain type in persistence mechanism
        /// - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="groupBy">field names to group table rows by</param>
        /// <returns>pairs of group and number of objects of group</returns>
        /// <typeparam name="T">type of persistent objects to count</typeparam>
        internal override IDictionary<string[], int> CountObjects<T>(string internalContainerName, FilterCriteria filterCriteria, string[] groupBy) {
            var viewName = RelationalDatabase.GetViewNameForContainer(internalContainerName);
            filterCriteria = this.AddFilterCriteriaForReadPermissions(filterCriteria);
            var sampleObject = this.CreateInstance<T>();
            var relationalJoins = new RelationalJoinBuilder(filterCriteria, SortCriterionCollection.Empty, this.GetInternalNameOfContainer, sampleObject).ToRelationalJoins();
            return this.db.CountTableRows(viewName, filterCriteria, relationalJoins, groupBy);
        }

        /// <summary>
        /// Gets all matching persistent objects from persistence
        /// mechanism - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="fullTextQuery">full-text query to select
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" ist the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <returns>all matching objects from persistence mechanism</returns>
        /// <typeparam name="T">type of persistent object to find</typeparam>
        internal override ReadOnlyCollection<T> Find<T>(string internalContainerName, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            return this.Find<T>(internalContainerName, fullTextQuery, filterCriteria, sortCriteria, startPosition, maxResults, SelectionMode.SelectAllResults);
        }

        /// <summary>
        /// Gets all matching persistent objects from persistence
        /// mechanism - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="fullTextQuery">full-text query to select
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" ist the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <param name="selectionMode">selection mode to apply</param>
        /// <returns>all matching objects from persistence mechanism</returns>
        /// <typeparam name="T">type of persistent object to find</typeparam>
        private ReadOnlyCollection<T> Find<T>(string internalContainerName, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, SelectionMode selectionMode) where T : PersistentObject, new() {
            var results = new List<T>();
            var viewName = RelationalDatabase.GetViewNameForContainer(internalContainerName);
            var sampleObject = this.CreateInstance<T>();
            var relationalJoins = new RelationalJoinBuilder(filterCriteria, sortCriteria, this.GetInternalNameOfContainer, sampleObject).ToRelationalJoins();
            var fieldNames = new List<string>(2) {
                nameof(PersistentObject.Id),
                nameof(AllowedGroups),
                "TypeName"
            };
            var areRequestedColumnsAllColumns = true;
            if (relationalJoins.Count > 0) {
                foreach (var sortCriterion in sortCriteria) {
                    if (!fieldNames.Contains(sortCriterion.FieldName)) {
                        fieldNames.Add(sortCriterion.FieldName);
                        areRequestedColumnsAllColumns = false;
                    }
                }
            }
            var relations = new List<Relation>(); // child objects are not created directly to avoid nested transactions
            var allowedGroupsForReadingCacheForCurrentUser = new Dictionary<Guid, bool>();
            this.db.SelectRowsFromTable(viewName, fieldNames, selectionMode, fullTextQuery, filterCriteria, sortCriteria, relationalJoins, startPosition, maxResults, delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    relations.Add(new Relation() {
                        ChildId = dataReader.GetGuid(0),
                        ChildAllowedGroupsId = Relation.GetNullableGuid(dataReader, 1),
                        ChildType = dataReader.GetString(2)
                    });
                }
                return;
            });
            this.PreloadGroupsForReadingForCurrentUser(relations, allowedGroupsForReadingCacheForCurrentUser);
            foreach (var relation in relations) {
                if (this.IsCurrentUserInGroupsForReadingOfAllowedGroupsWithId(relation.ChildAllowedGroupsId, allowedGroupsForReadingCacheForCurrentUser)) {
                    var instance = this.GetInstance(relation.ChildId, relation.ChildType) as T;
                    if (areRequestedColumnsAllColumns || !results.Contains(instance)) { // DISTINCT might not work correctly if additional columns were added for sorting purposes
                        results.Add(instance);
                    }
                }
            }
            return results.AsReadOnly();
        }

        /// <summary>
        /// Finds the aggregated values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="fieldNames">specific set of properties to
        /// find aggregated values for</param>
        /// <param name="selectionMode">selection mode to apply</param>
        /// <returns>aggregated values of specific properties</returns>
        /// <typeparam name="T">type of persistent object to find
        /// aggregated values for</typeparam>
        private ReadOnlyCollection<object> FindAggregatedValues<T>(string internalContainerName, FilterCriteria filterCriteria, string[] fieldNames, SelectionMode selectionMode) where T : PersistentObject, new() {
            var aggregatedValues = new object[fieldNames.LongLength];
            var viewName = RelationalDatabase.GetViewNameForContainer(internalContainerName);
            filterCriteria = this.AddFilterCriteriaForReadPermissions(filterCriteria);
            var sampleObject = this.CreateInstance<T>();
            var relationalJoinBuilder = new RelationalJoinBuilder(filterCriteria, SortCriterionCollection.Empty, this.GetInternalNameOfContainer, sampleObject);
            foreach (var fieldName in fieldNames) {
                relationalJoinBuilder.AdditionalFieldNames.Add(fieldName);
            }
            var relationalJoins = relationalJoinBuilder.ToRelationalJoins();
            this.db.SelectRowsFromTable(viewName, fieldNames, selectionMode, null, filterCriteria, SortCriterionCollection.Empty, relationalJoins, 0, ulong.MaxValue, delegate (DbDataReader dataReader) {
                if (dataReader.Read()) {
                    dataReader.GetValues(aggregatedValues);
                }
                return;
            });
            return new List<object>(aggregatedValues).AsReadOnly();
        }

        /// <summary>
        /// Finds the average values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="fieldNames">specific set of properties to
        /// find average values for</param>
        /// <returns>average values of specific properties</returns>
        /// <typeparam name="T">type of persistent object to find
        /// average values for</typeparam>
        internal override ReadOnlyCollection<object> FindAverageValues<T>(string internalContainerName, FilterCriteria filterCriteria, string[] fieldNames) {
            return this.FindAggregatedValues<T>(internalContainerName, filterCriteria, fieldNames, SelectionMode.SelectAverageResults);
        }

        /// <summary>
        /// Finds the complement set of all matching persistent
        /// objects from this container - including containers of sub
        /// types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="fullTextQuery">full-text query to select
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" ist the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <returns>matching persistent objects from persistence
        /// mechanism</returns>
        /// <typeparam name="T">type of persistent object to find</typeparam>
        internal override ReadOnlyCollection<T> FindComplement<T>(string internalContainerName, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            return this.Find<T>(internalContainerName, fullTextQuery, filterCriteria, sortCriteria, startPosition, maxResults, SelectionMode.SelectComplementResults);
        }

        /// <summary>
        /// Finds all distinct combinations of a specific set of
        /// properties - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort the distinct
        /// result combinations by</param>
        /// <param name="fieldNames">specific set of properties to
        /// find all distinct combinations for</param>
        /// <returns>all distinct combinations of a specific set of
        /// properties</returns>
        /// <typeparam name="T">type of persistent object to find
        /// distinct values for</typeparam>
        internal override ReadOnlyCollection<object[]> FindDistinctValues<T>(string internalContainerName, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string[] fieldNames) {
            var distinctValues = new List<object[]>();
            var viewName = RelationalDatabase.GetViewNameForContainer(internalContainerName);
            filterCriteria = this.AddFilterCriteriaForReadPermissions(filterCriteria);
            var sampleObject = this.CreateInstance<T>();
            var relationalJoinBuilder = new RelationalJoinBuilder(filterCriteria, sortCriteria, this.GetInternalNameOfContainer, sampleObject);
            foreach (var fieldName in fieldNames) {
                relationalJoinBuilder.AdditionalFieldNames.Add(fieldName);
            }
            var relationalJoins = relationalJoinBuilder.ToRelationalJoins();
            var allFieldNames = new List<string>(fieldNames);
            var areRequestedColumnsAllColumns = true;
            if (relationalJoins.Count > 0) {
                foreach (var sortCriterion in sortCriteria) {
                    if (!allFieldNames.Contains(sortCriterion.FieldName)) {
                        allFieldNames.Add(sortCriterion.FieldName);
                        areRequestedColumnsAllColumns = false;
                    }
                }
            }
            this.db.SelectRowsFromTable(viewName, allFieldNames, SelectionMode.SelectDistinctResults, null, filterCriteria, sortCriteria, relationalJoins, 0, ulong.MaxValue, delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    var allColumns = new object[allFieldNames.Count];
                    dataReader.GetValues(allColumns);
                    if (areRequestedColumnsAllColumns) {
                        distinctValues.Add(allColumns);
                    } else { // DISTINCT might not work correctly if additional columns were added for sorting purposes
                        var requestedColumns = new object[fieldNames.Length];
                        Array.Copy(allColumns, 0, requestedColumns, 0, fieldNames.Length);
                        var isContained = false;
                        foreach (var distinctValue in distinctValues) {
                            var areArraysEqual = true;
                            for (var i = 0; i < fieldNames.Length; i++) {
                                var a = distinctValue[i];
                                var b = requestedColumns[i];
                                if ((a == null && b != null) || (a != null && !a.Equals(b))) {
                                    areArraysEqual = false;
                                    break;
                                }
                            }
                            if (areArraysEqual) {
                                isContained = true;
                                break;
                            }
                        }
                        if (!isContained) {
                            distinctValues.Add(requestedColumns);
                        }
                    }
                }
                return;
            });
            return distinctValues.AsReadOnly();
        }

        /// <summary>
        /// Finds the sum of values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="fieldNames">specific set of properties to
        /// find sums of values for</param>
        /// <returns>sums of values of specific properties</returns>
        /// <typeparam name="T">type of persistent object to find
        /// sums of values for</typeparam>
        internal override ReadOnlyCollection<object> FindSumsOfValues<T>(string internalContainerName, FilterCriteria filterCriteria, string[] fieldNames) {
            return this.FindAggregatedValues<T>(internalContainerName, filterCriteria, fieldNames, SelectionMode.SelectSumsOfResults);
        }

        /// <summary>
        /// Gets filter criteria for IDs of persistent objects.
        /// </summary>
        /// <param name="persistentObjects">persistent objects to get
        /// ID filter criteria for</param>
        /// <param name="key">key of field to filter by</param>
        /// <returns>filter criteria for IDs of persistent objects</returns>
        private static FilterCriteria GetFilterCriteriaForIds(IEnumerable<PersistentObject> persistentObjects, string key) {
            FilterCriteria filterCriteria = null;
            foreach (var persistentObject in persistentObjects) {
                if (null != persistentObject) {
                    if (null == filterCriteria) {
                        filterCriteria = new FilterCriteria(key, RelationalOperator.IsEqualTo, persistentObject.Id);
                    } else {
                        filterCriteria = filterCriteria.Or(key, RelationalOperator.IsEqualTo, persistentObject.Id);
                    }
                }
            }
            return filterCriteria;
        }

        /// <summary>
        /// Creates a new instance of a persistent object of a type
        /// with the specified ID.
        /// </summary>
        /// <param name="id">ID to create object for</param>
        /// <param name="assemblyQualifiedName">assembly qualified
        /// type name of object to create</param>
        /// <returns>new instance of a persistent object of type with
        /// the specified ID</returns>
        private PersistentObject GetInstance(Guid id, string assemblyQualifiedName) {
            PersistentObject persistentObject;
            if (this.ObjectCache.Contains(id)) {
                persistentObject = this.ObjectCache[id];
            } else {
                var persistentContainer = this.FindContainer(assemblyQualifiedName);
                persistentObject = persistentContainer.CreateInstance(id);
                this.ObjectCache.Add(persistentObject);
            }
            return persistentObject;
        }

        /// <summary>
        /// Gets unique keys for parent fields.
        /// </summary>
        /// <param name="keyChains">key chains to unique parent keys
        /// for</param>
        /// <returns>unique keys for parent fields</returns>
        private static IList<string> GetKeysForParentFields(ICollection<string[]> keyChains) {
            var keys = new List<string>(keyChains.Count);
            foreach (var keyChain in keyChains) {
                var key = keyChain[0];
                if (!keys.Contains(key)) {
                    keys.Add(key);
                }
            }
            return keys;
        }

        /// <summary>
        /// Gets all persistent objects to be preloaded of an
        /// enumerable of persistent object that are potentially
        /// supposed to be preloaded.
        /// </summary>
        /// <param name="persistentObjects">persistent objects to be
        /// preloaded potentially</param>
        /// <param name="keys">keys of persistent objects to preload
        /// values for</param>
        /// <param name="getPersistentFieldForKeyDelegate">delegate
        /// for getting a specific persistent field of a persistent
        /// object</param>
        /// <returns>persistent objects to be preloaded</returns>
        private static PersistentObjectCache GetPersistentObjectsToBePreloaded(IEnumerable<PersistentObject> persistentObjects, ICollection<string> keys, GetPersistentFieldForKeyDelegate getPersistentFieldForKeyDelegate) {
            var persistentObjectsToBePreloaded = new PersistentObjectCache();
            foreach (var persistentObject in persistentObjects) {
                if (null != persistentObject && !persistentObjectsToBePreloaded.Contains(persistentObject.Id)) {
                    var isRetrievedPartially = false;
                    foreach (var key in keys) {
                        if (nameof(PersistentObject.Id) != key && getPersistentFieldForKeyDelegate(persistentObject, key).IsRetrieved) {
                            isRetrievedPartially = true;
                            break;
                        }
                    }
                    if (!isRetrievedPartially) {
                        persistentObjectsToBePreloaded.Add(persistentObject);
                    }
                }
            }
            return persistentObjectsToBePreloaded;
        }

        /// <summary>
        /// Gets a dictionary of IDs and internal container names of
        /// all references to a persistent object regardless of
        /// permissions.
        /// </summary>
        /// <param name="persistentObject">persistent object to get
        /// references to for</param>
        /// <returns>dictionary of IDs and internal container names
        /// of all references to a persistent object</returns>
        private IDictionary<Guid, string> GetReferencingIdContainerPairsTo(PersistentObject persistentObject) {
            var idTypePairs = new Dictionary<Guid, string>();
            var filterCriteria = new FilterCriteria("ChildID", RelationalOperator.IsEqualTo, persistentObject.Id)
                .And("ParentID", RelationalOperator.IsNotEqualTo, persistentObject.Id);
            RelationalDatabaseConnector<TConnection>.SelectedRowsAction selectedRowsAction = delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    var internalContainerName = dataReader.GetString(0);
                    var id = dataReader.GetGuid(1);
                    if (!idTypePairs.ContainsKey(id)) {
                        idTypePairs.Add(id, internalContainerName);
                    }
                }
                return;
            };
            this.db.SelectRowsFromTable(RelationalDatabase.CollectionRelationsTableName, new string[] { "ParentTable", "ParentID" }, SelectionMode.SelectDistinctResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, ulong.MaxValue, selectedRowsAction);
            var relationsViewName = RelationalDatabase.GetViewNameForSingleRelations(persistentObject.ParentPersistentContainer.InternalName);
            this.db.SelectRowsFromTable(relationsViewName, new string[] { "ParentTable", "ParentID" }, SelectionMode.SelectDistinctResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, ulong.MaxValue, selectedRowsAction);
            return idTypePairs;
        }

        /// <summary>
        /// Gets all references to a persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to get
        /// references to for</param>
        /// <returns>all references to persistent object</returns>
        internal override ReadOnlyCollection<PersistentObject> GetReferencingPersistentObjectsTo(PersistentObject persistentObject) {
            var referencingIdContainerPairs = this.GetReferencingIdContainerPairsTo(persistentObject);
            var references = new List<PersistentObject>(referencingIdContainerPairs.Count);
            foreach (var referencingIdContainerPair in referencingIdContainerPairs) {
                references.Add(this.GetInstance(referencingIdContainerPair.Key, this.GetAssemblyQualifiedTypeNameOfContainer(referencingIdContainerPair.Value)));
            }
            return references.AsReadOnly();
        }

        /// <summary>
        /// Gets a list of pairs of name of database table as key and
        /// name of table field as value for all combinations of
        /// table/field which contain n:1 relations to a specific
        /// persistent object.
        /// </summary>
        /// <param name="internalContainerName">name of parent
        /// database table of persistent object to find relations to
        /// for</param>
        /// <param name="id">ID of persistent object to find
        /// relations to for</param>
        /// <returns>list if pairs of name of database table as key
        /// and name of table field as value for all combinations of
        /// table/field which contain n:1 relations to specific
        /// persistent object</returns>
        private IEnumerable<KeyValuePair<string, string>> GetTableFieldPairsForSingleRelations(string internalContainerName, Guid id) {
            var tableFieldPairs = new List<KeyValuePair<string, string>>();
            var relationsViewName = RelationalDatabase.GetViewNameForSingleRelations(internalContainerName);
            var filterCriteria = new FilterCriteria("ChildID", RelationalOperator.IsEqualTo, id);
            this.db.SelectRowsFromTable(relationsViewName, new string[] { "ParentTable", "ParentField" }, SelectionMode.SelectDistinctResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    tableFieldPairs.Add(new KeyValuePair<string, string>(dataReader.GetString(0), dataReader.GetString(1)));
                }
                return;
            });
            return tableFieldPairs;
        }

        /// <summary>
        /// Returns true if current user is in groups for reading of
        /// allowed groups with a specific ID. If checking of
        /// permissions is enabled this method does not use any cache
        /// and will cause a database call.
        /// </summary>
        /// <param name="allowedGroupsId">ID of allowed groups to
        /// check user read permissions in</param>
        /// <returns>true if current user is in groups for reading of
        /// allowed groups with specific ID, false otherwise</returns>
        private bool IsCurrentUserInGroupsForReadingOfAllowedGroupsWithId(Guid? allowedGroupsId) {
            bool isCurrentUserInGroupsForReading;
            if (SecurityModel.ApplyPermissions == this.SecurityModel) {
                if (allowedGroupsId.HasValue) {
                    var filterCriteria = new FilterCriteria("ParentID", RelationalOperator.IsEqualTo, allowedGroupsId.Value)
                        .And("ParentField", RelationalOperator.IsEqualTo, "ForReading", FilterTarget.IsOtherTextValue)
                        .And(new FilterCriteria("r1.Value", RelationalOperator.IsEqualTo, Guid.Empty).Or("r1.Value", RelationalOperator.IsEqualTo, this.UserDirectory.CurrentUser.Id));
                    var relationalJoin = new RelationalJoin(this.GroupMembersTableName, "r1") {
                        FullFieldName = "r1"
                    };
                    relationalJoin.FieldPredicates.Add(RelationalJoin.RootTableAlias + ".ChildID", "r1.ParentID");
                    isCurrentUserInGroupsForReading = this.db.CountTableRows(RelationalDatabase.CollectionRelationsTableName, filterCriteria, new RelationalJoin[] { relationalJoin }) > 0;
                } else {
                    isCurrentUserInGroupsForReading = false;
                }
            } else if (SecurityModel.IgnorePermissions == this.SecurityModel) {
                isCurrentUserInGroupsForReading = true;
            } else {
                throw new PersistenceException("Security model \"" + this.SecurityModel.ToString() + "\" is not supported.");
            }
            return isCurrentUserInGroupsForReading;
        }

        /// <summary>
        /// Returns true if current user is in groups for reading of
        /// allowed groups with a specific ID. If checking of
        /// permissions is enabled this method uses a cache and will
        /// only cause a database call if the required information
        /// is not cached yet.
        /// </summary>
        /// <param name="allowedGroupsId">ID of allowed groups to
        /// check user read permissions in</param>
        /// <param name="allowedGroupsForReadingCacheForCurrentUser">
        /// in-mermory cache with ID of allowed groups as key and
        /// boolean value indicating whether current user has read
        /// permissions as value - this cache should not be used
        /// accross multiple requests because it might be outdated
        /// soon</param>
        /// <returns>true if current user is in groups for reading of
        /// allowed groups with specific ID, false otherwise</returns>
        private bool IsCurrentUserInGroupsForReadingOfAllowedGroupsWithId(Guid? allowedGroupsId, Dictionary<Guid, bool> allowedGroupsForReadingCacheForCurrentUser) {
            bool isCurrentUserInGroupsForReading;
            if (SecurityModel.ApplyPermissions == this.SecurityModel && allowedGroupsId.HasValue) {
                if (!allowedGroupsForReadingCacheForCurrentUser.TryGetValue(allowedGroupsId.Value, out isCurrentUserInGroupsForReading)) {
                    isCurrentUserInGroupsForReading = this.IsCurrentUserInGroupsForReadingOfAllowedGroupsWithId(allowedGroupsId);
                    allowedGroupsForReadingCacheForCurrentUser.Add(allowedGroupsId.Value, isCurrentUserInGroupsForReading);
                }
            } else {
                isCurrentUserInGroupsForReading = this.IsCurrentUserInGroupsForReadingOfAllowedGroupsWithId(allowedGroupsId);
            }
            return isCurrentUserInGroupsForReading;
        }

        /// <summary>
        /// Determines whether an object with a specific ID is
        /// deleted. Permissions are ignored.
        /// </summary>
        /// <param name="id">specific ID to look for</param>
        /// <returns>true if object with specific ID is deleted,
        /// false otherwise</returns>
        public sealed override bool IsIdDeleted(Guid id) {
            return this.TableOrViewContainsId(RelationalDatabase.DeletedIDsTableName, id);
        }

        /// <summary>
        /// Preloads the allowed groups for reading of relations for
        /// current user into cache.
        /// </summary>
        /// <param name="relations">relations with allowed groups to
        /// preload read permissions for current user for</param>
        /// <param name="allowedGroupsForReadingCacheForCurrentUser">
        /// in-mermory cache to preload data into with ID of allowed
        /// groups as key and boolean value indicating whether
        /// current user has read permissions as value</param>
        private void PreloadGroupsForReadingForCurrentUser(IEnumerable<Relation> relations, Dictionary<Guid, bool> allowedGroupsForReadingCacheForCurrentUser) {
            if (SecurityModel.ApplyPermissions == this.SecurityModel) {
                FilterCriteria idFilterCriteria = null;
                foreach (var relation in relations) {
                    if (relation.ChildAllowedGroupsId.HasValue && !allowedGroupsForReadingCacheForCurrentUser.ContainsKey(relation.ChildAllowedGroupsId.Value)) {
                        if (null == idFilterCriteria) {
                            idFilterCriteria = new FilterCriteria("ParentID", RelationalOperator.IsEqualTo, relation.ChildAllowedGroupsId.Value);
                        } else {
                            idFilterCriteria = idFilterCriteria.Or("ParentID", RelationalOperator.IsEqualTo, relation.ChildAllowedGroupsId.Value);
                        }
                        allowedGroupsForReadingCacheForCurrentUser.Add(relation.ChildAllowedGroupsId.Value, false); // will be set to true later in this method if applicable
                    }
                }
                if (null != idFilterCriteria) {
                    var filterCriteria = new FilterCriteria(idFilterCriteria)
                        .And("ParentField", RelationalOperator.IsEqualTo, "ForReading", FilterTarget.IsOtherTextValue)
                        .And(new FilterCriteria("r1.Value", RelationalOperator.IsEqualTo, Guid.Empty).Or("r1.Value", RelationalOperator.IsEqualTo, this.UserDirectory.CurrentUser.Id));
                    var relationalJoin = new RelationalJoin(this.GroupMembersTableName, "r1") {
                        FullFieldName = "r1"
                    };
                    relationalJoin.FieldPredicates.Add(RelationalJoin.RootTableAlias + ".ChildID", "r1.ParentID");
                    this.db.SelectRowsFromTable(RelationalDatabase.CollectionRelationsTableName, "ParentID", SelectionMode.SelectDistinctResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[] { relationalJoin }, 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                        while (dataReader.Read()) {
                            allowedGroupsForReadingCacheForCurrentUser[dataReader.GetGuid(0)] = true; // only IDs of matching allowed groups are returned
                        }
                        return;
                    });
                }
            }
            return;
        }

        /// <summary>
        /// Preloads the state of all fields for collections of
        /// elements of persistent objects from persistence
        /// mechanism.
        /// </summary>
        /// <param name="persistentObjects">persistent objects to
        /// preload fields for collections of elements for</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="keys">keys of fields to preload</param>
        private void PreloadFieldsForCollectionsOfElements(IEnumerable<PersistentObject> persistentObjects, string internalContainerName, ICollection<string> keys) {
            if (keys.Count > 0) {
                var persistentObjectsToBePreloaded = RelationalDatabase<TConnection>.GetPersistentObjectsToBePreloaded(persistentObjects, keys, delegate (PersistentObject persistentObject, string key) {
                    return persistentObject.GetPersistentFieldForCollectionOfElements(key);
                });
                var filterCriteria = RelationalDatabase<TConnection>.GetFilterCriteriaForIds(persistentObjectsToBePreloaded, "ParentID");
                if (null != filterCriteria) {
                    var sortCriteria = new SortCriterionCollection() {
                        { "ParentID", SortDirection.Ascending },
                        { "ParentIndex", SortDirection.Ascending }
                    };
                    foreach (var key in keys) {
                        foreach (var persistentObjectToBePreloaded in persistentObjectsToBePreloaded) {
                            persistentObjectToBePreloaded.GetPersistentFieldForCollectionOfElements(key).InitializeTemporaryCollectionForDbDataReader();
                        }
                        var viewName = RelationalDatabase.GetViewNameForSubTable(internalContainerName, key);
                        this.db.SelectRowsFromTable(viewName, new string[] { "Value", "ParentID" }, SelectionMode.SelectAllResults, null, filterCriteria, sortCriteria, new RelationalJoin[0], 0, ulong.MaxValue, delegate (DbDataReader dataReader) {
                            var persistentObjectToBePreloaded = new PersistentObject();
                            PersistentFieldForCollection persistentField = null;
                            while (dataReader.Read()) {
                                var id = dataReader.GetGuid(1);
                                if (persistentObjectToBePreloaded.Id != id) {
                                    persistentObjectToBePreloaded = persistentObjectsToBePreloaded[id];
                                    persistentField = persistentObjectToBePreloaded.GetPersistentFieldForCollectionOfElements(key);
                                }
                                persistentField.LoadValueFromDbDataReader(dataReader);
                            }
                            return;
                        });
                        foreach (var persistentObjectToBePreloaded in persistentObjectsToBePreloaded) {
                            var persistentField = persistentObjectToBePreloaded.GetPersistentFieldForCollectionOfElements(key);
                            persistentField.IsRetrieved = true;
                            persistentField.SetValuesFromDbDataReader();
                            persistentField.SetIsChangedToFalse();
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Preloads the state of all fields for collections of
        /// persistent objects of persistent objects from persistence
        /// mechanism.
        /// </summary>
        /// <param name="persistentObjects">persistent objects to
        /// preload fields for collections of persistent objects for</param>
        /// <param name="keyChains">key chains of fields to preload</param>
        /// <param name="allowedGroupsForReadingCacheForCurrentUser">
        /// in-mermory cache with ID of allowed groups as key and
        /// boolean value indicating whether current user has read
        /// permissions as value - this cache should not be used
        /// accross multiple requests because it might be outdated
        /// soon</param>
        private void PreloadFieldsForCollectionsOfPersistentObjects(IEnumerable<PersistentObject> persistentObjects, ICollection<string[]> keyChains, Dictionary<Guid, bool> allowedGroupsForReadingCacheForCurrentUser) {
            if (keyChains.Count > 0) {
                var keysForParentFields = RelationalDatabase<TConnection>.GetKeysForParentFields(keyChains);
                var persistentObjectsToBePreloaded = RelationalDatabase<TConnection>.GetPersistentObjectsToBePreloaded(persistentObjects, keysForParentFields, delegate (PersistentObject persistentObject, string key) {
                    return persistentObject.GetPersistentFieldForCollectionOfPersistentObjects(key);
                });
                var idFilterCriteria = RelationalDatabase<TConnection>.GetFilterCriteriaForIds(persistentObjectsToBePreloaded, "ParentID");
                if (null != idFilterCriteria) {
                    try {
                        foreach (var persistentObjectToBePreloaded in persistentObjectsToBePreloaded) {
                            foreach (var keyForParentField in keysForParentFields) {
                                var persistentField = persistentObjectToBePreloaded.GetPersistentFieldForCollectionOfPersistentObjects(keyForParentField);
                                persistentField.IsRetrieved = true;
                                persistentField.InitialValues.Clear();
                                persistentField.Clear();
                                persistentField.SetIsChangedToFalse();
                            }
                        }
                        foreach (var keyForParentField in keysForParentFields) {
                            var samplePersistentField = persistentObjectsToBePreloaded[0].GetPersistentFieldForCollectionOfPersistentObjects(keyForParentField);
                            var childContainerName = this.GetInternalNameOfContainer(samplePersistentField.GetContentType());
                            childContainerName = RelationalDatabase.GetViewNameForContainer(childContainerName);
                            var columnNames = new string[] {
                                "ParentID",
                                "ChildID",
                                "r1." + nameof(PersistentObject.AllowedGroups),
                                "r1.TypeName",
                                "ParentIndex"
                            };
                            var filterCriteria = new FilterCriteria("ParentField", RelationalOperator.IsEqualTo, keyForParentField, FilterTarget.IsOtherTextValue).And(idFilterCriteria);
                            var sortCriteria = new SortCriterionCollection("ParentIndex", SortDirection.Ascending);
                            var relationalJoin = new RelationalJoin(childContainerName, "r1") {
                                FullFieldName = "r1"
                            };
                            relationalJoin.FieldPredicates.Add(RelationalJoin.RootTableAlias + '.' + "ChildID", "r1." + nameof(PersistentObject.Id));
                            var relations = new List<Relation>(); // child objects are not created directly to avoid nested transactions
                            this.db.SelectRowsFromTable(RelationalDatabase.CollectionRelationsTableName, columnNames, SelectionMode.SelectAllResults, null, filterCriteria, sortCriteria, new RelationalJoin[] { relationalJoin }, 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                                while (dataReader.Read()) {
                                    relations.Add(new Relation() {
                                        ParentId = dataReader.GetGuid(0),
                                        ChildId = dataReader.GetGuid(1),
                                        ChildAllowedGroupsId = Relation.GetNullableGuid(dataReader, 2),
                                        ChildType = dataReader.GetString(3)
                                    });
                                }
                                return;
                            });
                            this.PreloadGroupsForReadingForCurrentUser(relations, allowedGroupsForReadingCacheForCurrentUser);
                            foreach (var relation in relations) {
                                if (this.IsCurrentUserInGroupsForReadingOfAllowedGroupsWithId(relation.ChildAllowedGroupsId, allowedGroupsForReadingCacheForCurrentUser)) {
                                    var persistentObjectToBePreloaded = persistentObjectsToBePreloaded[relation.ParentId];
                                    var child = this.GetInstance(relation.ChildId, relation.ChildType);
                                    var persistentField = persistentObjectToBePreloaded.GetPersistentFieldForCollectionOfPersistentObjects(keyForParentField);
                                    persistentField.InitialValues.Add(child);
                                    persistentField.AddObject(child);
                                    persistentField.SetIsChangedToFalse();
                                }
                            }
                        }
                    } catch (Exception) {
                        foreach (var persistentObjectToBePreloaded in persistentObjectsToBePreloaded) {
                            foreach (var keyChain in keyChains) {
                                persistentObjectToBePreloaded.GetPersistentFieldForCollectionOfPersistentObjects(keyChain[0]).IsRetrieved = false;
                            }
                        }
                        throw;
                    }
                }
                foreach (var keyForParentField in keysForParentFields) {
                    var childKeyChains = new List<string[]>();
                    foreach (var keyChain in keyChains) {
                        if (keyForParentField == keyChain[0] && keyChain.LongLength > 1) {
                            childKeyChains.Add(KeyChain.RemoveFirstLinkOf(keyChain));
                        }
                    }
                    if (childKeyChains.Count > 0) {
                        var childObjects = new List<PersistentObject>();
                        Type contentType = null;
                        foreach (var persistentObject in persistentObjects) {
                            if (null != persistentObject) {
                                var persistentField = persistentObject.GetPersistentFieldForCollectionOfPersistentObjects(keyForParentField);
                                foreach (var childObject in persistentField.GetValuesAsPersistentObject()) {
                                    childObjects.Add(childObject);
                                }
                                if (null == contentType) {
                                    contentType = persistentField.GetContentType();
                                }
                            }
                        }
                        if (null != contentType) {
                            this.PreloadObjects(contentType, childObjects, childKeyChains, allowedGroupsForReadingCacheForCurrentUser);
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Preloads the state of all fields for elements of
        /// persistent objects from persistence mechanism.
        /// </summary>
        /// <param name="persistentObjects">persistent objects to
        /// preload fields for elements for</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="keys">keys of fields to preload</param>
        private void PreloadFieldsForElements(IEnumerable<PersistentObject> persistentObjects, string internalContainerName, ICollection<string> keys) {
            if (keys.Count > 0) {
                var persistentObjectsToBePreloaded = RelationalDatabase<TConnection>.GetPersistentObjectsToBePreloaded(persistentObjects, keys, delegate (PersistentObject persistentObject, string key) {
                    return persistentObject.GetPersistentFieldForElement(key);
                });
                var filterCriteria = RelationalDatabase<TConnection>.GetFilterCriteriaForIds(persistentObjectsToBePreloaded, nameof(PersistentObject.Id));
                if (null != filterCriteria) {
                    var viewName = RelationalDatabase.GetViewNameForContainer(internalContainerName);
                    var columnNames = new List<string>(keys.Count + 1) {
                        nameof(PersistentObject.Id)
                    };
                    columnNames.AddRange(keys);
                    this.db.SelectRowsFromTable(viewName, columnNames, SelectionMode.SelectAllResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0, ulong.MaxValue, delegate (DbDataReader dataReader) {
                        while (dataReader.Read()) {
                            var id = dataReader.GetGuid(0);
                            var persistentObjectToBePreloaded = persistentObjectsToBePreloaded[id];
                            for (var i = 1; i < columnNames.Count; i++) {
                                var persistentField = persistentObjectToBePreloaded.GetPersistentFieldForElement(columnNames[i]);
                                persistentField.LoadValueFromDbDataReader(dataReader, i);
                            }
                        }
                        return;
                    });
                    foreach (var persistentObjectToBePreloaded in persistentObjectsToBePreloaded) {
                        for (var i = 1; i < columnNames.Count; i++) {
                            var persistentField = persistentObjectToBePreloaded.GetPersistentFieldForElement(columnNames[i]);
                            persistentField.IsRetrieved = true;
                            persistentField.SetValueFromDbDataReader();
                            persistentField.SetIsChangedToFalse();
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Preloads the state of all fields for persistent objects
        /// of persistent objects from persistence mechanism.
        /// </summary>
        /// <param name="persistentObjects">persistent objects to
        /// preload fields for persistent objects for</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="keyChains">key chains of fields to preload</param>
        /// <param name="allowedGroupsForReadingCacheForCurrentUser">
        /// in-mermory cache with ID of allowed groups as key and
        /// boolean value indicating whether current user has read
        /// permissions as value - this cache should not be used
        /// accross multiple requests because it might be outdated
        /// soon</param>
        private void PreloadFieldsForPersistentObjects(IEnumerable<PersistentObject> persistentObjects, string internalContainerName, ICollection<string[]> keyChains, Dictionary<Guid, bool> allowedGroupsForReadingCacheForCurrentUser) {
            if (keyChains.Count > 0) {
                var keysForParentFields = RelationalDatabase<TConnection>.GetKeysForParentFields(keyChains);
                var persistentObjectsToBePreloaded = RelationalDatabase<TConnection>.GetPersistentObjectsToBePreloaded(persistentObjects, keysForParentFields, delegate (PersistentObject persistentObject, string key) {
                    return persistentObject.GetPersistentFieldForPersistentObject(key);
                });
                var filterCriteria = RelationalDatabase<TConnection>.GetFilterCriteriaForIds(persistentObjectsToBePreloaded, nameof(PersistentObject.Id));
                if (null != filterCriteria) {
                    try {
                        foreach (var persistentObjectToBePreloaded in persistentObjectsToBePreloaded) {
                            foreach (var keyForParentField in keysForParentFields) {
                                var persistentField = persistentObjectToBePreloaded.GetPersistentFieldForPersistentObject(keyForParentField);
                                persistentField.IsRetrieved = true;
                                persistentField.InitialValue = null;
                                persistentField.ValueAsObject = null;
                                persistentField.SetIsChangedToFalse();
                            }
                        }
                        foreach (var keyForParentField in keysForParentFields) {
                            var parentContainerName = RelationalDatabase.GetViewNameForContainer(internalContainerName);
                            var samplePersistentField = persistentObjectsToBePreloaded[0].GetPersistentFieldForPersistentObject(keyForParentField);
                            var childContainerName = this.GetInternalNameOfContainer(samplePersistentField.GetContentType());
                            childContainerName = RelationalDatabase.GetViewNameForContainer(childContainerName);
                            var columnNames = new string[] {
                                nameof(PersistentObject.Id),
                                keyForParentField,
                                "r1." + nameof(PersistentObject.AllowedGroups),
                                "r1.TypeName"
                            };
                            var relationalJoin = new RelationalJoin(childContainerName, "r1") {
                                FullFieldName = "r1"
                            };
                            relationalJoin.FieldPredicates.Add(RelationalJoin.RootTableAlias + '.' + keyForParentField, "r1." + nameof(PersistentObject.Id));
                            var relations = new List<Relation>(); // child objects are not created directly to avoid nested transactions
                            this.db.SelectRowsFromTable(parentContainerName, columnNames, SelectionMode.SelectAllResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[] { relationalJoin }, 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                                while (dataReader.Read()) {
                                    relations.Add(new Relation() {
                                        ParentId = dataReader.GetGuid(0),
                                        ChildId = dataReader.GetGuid(1),
                                        ChildAllowedGroupsId = Relation.GetNullableGuid(dataReader, 2),
                                        ChildType = dataReader.GetString(3)
                                    });
                                }
                                return;
                            });
                            this.PreloadGroupsForReadingForCurrentUser(relations, allowedGroupsForReadingCacheForCurrentUser);
                            foreach (var relation in relations) {
                                if (this.IsCurrentUserInGroupsForReadingOfAllowedGroupsWithId(relation.ChildAllowedGroupsId, allowedGroupsForReadingCacheForCurrentUser)) {
                                    var persistentObjectToBePreloaded = persistentObjectsToBePreloaded[relation.ParentId];
                                    var child = this.GetInstance(relation.ChildId, relation.ChildType);
                                    var persistentField = persistentObjectToBePreloaded.GetPersistentFieldForPersistentObject(keyForParentField);
                                    if (null != persistentField.ValueAsObject) {
                                        throw new ObjectNotUniqueException("Value for field \""
                                            + persistentField.Key + "\" of object with id \""
                                            + persistentField.ParentPersistentObject.Id + "\" is not unique in persistence mechanism.");
                                    }
                                    persistentField.InitialValue = child;
                                    persistentField.ValueAsObject = child;
                                    persistentField.SetIsChangedToFalse();
                                }
                            }
                        }
                    } catch (Exception) {
                        foreach (var persistentObjectToBePreloaded in persistentObjectsToBePreloaded) {
                            foreach (var keyChain in keyChains) {
                                persistentObjectToBePreloaded.GetPersistentFieldForPersistentObject(keyChain[0]).IsRetrieved = false;
                            }
                        }
                        throw;
                    }
                }
                foreach (var keyForParentField in keysForParentFields) {
                    var childKeyChains = new List<string[]>();
                    foreach (var keyChain in keyChains) {
                        if (keyForParentField == keyChain[0] && keyChain.LongLength > 1) {
                            childKeyChains.Add(KeyChain.RemoveFirstLinkOf(keyChain));
                        }
                    }
                    if (childKeyChains.Count > 0) {
                        var childObjects = new List<PersistentObject>();
                        Type contentType = null;
                        foreach (var persistentObject in persistentObjects) {
                            if (null != persistentObject) {
                                var persistentField = persistentObject.GetPersistentFieldForPersistentObject(keyForParentField);
                                childObjects.Add(persistentField.ValueAsPersistentObject);
                                if (null == contentType) {
                                    contentType = persistentField.GetContentType();
                                }
                            }
                        }
                        if (null != contentType) {
                            this.PreloadObjects(contentType, childObjects, childKeyChains, allowedGroupsForReadingCacheForCurrentUser);
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Preloads the state of multiple persistent objects from
        /// persistence mechanism.
        /// </summary>
        /// <param name="contentType">type of common base class of
        /// objects to be preloaded</param>
        /// <param name="persistentObjects">persistent objects to be preloaded</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        /// <param name="allowedGroupsForReadingCacheForCurrentUser">
        /// in-mermory cache with ID of allowed groups as key and
        /// boolean value indicating whether current user has read
        /// permissions as value - this cache should not be used
        /// accross multiple requests because it might be outdated
        /// soon</param>
        private void PreloadObjects(Type contentType, IEnumerable<PersistentObject> persistentObjects, IEnumerable<string[]> keyChains, Dictionary<Guid, bool> allowedGroupsForReadingCacheForCurrentUser) {
            // TODO: This code should not be duplicated in PersistentContainer.Preload(...)!
            var sampleObject = this.CreateInstance(contentType);
            var internalContainerName = this.GetInternalNameOfContainer(contentType);
            var isRetrievalRequired = false;
            foreach (var keyChain in keyChains) {
                isRetrievalRequired = true;
                break;
            }
            if (isRetrievalRequired) {
                var autoRetrievalEnabled = new List<bool>();
                var eventFiringEnabled = new List<bool>();
                var uniqueGuids = new List<Guid>();
                try {
                    foreach (var item in persistentObjects) {
                        if (null != item && !uniqueGuids.Contains(item.Id)) {
                            autoRetrievalEnabled.Add(item.IsAutoRetrievalEnabled);
                            eventFiringEnabled.Add(item.IsEventFiringEnabled);
                            uniqueGuids.Add(item.Id);
                            item.IsAutoRetrievalEnabled = false;
                            item.IsEventFiringEnabled = false;
                        }
                    }
                    this.PreloadObjects(sampleObject, persistentObjects, internalContainerName, keyChains, allowedGroupsForReadingCacheForCurrentUser);
                } finally {
                    var i = 0;
                    uniqueGuids.Clear();
                    foreach (var item in persistentObjects) {
                        if (null != item && !uniqueGuids.Contains(item.Id)) {
                            item.IsAutoRetrievalEnabled = autoRetrievalEnabled[i];
                            item.IsEventFiringEnabled = eventFiringEnabled[i];
                            uniqueGuids.Add(item.Id);
                            i++;
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Preloads the state of multiple persistent objects from
        /// persistence mechanism.
        /// </summary>
        /// <param name="sampleObject">sample object of common base
        /// class of objects to be preloaded</param>
        /// <param name="persistentObjects">persistent objects to
        /// preload</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        internal sealed override void PreloadObjects(PersistentObject sampleObject, IEnumerable<PersistentObject> persistentObjects, string internalContainerName, IEnumerable<string[]> keyChains) {
            var allowedGroupsForReadingCacheForCurrentUser = new Dictionary<Guid, bool>();
            this.PreloadObjects(sampleObject, persistentObjects, internalContainerName, keyChains, allowedGroupsForReadingCacheForCurrentUser);
            return;
        }

        /// <summary>
        /// Preloads the state of multiple persistent objects from
        /// persistence mechanism.
        /// </summary>
        /// <param name="sampleObject">sample object of common base
        /// class of objects to be preloaded</param>
        /// <param name="persistentObjects">persistent objects to
        /// preload</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        /// <param name="allowedGroupsForReadingCacheForCurrentUser">
        /// in-mermory cache with ID of allowed groups as key and
        /// boolean value indicating whether current user has read
        /// permissions as value - this cache should not be used
        /// accross multiple requests because it might be outdated
        /// soon</param>
        protected virtual void PreloadObjects(PersistentObject sampleObject, IEnumerable<PersistentObject> persistentObjects, string internalContainerName, IEnumerable<string[]> keyChains, Dictionary<Guid, bool> allowedGroupsForReadingCacheForCurrentUser) {
            var keysOfFieldsForElements = new List<string>();
            var keysOfFieldsForCollectionsOfElements = new List<string>();
            var keyChainsOfFieldsForPersistentObjects = new List<string[]>();
            var keyChainsOfFieldsForCollectionsOfPersistentObjects = new List<string[]>();
            foreach (var keyChain in keyChains) {
                var isFieldFound = false;
                if (keyChain.LongLength > 0) {
                    var key = keyChain[0];
                    if (1 == keyChain.LongLength) {
                        var fieldForElement = sampleObject.GetPersistentFieldForElement(key);
                        if (null != fieldForElement) {
                            if (!keysOfFieldsForElements.Contains(key)) {
                                keysOfFieldsForElements.Add(key);
                            }
                            isFieldFound = true;
                        }
                        if (!isFieldFound) {
                            var fieldForCollectionOfElements = sampleObject.GetPersistentFieldForCollectionOfElements(key);
                            if (null != fieldForCollectionOfElements) {
                                if (!keysOfFieldsForCollectionsOfElements.Contains(key)) {
                                    keysOfFieldsForCollectionsOfElements.Add(key);
                                }
                                isFieldFound = true;
                            }
                        }
                    }
                    if (!isFieldFound) {
                        var fieldForPersistentObject = sampleObject.GetPersistentFieldForPersistentObject(key);
                        if (null != fieldForPersistentObject) {
                            if (!KeyChain.IsContainedIn(keyChainsOfFieldsForPersistentObjects, keyChain)) {
                                keyChainsOfFieldsForPersistentObjects.Add(keyChain);
                            }
                            isFieldFound = true;
                        }
                    }
                    if (!isFieldFound) {
                        var fieldForCollectionOfPersistentObjects = sampleObject.GetPersistentFieldForCollectionOfPersistentObjects(key);
                        if (null != fieldForCollectionOfPersistentObjects) {
                            if (!KeyChain.IsContainedIn(keyChainsOfFieldsForCollectionsOfPersistentObjects, keyChain)) {
                                keyChainsOfFieldsForCollectionsOfPersistentObjects.Add(keyChain);
                            }
                            isFieldFound = true;
                        }
                    }
                    if (!isFieldFound) {
                        throw new KeyNotFoundException("Key chain " + KeyChain.ToKey(keyChain) + " cannot be found in persistent objects of type " + sampleObject.Type.FullName + ".");
                    }
                }
            }
            this.PreloadFieldsForElements(persistentObjects, internalContainerName, keysOfFieldsForElements);
            this.PreloadFieldsForCollectionsOfElements(persistentObjects, internalContainerName, keysOfFieldsForCollectionsOfElements);
            this.PreloadFieldsForPersistentObjects(persistentObjects, internalContainerName, keyChainsOfFieldsForPersistentObjects, allowedGroupsForReadingCacheForCurrentUser);
            this.PreloadFieldsForCollectionsOfPersistentObjects(persistentObjects, keyChainsOfFieldsForCollectionsOfPersistentObjects, allowedGroupsForReadingCacheForCurrentUser);
            return;
        }

        /// <summary>
        /// Removes an ID from the table of deleted IDs.
        /// </summary>
        /// <param name="id">ID to remove from table of deleted IDs</param>
        /// <returns>true if ID was deleted from table of deleted
        /// IDs, false otherwise</returns>
        private bool RemoveFromDeletedIDsTable(Guid id) {
            var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, id);
            return this.db.DeleteRowsFromTable(RelationalDatabase.DeletedIDsTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection()) > 0;
        }

        /// <summary>
        /// Removes a specific object from persistence mechanism and
        /// stores its id for replication scenarios.
        /// </summary>
        /// <param name="persistentObject">object to remove</param>
        /// <param name="isToBeRemovedIfNotReferencedOnly">true if
        /// object is to be removed if not referenced by other
        /// objects only, false otherwise</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, null if it was not removed because
        /// it is referenced by other objects, false otherwise or if
        /// object was not contained in persistence mechanism</returns>
        protected internal override bool? RemoveObject(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly, string internalContainerName) {
            bool? success = false;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                bool isToBeRemoved;
                if (isToBeRemovedIfNotReferencedOnly) {
                    isToBeRemoved = !this.ContainsReferencingPersistentObjectsTo(persistentObject);
                } else {
                    isToBeRemoved = true;
                }
                if (isToBeRemoved) {
                    this.AddToDeletedIDsTable(persistentObject.Id);
                    var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, persistentObject.Id);
                    filterCriteria = this.AddFilterCriteriaForWritePermissions(filterCriteria);
                    var relationalSubqueries = new RelationalSubqueryBuilder(filterCriteria, this.GetInternalNameOfContainer, persistentObject).ToRelationalSubqueries();
                    if (this.db.DeleteRowsFromTable(internalContainerName, filterCriteria, new RelationalJoin[0], relationalSubqueries) > 0) {
                        success = true;
                        this.RemoveObjectFromFullTextTable(persistentObject.Id);
                        this.RemoveUserIdFromSubTables(persistentObject);
                        this.RemoveObjectRelationsUnsafe(persistentObject);
                    }
                } else {
                    success = null;
                }
                transactionScope.Complete();
            }
            return success;
        }

        /// <summary>
        /// Removes a specific object and all child objects to be
        /// removed cascadedly from persistence mechanism
        /// immediately.
        /// </summary>
        /// <param name="persistentObject">object to remove
        /// cascadedly</param>
        /// <param name="isToBeRemovedIfNotReferencedOnly">true if
        /// object is to be removed if not referenced by other
        /// objects only, false otherwise</param>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, null if it was not removed because
        /// it is referenced by other objects, false otherwise or if
        /// object was not contained in persistence mechanism</returns>
        internal override bool? RemoveObjectCascadedly(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly) {
            bool? success;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                success = base.RemoveObjectCascadedly(persistentObject, isToBeRemovedIfNotReferencedOnly);
                if (false != success) {
                    transactionScope.Complete();
                }
            }
            return success;
        }

        /// <summary>
        /// Removes an object from full-text table.
        /// </summary>
        /// <param name="id">ID of object to remove from full-text
        /// table</param>
        private void RemoveObjectFromFullTextTable(Guid id) {
            if (this.HasFullTextSearchEnabled) {
                var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, id);
                this.db.DeleteRowsFromTable(RelationalDatabase.FullTextTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection());
            }
            return;
        }

        /// <summary>
        /// Removes all references to a specific persistent object.
        /// This method is unsafe because it has to be called out of
        /// a transaction scope.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// remove references to for</param>
        private void RemoveObjectRelationsUnsafe(PersistentObject persistentObject) {
            var filterCriteria = new FilterCriteria("ParentID", RelationalOperator.IsEqualTo, persistentObject.Id)
                .Or("ChildID", RelationalOperator.IsEqualTo, persistentObject.Id);
            this.db.DeleteRowsFromTable(RelationalDatabase.CollectionRelationsTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection());
            var tableFieldPairs = this.GetTableFieldPairsForSingleRelations(persistentObject.ParentPersistentContainer.InternalName, persistentObject.Id);
            foreach (var tableFieldPair in tableFieldPairs) {
                var setNullFilterCriteria = new FilterCriteria(tableFieldPair.Value, RelationalOperator.IsEqualTo, persistentObject.Id);
                var fieldsToBeUpdated = new PersistentFieldForElement[] {
                    new PersistentFieldForNullableGuid(tableFieldPair.Value, null),
                    new PersistentFieldForString(tableFieldPair.Value + '_', null)
                };
                this.db.UpdateTableRow(tableFieldPair.Key, setNullFilterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection(), fieldsToBeUpdated);
            }
            return;
        }

        /// <summary>
        /// Removes the ID of a user object from all sub tables.
        /// </summary>
        /// <param name="persistentObject">potential user object
        /// whose ID to remove from all sub tables</param>
        private void RemoveUserIdFromSubTables(PersistentObject persistentObject) {
            if (persistentObject is IUser) {
                var filterCriteria = new FilterCriteria("Value", RelationalOperator.IsEqualTo, persistentObject.Id);
                var subTableNames = new List<string>();
                this.db.SelectRowsFromTable(RelationalDatabase.SubTablesTableName, "SubTableName", SelectionMode.SelectAllResults, null, FilterCriteria.Empty, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                    while (dataReader.Read()) {
                        subTableNames.Add(dataReader.GetString(0));
                    }
                    return;
                });
                foreach (var subTableName in subTableNames) {
                    if (this.db.GetUpperCaseGuidFieldNamesOf(subTableName).Contains("VALUE")) {
                        this.db.DeleteRowsFromTable(subTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection());
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Retrieves the state of all fields for elements of
        /// persistent object from persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// retrieve fields for elements for</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal sealed override void RetrieveFieldsForElements(PersistentObject persistentObject, string internalContainerName) {
            var columnNames = new List<string>();
            var persistentFields = persistentObject.PersistentFieldsForElements;
            foreach (var persistentField in persistentFields) {
                columnNames.Add(persistentField.Key);
            }
            var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, persistentObject.Id);
            this.db.SelectRowsFromTable(internalContainerName, columnNames, SelectionMode.SelectAllResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0, ulong.MaxValue, delegate (DbDataReader dataReader) {
                if (dataReader.Read()) {
                    var index = 0;
                    foreach (var persistentField in persistentFields) {
                        persistentField.LoadValueFromDbDataReader(dataReader, index);
                        index++;
                    }
                } else {
                    throw new ObjectNotFoundException("Object of type " + persistentObject.Type.FullName
                        + " with ID " + persistentObject.Id + " was not found in persistence mechanism.");
                }
                return;
            });
            foreach (var persistentField in persistentFields) {
                persistentField.IsRetrieved = true;
                persistentField.SetValueFromDbDataReader();
                persistentField.SetIsChangedToFalse();
            }
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// elements from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal sealed override void RetrieveFieldForCollectionOfElements(PersistentFieldForCollection persistentField, string internalContainerName) {
            var subTableName = RelationalDatabase.GetInternalNameOfSubTable(internalContainerName, persistentField.Key);
            var filterCriteria = new FilterCriteria("ParentID", RelationalOperator.IsEqualTo, persistentField.ParentPersistentObject.Id);
            var sortCriteria = new SortCriterionCollection() {
                { "ParentIndex", SortDirection.Ascending }
            };
            persistentField.InitializeTemporaryCollectionForDbDataReader();
            this.db.SelectRowsFromTable(subTableName, "Value", SelectionMode.SelectAllResults, null, filterCriteria, sortCriteria, new RelationalJoin[0], 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                while (dataReader.Read()) {
                    persistentField.LoadValueFromDbDataReader(dataReader);
                }
                return;
            });
            persistentField.IsRetrieved = true;
            persistentField.SetValuesFromDbDataReader();
            persistentField.SetIsChangedToFalse();
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a persistent object
        /// from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal sealed override void RetrieveFieldForPersistentObject(PersistentFieldForPersistentObject persistentField, string internalContainerName) {
            this.RetrieveFieldForPersistentObject(persistentField, internalContainerName, new Dictionary<Guid, bool>());
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a persistent object
        /// from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="allowedGroupsForReadingCacheForCurrentUser">
        /// in-mermory cache with ID of allowed groups as key and
        /// boolean value indicating whether current user has read
        /// permissions as value - this cache should not be used
        /// accross multiple requests because it might be outdated
        /// soon</param>
        private void RetrieveFieldForPersistentObject(PersistentFieldForPersistentObject persistentField, string internalContainerName, Dictionary<Guid, bool> allowedGroupsForReadingCacheForCurrentUser) {
            var wasRetrieved = persistentField.IsRetrieved;
            try {
                persistentField.IsRetrieved = true;
                var childContainerName = this.GetInternalNameOfContainer(persistentField.GetContentType());
                childContainerName = RelationalDatabase.GetViewNameForContainer(childContainerName);
                var columnNames = new string[] {
                    persistentField.Key,
                    "r1." + nameof(PersistentObject.AllowedGroups),
                    "r1.TypeName"
                };
                var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, persistentField.ParentPersistentObject.Id);
                var relationalJoin = new RelationalJoin(childContainerName, "r1") {
                    FullFieldName = "r1"
                };
                relationalJoin.FieldPredicates.Add(RelationalJoin.RootTableAlias + '.' + persistentField.Key, "r1." + nameof(PersistentObject.Id));
                Relation relation = null; // child object is not created directly to avoid nested transactions
                this.db.SelectRowsFromTable(internalContainerName, columnNames, SelectionMode.SelectAllResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[] { relationalJoin }, 0L, 1L, delegate (DbDataReader dataReader) {
                    if (dataReader.Read()) {
                        relation = new Relation() {
                            ChildId = dataReader.GetGuid(0),
                            ChildAllowedGroupsId = Relation.GetNullableGuid(dataReader, 1),
                            ChildType = dataReader.GetString(2)
                        };
                    }
                    return;
                });
                if (null == relation || !this.IsCurrentUserInGroupsForReadingOfAllowedGroupsWithId(relation.ChildAllowedGroupsId, allowedGroupsForReadingCacheForCurrentUser)) {
                    persistentField.InitialValue = null;
                    persistentField.ValueAsObject = null;
                } else {
                    var child = this.GetInstance(relation.ChildId, relation.ChildType);
                    persistentField.InitialValue = child;
                    persistentField.ValueAsObject = child;
                }
                persistentField.SetIsChangedToFalse();
            } catch (Exception) {
                persistentField.IsRetrieved = wasRetrieved;
                throw;
            }
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// persistent objects from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal sealed override void RetrieveFieldForCollectionOfPersistentObjects(PersistentFieldForPersistentObjectCollection persistentField, string internalContainerName) {
            this.RetrieveFieldForCollectionOfPersistentObjects(persistentField, internalContainerName, new Dictionary<Guid, bool>());
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// persistent objects from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="allowedGroupsForReadingCacheForCurrentUser">
        /// in-mermory cache with ID of allowed groups as key and
        /// boolean value indicating whether current user has read
        /// permissions as value - this cache should not be used
        /// accross multiple requests because it might be outdated
        /// soon</param>
        private void RetrieveFieldForCollectionOfPersistentObjects(PersistentFieldForPersistentObjectCollection persistentField, string internalContainerName, Dictionary<Guid, bool> allowedGroupsForReadingCacheForCurrentUser) {
            var wasRetrieved = persistentField.IsRetrieved;
            try {
                persistentField.IsRetrieved = true;
                var childContainerName = this.GetInternalNameOfContainer(persistentField.GetContentType());
                childContainerName = RelationalDatabase.GetViewNameForContainer(childContainerName);
                var columnNames = new string[] {
                    "ChildID",
                    "r1." + nameof(PersistentObject.AllowedGroups),
                    "r1.TypeName",
                    "ParentIndex"
                };
                var filterCriteria = new FilterCriteria("ParentID", RelationalOperator.IsEqualTo, persistentField.ParentPersistentObject.Id)
                    .And("ParentField", RelationalOperator.IsEqualTo, persistentField.Key, FilterTarget.IsOtherTextValue);
                var sortCriteria = new SortCriterionCollection("ParentIndex", SortDirection.Ascending);
                var relationalJoin = new RelationalJoin(childContainerName, "r1") {
                    FullFieldName = "r1"
                };
                relationalJoin.FieldPredicates.Add(RelationalJoin.RootTableAlias + '.' + "ChildID", "r1." + nameof(PersistentObject.Id));
                persistentField.InitialValues.Clear();
                persistentField.Clear();
                var relations = new List<Relation>(); // child objects are not created directly to avoid nested transactions
                this.db.SelectRowsFromTable(RelationalDatabase.CollectionRelationsTableName, columnNames, SelectionMode.SelectAllResults, null, filterCriteria, sortCriteria, new RelationalJoin[] { relationalJoin }, 0L, ulong.MaxValue, delegate (DbDataReader dataReader) {
                    while (dataReader.Read()) {
                        relations.Add(new Relation() {
                            ChildId = dataReader.GetGuid(0),
                            ChildAllowedGroupsId = Relation.GetNullableGuid(dataReader, 1),
                            ChildType = dataReader.GetString(2)
                        });
                    }
                    return;
                });
                this.PreloadGroupsForReadingForCurrentUser(relations, allowedGroupsForReadingCacheForCurrentUser);
                foreach (var relation in relations) {
                    if (this.IsCurrentUserInGroupsForReadingOfAllowedGroupsWithId(relation.ChildAllowedGroupsId, allowedGroupsForReadingCacheForCurrentUser)) {
                        var child = this.GetInstance(relation.ChildId, relation.ChildType);
                        persistentField.InitialValues.Add(child);
                        persistentField.AddObject(child);
                    }
                }
                persistentField.SetIsChangedToFalse();
            } catch (Exception) {
                persistentField.IsRetrieved = wasRetrieved;
                throw;
            }
            return;
        }

        /// <summary>
        /// Retrieves the state of a persistent object from
        /// persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal sealed override void RetrieveObject(PersistentObject persistentObject, string internalContainerName) {
            this.RetrieveFieldsForElements(persistentObject, internalContainerName);
            foreach (var persistentField in persistentObject.PersistentFieldsForCollectionsOfElements) {
                this.RetrieveFieldForCollectionOfElements(persistentField, internalContainerName);
            }
            var allowedGroupsForReadingCacheForCurrentUser = new Dictionary<Guid, bool>();
            foreach (var persistentField in persistentObject.PersistentFieldsForPersistentObjects) {
                this.RetrieveFieldForPersistentObject(persistentField, internalContainerName, allowedGroupsForReadingCacheForCurrentUser);
            }
            foreach (var persistentField in persistentObject.PersistentFieldsForCollectionsOfPersistentObjects) {
                this.RetrieveFieldForCollectionOfPersistentObjects(persistentField, internalContainerName, allowedGroupsForReadingCacheForCurrentUser);
            }
            return;
        }

        /// <summary>
        /// Determines whether a container contains a specific
        /// object - including containers of sub types. Permissions
        /// are ignored.
        /// </summary>
        /// <param name="tableOrViewName">name of the related table
        /// or view</param>
        /// <param name="id">specific ID to look for</param>
        /// <returns>true if specific ID is contained, false
        /// otherwise</returns>
        private bool TableOrViewContainsId(string tableOrViewName, Guid id) {
            var isIdContained = false;
            var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, id);
            this.db.SelectRowsFromTable(tableOrViewName, nameof(PersistentObject.Id), SelectionMode.SelectAllResults, null, filterCriteria, SortCriterionCollection.Empty, new RelationalJoin[0], 0L, 1L, delegate (DbDataReader dataReader) {
                if (dataReader.Read()) {
                    isIdContained = true;
                }
                return;
            });
            return isIdContained;
        }

        /// <summary>
        /// Updates a specific object in a container.
        /// </summary>
        /// <param name="persistentObject">specific object to update
        /// in container</param>
        /// <param name="persistentContainer">persistentContainer to
        /// update object in</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        internal override bool UpdateObject(PersistentObject persistentObject, PersistentContainer persistentContainer) {
            bool success;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                success = base.UpdateObject(persistentObject, persistentContainer);
                transactionScope.Complete();
            }
            return success;
        }

        /// <summary>
        /// Updates a persistent object in persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to update</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        /// <returns>true if object was updated in persistence
        /// mechanism successfully, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        protected internal override bool UpdateObject(PersistentObject persistentObject, string internalContainerName, IList<PersistentObject> potentialBrokenReferences) {
            bool success;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                success = this.UpdateObjectInParentTable(persistentObject, internalContainerName, potentialBrokenReferences);
                if (success) {
                    this.UpdateObjectInSubTables(internalContainerName, persistentObject.Id, persistentObject.PersistentFieldsForCollectionsOfElements);
                    this.UpdateObjectFieldsForCollectionsOfPersistentObjects(internalContainerName, persistentObject.Id, persistentObject.PersistentFieldsForCollectionsOfPersistentObjects, potentialBrokenReferences);
                    this.AddOrUpdateObjectInFullTextTable(persistentObject);
                }
                transactionScope.Complete();
            }
            return success;
        }

        /// <summary>
        /// Updates an object in a container and all of its
        /// persistent child objects cascadedly. Missing persistent
        /// child objects are added cascadedly.
        /// </summary>
        /// <param name="persistentObject">object to update in
        /// container cascadedly</param>
        /// <param name="persistentContainer">persistentContainer to
        /// update object in cascadedly</param>
        internal override void UpdateObjectCascadedly(PersistentObject persistentObject, PersistentContainer persistentContainer) {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required)) {
                base.UpdateObjectCascadedly(persistentObject, persistentContainer);
                transactionScope.Complete();
            }
            return;
        }

        /// <summary>
        /// Updates the value for created at of a persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// update value for created at for</param>
        /// <param name="createdAt">new date/time value to be set for
        /// created at</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        internal override bool UpdateObjectCreatedAt(PersistentObject persistentObject, DateTime createdAt) {
            return this.UpdateObjectProtectedField(persistentObject, nameof(PersistentObject.CreatedAt), createdAt);
        }

        /// <summary>
        /// Updates the value for created by of a persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// update value for created by for</param>
        /// <param name="createdBy">new user value to be set for
        /// created by</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        internal override bool UpdateObjectCreatedBy(PersistentObject persistentObject, IUser createdBy) {
            return this.UpdateObjectProtectedField(persistentObject, nameof(PersistentObject.CreatedBy), createdBy);
        }

        /// <summary>
        /// Updates n:m relations of a persistent object to other
        /// persistent objects in the relations table.
        /// </summary>
        /// <param name="tableName">name of parent database table</param>
        /// <param name="id">id of persistent object to update
        /// relations for</param>
        /// <param name="fields">fields to update</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        private void UpdateObjectFieldsForCollectionsOfPersistentObjects(string tableName, Guid id, IEnumerable<PersistentFieldForPersistentObjectCollection> fields, IList<PersistentObject> potentialBrokenReferences) {
            foreach (var field in fields) {
                if (field.IsChanged) {
                    var filterCriteria = new FilterCriteria("ParentID", RelationalOperator.IsEqualTo, id)
                        .And("ParentField", RelationalOperator.IsEqualTo, field.Key, FilterTarget.IsOtherTextValue);
                    this.db.DeleteRowsFromTable(RelationalDatabase.CollectionRelationsTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection());
                    var index = 0;
                    foreach (var childObject in field.GetValuesAsPersistentObject()) {
                        this.AddObjectFieldValueForCollectionOfPersistentObjects(tableName, id, field.Key, index, childObject, potentialBrokenReferences);
                        index++;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Updates fields for single elements of simple types and
        /// persistent objects in parent database table.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// insert fields for elements and fields for persistent
        /// objects for</param>
        /// <param name="tableName">name of parent database table</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        /// <returns>true if object was updated in persistence
        /// mechanism successfully, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        private bool UpdateObjectInParentTable(PersistentObject persistentObject, string tableName, IList<PersistentObject> potentialBrokenReferences) {
            var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, persistentObject.Id);
            filterCriteria = this.AddFilterCriteriaForWritePermissions(filterCriteria);
            var relationalSubqueries = new RelationalSubqueryBuilder(filterCriteria, this.GetInternalNameOfContainer, persistentObject).ToRelationalSubqueries();
            var fieldsToBeUpdated = new List<PersistentFieldForElement>();
            foreach (var persistentFieldForElement in persistentObject.PersistentFieldsForElements) {
                if (persistentFieldForElement.IsChanged
                    && persistentFieldForElement.Key != nameof(PersistentObject.Id)
                    && persistentFieldForElement.Key != nameof(PersistentObject.CreatedAt)
                    && persistentFieldForElement.Key != nameof(PersistentObject.CreatedBy)) {
                    fieldsToBeUpdated.Add(persistentFieldForElement);
                }
            }
            foreach (var persistentFieldForPersistentObject in persistentObject.PersistentFieldsForPersistentObjects) {
                if (persistentFieldForPersistentObject.IsChanged) {
                    var childObject = persistentFieldForPersistentObject.ValueAsPersistentObject;
                    if (null == childObject || this.IsIdDeleted(childObject.Id)) {
                        fieldsToBeUpdated.Add(new PersistentFieldForNullableGuid(persistentFieldForPersistentObject.Key, null));
                        fieldsToBeUpdated.Add(new PersistentFieldForString(persistentFieldForPersistentObject.Key + '_', null));
                    } else {
                        if (childObject.IsNew) {
                            potentialBrokenReferences.Add(childObject);
                        }
                        fieldsToBeUpdated.Add(new PersistentFieldForNullableGuid(persistentFieldForPersistentObject.Key, childObject.Id));
                        fieldsToBeUpdated.Add(new PersistentFieldForString(persistentFieldForPersistentObject.Key + '_', childObject.Type.AssemblyQualifiedName));
                    }
                }
            }
            return fieldsToBeUpdated.Count < 1 || this.db.UpdateTableRow(tableName, filterCriteria, new RelationalJoin[0], relationalSubqueries, fieldsToBeUpdated) > 0;
        }

        /// <summary>
        /// Updates simple element collections in sub tables.
        /// </summary>
        /// <param name="tableName">name of parent database table</param>
        /// <param name="id">id of persistent object to update fields
        /// for</param>
        /// <param name="fields">fields to update in table</param>
        private void UpdateObjectInSubTables(string tableName, Guid id, IEnumerable<PersistentFieldForCollection> fields) {
            var fieldsToBeUpdated = new List<PersistentFieldForCollection>();
            var filterCriteria = new FilterCriteria("ParentID", RelationalOperator.IsEqualTo, id);
            foreach (var field in fields) {
                if (field.IsChanged) {
                    fieldsToBeUpdated.Add(field);
                    var subTableName = RelationalDatabase.GetInternalNameOfSubTable(tableName, field.Key);
                    this.db.DeleteRowsFromTable(subTableName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection());
                }
            }
            if (fieldsToBeUpdated.Count > 0) {
                this.AddObjectToSubTables(tableName, id, fieldsToBeUpdated);
            }
            return;
        }

        /// <summary>
        /// Updates the value of a protected persistent field of a
        /// persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// update value for protected field for</param>
        /// <param name="key">key of field to be set</param>
        /// <param name="value">new value to be set for field</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        private bool UpdateObjectProtectedField(PersistentObject persistentObject, string key, object value) {
            if (SecurityModel.IgnorePermissions != this.SecurityModel) {
                throw new PersistenceException("Value of " + key + " can be updated with elevated privileges only.");
            }
            var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, persistentObject.Id);
            var fieldsToBeUpdated = new List<PersistentFieldForElement>(1);
            var fieldToBeUpdated = persistentObject.GetPersistentFieldForElement(key);
            fieldToBeUpdated.ValueAsObject = value;
            fieldsToBeUpdated.Add(fieldToBeUpdated);
            return this.db.UpdateTableRow(persistentObject.ParentPersistentContainer.InternalName, filterCriteria, new RelationalJoin[0], new RelationalSubqueryCollection(), fieldsToBeUpdated) > 0;
        }

    }

}