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

namespace Framework.Persistence {

    using Framework.Model;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Transactions;

    // Partial class for object operations of persistence mechanism.
    public partial class PersistenceMechanism {

        /// <summary>
        /// Cache for persistent objects.
        /// </summary>
        protected KeyedCollection<Guid, PersistentObject> ObjectCache {
            get {
                return this.objectCache;
            }
        }
        private readonly PersistentObjectCache objectCache = new PersistentObjectCache();

        /// <summary>
        /// Initializer to be used for initialization of persistent
        /// objects.
        /// </summary>
        public IPersistentObjectInitializer PersistentObjectInitializer { get; set; }

        /// <summary>
        /// Adds an object to a container. 
        /// </summary>
        /// <param name="persistentObject">object to add to the
        /// container</param>
        /// <param name="persistentContainer">persistent container to
        /// add object to</param>
        /// <returns>true if object was added to container
        /// successfully, false otherwise</returns>
        internal virtual bool AddObject(PersistentObject persistentObject, PersistentContainer persistentContainer) {
            PersistentContainer container = persistentContainer;
            if (persistentObject.Type != container.ContentType) {
                container = this.FindContainer(persistentObject.Type);
            }
            var potentialBrokenReferences = new List<PersistentObject>();
            var success = this.AddObject(persistentObject, container, true, potentialBrokenReferences);
            if (this.AreReferencesBroken(potentialBrokenReferences)) {
                throw new PersistenceException("Persistent object of type  " + persistentObject.Type + " with ID " + persistentObject.Id + " cannot be added to persistence mechanism because it references persistent objects which do not exist in persistence mechanism.");
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
        protected internal abstract bool AddObject(PersistentObject persistentObject, string internalContainerName, IList<PersistentObject> potentialBrokenReferences);

        /// <summary>
        /// Adds an object to a container. 
        /// </summary>
        /// <param name="persistentObject">object to add to the
        /// container</param>
        /// <param name="persistentContainer">persistent container to
        /// add object to</param>
        /// <param name="isSupposedToHandDownAllowedGroups">true if
        /// allowed groups are supposed to be handed down during add
        /// operations</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        /// <returns>true if object was added to container
        /// successfully, false otherwise</returns>
        private bool AddObject(PersistentObject persistentObject, PersistentContainer persistentContainer, bool isSupposedToHandDownAllowedGroups, IList<PersistentObject> potentialBrokenReferences) {
            bool success;
            var previousCreatedAt = persistentObject.CreatedAt;
            var previousCreatedBy = persistentObject.CreatedBy;
            var previousModifiedAt = persistentObject.ModifiedAt;
            var previousModifiedBy = persistentObject.ModifiedBy;
            var previousPersistentContainer = persistentObject.ParentPersistentContainer;
            var currentTime = UtcDateTime.Now;
            var userForModifications = this.UserDirectory.GetUserForModifications();
            try {
                if (isSupposedToHandDownAllowedGroups) {
                    persistentObject.HandDownAllowedGroups();
                }
                if (!this.HasSupportForIndividualAllowedGroupsOfAllowedGroups && TypeOf.AllowedGroups == persistentObject.Type && persistentObject != persistentObject.AllowedGroups) {
                    throw new PersistenceException("Allowed groups must have themselves as allowed groups. You might consider setting the propery " + nameof(PersistenceMechanism) + '.' + nameof(this.HasSupportForIndividualAllowedGroupsOfAllowedGroups) + " to allow individual allowed groups of allowed groups anyway.");
                }
                if (persistentObject.IsNew) {
                    persistentObject.CreatedAt = currentTime;
                    persistentObject.CreatedBy = userForModifications;
                }
                if (persistentObject.IsNew || persistentObject.IsChanged) {
                    persistentObject.ModifiedAt = currentTime;
                    persistentObject.ModifiedBy = userForModifications;
                }
                success = this.AddObject(persistentObject, persistentContainer.InternalName, potentialBrokenReferences);
                if (success) {
                    if (null == previousPersistentContainer) {
                        // persistent object is new
                        persistentObject.ParentPersistentContainer = persistentContainer;
                        persistentObject.SetIsChangedToFalse();
                        if (!this.ObjectCache.Contains(persistentObject.Id)) {
                            this.ObjectCache.Add(persistentObject);
                        }
                    } else {
                        // persistent object exists in another persistence mechanism already
                        persistentObject = persistentContainer.FindOnePersistentObject(persistentObject.Id);
                    }
                }
            } catch (Exception) {
                persistentObject.CreatedAt = previousCreatedAt;
                persistentObject.CreatedBy = previousCreatedBy;
                persistentObject.ModifiedAt = previousModifiedAt;
                persistentObject.ModifiedBy = previousModifiedBy;
                persistentObject.ParentPersistentContainer = previousPersistentContainer;
                throw;
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
        internal virtual void AddObjectCascadedly(PersistentObject persistentObject, PersistentContainer persistentContainer) {
            PersistentContainer container = persistentContainer;
            if (persistentObject.Type != container.ContentType) {
                container = this.FindContainer(persistentObject.Type);
            }
            var potentialBrokenReferences = new List<PersistentObject>();
            this.AddObjectCascadedly(persistentObject, container, new List<Guid>(), potentialBrokenReferences);
            if (this.AreReferencesBroken(potentialBrokenReferences)) {
                throw new PersistenceException("Persistent object of type " + persistentObject.Type + " with ID " + persistentObject.Id + " cannot be added to persistence mechanism because it or child objects of it reference persistent objects which do not exist in persistence mechanism.");
            }
            return;
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
        /// <param name="idsOfCheckedObjects">IDs of objects that
        /// have already been checked during this recursion</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        private void AddObjectCascadedly(PersistentObject persistentObject, PersistentContainer persistentContainer, List<Guid> idsOfCheckedObjects, List<PersistentObject> potentialBrokenReferences) {
            if (null != persistentObject) {
                if (persistentObject.IsNew || (persistentObject.ParentPersistentContainer.ParentPersistenceMechanism != this && !persistentContainer.Contains(persistentObject.Id))) {
                    // if (!persistentObject.IsNew) System.Diagnostics.Debug.WriteLine("Warning: Persistent object to add of type " + persistentObject.Type + " with ID " + persistentObject.ID + " already exists in another persistence mechanism.");
                    // add cascadedly
                    if (!idsOfCheckedObjects.Contains(persistentObject.Id)) {
                        persistentObject.HandDownAllowedGroups();
                        bool thisIsARecursiveCallOfThisMethod = 0 != idsOfCheckedObjects.Count;
                        idsOfCheckedObjects.Add(persistentObject.Id);
                        foreach (var fieldForElement in persistentObject.PersistentFieldsForPersistentObjects) {
                            var childObject = fieldForElement.ValueAsPersistentObject;
                            PersistenceMechanism.ValidateSecurityModelOf(persistentObject, childObject);
                            this.AddObjectCascadedly(childObject, persistentContainer, idsOfCheckedObjects, potentialBrokenReferences);
                        }
                        foreach (var fieldForList in persistentObject.PersistentFieldsForCollectionsOfPersistentObjects) {
                            foreach (var childObject in fieldForList.GetValuesAsPersistentObject()) {
                                PersistenceMechanism.ValidateSecurityModelOf(persistentObject, childObject);
                                this.AddObjectCascadedly(childObject, persistentContainer, idsOfCheckedObjects, potentialBrokenReferences);
                            }
                        }
                        PersistentContainer container = persistentContainer;
                        if (thisIsARecursiveCallOfThisMethod) {
                            container = this.FindContainer(persistentObject.Type);
                        }
                        if (!container.Contains(persistentObject.Id)) {
                            if (!this.AddObject(persistentObject, container, false, potentialBrokenReferences)) {
                                throw new PersistenceException("Persistent object of type " + persistentObject.Type + " with ID " + persistentObject.Id + " could not be added to persistence mechanism.");
                            }
                        }
                    }
                } else {
                    // update cascadedly
                    this.UpdateObjectCascadedly(persistentObject, persistentContainer, idsOfCheckedObjects, potentialBrokenReferences);
                }
            }
            return;
        }

        /// <summary>
        /// Adds a specific persistent object as version.
        /// </summary>
        /// <param name="versionValue">persistent object to add as
        /// version - all relations will be removed</param>
        private void AddVersion(PersistentObject versionValue) {
            if (null != versionValue) {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Suppress)) {
                    Guid sourceId = versionValue.Id;
                    versionValue.Id = Guid.NewGuid();
                    foreach (var versionField in versionValue.PersistentFieldsForPersistentObjects) {
                        try {
                            versionField.ValueAsObject = null; // ignore relations
                        } catch (Exception) {
                            // ignore exceptions
                        }
                    }
                    foreach (var versionField in versionValue.PersistentFieldsForCollectionsOfPersistentObjects) {
                        try {
                            versionField.Clear(); // ignore relations
                        } catch (Exception) {
                            // ignore exceptions
                        }
                    }
                    var versioningRepository = this.VersioningRepository;
                    var versionContainer = versioningRepository.FindContainer<Model.Version>();
                    var filterCriteria = new FilterCriteria(nameof(Model.Version.ModifiedAt), RelationalOperator.IsEqualTo, versionValue.ModifiedAt);
                    if (!versionContainer.Contains(filterCriteria)) {
                        var valueContainer = versioningRepository.FindContainer(versionValue.Type);
                        versioningRepository.AddObject(versionValue, valueContainer.InternalName, new List<PersistentObject>());
                        var version = new Model.Version(sourceId, versionValue.Id);
                        version.CreatedAt = version.ModifiedAt = versionValue.ModifiedAt;
                        version.CreatedBy = version.ModifiedBy = versionValue.ModifiedBy;
                        versioningRepository.AddObject(version, versionContainer.InternalName, new List<PersistentObject>());
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Indicates whether a list of potential broken references
        /// contains actual broken references.
        /// </summary>
        /// <param name="potentialBrokenReferences">list of potential
        /// broken references to check</param>
        /// <returns>true if broken references are contained in list,
        /// false otherwise</returns>
        private bool AreReferencesBroken(List<PersistentObject> potentialBrokenReferences) {
            bool areReferencesBroken = false;
            foreach (var potentialBrokenReference in potentialBrokenReferences) {
                if (potentialBrokenReference.IsNew && !potentialBrokenReference.IsRemoved && !this.FindContainer(potentialBrokenReference.Type).Contains(potentialBrokenReference.Id)) {
                    areReferencesBroken = true;
                    break;
                }
            }
            return areReferencesBroken;
        }

        /// <summary>
        /// Clears the object cache. Be aware that all items that
        /// have been in the cache of this persistence mechanism
        /// context won't be connected to any persistence mechanism
        /// context in memory any more afterwards. This method should
        /// only be used in exceptional cases, e. g. if out-of-memory
        /// exceptions occure while working with large sets of data.
        /// </summary>
        public void ClearObjectCache() {
            var previouslyCachedItems = new List<PersistentObject>(this.ObjectCache);
            this.ObjectCache.Clear();
            foreach (var previouslyCachedItem in previouslyCachedItems) {
                previouslyCachedItem.ParentPersistentContainer = null;
            }
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
        internal abstract bool ContainsID(string internalContainerName, Guid id);

        /// <summary>
        /// Checks the existence of references to a persistent object
        /// regardless of permissions.
        /// </summary>
        /// <param name="persistentObject">persistent object to check
        /// existence of references to for</param>
        /// <returns>true if references to persistent object exist,
        /// false otherwise</returns>
        internal abstract bool ContainsReferencingPersistentObjectsTo(PersistentObject persistentObject);

        /// <summary>
        /// Counts all objects of a certain type in persistence
        /// mechanism - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <returns>number of objects of type</returns>
        internal int CountObjects(string internalContainerName) {
            return this.CountObjects<PersistentObject>(internalContainerName, FilterCriteria.Empty);
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
        internal abstract int CountObjects<T>(string internalContainerName, FilterCriteria filterCriteria) where T : PersistentObject, new();

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
        internal abstract IDictionary<string[], int> CountObjects<T>(string internalContainerName, FilterCriteria filterCriteria, string[] groupBy) where T : PersistentObject, new();

        /// <summary>
        /// Creates a new instance of a persistent object.
        /// </summary>
        /// <param name="type">type of persistent object to create
        /// instance of</param>
        /// <returns>new instance of a persistent object</returns>
        internal PersistentObject CreateInstance(Type type) {
            var persistentObject = Activator.CreateInstance(type) as PersistentObject;
            this.PersistentObjectInitializer?.InitializeInstance(persistentObject);
            return persistentObject;
        }

        /// <summary>
        /// Creates a new instance of a persistent object.
        /// </summary>
        /// <returns>new instance of a persistent object</returns>
        /// <typeparam name="T">type of persistent object to create
        /// instance of</typeparam>
        internal T CreateInstance<T>() where T : PersistentObject, new() {
            var persistentObject = new T();
            this.PersistentObjectInitializer?.InitializeInstance(persistentObject);
            return persistentObject;
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
        /// <returns>matching persistent objects from persistence
        /// mechanism</returns>
        /// <typeparam name="T">type of persistent object to find</typeparam>
        internal abstract ReadOnlyCollection<T> Find<T>(string internalContainerName, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) where T : PersistentObject, new();

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
        internal abstract ReadOnlyCollection<object> FindAverageValues<T>(string internalContainerName, FilterCriteria filterCriteria, string[] fieldNames) where T : PersistentObject, new();

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
        internal abstract ReadOnlyCollection<T> FindComplement<T>(string internalContainerName, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) where T : PersistentObject, new();

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
        internal abstract ReadOnlyCollection<object[]> FindDistinctValues<T>(string internalContainerName, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string[] fieldNames) where T : PersistentObject, new();

        /// <summary>
        /// Gets the first matching persistent object from
        /// persistence mechanism - including containers of sub
        /// types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>matching persistent objects from persistence
        /// mechanism</returns>
        /// <typeparam name="T">type of persistent object to find</typeparam>
        internal T FindOne<T>(string internalContainerName, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria) where T : PersistentObject, new() {
            T match = null;
            if (filterCriteria.IsLastFilterOfChain && nameof(PersistentObject.Id) == filterCriteria.FieldName && RelationalOperator.IsEqualTo == filterCriteria.RelationalOperator && TypeOf.Guid == filterCriteria.ContentBaseType) {
                var id = (Guid)filterCriteria.ValueOrOtherFieldName;
                if (this.ObjectCache.Contains(id)) {
                    match = this.ObjectCache[id] as T;
                }
            }
            if (null == match) {
                var results = this.Find<T>(internalContainerName, null, filterCriteria, sortCriteria, 0, 1);
                if (results.Count > 0) {
                    match = results[0];
                }
            }
            return match;
        }

        /// <summary>
        /// Finds the first persistent object of complement set of
        /// matching objects from this container - including
        /// containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>matching persistent objects from persistence
        /// mechanism</returns>
        /// <typeparam name="T">type of persistent object to find</typeparam>
        internal T FindOneComplement<T>(string internalContainerName, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria) where T : PersistentObject, new() {
            T match = null;
            var results = this.FindComplement<T>(internalContainerName, null, filterCriteria, sortCriteria, 0, 1);
            if (results.Count > 0) {
                match = results[0];
            }
            return match;
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
        internal abstract ReadOnlyCollection<object> FindSumsOfValues<T>(string internalContainerName, FilterCriteria filterCriteria, string[] fieldNames) where T : PersistentObject, new();

        /// <summary>
        /// Gets all elevated initial child objects of a persistent
        /// object to be removed.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// remove initial values of</param>
        /// <returns>elevated initial child objects of persistent
        /// object to be removed</returns>
        private IEnumerable<PersistentObject> GetElevatedInitialValuesToBeRemoved(PersistentObject persistentObject) {
            var elevatedInitialValues = new List<PersistentObject>();
            bool hasChangedFieldsForPersistentObjects = false;
            foreach (var persistentField in persistentObject.PersistentFieldsForPersistentObjects) {
                if (persistentField.IsChanged) {
                    hasChangedFieldsForPersistentObjects = true;
                    break;
                }
            }
            if (!hasChangedFieldsForPersistentObjects) {
                foreach (var persistentField in persistentObject.PersistentFieldsForCollectionsOfPersistentObjects) {
                    if (persistentField.IsChanged) {
                        hasChangedFieldsForPersistentObjects = true;
                        break;
                    }
                }
            }
            if (hasChangedFieldsForPersistentObjects) {
                var elevatedPersistenceMechanism = persistentObject.ParentPersistentContainer?.ParentPersistenceMechanism.CopyWithElevatedPrivilegesNoCache(); // it is important to suppress caching here to avoid side effects
                var elevatedPersistentObject = elevatedPersistenceMechanism?.FindContainer(persistentObject.Type.AssemblyQualifiedName).FindOnePersistentObject(persistentObject.Id);
                if (null != elevatedPersistentObject) {
                    foreach (var persistentField in persistentObject.PersistentFieldsForPersistentObjects) {
                        if (persistentField.IsChanged) {
                            foreach (var elevatedPersistentField in elevatedPersistentObject.PersistentFieldsForPersistentObjects) {
                                if (elevatedPersistentField.Key == persistentField.Key) {
                                    elevatedPersistentField.ValueAsObject = persistentField.ValueAsObject;
                                    var elevatedInitialValue = elevatedPersistentField.GetInitialValueToBeRemovedIfNotReferencedOnRemovalOfParent();
                                    if (null != elevatedInitialValue) {
                                        elevatedInitialValues.Add(elevatedInitialValue);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    foreach (var persistentField in persistentObject.PersistentFieldsForCollectionsOfPersistentObjects) {
                        if (persistentField.IsChanged) {
                            foreach (var elevatedPersistentField in elevatedPersistentObject.PersistentFieldsForCollectionsOfPersistentObjects) {
                                if (elevatedPersistentField.Key == persistentField.Key) {
                                    elevatedPersistentField.Clear();
                                    foreach (var value in persistentField.GetValuesAsObject()) {
                                        elevatedPersistentField.AddObject(value);
                                    }
                                    foreach (var elevatedInitialValue in elevatedPersistentField.GetInitialValuesToBeRemovedIfNotReferencedOnRemovalOfParent()) {
                                        if (null != elevatedInitialValue) {
                                            elevatedInitialValues.Add(elevatedInitialValue);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return elevatedInitialValues;
        }

        /// <summary>
        /// Gets all references to a persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to get
        /// references to for</param>
        /// <returns>all references to persistent object</returns>
        internal abstract ReadOnlyCollection<PersistentObject> GetReferencingPersistentObjectsTo(PersistentObject persistentObject);

        /// <summary>
        /// Gets reordered persistent fields for persistent objects
        /// with persistent field for allowed groups at last
        /// position.
        /// </summary>
        /// <param name="persistentFieldsForPersistentObjects">
        /// persistent fields for persistent objects to reorder</param>
        /// <returns>reordered persistent fields for persistent
        /// objects with persistent field for allowed groups at last
        /// position</returns>
        private static IEnumerable<PersistentFieldForPersistentObject> GetReordered(IEnumerable<PersistentFieldForPersistentObject> persistentFieldsForPersistentObjects) {
            PersistentFieldForPersistentObject persistentFieldForAllowedGroups = null;
            foreach (var persistentFieldForPersistentObject in persistentFieldsForPersistentObjects) {
                if (nameof(PersistentObject.AllowedGroups) == persistentFieldForPersistentObject.Key) {
                    persistentFieldForAllowedGroups = persistentFieldForPersistentObject;
                } else {
                    yield return persistentFieldForPersistentObject;
                }
            }
            if (null != persistentFieldForAllowedGroups) {
                yield return persistentFieldForAllowedGroups;
            }
        }

        /// <summary>
        /// Determines whether an object with a specific ID is
        /// deleted. Permissions are ignored.
        /// </summary>
        /// <param name="id">specific ID to look for</param>
        /// <returns>true if object with specific ID is deleted,
        /// false otherwise</returns>
        public abstract bool IsIdDeleted(Guid id);

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
        internal abstract void PreloadObjects(PersistentObject sampleObject, IEnumerable<PersistentObject> persistentObjects, string internalContainerName, IEnumerable<string[]> keyChains);

        /// <summary>
        /// Removes all temporary files of current user or older than
        /// a one day from persistence mechanism.
        /// </summary>
        public void RemoveExpiredTemporaryFiles() {
            if (SecurityModel.IgnorePermissions != this.SecurityModel) {
                this.CopyWithElevatedPrivileges().RemoveExpiredTemporaryFiles();
            } else {
                if (this.ContainsContainer<TemporaryFile>()) {
                    var container = this.FindContainer<TemporaryFile>();
                    var filterCriteria = new FilterCriteria(nameof(TemporaryFile.CreatedBy), RelationalOperator.IsEqualTo, this.UserDirectory.CurrentUser)
                        .Or(nameof(TemporaryFile.CreatedAt), RelationalOperator.IsLessThan, UtcDateTime.Now.Subtract(TimeSpan.FromDays(1D)));
                    var temporaryFiles = container.Find(filterCriteria, SortCriterionCollection.Empty);
                    foreach (var temporaryFile in temporaryFiles) {
                        temporaryFile.RemoveCascadedly();
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Removes a specific object from a container immediately.
        /// </summary>
        /// <param name="persistentObject">specific object to remove
        /// from the container</param>
        /// <param name="isToBeRemovedIfNotReferencedOnly">true if
        /// object is to be removed if not referenced by other
        /// objects only, false otherwise</param>
        /// <returns>true if object was removed successfully from
        /// container, null if it was not removed because it is
        /// referenced by other objects, false otherwise or if object
        /// was not contained in persistence mechanism</returns>
        internal bool? RemoveObject(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly) {
            bool? success = this.RemoveObject(persistentObject, isToBeRemovedIfNotReferencedOnly, persistentObject.ParentPersistentContainer.InternalName);
            if (true == success) {
                persistentObject.IsRemoved = true;
                if (this.ObjectCache.Contains(persistentObject.Id)) {
                    this.ObjectCache.Remove(persistentObject.Id);
                }
                this.RemoveVersionsOf(persistentObject);
            }
            return success;
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
        protected internal abstract bool? RemoveObject(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly, string internalContainerName);

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
        internal virtual bool? RemoveObjectCascadedly(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly) {
            return this.RemoveObjectCascadedly(persistentObject, isToBeRemovedIfNotReferencedOnly, new List<object>(0));
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
        /// <param name="elevatedObjectsToBeProcessedByParent">
        /// elevated objects that will be processed by parent object
        /// and can be ignored for conditional cascaded removal for
        /// now</param>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, null if it was not removed because
        /// it is referenced by other objects, false otherwise or if
        /// object was not contained in persistence mechanism</returns>
        private bool? RemoveObjectCascadedly(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly, IList<object> elevatedObjectsToBeProcessedByParent) {
            bool? success = true;
            // the later comparison to objects to be ignored for conditional removal is a performance
            // optimization to avoid redundant checks of objects for being referenced by other objects
            var elevatedObjectsToBeIgnoredForConditionalRemoval = new List<object>(elevatedObjectsToBeProcessedByParent);
            var elevatedPersistentObject = persistentObject.GetWithElevatedPrivileges();
            if (null != elevatedPersistentObject) {
                // fields for elements may not be retrieved, because retrieval of users in persistent user directories in SQL servers would
                // cancel the transaction if multiple active result sets are not enabled, which should be the case in production environments
                foreach (var persistentField in elevatedPersistentObject.PersistentFieldsForPersistentObjects) {
                    persistentField.Retrieve();
                    elevatedObjectsToBeIgnoredForConditionalRemoval.Add(persistentField.ValueAsObject);
                }
                foreach (var persistentField in elevatedPersistentObject.PersistentFieldsForCollectionsOfPersistentObjects) {
                    persistentField.Retrieve();
                    elevatedObjectsToBeIgnoredForConditionalRemoval.AddRange(persistentField.GetValuesAsObject());
                }
            }
            if (isToBeRemovedIfNotReferencedOnly) {
                success = persistentObject.RemoveIfNotReferenced();
            } else {
                success = persistentObject.Remove();
            }
            if (true == success) {
                if (null != elevatedPersistentObject) {
                    elevatedPersistentObject.IsRemoved = true;
                    // values of fields for collections of persistent objects have to be removed first...
                    foreach (var persistentField in elevatedPersistentObject.PersistentFieldsForCollectionsOfPersistentObjects) {
                        foreach (var value in persistentField.GetValuesAsPersistentObject()) {
                            elevatedObjectsToBeIgnoredForConditionalRemoval.Remove(value);
                            if (null != value && !value.IsNew) {
                                if (CascadedRemovalBehavior.RemoveValuesForcibly == persistentField.CascadedRemovalBehavior) {
                                    success = true == success && true == this.RemoveObjectCascadedly(value, false, elevatedObjectsToBeIgnoredForConditionalRemoval);
                                } else if (CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced == persistentField.CascadedRemovalBehavior && !elevatedObjectsToBeIgnoredForConditionalRemoval.Contains(value)) {
                                    success = true == success && false != this.RemoveObjectCascadedly(value, true, elevatedObjectsToBeIgnoredForConditionalRemoval);
                                }
                            }
                        }
                    }
                    // ...and persistent fields need to be reordered to make sure that the allowed groups are removed last for performance reasons
                    foreach (var persistentField in PersistenceMechanism.GetReordered(elevatedPersistentObject.PersistentFieldsForPersistentObjects)) {
                        var value = persistentField.ValueAsPersistentObject;
                        elevatedObjectsToBeIgnoredForConditionalRemoval.Remove(value);
                        if (null != value && !value.IsNew) {
                            if (CascadedRemovalBehavior.RemoveValuesForcibly == persistentField.CascadedRemovalBehavior) {
                                success = true == success && true == this.RemoveObjectCascadedly(value, false, elevatedObjectsToBeIgnoredForConditionalRemoval);
                            } else if (CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced == persistentField.CascadedRemovalBehavior && !elevatedObjectsToBeIgnoredForConditionalRemoval.Contains(value)) {
                                success = true == success && false != this.RemoveObjectCascadedly(value, true, elevatedObjectsToBeIgnoredForConditionalRemoval);
                            }
                        }
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Removes all versions of a specific persistent object.
        /// </summary>
        /// <param name="persistentObject">specific persistent object
        /// to remove all versions for</param>
        private void RemoveVersionsOf(PersistentObject persistentObject) {
            if (this.HasVersioningEnabled) {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Suppress)) {
                    var versionContainer = this.VersioningRepository.FindContainer<Model.Version>();
                    var versionValueContainer = this.VersioningRepository.FindContainer(persistentObject.Type);
                    var filterCriteria = new FilterCriteria(nameof(Model.Version.SourceId), RelationalOperator.IsEqualTo, persistentObject.Id);
                    var versions = versionContainer.Find(filterCriteria, SortCriterionCollection.Empty);
                    foreach (var version in versions) {
                        var versionValue = versionValueContainer.FindOnePersistentObject(version.ValueId);
                        if (null != versionValue) {
                            versionValue.Remove();
                        }
                        version.Remove();
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
        internal abstract void RetrieveFieldsForElements(PersistentObject persistentObject, string internalContainerName);

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// elements from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal abstract void RetrieveFieldForCollectionOfElements(PersistentFieldForCollection persistentField, string internalContainerName);

        /// <summary>
        /// Retrieves the state of a field for a persistent object
        /// from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal abstract void RetrieveFieldForPersistentObject(PersistentFieldForPersistentObject persistentField, string internalContainerName);

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// persistent objects from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal abstract void RetrieveFieldForCollectionOfPersistentObjects(PersistentFieldForPersistentObjectCollection persistentField, string internalContainerName);

        /// <summary>
        /// Retrieves the state of a persistent object from
        /// persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal abstract void RetrieveObject(PersistentObject persistentObject, string internalContainerName);

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
        internal virtual bool UpdateObject(PersistentObject persistentObject, PersistentContainer persistentContainer) {
            var potentialBrokenReferences = new List<PersistentObject>();
            var success = this.UpdateObject(persistentObject, persistentContainer, true, potentialBrokenReferences);
            if (this.AreReferencesBroken(potentialBrokenReferences)) {
                throw new PersistenceException("Persistent object of type " + persistentObject.Type + " with ID " + persistentObject.Id + " cannot be updated in persistence mechanism because it references persistent objects which do not exist in persistence mechanism.");
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
        protected internal abstract bool UpdateObject(PersistentObject persistentObject, string internalContainerName, IList<PersistentObject> potentialBrokenReferences);

        /// <summary>
        /// Updates a specific object in a container.
        /// </summary>
        /// <param name="persistentObject">specific object to update
        /// in container</param>
        /// <param name="persistentContainer">persistentContainer to
        /// update object in</param>
        /// <param name="isSupposedToHandDownAllowedGroups">true if
        /// allowed groups are supposed to be handed down during add
        /// operations</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        private bool UpdateObject(PersistentObject persistentObject, PersistentContainer persistentContainer, bool isSupposedToHandDownAllowedGroups, IList<PersistentObject> potentialBrokenReferences) {
            bool success;
            if (RemovalType.RemoveCascadedly == persistentObject.RemoveOnUpdate) {
                success = true == this.RemoveObjectCascadedly(persistentObject, false);
            } else if (RemovalType.Remove == persistentObject.RemoveOnUpdate) {
                success = persistentObject.Remove();
            } else {
                if (isSupposedToHandDownAllowedGroups) {
                    persistentObject.HandDownAllowedGroups();
                }
                if (!this.HasSupportForIndividualAllowedGroupsOfAllowedGroups && TypeOf.AllowedGroups == persistentObject.Type && persistentObject != persistentObject.AllowedGroups) {
                    throw new PersistenceException("Allowed groups must have themselves as allowed groups. You might consider setting the propery " + nameof(PersistenceMechanism) + '.' + nameof(this.HasSupportForIndividualAllowedGroupsOfAllowedGroups) + " to allow individual allowed groups of allowed groups anyway.");
                }
                if (persistentObject.IsChanged) {
                    var previousModifiedAt = persistentObject.ModifiedAt;
                    var previousModifiedBy = persistentObject.ModifiedBy;
                    try {
                        PersistentObject versionValue;
                        if (this.HasVersioningEnabled) {
                            versionValue = this.CopyWithElevatedPrivilegesNoCache().FindContainer(persistentObject.Type).FindOnePersistentObject(persistentObject.Id);
                            if (null != versionValue) {
                                versionValue.Retrieve();
                            }
                        } else {
                            versionValue = null;
                        }
                        persistentObject.ModifiedAt = UtcDateTime.Now;
                        persistentObject.ModifiedBy = this.UserDirectory.GetUserForModifications();
                        success = this.UpdateObject(persistentObject, persistentContainer.InternalName, potentialBrokenReferences);
                        if (success) {
                            persistentObject.SetIsChangedToFalse();
                            this.AddVersion(versionValue);
                        }
                    } catch (Exception) {
                        persistentObject.ModifiedAt = previousModifiedAt;
                        persistentObject.ModifiedBy = previousModifiedBy;
                        throw;
                    }
                } else {
                    success = true;
                }
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
        internal virtual void UpdateObjectCascadedly(PersistentObject persistentObject, PersistentContainer persistentContainer) {
            bool isUpdateForced = !persistentObject.IsNew && !persistentObject.IsChanged && persistentObject.GetIsChangedCascededly();
            var potentialBrokenReferences = new List<PersistentObject>();
            this.UpdateObjectCascadedly(persistentObject, persistentContainer, new List<Guid>(), potentialBrokenReferences);
            if (this.AreReferencesBroken(potentialBrokenReferences)) {
                throw new PersistenceException("Persistent object of type " + persistentObject.Type + " with ID " + persistentObject.Id + " cannot be updated in persistence mechanism because it or child objects of it reference persistent objects which do not exist in persistence mechanism.");
            }
            if (isUpdateForced) { // make sure modification info of parent object is updated if child objects were updated
                var elevatedPersistenceMechanism = this.CopyWithElevatedPrivilegesNoCache();
                var elevatedPersistentContainer = elevatedPersistenceMechanism.FindContainer(persistentObject.Type);
                var elevatedPersistentObject = elevatedPersistentContainer.FindOnePersistentObject(persistentObject.Id);
                if (null != elevatedPersistentObject) {
                    elevatedPersistentObject.ModifiedAt = UtcDateTime.Now;
                    elevatedPersistentObject.ModifiedBy = this.UserDirectory.CurrentUser;
                    elevatedPersistenceMechanism.UpdateObject(elevatedPersistentObject, elevatedPersistentContainer, true, potentialBrokenReferences);
                }
            }
            return;
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
        /// <param name="idsOfCheckedObjects">IDs of objects that
        /// have already been checked during this recursion</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        private void UpdateObjectCascadedly(PersistentObject persistentObject, PersistentContainer persistentContainer, List<Guid> idsOfCheckedObjects, List<PersistentObject> potentialBrokenReferences) {
            if (persistentObject.IsNew) {
                // add cascadedly
                this.AddObjectCascadedly(persistentObject, persistentContainer, idsOfCheckedObjects, potentialBrokenReferences);
            } else {
                // update cascadedly
                if (persistentObject.IsRetrievedPartially && !idsOfCheckedObjects.Contains(persistentObject.Id)) {
                    persistentObject.HandDownAllowedGroups();
                    bool thisIsARecursiveCallOfThisMethod = 0 != idsOfCheckedObjects.Count;
                    idsOfCheckedObjects.Add(persistentObject.Id);
                    foreach (var fieldForElement in persistentObject.PersistentFieldsForPersistentObjects) {
                        var childObject = fieldForElement.ValueAsPersistentObject;
                        if (null != childObject) {
                            PersistenceMechanism.ValidateSecurityModelOf(persistentObject, childObject);
                            this.UpdateObjectCascadedly(childObject, persistentContainer, idsOfCheckedObjects, potentialBrokenReferences);
                        }
                    }
                    foreach (var fieldForList in persistentObject.PersistentFieldsForCollectionsOfPersistentObjects) {
                        foreach (var childObject in fieldForList.GetValuesAsPersistentObject()) {
                            if (null != childObject) {
                                PersistenceMechanism.ValidateSecurityModelOf(persistentObject, childObject);
                                this.UpdateObjectCascadedly(childObject, persistentContainer, idsOfCheckedObjects, potentialBrokenReferences);
                            }
                        }
                    }
                    PersistentContainer container = persistentContainer;
                    if (thisIsARecursiveCallOfThisMethod) {
                        container = this.FindContainer(persistentObject.Type);
                    }
                    var elevatedInitialValuesToBeRemoved = this.GetElevatedInitialValuesToBeRemoved(persistentObject);
                    if (!persistentObject.IsRemoved && !this.UpdateObject(persistentObject, container, false, potentialBrokenReferences)) {
                        if (null == persistentObject.ParentPersistentContainer) {
                            throw new PersistenceException("Persistent object of type " + persistentObject.Type + " with ID " + persistentObject.Id + " could not be updated because it is not connected to a persistence mechanism.");
                        } else if (persistentObject.ParentPersistentContainer.ParentPersistenceMechanism == this) {
                            throw new PersistenceException("Persistent object of type " + persistentObject.Type + " with ID " + persistentObject.Id + " could not be updated in persistence mechanism. Please make sure the related object is not write protected and that no other circumstances keep it from being updated.");
                        } else {
                            throw new PersistenceException("Persistent object of type " + persistentObject.Type + " with ID " + persistentObject.Id + " could not be updated, because it is connected to another persistence mechanism.");
                        }
                    } else {
                        foreach (var elevatedInitialValueToBeRemoved in elevatedInitialValuesToBeRemoved) {
                            if (!elevatedInitialValueToBeRemoved.IsRemoved) {
                                elevatedInitialValueToBeRemoved.ParentPersistentContainer.ParentPersistenceMechanism.RemoveObjectCascadedly(elevatedInitialValueToBeRemoved, true);
                            }
                        }
                    }
                }
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
        internal abstract bool UpdateObjectCreatedAt(PersistentObject persistentObject, DateTime createdAt);

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
        internal abstract bool UpdateObjectCreatedBy(PersistentObject persistentObject, IUser createdBy);

        /// <summary>
        /// Validates whether the underlying security models are
        /// compatible to each other.
        /// </summary>
        /// <param name="a">first persistent object to check security
        /// model for</param>
        /// <param name="b">second persistent object to check
        /// security model for</param>
        private static void ValidateSecurityModelOf(PersistentObject a, PersistentObject b) {
            if (null != a?.ParentPersistentContainer?.ParentPersistenceMechanism
                && null != b?.ParentPersistentContainer?.ParentPersistenceMechanism
                && a.ParentPersistentContainer.ParentPersistenceMechanism.SecurityModel != b.ParentPersistentContainer.ParentPersistenceMechanism.SecurityModel) {
                throw new PersistenceException("Persistent objects cannot be updated cascadedly because they use different security models.");
            }
            return;
        }

    }

}