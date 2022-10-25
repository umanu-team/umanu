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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Base class for containers for persistent objects.
    /// </summary>
    public class PersistentContainer {

        /// <summary>
        /// Type name including assembly and namespace of objects
        /// contained in the container.
        /// </summary>
        public string AssemblyQualifiedTypeName { get; private set; }

        /// <summary>
        /// Gets the number of objects contained in the container -
        /// including containers of sub types.
        /// </summary>
        public int Count {
            get {
                return this.ParentPersistenceMechanism.CountObjects(this.InternalName);
            }
        }

        /// <summary>
        /// Type of objects contained in the container.
        /// </summary>
        public Type ContentType {
            get {
                if (null == this.contentType) {
                    this.contentType = Type.GetType(this.AssemblyQualifiedTypeName, true);
                }
                return this.contentType;
            }
        }
        private Type contentType = null;

        /// <summary>
        /// Internal name of this container in persistence mechanism
        /// (e.g. the name of the related SQL database table).
        /// </summary>
        protected internal string InternalName {
            get {
                if (null == this.internalName) {
                    this.internalName = this.ParentPersistenceMechanism.GetInternalNameOfContainer(this.AssemblyQualifiedTypeName);
                    if (string.IsNullOrEmpty(this.internalName)) {
                        throw new InvalidOperationException("Container for storing persistent objects of type \""
                            + this.AssemblyQualifiedTypeName + "\" does not exist in persistence mechanism.");
                    }
                }
                return this.internalName;
            }
        }
        private string internalName = null;

        /// <summary>
        /// Gets a value indicating whether the container is
        /// read-only.
        /// </summary>
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <summary>
        /// Parent persistence mechanism.
        /// </summary>
        public PersistenceMechanism ParentPersistenceMechanism { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPersistenceMechanism">parent
        /// persistence mechanism</param>
        /// <param name="assemblyQualifiedTypeName">assembly qualified type name
        /// of objects contained in the container</param>
        protected PersistentContainer(PersistenceMechanism parentPersistenceMechanism, string assemblyQualifiedTypeName) {
            this.ParentPersistenceMechanism = parentPersistenceMechanism;
            this.AssemblyQualifiedTypeName = assemblyQualifiedTypeName;
        }

        /// <summary>
        /// Determines whether the container contains a specific
        /// object - including containers of sub types. Permissions
        /// are ignored.
        /// </summary>
        /// <param name="id">specific ID to find</param>
        /// <returns>true if specific ID is contained, false
        /// otherwise</returns>
        public bool Contains(Guid id) {
            return this.ParentPersistenceMechanism.ContainsID(this.InternalName, id);
        }

        /// <summary>
        /// Creates a new instance of a persistent object of this
        /// container with the specified ID.
        /// </summary>
        /// <param name="id">ID to create object for</param>
        /// <returns>new instance of a persistent object of this
        /// container with the specified ID</returns>
        internal virtual PersistentObject CreateInstance(Guid id) {
            throw new PersistenceException("Instance cannot be created by non-generic container.");
        }

        /// <summary>
        /// Finds the first matching persistent object from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="id">ID to find object for</param>
        /// <returns>first matching object from this container or
        /// null if no match was found</returns>
        public PersistentObject FindOnePersistentObject(Guid id) {
            FilterCriteria filterCriteria =
                new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, id);
            SortCriterionCollection sortCriteria = new SortCriterionCollection {
                { nameof(PersistentObject.ModifiedAt), SortDirection.Descending }
            };
            return this.FindOnePersistentObject(filterCriteria, sortCriteria);
        }

        /// <summary>
        /// Finds the first matching persistent object from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="id">ID to find object for</param>
        /// <returns>first matching object from this container or
        /// null if no match was found</returns>
        public PersistentObject FindOnePersistentObject(string id) {
            if (!Guid.TryParse(id, out Guid guid)) {
                throw new FormatException("Format of ID \"" + id + "\" is not valid.");
            }
            return this.FindOnePersistentObject(guid);
        }

        /// <summary>
        /// Finds the first matching persistent object from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// object for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>first matching object from this container or
        /// null if no match was found</returns>
        public PersistentObject FindOnePersistentObject(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria) {
            return this.ParentPersistenceMechanism.FindOne<PersistentObject>(this.InternalName, filterCriteria, sortCriteria);
        }

        /// <summary>
        /// Finds a specific version of a specific persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to find
        /// specific version for</param>
        /// <param name="modificationDate">modification date/time of
        /// version to find</param>
        /// <returns>specific version of specific persistent object</returns>
        internal Model.Version FindOneVersion(PersistentObject persistentObject, DateTime modificationDate) {
            Model.Version version;
            if (this.ParentPersistenceMechanism.HasVersioningEnabled) {
                var versionContainer = this.ParentPersistenceMechanism.VersioningRepository.FindContainer<Model.Version>();
                var filterCriteria = new FilterCriteria(nameof(Model.Version.SourceId), RelationalOperator.IsEqualTo, persistentObject.Id)
                    .And(nameof(Model.Version.CreatedAt), RelationalOperator.IsLessThanOrEqualTo, modificationDate);
                var sortCriteria = new SortCriterionCollection {
                    { nameof(Model.Version.CreatedAt), SortDirection.Descending }
                };
                version = versionContainer.FindOne(filterCriteria, sortCriteria);
                this.SetValueFor(version);
            } else {
                version = null;
            }
            return version;
        }

        /// <summary>
        /// Finds or creates a persistent object with a specific ID.
        /// </summary>
        /// <param name="id">ID to find or create persistent object
        /// for</param>
        /// <returns>persistent object with specific ID</returns>
        private PersistentObject FindOrCreate(Guid id) {
            var item = this.FindOnePersistentObject(id);
            if (null == item && !this.ParentPersistenceMechanism.IsIdDeleted(id)) {
                item = this.ParentPersistenceMechanism.CreateInstance(this.ContentType);
                item.Id = id;
            }
            return item;
        }

        /// <summary>
        /// Finds the first matching persistent object from this
        /// container - including containers of sub types.
        /// </summary>
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
        /// <returns>first matching object from this container or
        /// null if no match was found</returns>
        public ReadOnlyCollection<PersistentObject> FindPersistentObjects(string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            return this.ParentPersistenceMechanism.Find<PersistentObject>(this.InternalName, fullTextQuery, filterCriteria, sortCriteria, startPosition, maxResults);
        }

        /// <summary>
        /// Finds the versions of a specific persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to find
        /// versions for</param>
        /// <param name="startPosition">number of versions to skip</param>
        /// <param name="maxResults">maximum number of versions to return</param>
        /// <returns>versions of specific persistent object</returns>
        internal ReadOnlyCollection<Model.Version> FindVersions(PersistentObject persistentObject, ulong startPosition, ulong maxResults) {
            ReadOnlyCollection<Model.Version> versions;
            if (this.ParentPersistenceMechanism.HasVersioningEnabled) {
                var versionContainer = this.ParentPersistenceMechanism.VersioningRepository.FindContainer<Model.Version>();
                var filterCriteria = new FilterCriteria(nameof(Model.Version.SourceId), RelationalOperator.IsEqualTo, persistentObject.Id);
                var sortCriteria = new SortCriterionCollection {
                    { nameof(Model.Version.CreatedAt), SortDirection.Descending }
                };
                versions = versionContainer.Find(filterCriteria, sortCriteria, startPosition, maxResults);
                foreach (var version in versions) {
                    this.SetValueFor(version);
                }
            } else {
                versions = new List<Model.Version>(0).AsReadOnly();
            }
            return versions;
        }

        /// <summary>
        /// Preloads the state of multiple persistent objects from
        /// persistence mechanism.
        /// </summary>
        /// <param name="items">persistent objects to be preloaded</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        internal void Preload(IEnumerable<PersistentObject> items, IEnumerable<string[]> keyChains) {
            this.Preload(this.ParentPersistenceMechanism.CreateInstance(this.ContentType), items, keyChains);
            return;
        }

        /// <summary>
        /// Preloads the state of multiple persistent objects from
        /// persistence mechanism.
        /// </summary>
        /// <param name="sampleObject">sample object of common base
        /// class of objects to be preloaded</param>
        /// <param name="items">persistent objects to be preloaded</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        protected void Preload(PersistentObject sampleObject, IEnumerable<PersistentObject> items, IEnumerable<string[]> keyChains) {
            bool isRetrievalRequired = false;
            foreach (var keyChain in keyChains) {
                isRetrievalRequired = true;
                break;
            }
            if (isRetrievalRequired) {
                var autoRetrievalEnabled = new List<bool>();
                var eventFiringEnabled = new List<bool>();
                var uniqueGuids = new List<Guid>();
                try {
                    foreach (var item in items) {
                        if (null != item && !uniqueGuids.Contains(item.Id)) {
                            autoRetrievalEnabled.Add(item.IsAutoRetrievalEnabled);
                            eventFiringEnabled.Add(item.IsEventFiringEnabled);
                            uniqueGuids.Add(item.Id);
                            item.IsAutoRetrievalEnabled = false;
                            item.IsEventFiringEnabled = false;
                        }
                    }
                    this.ParentPersistenceMechanism.PreloadObjects(sampleObject, items, this.InternalName, keyChains);
                } finally {
                    int i = 0;
                    uniqueGuids.Clear();
                    foreach (var item in items) {
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
        /// Retrieves a specific object from container.
        /// </summary>
        /// <param name="persistentObject">specific object to
        /// retrieve from container</param>
        internal void Retrieve(PersistentObject persistentObject) {
            bool autoRetrievalEnabled = persistentObject.IsAutoRetrievalEnabled;
            bool eventFiringEnabled = persistentObject.IsEventFiringEnabled;
            try {
                persistentObject.IsAutoRetrievalEnabled = false;
                persistentObject.IsEventFiringEnabled = false;
                this.ParentPersistenceMechanism.RetrieveObject(persistentObject, this.InternalName);
            } finally {
                persistentObject.IsAutoRetrievalEnabled = autoRetrievalEnabled;
                persistentObject.IsEventFiringEnabled = eventFiringEnabled;
            }
            return;
        }

        /// <summary>
        /// Retrieves the state of all fields for elements of
        /// persistent object from persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// retrieve fields for elements for</param>
        internal void RetrieveFieldsForElements(PersistentObject persistentObject) {
            bool autoRetrievalEnabled = persistentObject.IsAutoRetrievalEnabled;
            bool eventFiringEnabled = persistentObject.IsEventFiringEnabled;
            try {
                persistentObject.IsAutoRetrievalEnabled = false;
                persistentObject.IsEventFiringEnabled = false;
                this.ParentPersistenceMechanism.RetrieveFieldsForElements(persistentObject, this.InternalName);
            } finally {
                persistentObject.IsAutoRetrievalEnabled = autoRetrievalEnabled;
                persistentObject.IsEventFiringEnabled = eventFiringEnabled;
            }
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// elements from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        internal void RetrieveField(PersistentFieldForCollection persistentField) {
            PersistentObject persistentObject = persistentField.ParentPersistentObject;
            bool autoRetrievalEnabled = persistentObject.IsAutoRetrievalEnabled;
            bool eventFiringEnabled = persistentObject.IsEventFiringEnabled;
            try {
                persistentObject.IsAutoRetrievalEnabled = false;
                persistentObject.IsEventFiringEnabled = false;
                this.ParentPersistenceMechanism.RetrieveFieldForCollectionOfElements(persistentField, this.InternalName);
            } finally {
                persistentObject.IsAutoRetrievalEnabled = autoRetrievalEnabled;
                persistentObject.IsEventFiringEnabled = eventFiringEnabled;
            }
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a persistent object
        /// from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        internal void RetrieveField(PersistentFieldForPersistentObject persistentField) {
            PersistentObject persistentObject = persistentField.ParentPersistentObject;
            bool autoRetrievalEnabled = persistentObject.IsAutoRetrievalEnabled;
            bool eventFiringEnabled = persistentObject.IsEventFiringEnabled;
            try {
                persistentObject.IsAutoRetrievalEnabled = false;
                persistentObject.IsEventFiringEnabled = false;
                this.ParentPersistenceMechanism.RetrieveFieldForPersistentObject(persistentField, this.InternalName);
            } finally {
                persistentObject.IsAutoRetrievalEnabled = autoRetrievalEnabled;
                persistentObject.IsEventFiringEnabled = eventFiringEnabled;
            }
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// persistent objects from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        internal void RetrieveField(PersistentFieldForPersistentObjectCollection persistentField) {
            PersistentObject persistentObject = persistentField.ParentPersistentObject;
            bool autoRetrievalEnabled = persistentObject.IsAutoRetrievalEnabled;
            bool eventFiringEnabled = persistentObject.IsEventFiringEnabled;
            try {
                persistentObject.IsAutoRetrievalEnabled = false;
                persistentObject.IsEventFiringEnabled = false;
                this.ParentPersistenceMechanism.RetrieveFieldForCollectionOfPersistentObjects(persistentField, this.InternalName);
            } finally {
                persistentObject.IsAutoRetrievalEnabled = autoRetrievalEnabled;
                persistentObject.IsEventFiringEnabled = eventFiringEnabled;
            }
            return;
        }

        /// <summary>
        /// Sets value of a specific version.
        /// </summary>
        /// <param name="version">version to set value for</param>
        private void SetValueFor(Model.Version version) {
            if (null != version) {
                var versionValueContainer = this.ParentPersistenceMechanism.VersioningRepository.FindContainer(this.AssemblyQualifiedTypeName);
                version.Value = versionValueContainer.FindOnePersistentObject(version.ValueId);
            }
            return;
        }

        /// <summary>
        /// Two-way-synchronizes the values of an object with the
        /// corresponding object of container.
        /// </summary>
        /// <param name="source">object to synchronize in container</param>
        /// <returns>target object which was synced from container or
        /// null</returns>
        protected PersistentObject Synchronize(PersistentObject source) {
            return this.Synchronize(source, new PersistentObjectCache(), new PersistentObjectCache());
        }

        /// <summary>
        /// Two-way-synchronizes the values of an object with the
        /// corresponding object of container.
        /// </summary>
        /// <param name="source">object to synchronize in container</param>
        /// <param name="processedSourceObjects">list of processed
        /// source objects</param>
        /// <param name="processedTargetObjects">list of processed
        /// target objects</param>
        /// <returns>target object which was synced from container or
        /// null</returns>
        internal PersistentObject Synchronize(PersistentObject source, PersistentObjectCache processedSourceObjects, PersistentObjectCache processedTargetObjects) {
            PersistentObject target = null;
            if (null != source && !processedSourceObjects.Contains(source.Id)) {
                processedSourceObjects.Add(source);
                target = this.FindOrCreate(source.Id);
                if (null != target && !processedTargetObjects.Contains(target.Id)) {
                    processedTargetObjects.Add(target);
                    if (source.IsNew || source.IsChanged || source.ModifiedAt > target.ModifiedAt) {
                        target.CopyFieldsForElementsFrom(source);
                        target.SyncCopyFieldsForCollectionsOfElementsFrom(source);
                        target.SyncCopyFieldsForPersistentObjectsFrom(source, this.ParentPersistenceMechanism, processedSourceObjects, processedTargetObjects);
                        target.SyncCopyFieldsForCollectionsOfPersistentObjectsFrom(source, this.ParentPersistenceMechanism, processedSourceObjects, processedTargetObjects);
                        if (target.IsNew) {
                            this.ParentPersistenceMechanism.AddObjectCascadedly(target, this);
                        } else {
                            this.ParentPersistenceMechanism.UpdateObjectCascadedly(target, this);
                        }
                    } else if (target.IsNew || target.IsChanged || source.ModifiedAt < target.ModifiedAt) {
                        source.ParentPersistentContainer.Synchronize(target, processedTargetObjects, processedSourceObjects);
                    } else { // !source.IsNew && !target.IsNew && !source.IsChanged && !target.IsChanged && source.ModifiedAt == target.ModifiedAt
                        foreach (var targetField in target.PersistentFieldsForPersistentObjects) {
                            var sourceField = source.GetPersistentFieldForPersistentObject(targetField.Key);
                            if (sourceField.IsComposition || sourceField.IsChanged) {
                                this.Synchronize(sourceField.ValueAsPersistentObject, processedSourceObjects, processedTargetObjects);
                            }
                        }
                        foreach (var targetField in target.PersistentFieldsForCollectionsOfPersistentObjects) {
                            var sourceField = source.GetPersistentFieldForCollectionOfPersistentObjects(targetField.Key);
                            if (sourceField.IsComposition || sourceField.IsChanged) {
                                foreach (var sourceFieldChildObject in sourceField.GetValuesAsPersistentObject()) {
                                    this.Synchronize(sourceFieldChildObject, processedSourceObjects, processedTargetObjects);
                                }
                            }
                        }
                    }
                }
            }
            return target;
        }

    }

    /// <summary>
    /// Container for persistent objects of type T.
    /// </summary>
    /// <typeparam name="T">type of persistent objects to store</typeparam>
    public sealed class PersistentContainer<T> : PersistentContainer, ICollection<T>, IEquatable<PersistentContainer<T>>
        where T : PersistentObject, new() {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPersistenceMechanism">parent
        /// persistence mechanism</param>
        /// <param name="assemblyQualifiedTypeName">assembly qualified type name
        /// of objects contained in the container</param>
        internal PersistentContainer(PersistenceMechanism parentPersistenceMechanism, string assemblyQualifiedTypeName)
            : base(parentPersistenceMechanism, assemblyQualifiedTypeName) {
            // nothing to do
        }

        /// <summary>
        /// Adds an object to the container. 
        /// </summary>
        /// <param name="item">object to add to the container</param>
        public void Add(T item) {
            this.ParentPersistenceMechanism.AddObject(item, this);
            return;
        }

        /// <summary>
        /// Adds an object to the container and adds all of its
        /// persistent child objects cascadedly.
        /// </summary>
        /// <param name="persistentObject">object to add to the
        /// container cascadedly</param>
        public void AddCascadedly(T persistentObject) {
            this.ParentPersistenceMechanism.AddObjectCascadedly(persistentObject, this);
            return;
        }

        /// <summary>
        /// Adds an object to the container or updates it if it
        /// exists already.
        /// </summary>
        /// <param name="persistentObject">object to add or update</param>
        /// <returns>true if object was added to or updated in
        /// container successfully, false otherwise</returns>
        public bool AddOrUpdate(T persistentObject) {
            bool success;
            if (persistentObject.IsNew) {
                success = this.ParentPersistenceMechanism.AddObject(persistentObject, this);
            } else {
                PersistentContainer container = this;
                if (persistentObject.Type != container.ContentType) {
                    container = this.ParentPersistenceMechanism.FindContainer(persistentObject.Type);
                }
                success = this.ParentPersistenceMechanism.UpdateObject(persistentObject, container);
            }
            return success;
        }

        /// <summary>
        /// Adds or updates an object and all of its persistent child
        /// objects cascadedly.
        /// </summary>
        /// <param name="persistentObject">object to add or update</param>
        public void AddOrUpdateCascadedly(T persistentObject) {
            if (persistentObject.IsNew) {
                this.AddCascadedly(persistentObject);
            } else {
                PersistentContainer container = this;
                if (persistentObject.Type != this.ContentType) {
                    container = this.ParentPersistenceMechanism.FindContainer(persistentObject.Type);
                }
                this.ParentPersistenceMechanism.UpdateObjectCascadedly(persistentObject, container);
            }
            return;
        }

        /// <summary>
        /// Removes all objects from the container.
        /// </summary>
        public void Clear() {
            foreach (var persistentObject in this) {
                persistentObject.Remove();
            }
            return;
        }

        /// <summary>
        /// Removes all objects and all of their persistent child
        /// objects from container cascadedly.
        /// </summary>
        public void ClearCascadedly() {
            foreach (var persistentObject in this) {
                persistentObject.RemoveCascadedly();
            }
            return;
        }

        /// <summary>
        /// Determines whether the container contains a specific
        /// object.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to apply</param>
        /// <returns>true if specific object is contained, false
        /// otherwise</returns>
        public bool Contains(FilterCriteria filterCriteria) {
            return this.ParentPersistenceMechanism.Find<T>(this.InternalName, null, filterCriteria, SortCriterionCollection.Empty, 0, 1).Count > 0;
        }

        /// <summary>
        /// Determines whether the container contains a specific
        /// object.
        /// </summary>
        /// <param name="item">specific object to find by ID</param>
        /// <returns>true if specific object is contained, false
        /// otherwise</returns>
        public bool Contains(T item) {
            return this.Contains(item.Id);
        }

        /// <summary>
        /// Counts a subset of objects contained in the container -
        /// including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <returns>pairs of group and number of objects of group</returns>
        public int CountFiltered(FilterCriteria filterCriteria) {
            return this.ParentPersistenceMechanism.CountObjects<T>(this.InternalName, filterCriteria);
        }

        /// <summary>
        /// Counts a subset of objects contained in the container -
        /// including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="groupBy">field names to group objects by</param>
        /// <returns>pairs of group and number of objects of group</returns>
        public IDictionary<string[], int> CountFiltered(FilterCriteria filterCriteria, string[] groupBy) {
            return this.ParentPersistenceMechanism.CountObjects<T>(this.InternalName, filterCriteria, groupBy);
        }

        /// <summary>
        /// Copies the objects of the container to an array,
        /// starting at a particular array index. 
        /// </summary>
        /// <param name="array">array to copy the objects into</param>
        /// <param name="arrayIndex">array index to start at</param>
        public void CopyTo(T[] array, int arrayIndex) {
            this.Find(FilterCriteria.Empty, SortCriterionCollection.Empty).CopyTo(array, arrayIndex);
            return;
        }

        /// <summary>
        /// Creates a new instance of a persistent object of this
        /// container with the specified ID.
        /// </summary>
        /// <param name="id">ID to create object for</param>
        /// <returns>new instance of a persistent object of this
        /// container with the specified ID</returns>
        internal override PersistentObject CreateInstance(Guid id) {
            var persistentObject = this.ParentPersistenceMechanism.CreateInstance<T>();
            persistentObject.Id = id;
            persistentObject.ParentPersistentContainer = this;
            return persistentObject;
        }

        /// <summary>
        /// Checks if all elements of this container are equal to
        /// the elements of another persistent container.
        /// </summary>
        /// <param name="other">persistent container to compare this
        /// container to</param>
        /// <returns>true if all elements of this container are
        /// equal to the elements of the other container, false
        /// otherwise</returns>
        public bool Equals(PersistentContainer<T> other) {
            bool isEqual = true;
            if (null == other || this.Count != other.Count) {
                isEqual = false;
            } else {
                foreach (T persistentObject in this) {
                    if (!other.Contains(persistentObject)) {
                        isEqual = false;
                        break;
                    }
                }
            }
            return isEqual;
        }

        /// <summary>
        /// Finds all matching persistent objects from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>all matching objects from this container</returns>
        public ReadOnlyCollection<T> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria) {
            return this.Find(filterCriteria, sortCriteria, 0);
        }

        /// <summary>
        /// Finds all matching persistent objects from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <returns>all matching objects from this container</returns>
        public ReadOnlyCollection<T> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition) {
            return this.Find(filterCriteria, sortCriteria, startPosition, ulong.MaxValue);
        }

        /// <summary>
        /// Finds all matching persistent objects from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to return</param>
        /// <returns>all matching objects from this container</returns>
        public ReadOnlyCollection<T> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            return this.ParentPersistenceMechanism.Find<T>(this.InternalName, null, filterCriteria, sortCriteria, startPosition, maxResults);
        }

        /// <summary>
        /// Finds the average values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="propertyNames">keys of properties to find
        /// average values for</param>
        /// <returns>average values of specific properties</returns>
        public ReadOnlyCollection<object> FindAverageValue(FilterCriteria filterCriteria, string[] propertyNames) {
            return this.ParentPersistenceMechanism.FindAverageValues<T>(this.InternalName, filterCriteria, propertyNames);
        }

        /// <summary>
        /// Finds the average values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="propertyKeyChains">key chains of properties
        /// to find average values for</param>
        /// <returns>average values of specific properties</returns>
        public ReadOnlyCollection<object> FindAverageValue(FilterCriteria filterCriteria, string[][] propertyKeyChains) {
            var propertyNames = new string[propertyKeyChains.LongLength];
            for (long i = 0; i < propertyKeyChains.LongLength; i++) {
                propertyNames[i] = KeyChain.ToKey(propertyKeyChains[i]);
            }
            return this.FindAverageValue(filterCriteria, propertyNames);
        }

        /// <summary>
        /// Finds the complement set of all matching persistent
        /// objects from this container - including containers of sub
        /// types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>all matching objects from this container</returns>
        public ReadOnlyCollection<T> FindComplement(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria) {
            return this.FindComplement(filterCriteria, sortCriteria, 0);
        }

        /// <summary>
        /// Finds the complement set of all matching persistent
        /// objects from this container - including containers of sub
        /// types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <returns>all matching objects from this container</returns>
        public ReadOnlyCollection<T> FindComplement(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition) {
            return this.FindComplement(filterCriteria, sortCriteria, startPosition, ulong.MaxValue);
        }

        /// <summary>
        /// Finds the complement set of all matching persistent
        /// objects from this container - including containers of sub
        /// types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to return</param>
        /// <returns>all matching objects from this container</returns>
        public ReadOnlyCollection<T> FindComplement(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            return this.ParentPersistenceMechanism.FindComplement<T>(this.InternalName, null, filterCriteria, sortCriteria, startPosition, maxResults);
        }

        /// <summary>
        /// Finds all distinct combinations of a specific set of
        /// properties - including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort the distinct
        /// result combinations by</param>
        /// <param name="propertyNames">keys of properties to find
        /// all distinct combinations for</param>
        /// <returns>all distinct combinations of a specific set of
        /// properties</returns>
        public ReadOnlyCollection<object[]> FindDistinctValues(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string[] propertyNames) {
            return this.ParentPersistenceMechanism.FindDistinctValues<T>(this.InternalName, filterCriteria, sortCriteria, propertyNames);
        }

        /// <summary>
        /// Finds all distinct combinations of a specific set of
        /// properties.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort the distinct
        /// result combinations by</param>
        /// <param name="propertyKeyChains">keys chains of properties
        /// to find all distinct combinations for</param>
        /// <returns>all distinct combinations of a specific set of
        /// properties</returns>
        public ReadOnlyCollection<object[]> FindDistinctValues(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string[][] propertyKeyChains) {
            var propertyNames = new string[propertyKeyChains.LongLength];
            for (long i = 0; i < propertyKeyChains.LongLength; i++) {
                propertyNames[i] = KeyChain.ToKey(propertyKeyChains[i]);
            }
            return this.FindDistinctValues(filterCriteria, sortCriteria, propertyNames);
        }

        /// <summary>
        /// Finds all matching persistent objects from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to find
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        public ReadOnlyCollection<T> FindFullText(string fullTextQuery, FilterCriteria filterCriteria) {
            return this.FindFullText(fullTextQuery, filterCriteria, 0);
        }

        /// <summary>
        /// Finds all matching persistent objects from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to find
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <returns>all matching objects from this container</returns>
        public ReadOnlyCollection<T> FindFullText(string fullTextQuery, FilterCriteria filterCriteria, ulong startPosition) {
            return this.FindFullText(fullTextQuery, filterCriteria, startPosition, ulong.MaxValue);
        }

        /// <summary>
        /// Finds all matching persistent objects from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to find
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to return</param>
        /// <returns>all matching objects from this container</returns>
        public ReadOnlyCollection<T> FindFullText(string fullTextQuery, FilterCriteria filterCriteria, ulong startPosition, ulong maxResults) {
            return this.ParentPersistenceMechanism.Find<T>(this.InternalName, fullTextQuery, filterCriteria, SortCriterionCollection.Empty, startPosition, maxResults);
        }

        /// <summary>
        /// Finds the complement set of all matching persistent
        /// objects from this container - including containers of sub
        /// types.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to find
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        public ReadOnlyCollection<T> FindFullTextComplement(string fullTextQuery, FilterCriteria filterCriteria) {
            return this.FindFullTextComplement(fullTextQuery, filterCriteria, 0);
        }

        /// <summary>
        /// Finds the complement set of all matching persistent
        /// objects from this container - including containers of sub
        /// types.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to find
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <returns>all matching objects from this container</returns>
        public ReadOnlyCollection<T> FindFullTextComplement(string fullTextQuery, FilterCriteria filterCriteria, ulong startPosition) {
            return this.FindFullTextComplement(fullTextQuery, filterCriteria, startPosition, ulong.MaxValue);
        }

        /// <summary>
        /// Finds the complement set of all matching persistent
        /// objects from this container - including containers of sub
        /// types.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to find
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to return</param>
        /// <returns>all matching objects from this container</returns>
        public ReadOnlyCollection<T> FindFullTextComplement(string fullTextQuery, FilterCriteria filterCriteria, ulong startPosition, ulong maxResults) {
            return this.ParentPersistenceMechanism.FindComplement<T>(this.InternalName, fullTextQuery, filterCriteria, SortCriterionCollection.Empty, startPosition, maxResults);
        }

        /// <summary>
        /// Finds the first matching persistent object from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="id">ID to find object for</param>
        /// <returns>first matching object from this container or
        /// null if no match was found</returns>
        public T FindOne(Guid id) {
            var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, id);
            var sortCriteria = new SortCriterionCollection {
                { nameof(PersistentObject.ModifiedAt), SortDirection.Descending }
            };
            return this.FindOne(filterCriteria, sortCriteria);
        }

        /// <summary>
        /// Finds the first matching persistent object from this
        /// container - including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// object for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>first matching object from this container or
        /// null if no match was found</returns>
        public T FindOne(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria) {
            return this.ParentPersistenceMechanism.FindOne<T>(this.InternalName, filterCriteria, sortCriteria);
        }

        /// <summary>
        /// Finds the first persistent object of complement set of
        /// matching objects from this container - including
        /// containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// object for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>first matching object from this container or
        /// null if no match was found</returns>
        public T FindOneComplement(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria) {
            return this.ParentPersistenceMechanism.FindOneComplement<T>(this.InternalName, filterCriteria, sortCriteria);
        }

        /// <summary>
        /// Finds the sum of values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="propertyNames">keys of properties to find sums
        /// of values for</param>
        /// <returns>sums of values of specific properties</returns>
        public ReadOnlyCollection<object> FindSumsOfValues(FilterCriteria filterCriteria, string[] propertyNames) {
            return this.ParentPersistenceMechanism.FindSumsOfValues<T>(this.InternalName, filterCriteria, propertyNames);
        }

        /// <summary>
        /// Finds the sum of values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="propertyKeyChains">key chains of properties
        /// to find sums of values for</param>
        /// <returns>sums of values of specific properties</returns>
        public ReadOnlyCollection<object> FindSumsOfValues(FilterCriteria filterCriteria, string[][] propertyKeyChains) {
            var propertyNames = new string[propertyKeyChains.LongLength];
            for (long i = 0; i < propertyKeyChains.LongLength; i++) {
                propertyNames[i] = KeyChain.ToKey(propertyKeyChains[i]);
            }
            return this.FindSumsOfValues(filterCriteria, propertyNames);
        }        

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// container. 
        /// </summary>
        /// <returns>enumerator that iterates through the container</returns>
        public IEnumerator<T> GetEnumerator() {
            return this.Find(FilterCriteria.Empty, SortCriterionCollection.Empty).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// container. 
        /// </summary>
        /// <returns>enumerator that iterates through the container</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            foreach (var value in this.Find(FilterCriteria.Empty, SortCriterionCollection.Empty)) {
                yield return value;
            }
        }

        /// <summary>
        /// Preloads the state of multiple persistent objects from
        /// persistence mechanism.
        /// </summary>
        /// <param name="items">persistent objects to preload</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        public void Preload(IEnumerable<T> items, IEnumerable<string[]> keyChains) {
            this.Preload(this.ParentPersistenceMechanism.CreateInstance<T>(), items, keyChains);
            return;
        }

        /// <summary>
        /// Removes a specific object from the container.
        /// </summary>
        /// <param name="item">specific object to remove
        /// from the container</param>
        /// <returns>true if object was successfully removed from the
        /// container, false otherwise or if object was not
        /// contained in container</returns>
        public bool Remove(T item) {
            return item.Remove();
        }

        /// <summary>
        /// Removes a specific object and all child objects to be
        /// removed cascadedly from persistence mechanism
        /// immediately.
        /// </summary>
        /// <param name="item">specific object to remove
        /// from the container cascadedly</param>
        /// <returns>true if object was successfully removed from the
        /// container, false otherwise or if object was not
        /// contained in container</returns>
        public bool RemoveCascadedly(T item) {
            return item.RemoveCascadedly();
        }

        /// <summary>
        /// Two-way-synchronizes the values of an object with the
        /// corresponding object of container.
        /// </summary>
        /// <param name="item">object to synchronize in container</param>
        public void Synchronize(T item) {
            base.Synchronize(item);
            return;
        }

    }

}