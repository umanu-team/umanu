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
    using Framework.Persistence.Directories;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// Represents a relational database.
    /// </summary>
    public abstract class RelationalDatabase : PersistenceMechanism {

        /// <summary>
        /// Name of relations table.
        /// </summary>
        internal const string CollectionRelationsTableName = "Relations";

        /// <summary>
        /// Name of containers table.
        /// </summary>
        internal const string ContainersTableName = "Containers";

        /// <summary>
        /// Name of deleted IDs table.
        /// </summary>
        internal const string DeletedIDsTableName = "DeletedIDs";

        /// <summary>
        /// Name of full-text table.
        /// </summary>
        internal const string FullTextTableName = "FullText";

        /// <summary>
        /// Name of sub tables table.
        /// </summary>
        protected const string SubTablesTableName = "SubTables";

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="userDirectory">directory to use for user
        /// resolval</param>
        /// <param name="securityModel">security model to use for
        /// accessing this persistence mechanism</param>
        protected RelationalDatabase(UserDirectory userDirectory, SecurityModel securityModel)
            : base(userDirectory, securityModel) {
            // nothing to do
        }

        /// <summary>
        /// Gets the name of a sub table of a container.
        /// </summary>
        /// <param name="internalContainerName">name of internal
        /// container for parent type</param>
        /// <param name="fieldName">name of field to get sub table
        /// for</param>
        /// <returns>name of sub table of container</returns>
        protected internal static string GetInternalNameOfSubTable(string internalContainerName, string fieldName) {
            return 'F' + internalContainerName.Substring(1) + '_' + fieldName;
        }

        /// <summary>
        /// Gets the name of the view containing all objects of a type
        /// and all subtypes.
        /// </summary>
        /// <param name="internalContainerName">name of internal
        /// container of parent type</param>
        /// <returns>name of view containing all objects of a type
        /// and all subtypes</returns>
        protected internal static string GetViewNameForContainer(string internalContainerName) {
            return 'P' + internalContainerName.Substring(1);
        }

        /// <summary>
        /// Gets the name of the view containing all n:1 relations to
        /// objects of a type.
        /// </summary>
        /// <param name="internalContainerName">name of internal
        /// container of type to get view name for relations for</param>
        /// <returns>name of view containing all n:1 relations to
        /// objects of a type</returns>
        protected internal static string GetViewNameForSingleRelations(string internalContainerName) {
            return 'R' + internalContainerName.Substring(1);
        }

        /// <summary>
        /// Gets the name of the view containing all sub tables for a
        /// specific field of a type and all subtypes.
        /// </summary>
        /// <param name="internalSubTableName">name of sub table of
        /// container</param>
        /// <returns>name of view containing all sub tables for a
        /// specific field of a type and all subtypes</returns>
        protected internal static string GetViewNameForSubTable(string internalSubTableName) {
            return 'G' + internalSubTableName.Substring(1);
        }

        /// <summary>
        /// Gets the name of the view containing all sub tables for a
        /// specific field of a type and all subtypes.
        /// </summary>
        /// <param name="internalContainerName">name of internal
        /// container of parent type</param>
        /// <param name="fieldName">name of field to get sub table
        /// for</param>
        /// <returns>name of view containing all sub tables for a
        /// specific field of a type and all subtypes</returns>
        protected internal static string GetViewNameForSubTable(string internalContainerName, string fieldName) {
            return RelationalDatabase.GetViewNameForSubTable(RelationalDatabase.GetInternalNameOfSubTable(internalContainerName, fieldName));
        }

    }

    /// <summary>
    /// Represents a relational database.
    /// </summary>
    /// <typeparam name="TConnection">type of connections</typeparam>
    public abstract partial class RelationalDatabase<TConnection> : RelationalDatabase
        where TConnection : DbConnection, new() {

        /// <summary>
        /// Connector to relational database.
        /// </summary>
        protected RelationalDatabaseConnector<TConnection> db;

        /// <summary>
        /// Command timeout in seconds or null to use default command
        /// timeout.
        /// </summary>
        public int? CommandTimeoutInSeconds {
            get { return this.db.CommandTimeoutInSeconds; }
            set { this.db.CommandTimeoutInSeconds = value; }
        }

        /// <summary>
        /// Connection settings for accessing the relational
        /// database.
        /// </summary>
        public string ConnectionString {
            get {
                return this.db.ConnectionString;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="databaseConnector">connector to relational
        /// database</param>
        /// <param name="userDirectory">directory to use for user
        /// resolval</param>
        /// <param name="securityModel">security model to use for
        /// accessing this persistence mechanism</param>
        protected RelationalDatabase(RelationalDatabaseConnector<TConnection> databaseConnector, UserDirectory userDirectory, SecurityModel securityModel)
            : base(userDirectory, securityModel) {
            this.db = databaseConnector;
            this.HasFullTextSearchEnabled = false;
            this.Log = databaseConnector.DbCommandLogger.Log;
            this.containerNameCache = RelationalDatabase<TConnection>.containerNameCaches.GetOrAdd(this.ConnectionString, delegate (string key) {
                return new ConcurrentDictionary<string, string>();
            });
        }

        /// <summary>
        /// Cleans up this persistence mechanism.
        /// </summary>
        public override void CleanUp() {
            this.CleanUp(true);
            return;
        }

        /// <summary>
        /// Cleans up this persistence mechanism.
        /// </summary>
        /// <param name="isRemovingBrokenRelations">true to remove
        /// broken relations, false otherwise</param>
        public void CleanUp(bool isRemovingBrokenRelations) {
            this.db.DbCommandLogger.Log?.WriteEntry("Cleaning up database.", LogLevel.Information);
            var oldInventory = this.GetInventory();
            base.CleanUp();
            if (isRemovingBrokenRelations) {
                this.db.DbCommandLogger.Log?.WriteEntry("Searching for broken relations.", LogLevel.Information);
                this.RemoveBrokenRelations();
            }
            this.DetectRemovedElements(oldInventory);
            this.DetectElementsWithoutAllowedGroups();
            this.CleanUpEventsWithoutTimeZoneSet();
            this.CleanUpFilesWithoutFileSizeSet();
            this.CleanUpHistoryItemsWithoutDurationSet();
            this.DetectSuspiciousCompositions();
            return;
        }

        /// <summary>
        /// Cleans up events without time zone set.
        /// </summary>
        private void CleanUpEventsWithoutTimeZoneSet() {
            if (this.ContainsContainer<Model.Calendar.Event>()) {
                this.db.DbCommandLogger.Log?.WriteEntry("Searching for events without time zone set.", LogLevel.Information);
                foreach (var e in this.FindContainer<Model.Calendar.Event>().Find(new FilterCriteria(nameof(Model.Calendar.Event.TimeZone), RelationalOperator.IsEqualTo, null), SortCriterionCollection.Empty)) {
                    this.db.DbCommandLogger.Log?.WriteEntry("Updating time zone for \"" + e.Title + "\".", LogLevel.Warning);
                    var copiedPersistenceMechanism = this.CopyWithElevatedPrivilegesNoCache();
                    var eventToBeUpdated = copiedPersistenceMechanism.FindContainer<Model.Calendar.Event>().FindOne(e.Id);
                    eventToBeUpdated.TimeZone = System.TimeZoneInfo.Local;
                    eventToBeUpdated.UpdateCascadedly();
                }
            }
            return;
        }

        /// <summary>
        /// Cleans up files without file size set.
        /// </summary>
        private void CleanUpFilesWithoutFileSizeSet() {
            if (this.ContainsContainer<File>()) {
                this.db.DbCommandLogger.Log?.WriteEntry("Searching for files without size set.", LogLevel.Information);
                foreach (var file in this.FindContainer<File>().Find(new FilterCriteria(nameof(File.Size), RelationalOperator.IsEqualTo, null), SortCriterionCollection.Empty)) {
                    if (file.HasBlob) {
                        this.db.DbCommandLogger.Log?.WriteEntry("Updating file size for \"" + file.Name + "\".", LogLevel.Warning);
                        var copiedPersistenceMechanism = this.CopyWithElevatedPrivilegesNoCache();
                        var fileToBeUpdated = copiedPersistenceMechanism.FindContainer<File>().FindOne(file.Id);
                        var bytes = fileToBeUpdated.Bytes;
                        fileToBeUpdated.Bytes = null;
                        fileToBeUpdated.Bytes = bytes;
                        fileToBeUpdated.UpdateCascadedly();
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Cleans up history items without duration set.
        /// </summary>
        private void CleanUpHistoryItemsWithoutDurationSet() {
            if (this.ContainsContainer<BusinessApplications.Workflows.HistoryItem>()) {
                this.db.DbCommandLogger.Log?.WriteEntry("Searching for history items without duration set.", LogLevel.Information);
                var workflowStepSequences = this.FindContainer<BusinessApplications.Workflows.WorkflowStepSequence>();
                var filterCriteria = new FilterCriteria(new string[] { nameof(BusinessApplications.Workflows.WorkflowStepSequence.History), nameof(BusinessApplications.Workflows.HistoryItem.Duration) }, RelationalOperator.IsEqualTo, null);
                var workflowStepSequencesToBeProcessed = workflowStepSequences.Find(filterCriteria, SortCriterionCollection.Empty);
                var processedWorkflowStepSequencesCount = 0;
                foreach (var workflowStepSequence in workflowStepSequencesToBeProcessed) {
                    this.db.DbCommandLogger.Log?.WriteEntry((++processedWorkflowStepSequencesCount).ToString() + '/' + workflowStepSequencesToBeProcessed.Count + ": Setting durations for workflow step sequence instance with ID " + workflowStepSequence.Id + ".", LogLevel.Information);
                    DateTime? previousStepPassedAt = null;
                    foreach (var historyItem in workflowStepSequence.History) {
                        if (previousStepPassedAt.HasValue) {
                            historyItem.Duration = historyItem.PassedAt - previousStepPassedAt.Value;
                        } else {
                            historyItem.Duration = TimeSpan.FromSeconds(1); // to make sure duration value is tracked as changed
                            historyItem.Duration = TimeSpan.Zero;
                        }
                        previousStepPassedAt = historyItem.PassedAt;
                    }
                    workflowStepSequence.UpdateCascadedly();
                }
            }
            return;
        }

        /// <summary>
        /// Copys the permission independent state of a source
        /// instance into this one.
        /// </summary>
        /// <param name="source">source instance to copy permission
        /// independent state from</param>
        protected void CopyPartiallyFrom(RelationalDatabase<TConnection> source) {
            base.CopyPartiallyFrom(source);
            this.CommandTimeoutInSeconds = source.CommandTimeoutInSeconds;
            this.containerNameCache = source.containerNameCache;
            return;
        }

        /// <summary>
        /// Detects elements without allowed groups.
        /// </summary>
        private void DetectElementsWithoutAllowedGroups() {
            this.db.DbCommandLogger.Log?.WriteEntry("Searching for elements without allowed groups.", LogLevel.Information);
            foreach (var containerInfo in this.GetContainerInfos()) {
                var persistentContainer = this.FindContainer(containerInfo.AssemblyQualifiedTypeName);
                var filterCriteria = new FilterCriteria(nameof(PersistentObject.AllowedGroups), RelationalOperator.IsEqualTo, null);
                foreach (var persistentObjectWithoutAllowedGroups in persistentContainer.FindPersistentObjects(null, filterCriteria, SortCriterionCollection.Empty, 0L, ulong.MaxValue)) {
                    this.db.DbCommandLogger.Log?.WriteEntry("Element of type " + containerInfo.AssemblyQualifiedTypeName + " with ID " + persistentObjectWithoutAllowedGroups.Id + " does not have any allowed groups set.", LogLevel.Warning);
                }
            }
            return;
        }

        /// <summary>
        /// Detects whether elements were removed during previous
        /// cleanup steps.
        /// </summary>
        /// <param name="oldInventory">previous inventory of database
        /// to use as reference for cleaned items</param>
        private void DetectRemovedElements(IDictionary<string, int> oldInventory) {
            var newInventory = this.GetInventory();
            var isFirstTypeWithRemovedElements = true;
            foreach (var assemblyQualifiedTypeName in oldInventory.Keys) {
                var oldInventoryItemValue = oldInventory[assemblyQualifiedTypeName];
                var newInventoryItemValue = newInventory[assemblyQualifiedTypeName];
                if (oldInventoryItemValue != newInventoryItemValue) {
                    if (isFirstTypeWithRemovedElements) {
                        isFirstTypeWithRemovedElements = false;
                        this.db.DbCommandLogger.Log?.WriteEntry("Elements were removed during cleanup. Usually this should not happen - please check your application code.", LogLevel.Warning);
                    }
                    var numberOfRemovedItems = oldInventoryItemValue - newInventoryItemValue;
                    this.db.DbCommandLogger.Log?.WriteEntry(numberOfRemovedItems + " element(s) of type " + assemblyQualifiedTypeName + " was/were removed during cleanup.", LogLevel.Warning);
                }
            }
            return;
        }

        /// <summary>
        /// Checks a compositon for suspiciousness.
        /// </summary>
        /// <param name="containerInfo">info about container of
        /// parent object of composition</param>
        /// <param name="fieldContentType">content type of field of
        /// composition</param>
        /// <param name="fieldKey">key of field of composition</param>
        /// <param name="relationsTableName">internal name of database table
        /// containing relations to objects of type of content type
        /// of field of composition</param>
        private void DetectSuspiciousComposition(ContainerInfo containerInfo, System.Type fieldContentType, string fieldKey, string relationsTableName) {
            ulong numberOfObjectsInMoreThanOneComposition = 0;
            var filterCriteria = new FilterCriteria("ChildID", RelationalOperator.IsNotEqualTo, null)
                .And("ParentTable", RelationalOperator.IsEqualTo, containerInfo.InternalName, FilterTarget.IsOtherTextValue)
                .And("ParentField", RelationalOperator.IsEqualTo, fieldKey, FilterTarget.IsOtherTextValue);
            var referenceCountsPerObjects = this.db.CountTableRows(relationsTableName, filterCriteria, new RelationalJoin[0], new string[] { "ChildID" });
            foreach (var referenceCountPerObject in referenceCountsPerObjects) {
                if (referenceCountPerObject.Value > 1) {
                    numberOfObjectsInMoreThanOneComposition++;
                }
            }
            if (numberOfObjectsInMoreThanOneComposition > 0) {
                this.db.DbCommandLogger.Log?.WriteEntry(numberOfObjectsInMoreThanOneComposition + " element(s) of type " + fieldContentType.FullName + " is/are referenced by more than one object of type " + containerInfo.Type.FullName + " in field " + fieldKey + ".", LogLevel.Warning);
            }
            return;
        }

        /// <summary>
        /// Detects suspicious compositons.
        /// </summary>
        private void DetectSuspiciousCompositions() {
            this.db.DbCommandLogger.Log?.WriteEntry("Searching for suspicious compositions.", LogLevel.Information);
            var containerInfos = this.GetContainerInfos();
            foreach (var containerInfo in containerInfos) {
                var sampleInstance = this.CreateInstance(containerInfo.Type);
                foreach (var persistentFieldForPersistentObject in sampleInstance.PersistentFieldsForPersistentObjects) {
                    if (CascadedRemovalBehavior.RemoveValuesForcibly == persistentFieldForPersistentObject.CascadedRemovalBehavior) {
                        var fieldContentType = persistentFieldForPersistentObject.NewItemAsObject().GetType();
                        var relationsTableName = RelationalDatabase.GetViewNameForSingleRelations(this.GetInternalNameOfContainer(fieldContentType));
                        this.DetectSuspiciousComposition(containerInfo, fieldContentType, persistentFieldForPersistentObject.Key, relationsTableName);
                    }
                }
                foreach (var persistentFieldForPersistentObjectCollection in sampleInstance.PersistentFieldsForCollectionsOfPersistentObjects) {
                    if (CascadedRemovalBehavior.RemoveValuesForcibly == persistentFieldForPersistentObjectCollection.CascadedRemovalBehavior) {
                        var fieldContentType = persistentFieldForPersistentObjectCollection.NewItemAsObject().GetType();
                        var relationsTableName = RelationalDatabase.CollectionRelationsTableName;
                        this.DetectSuspiciousComposition(containerInfo, fieldContentType, persistentFieldForPersistentObjectCollection.Key, relationsTableName);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Gets the unique ID of this persistence mechanism.
        /// </summary>
        /// <returns>unique ID of this persistence mechanism</returns>
        protected override string GetId() {
            return this.ConnectionString;
        }

    }

}