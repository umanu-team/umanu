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

    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Base class for any unique object in persistence mechanism.
    /// </summary>    
    public class PersistentObject : INotifyPropertyChanged, IPresentableObject {

        /// <summary>
        /// Allowed groups for reading/writing this object.
        /// </summary>
        public AllowedGroups AllowedGroups {
            get { return this.allowedGroups.Value; }
            set { this.allowedGroups.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<AllowedGroups> allowedGroups =
            new PersistentFieldForPersistentObject<AllowedGroups>(nameof(AllowedGroups), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Specifies the behavior to apply on cascaded removal of
        /// allowed groups.
        /// </summary>
        protected CascadedRemovalBehavior AllowedGroupsCascadedRemovalBehavior {
            get { return this.allowedGroups.CascadedRemovalBehavior; }
            set { this.allowedGroups.CascadedRemovalBehavior = value; }
        }

        /// <summary>
        /// Time of creation of this object in persistence mechanism.
        /// </summary>
        public DateTime CreatedAt {
            get { return this.createdAt.Value; }
            internal set { this.createdAt.Value = value; }
        }
        private readonly PersistentFieldForDateTime createdAt =
            new PersistentFieldForDateTime(nameof(CreatedAt));

        /// <summary>
        /// User who created this object in persistence mechanism.
        /// </summary>
        public IUser CreatedBy {
            get { return this.createdBy.Value; }
            internal set { this.createdBy.Value = value; }
        }
        private readonly PersistentFieldForIUser createdBy =
            new PersistentFieldForIUser(nameof(CreatedBy));

        /// <summary>
        /// Indicates whether persistent object has versions.
        /// </summary>
        public bool HasVersions {
            get {
                bool hasVersions;
                if (null == this.ParentPersistentContainer) {
                    hasVersions = false;
                } else {
                    hasVersions = this.ParentPersistentContainer.FindVersions(this, 0, 1).Count > 0;
                }
                return hasVersions;
            }
        }

        /// <summary>
        /// Globally unique identifier of this object in persistence
        /// mechanism.
        /// </summary>
        public Guid Id {
            get { return this.id.Value; }
            internal set { this.id.Value = value; }
        }
        private readonly PersistentFieldForGuid id =
            new PersistentFieldForGuid(nameof(Id), Guid.NewGuid());

        /// <summary>
        /// True to retrieve persistent properties automatically,
        /// false otherwise.
        /// </summary>
        internal bool IsAutoRetrievalEnabled { get; set; }

        /// <summary>
        /// True if the values of this object were changed since last
        /// retrieval, false otherwise.
        /// </summary>
        public bool IsChanged {
            get {
                var isChanged = false;
                foreach (var persistentField in this.GetPersistentFields()) {
                    if (persistentField.IsChanged) {
                        isChanged = true;
                        break;
                    }
                }
                return isChanged;
            }
        }

        /// <summary>
        /// True to enable firing of events, false otherwise.
        /// </summary>
        public bool IsEventFiringEnabled { get; set; }

        /// <summary>
        /// Indicates whether object is queryable via full-text
        /// search.
        /// </summary>
        public bool IsFullTextQueryable { get; set; }

        /// <summary>
        /// True if this object was not read from persistence
        /// mechanism before, false otherwise.
        /// </summary>
        public bool IsNew {
            get {
                return null == this.ParentPersistentContainer;
            }
        }

        /// <summary>
        /// True if this object is read protected, false otherwise.
        /// </summary>
        public bool IsReadProtected {
            get {
                bool isReadProtected;
                if (this.IsNew || (null != this.ParentPersistentContainer.ParentPersistenceMechanism && this.ParentPersistentContainer.ParentPersistenceMechanism.SecurityModel == SecurityModel.IgnorePermissions)) {
                    isReadProtected = false;
                } else {
                    if (null == this.AllowedGroups) {
                        isReadProtected = true;
                    } else {
                        var currentUser = this.ParentPersistentContainer.ParentPersistenceMechanism.UserDirectory.CurrentUser;
                        isReadProtected = !this.AllowedGroups.ForReading.ContainsPermissionsFor(currentUser);
                    }
                }
                return isReadProtected;
            }
        }

        /// <summary>
        /// True if this object is deleted in persistence mechanism,
        /// false otherwise.
        /// </summary>
        public bool IsRemoved {
            get {
                return this.isRemoved;
            }
            internal set {
                this.isRemoved = value;
                if (this.isRemoved) {
                    this.ParentPersistentContainer = null;
                }
            }
        }
        private bool isRemoved;

        /// <summary>
        /// True if this object is persistent and initialized with
        /// values from persistence mechanism completely, false
        /// otherwise.
        /// </summary>
        private bool IsRetrievedCompletely {
            get {
                var hasPersistentFields = false;
                var isRetrieved = true;
                foreach (var persistentField in this.GetPersistentFields()) {
                    hasPersistentFields = true;
                    if (!persistentField.IsRetrieved) {
                        isRetrieved = false;
                        break;
                    }
                }
                return hasPersistentFields && isRetrieved;
            }
        }

        /// <summary>
        /// True if this object is persistent and initialized with
        /// values from persistence mechanism at least partially,
        /// false otherwise.
        /// </summary>
        internal bool IsRetrievedPartially {
            get {
                var isRetrieved = false;
                foreach (var persistentField in this.GetPersistentFields()) {
                    if (persistentField.IsRetrieved && nameof(PersistentObject.Id) != persistentField.Key) {
                        isRetrieved = true;
                        break;
                    }
                }
                return isRetrieved;
            }
        }

        /// <summary>
        /// True if this object is write protected, false otherwise.
        /// </summary>
        public bool IsWriteProtected {
            get {
                bool isWriteProtected;
                if (this.IsNew || (null != this.ParentPersistentContainer.ParentPersistenceMechanism && this.ParentPersistentContainer.ParentPersistenceMechanism.SecurityModel == SecurityModel.IgnorePermissions)) {
                    isWriteProtected = false;
                } else {
                    if (null == this.AllowedGroups) {
                        isWriteProtected = true;
                    } else {
                        var currentUser = this.ParentPersistentContainer.ParentPersistenceMechanism.UserDirectory.CurrentUser;
                        isWriteProtected = !this.AllowedGroups.ForWriting.ContainsPermissionsFor(currentUser);
                    }
                }
                return isWriteProtected;
            }
        }

        /// <summary>
        /// Time of last modification in persistence mechanism.
        /// </summary>
        public DateTime ModifiedAt {
            get { return this.modifiedAt.Value; }
            internal set { this.modifiedAt.Value = value; }
        }
        private readonly PersistentFieldForDateTime modifiedAt =
            new PersistentFieldForDateTime(nameof(ModifiedAt));

        /// <summary>
        /// User who modified this object in persistence mechanism.
        /// </summary>
        public IUser ModifiedBy {
            get { return this.modifiedBy.Value; }
            internal set { this.modifiedBy.Value = value; }
        }
        private readonly PersistentFieldForIUser modifiedBy =
            new PersistentFieldForIUser(nameof(ModifiedBy));

        /// <summary>
        /// Parent persistent mechanism of this persistent object.
        /// </summary>
        public PersistentContainer ParentPersistentContainer { get; internal set; }

        /// <summary>
        /// Enumerable of all persistent fields for collections
        /// containing values of types other than PersistentObject.
        /// </summary>
        internal IEnumerable<PersistentFieldForCollection> PersistentFieldsForCollectionsOfElements {
            get {
                return this.persistentFieldsForCollectionsOfElements;
            }
        }
        private readonly List<PersistentFieldForCollection> persistentFieldsForCollectionsOfElements;

        /// <summary>
        /// Enumerable of all persistent fields for collections
        /// containing values of type PersistentObject.
        /// </summary>
        internal IEnumerable<PersistentFieldForPersistentObjectCollection> PersistentFieldsForCollectionsOfPersistentObjects {
            get {
                return this.persistentFieldsForCollectionsOfPersistentObjects;
            }
        }
        private readonly List<PersistentFieldForPersistentObjectCollection> persistentFieldsForCollectionsOfPersistentObjects;

        /// <summary>
        /// Enumerable of all persistent fields for elements of any
        /// type other than PersistentObject.
        /// </summary>
        internal IEnumerable<PersistentFieldForElement> PersistentFieldsForElements {
            get {
                return this.persistentFieldsForElements;
            }
        }
        private readonly List<PersistentFieldForElement> persistentFieldsForElements;

        /// <summary>
        /// Enumerable of all persistent fields for single elements
        /// of type PersistentObject.
        /// </summary>
        internal IEnumerable<PersistentFieldForPersistentObject> PersistentFieldsForPersistentObjects {
            get {
                return this.persistentFieldsForPersistentObjects;
            }
        }
        private readonly List<PersistentFieldForPersistentObject> persistentFieldsForPersistentObjects;

        /// <summary>
        /// Event handler called after change of any persistent value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Indicates whether to remove this object from persistence
        /// mechanism on next call of update method.
        /// </summary>
        public RemovalType RemoveOnUpdate {
            get {
                return this.removeOnUpdate;
            }
            set {
                if (this.IsAutoRetrievalEnabled && !this.IsNew && !this.IsRetrievedCompletely) {
                    this.Retrieve();
                }
                this.removeOnUpdate = value;
            }
        }
        private RemovalType removeOnUpdate;

        /// <summary>
        /// Type of this object.
        /// </summary>
        internal Type Type {
            get {
                if (null == this.type) {
                    this.type = this.GetType();
                }
                return this.type;
            }
        }
        private Type type;

        /// <summary>
        /// Enumerable of previous persistent versions of this
        /// object.
        /// </summary>
        public ReadOnlyCollection<Model.Version> Versions {
            get {
                ReadOnlyCollection<Model.Version> versions;
                if (null == this.ParentPersistentContainer) {
                    versions = new List<Model.Version>(0).AsReadOnly();
                } else {
                    versions = this.ParentPersistentContainer.FindVersions(this, 0, ulong.MaxValue);
                }
                return versions;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PersistentObject() {
            this.IsAutoRetrievalEnabled = true;
            this.IsEventFiringEnabled = true;
            this.IsRemoved = false;
            this.persistentFieldsForCollectionsOfElements = new List<PersistentFieldForCollection>();
            this.persistentFieldsForCollectionsOfPersistentObjects = new List<PersistentFieldForPersistentObjectCollection>();
            this.persistentFieldsForElements = new List<PersistentFieldForElement>();
            this.persistentFieldsForPersistentObjects = new List<PersistentFieldForPersistentObject>();
            this.RegisterPersistentField(this.allowedGroups);
            this.id.IsIndexed = true;
            this.RegisterPersistentField(this.id);
            this.RegisterPersistentField(this.createdAt);
            this.RegisterPersistentField(this.createdBy);
            this.RegisterPersistentField(this.modifiedAt);
            this.RegisterPersistentField(this.modifiedBy);
            this.IsFullTextQueryable = false;
            this.RemoveOnUpdate = RemovalType.False;
        }

        /// <summary>
        /// Appends text to full text.
        /// </summary>
        /// <param name="text">text to append to full text</param>
        /// <param name="fullText">full text collected so far</param>
        private static void AppendToFullText(string text, StringBuilder fullText) {
            if (!string.IsNullOrEmpty(text)) {
                text = text.Trim();
                if (fullText.Length > 0 && text.Length > 0) {
                    fullText.Append(' ');
                }
                fullText.Append(text);
            }
            return;
        }

        /// <summary>
        /// Copies the state of all fields for collections of
        /// elements of another instance of this type into this
        /// instance.
        /// </summary>
        /// <param name="source">source instance to copy state from</param>
        private void CopyFieldsForCollectionsOfElementsFrom(PersistentObject source) {
            foreach (var targetField in this.PersistentFieldsForCollectionsOfElements) {
                var sourceField = source.GetPersistentFieldForCollectionOfElements(targetField.Key);
                if (null != sourceField && targetField.ContentBaseType == sourceField.ContentBaseType) {
                    targetField.Clear();
                    foreach (var value in sourceField.CopyValues()) {
                        targetField.AddObject(value);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Copies the state of all fields for collections of
        /// persistent objects of another instance of this type into
        /// this instance.
        /// </summary>
        /// <param name="source">source instance to copy state from</param>
        /// <param name="copyBehaviorForAllowedGroups">determines how
        /// to copy allowed groups</param>
        /// <param name="copyBehaviorForAggregations">determines how
        /// to copy child objects that are not "owned" by their
        /// parent object</param>
        /// <param name="copyBehaviorForCompositions">determines how
        /// to copy child objects that are "owned" by their parent
        /// object</param>
        /// <param name="processedObjects">list of processed objects</param>
        private void CopyFieldsForCollectionsOfPersistentObjectsFrom(PersistentObject source, CopyBehaviorForAllowedGroups copyBehaviorForAllowedGroups, CopyBehaviorForAggregations copyBehaviorForAggregations, CopyBehaviorForCompositions copyBehaviorForCompositions, PersistentObjectCache processedObjects) {
            foreach (var targetField in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                var sourceField = source.GetPersistentFieldForCollectionOfPersistentObjects(targetField.Key);
                if (null != sourceField) {
                    if (sourceField.IsComposition && CopyBehaviorForCompositions.DeepCopy == copyBehaviorForCompositions) {
                        targetField.Clear();
                        foreach (var sourceObject in sourceField.GetValuesAsPersistentObject()) {
                            if (null == sourceObject) {
                                targetField.AddObject(null);
                            } else if (processedObjects.Contains(sourceObject.Id)) {
                                targetField.AddObject(processedObjects[sourceObject.Id]);
                            } else {
                                PersistentObject persistentObject;
                                var parentPersistenceMechanism = this.ParentPersistentContainer?.ParentPersistenceMechanism ?? source.ParentPersistentContainer?.ParentPersistenceMechanism;
                                if (null == parentPersistenceMechanism) {
                                    persistentObject = Activator.CreateInstance(sourceObject.Type) as PersistentObject;
                                } else {
                                    persistentObject = parentPersistenceMechanism.CreateInstance(sourceObject.Type);
                                }
                                processedObjects.Add(persistentObject);
                                persistentObject.CopyFrom(sourceObject, copyBehaviorForAllowedGroups, copyBehaviorForAggregations, copyBehaviorForCompositions, processedObjects);
                                targetField.AddObject(persistentObject);
                            }
                        }
                    } else if ((sourceField.IsComposition && CopyBehaviorForCompositions.ShallowCopy == copyBehaviorForCompositions) || (!sourceField.IsComposition && CopyBehaviorForAggregations.ShallowCopy == copyBehaviorForAggregations)) {
                        targetField.Clear();
                        foreach (var sourceObject in sourceField.GetValuesAsPersistentObject()) {
                            if (null == sourceObject) {
                                targetField.AddObject(null);
                            } else if (processedObjects.Contains(sourceObject.Id)) {
                                targetField.AddObject(processedObjects[sourceObject.Id]);
                            } else {
                                processedObjects.Add(sourceObject);
                                targetField.AddObject(sourceObject);
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Copies the state of all fields for elements of another
        /// instance of this type into this instance.
        /// </summary>
        /// <param name="source">source instance to copy state from</param>
        internal void CopyFieldsForElementsFrom(PersistentObject source) {
            foreach (var targetField in this.PersistentFieldsForElements) {
                if (nameof(PersistentObject.Id) != targetField.Key) {
                    var sourceField = source.GetPersistentFieldForElement(targetField.Key);
                    if (null != sourceField && targetField.ContentBaseType == sourceField.ContentBaseType) {
                        targetField.ValueAsObject = sourceField.CopyValue();
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Copies the state of all fields for persistent objects
        /// of another instance of this type into this instance.
        /// </summary>
        /// <param name="source">source instance to copy state from</param>
        /// <param name="copyBehaviorForAllowedGroups">determines how
        /// to copy allowed groups</param>
        /// <param name="copyBehaviorForAggregations">determines how
        /// to copy child objects that are not "owned" by their
        /// parent object</param>
        /// <param name="copyBehaviorForCompositions">determines how
        /// to copy child objects that are "owned" by their parent
        /// object</param>
        /// <param name="processedObjects">list of processed objects</param>
        private void CopyFieldsForPersistentObjectsFrom(PersistentObject source, CopyBehaviorForAllowedGroups copyBehaviorForAllowedGroups, CopyBehaviorForAggregations copyBehaviorForAggregations, CopyBehaviorForCompositions copyBehaviorForCompositions, PersistentObjectCache processedObjects) {
            foreach (var targetField in this.PersistentFieldsForPersistentObjects) {
                var sourceField = source.GetPersistentFieldForPersistentObject(targetField.Key);
                if (null != sourceField) {
                    if (nameof(PersistentObject.AllowedGroups) == targetField.Key) {
                        if (CopyBehaviorForAllowedGroups.ShallowCopy == copyBehaviorForAllowedGroups) {
                            targetField.ValueAsObject = sourceField.ValueAsObject;
                        }
                    } else {
                        if (sourceField.IsComposition && CopyBehaviorForCompositions.DeepCopy == copyBehaviorForCompositions) {
                            var sourceObject = sourceField.ValueAsPersistentObject;
                            if (null == sourceObject) {
                                targetField.ValueAsObject = null;
                            } else if (processedObjects.Contains(sourceObject.Id)) {
                                targetField.ValueAsObject = processedObjects[sourceObject.Id];
                            } else {
                                PersistentObject persistentObject;
                                var parentPersistenceMechanism = this.ParentPersistentContainer?.ParentPersistenceMechanism ?? source.ParentPersistentContainer?.ParentPersistenceMechanism;
                                if (null == parentPersistenceMechanism) {
                                    persistentObject = Activator.CreateInstance(sourceObject.Type) as PersistentObject;
                                } else {
                                    persistentObject = parentPersistenceMechanism.CreateInstance(sourceObject.Type);
                                }
                                if (true == targetField.ValueAsPersistentObject?.IsNew) {
                                    persistentObject.AllowedGroups = targetField.ValueAsPersistentObject.AllowedGroups;
                                }
                                processedObjects.Add(persistentObject);
                                persistentObject.CopyFrom(sourceObject, copyBehaviorForAllowedGroups, copyBehaviorForAggregations, copyBehaviorForCompositions, processedObjects);
                                targetField.ValueAsObject = persistentObject;
                            }
                        } else if ((sourceField.IsComposition && CopyBehaviorForCompositions.ShallowCopy == copyBehaviorForCompositions) || (!sourceField.IsComposition && CopyBehaviorForAggregations.ShallowCopy == copyBehaviorForAggregations)) {
                            var sourceObject = sourceField.ValueAsPersistentObject;
                            if (null == sourceObject) {
                                targetField.ValueAsObject = null;
                            } else if (processedObjects.Contains(sourceObject.Id)) {
                                targetField.ValueAsObject = processedObjects[sourceObject.Id];
                            } else {
                                processedObjects.Add(sourceObject);
                                targetField.ValueAsObject = sourceObject;
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Copies the state of another instance of this type
        /// into this instance.
        /// </summary>
        /// <param name="source">source instance to copy state from</param>
        /// <param name="copyBehaviorForAllowedGroups">determines how
        /// to copy allowed groups</param>
        /// <param name="copyBehaviorForAggregations">determines how
        /// to copy child objects that are not "owned" by their
        /// parent object</param>
        /// <param name="copyBehaviorForCompositions">determines how
        /// to copy child objects that are "owned" by their parent
        /// object</param>
        public void CopyFrom(PersistentObject source, CopyBehaviorForAllowedGroups copyBehaviorForAllowedGroups, CopyBehaviorForAggregations copyBehaviorForAggregations, CopyBehaviorForCompositions copyBehaviorForCompositions) {
            this.CopyFrom(source, copyBehaviorForAllowedGroups, copyBehaviorForAggregations, copyBehaviorForCompositions, new PersistentObjectCache());
            return;
        }

        /// <summary>
        /// Copies the state of another instance of this type
        /// into this instance.
        /// </summary>
        /// <param name="source">source instance to copy state from</param>
        /// <param name="copyBehaviorForAllowedGroups">determines how
        /// to copy allowed groups</param>
        /// <param name="copyBehaviorForAggregations">determines how
        /// to copy child objects that are not "owned" by their
        /// parent object</param>
        /// <param name="copyBehaviorForCompositions">determines how
        /// to copy child objects that are "owned" by their parent
        /// object</param>
        /// <param name="processedObjects">list of processed objects</param>
        private void CopyFrom(PersistentObject source, CopyBehaviorForAllowedGroups copyBehaviorForAllowedGroups, CopyBehaviorForAggregations copyBehaviorForAggregations, CopyBehaviorForCompositions copyBehaviorForCompositions, PersistentObjectCache processedObjects) {
            this.CopyFieldsForElementsFrom(source);
            this.CopyFieldsForCollectionsOfElementsFrom(source);
            this.CopyFieldsForPersistentObjectsFrom(source, copyBehaviorForAllowedGroups, copyBehaviorForAggregations, copyBehaviorForCompositions, processedObjects);
            this.CopyFieldsForCollectionsOfPersistentObjectsFrom(source, copyBehaviorForAllowedGroups, copyBehaviorForAggregations, copyBehaviorForCompositions, processedObjects);
            return;
        }

        /// <summary>
        /// Creates new child instances cascadedly for all persistent
        /// fields owned by their parent fields.
        /// </summary>
        public void CreateChildInstancesCascadedly() {
            this.CreateChildInstancesCascadedly(new List<string>());
            return;
        }

        /// <summary>
        /// Creates new child instances cascadedly for all persistent
        /// fields owned by their parent fields.
        /// </summary>
        private void CreateChildInstancesCascadedly(List<string> processedKeyTypeCombinations) {
            foreach (var persistentFieldForPersistentObject in this.PersistentFieldsForPersistentObjects) {
                if (persistentFieldForPersistentObject.IsComposition && null == persistentFieldForPersistentObject.ValueAsObject) {
                    var childObject = persistentFieldForPersistentObject.NewItemAsObject();
                    persistentFieldForPersistentObject.ValueAsObject = childObject;
                    var keyTypeCombination = persistentFieldForPersistentObject.Key + childObject.GetType().AssemblyQualifiedName;
                    if (!processedKeyTypeCombinations.Contains(keyTypeCombination)) {
                        processedKeyTypeCombinations.Add(keyTypeCombination);
                        (childObject as PersistentObject)?.CreateChildInstancesCascadedly(processedKeyTypeCombinations);
                    }
                }
            }
            foreach (var persistentFieldForCollectionOfPersistentObjects in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                if (persistentFieldForCollectionOfPersistentObjects.IsComposition && persistentFieldForCollectionOfPersistentObjects.Count < 1) {
                    var childObject = persistentFieldForCollectionOfPersistentObjects.NewItemAsObject();
                    persistentFieldForCollectionOfPersistentObjects.AddObject(childObject);
                    var keyTypeCombination = persistentFieldForCollectionOfPersistentObjects.Key + childObject.GetType().AssemblyQualifiedName;
                    if (!processedKeyTypeCombinations.Contains(keyTypeCombination)) {
                        processedKeyTypeCombinations.Add(keyTypeCombination);
                        (childObject as PersistentObject)?.CreateChildInstancesCascadedly(processedKeyTypeCombinations);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Finds the first presentable field for a specified key.
        /// </summary>
        /// <param name="key">key to find presentable field for</param>
        /// <returns>first presentable field for specified key or null</returns>
        public IPresentableField FindPresentableField(string key) {
            return this.FindPresentableField(KeyChain.FromKey(key));
        }

        /// <summary>
        /// Finds the first presentable field for a specified key
        /// chain.
        /// </summary>
        /// <param name="keyChain">key chain to find presentable
        /// field for</param>
        /// <returns>first presentable field for specified key chain
        /// or null</returns>
        public IPresentableField FindPresentableField(string[] keyChain) {
            IPresentableField resultField = null;
            foreach (var presentableField in this.FindPresentableFields(keyChain)) {
                if (null == resultField) {
                    resultField = presentableField;
                } else {
                    throw new ArgumentException("The key chain \"" + KeyChain.ToKey(keyChain) + "\" is not unique for persistent object of type " + this.Type.FullName + ".", nameof(keyChain));
                }
            }
            return resultField;
        }

        /// <summary>
        /// Finds all presentable fields for a specified key.
        /// </summary>
        /// <param name="key">key to find presentable fields for</param>
        /// <returns>all presentable fields for specified key or null</returns>
        public IEnumerable<IPresentableField> FindPresentableFields(string key) {
            return this.FindPresentableFields(KeyChain.FromKey(key));
        }

        /// <summary>
        /// Finds all presentable fields for a specified key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find presentable
        /// fields for</param>
        /// <returns>all presentable fields for specified key chain
        /// or null</returns>
        public IEnumerable<IPresentableField> FindPresentableFields(string[] keyChain) {
            return PresentableFieldBrowser.FindPresentableFields(keyChain, this.GetPresentableFields());
        }

        /// <summary>
        /// Gets the full text of object and all of its persistent
        /// child objects.
        /// </summary>
        /// <returns>full text of object and all of its persistent
        /// child objects</returns>
        internal string GetFullTextCascadedly() {
            var fullText = new StringBuilder();
            this.GetFullTextCascadedly(fullText, new List<Guid>());
            return fullText.ToString();
        }

        /// <summary>
        /// Gets the full text of object and all of its persistent
        /// child objects.
        /// </summary>
        /// <param name="fullText">full text collected so far</param>
        /// <param name="checkedIds">list of guids that were already
        /// checked</param>
        /// <returns>full text of object and all of its persistent
        /// child objects</returns>
        private void GetFullTextCascadedly(StringBuilder fullText, IList<Guid> checkedIds) {
            checkedIds.Add(this.Id);
            foreach (var persistentField in this.PersistentFieldsForElements) {
                if (persistentField.IsFullTextIndexed) {
                    PersistentObject.AppendToFullText(persistentField.GetValueAsPlainText(), fullText);
                }
            }
            foreach (var persistentField in this.PersistentFieldsForCollectionsOfElements) {
                if (persistentField.IsFullTextIndexed) {
                    foreach (var text in persistentField.GetValuesAsPlainText()) {
                        PersistentObject.AppendToFullText(text, fullText);
                    }
                }
            }
            foreach (var persistentField in this.PersistentFieldsForPersistentObjects) {
                if (persistentField.IsFullTextIndexed) {
                    if (persistentField.IsComposition) {
                        var value = persistentField.ValueAsPersistentObject;
                        if (null != value && !value.IsRemoved && !checkedIds.Contains(value.Id)) {
                            value.GetFullTextCascadedly(fullText, checkedIds);
                        }
                    } else {
                        var value = persistentField.ValueAsPersistentObject;
                        if (null != value && !value.IsRemoved && !checkedIds.Contains(value.Id)) {
                            PersistentObject.AppendToFullText((value as IProvidableObject)?.GetTitle(), fullText);
                        }
                    }
                }
            }
            foreach (var persistentField in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                if (persistentField.IsFullTextIndexed) {
                    if (persistentField.IsComposition) {
                        foreach (var objectValue in persistentField.GetValuesAsObject()) {
                            if (objectValue is PersistentObject value && !value.IsRemoved && !checkedIds.Contains(value.Id)) {
                                value.GetFullTextCascadedly(fullText, checkedIds);
                            }
                        }
                    } else {
                        foreach (var objectValue in persistentField.GetValuesAsObject()) {
                            if (objectValue is PersistentObject value && !value.IsRemoved && !checkedIds.Contains(value.Id)) {
                                PersistentObject.AppendToFullText((value as IProvidableObject)?.GetTitle(), fullText);
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Gets true if the values of this object or any of its
        /// persistent child objects were changed since last
        /// retrieval, false otherwise.
        /// </summary>
        public bool GetIsChangedCascededly() {
            return this.GetIsChangedCascededly(new List<Guid>());
        }

        /// <summary>
        /// Gets true if the values of this object or any of its
        /// persistent child objects were changed since last
        /// retrieval, false otherwise.
        /// </summary>
        /// <param name="checkedIds">list of guids that were
        /// already checked</param>
        private bool GetIsChangedCascededly(IList<Guid> checkedIds) {
            bool isChangedCascededly;
            checkedIds.Add(this.Id);
            if (this.IsChanged) {
                isChangedCascededly = true;
            } else {
                isChangedCascededly = false;
                foreach (var persistentField in this.PersistentFieldsForPersistentObjects) {
                    if (persistentField.IsRetrieved) {
                        var persistentChildObject = persistentField.ValueAsPersistentObject;
                        if (null != persistentChildObject && !checkedIds.Contains(persistentChildObject.Id)) {
                            isChangedCascededly = persistentChildObject.GetIsChangedCascededly(checkedIds);
                            if (isChangedCascededly) {
                                break;
                            }
                        }
                    }
                }
                if (!isChangedCascededly) {
                    foreach (var persistentField in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                        if (persistentField.IsRetrieved) {
                            foreach (var persistentChildObject in persistentField.GetValuesAsPersistentObject()) {
                                if (null != persistentChildObject && !checkedIds.Contains(persistentChildObject.Id)) {
                                    isChangedCascededly = persistentChildObject.GetIsChangedCascededly(checkedIds);
                                    if (isChangedCascededly) {
                                        break;
                                    }
                                }
                            }
                        }
                        if (isChangedCascededly) {
                            break;
                        }
                    }
                }
            }
            return isChangedCascededly;
        }

        /// <summary>
        /// Gets the persistent field for collection containing
        /// values of type other than PersistentObject with a
        /// specified key (case insensitive).
        /// </summary>
        /// <param name="key">key of persistent field to get</param>
        /// <returns>persistent field for specified key</returns>
        internal PersistentFieldForCollection GetPersistentFieldForCollectionOfElements(string key) {
            var keyToUpper = key.ToUpperInvariant();
            PersistentFieldForCollection persistentField = null;
            foreach (var field in this.persistentFieldsForCollectionsOfElements) {
                if (field.Key.ToUpperInvariant() == keyToUpper) {
                    persistentField = field;
                    break;
                }
            }
            return persistentField;
        }

        /// <summary>
        /// Gets the persistent field for collection containing
        /// values of type PersistentObject with a specified key
        /// (case insensitive).
        /// </summary>
        /// <param name="key">key of persistent field to get</param>
        /// <returns>persistent field for specified key</returns>
        internal PersistentFieldForPersistentObjectCollection GetPersistentFieldForCollectionOfPersistentObjects(string key) {
            var keyToUpper = key.ToUpperInvariant();
            PersistentFieldForPersistentObjectCollection persistentField = null;
            foreach (var field in this.persistentFieldsForCollectionsOfPersistentObjects) {
                if (field.Key.ToUpperInvariant() == keyToUpper) {
                    persistentField = field;
                    break;
                }
            }
            return persistentField;
        }

        /// <summary>
        /// Gets the persistent field for element of any type other
        /// than PersistentObject with a specified key (case
        /// insensitive).
        /// </summary>
        /// <param name="key">key of persistent field to get</param>
        /// <returns>persistent field for specified key</returns>
        internal PersistentFieldForElement GetPersistentFieldForElement(string key) {
            var keyToUpper = key.ToUpperInvariant();
            PersistentFieldForElement persistentField = null;
            foreach (var field in this.persistentFieldsForElements) {
                if (field.Key.ToUpperInvariant() == keyToUpper) {
                    persistentField = field;
                    break;
                }
            }
            return persistentField;
        }

        /// <summary>
        /// Gets the persistent field for element of type
        /// PersistentObject with a specified key (case insensitive).
        /// </summary>
        /// <param name="key">key of persistent field to get</param>
        /// <returns>persistent field for specified key</returns>
        internal PersistentFieldForPersistentObject GetPersistentFieldForPersistentObject(string key) {
            var keyToUpper = key.ToUpperInvariant();
            PersistentFieldForPersistentObject persistentField = null;
            foreach (var field in this.persistentFieldsForPersistentObjects) {
                if (field.Key.ToUpperInvariant() == keyToUpper) {
                    persistentField = field;
                    break;
                }
            }
            return persistentField;
        }

        /// <summary>
        /// Gets all persistent fields.
        /// </summary>
        /// <returns>enumerable of persistent fields</returns>
        public IEnumerable<PersistentField> GetPersistentFields() {
            foreach (var persistentFieldForElement in this.PersistentFieldsForElements) {
                yield return persistentFieldForElement;
            }
            foreach (var persistentFieldForCollectionOfElements in this.PersistentFieldsForCollectionsOfElements) {
                yield return persistentFieldForCollectionOfElements;
            }
            foreach (var persistentFieldForPersistentObject in this.PersistentFieldsForPersistentObjects) {
                yield return persistentFieldForPersistentObject;
            }
            foreach (var persistentFieldForCollectionOfPersistentObjects in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                yield return persistentFieldForCollectionOfPersistentObjects;
            }
        }

        /// <summary>
        /// Gets all persistent fields cascadedly.
        /// </summary>
        /// <returns>pairs of key chain and field of all persistent
        /// fields cascadedly</returns>
        public IEnumerable<KeyValuePair<string[], PersistentField>> GetPersistentFieldsCascadedly() {
            return this.GetPersistentFieldsCascadedly(new string[0], new List<Guid>());
        }

        /// <summary>
        /// Gets all persistent fields cascadedly.
        /// </summary>
        /// <param name="parentKeyChain">key chain of parent fields</param>
        /// <param name="returnedIds">list of IDs already returned</param>
        /// <returns>pairs of key chain and field of all persistent
        /// fields cascadedly</returns>
        private IEnumerable<KeyValuePair<string[], PersistentField>> GetPersistentFieldsCascadedly(string[] parentKeyChain, IList<Guid> returnedIds) {
            returnedIds.Add(this.Id);
            foreach (var persistentField in this.GetPersistentFields()) {
                var keyChain = new string[parentKeyChain.LongLength + 1L];
                parentKeyChain.CopyTo(keyChain, 0L);
                keyChain[keyChain.LongLength - 1L] = persistentField.Key;
                yield return new KeyValuePair<string[], PersistentField>(keyChain, persistentField);
            }
            foreach (var persistentFieldForPersistentObject in this.PersistentFieldsForPersistentObjects) {
                if (persistentFieldForPersistentObject.IsComposition && null != persistentFieldForPersistentObject.ValueAsPersistentObject && !returnedIds.Contains(persistentFieldForPersistentObject.ValueAsPersistentObject.Id)) {
                    var keyChain = new string[parentKeyChain.LongLength + 1L];
                    parentKeyChain.CopyTo(keyChain, 0L);
                    keyChain[keyChain.LongLength - 1L] = persistentFieldForPersistentObject.Key;
                    foreach (var persistentField in persistentFieldForPersistentObject.ValueAsPersistentObject.GetPersistentFieldsCascadedly(keyChain, returnedIds)) {
                        yield return persistentField;
                    }
                }
            }
            foreach (var persistentFieldForCollectionOfPersistentObjects in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                if (persistentFieldForCollectionOfPersistentObjects.IsComposition) {
                    var keyChain = new string[parentKeyChain.LongLength + 1L];
                    parentKeyChain.CopyTo(keyChain, 0L);
                    keyChain[keyChain.LongLength - 1L] = persistentFieldForCollectionOfPersistentObjects.Key;
                    ulong index = 0;
                    foreach (var persistentChildObject in persistentFieldForCollectionOfPersistentObjects.GetValuesAsPersistentObject()) {
                        if (null != persistentChildObject && !returnedIds.Contains(persistentChildObject.Id)) {
                            var persistentChildObjectKeyChain = new string[keyChain.LongLength];
                            keyChain.CopyTo(persistentChildObjectKeyChain, 0L);
                            persistentChildObjectKeyChain[persistentChildObjectKeyChain.LongLength - 1L] += "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
                            foreach (var persistentField in persistentChildObject.GetPersistentFieldsCascadedly(persistentChildObjectKeyChain, returnedIds)) {
                                yield return persistentField;
                            }
                        }
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all fields with child objects with cascaded removal
        /// behaviour set to RemoveValuesIfTheyAreNotReferenced or
        /// RemoveValuesForcibly having different allowed groups than
        /// their parent cascadedly.
        /// </summary>
        /// <returns>all fields with child objects with cascaded
        /// removal behaviour set to RemoveValuesIfTheyAreNotReferenced
        /// or RemoveValuesForcibly having different allowed groups
        /// than their parent cascadedly</returns>
        public IEnumerable<PersistentField> GetPersistentFieldsWithDifferentAllowedGroupsCascadedly() {
            return this.GetPersistentFieldsWithDifferentAllowedGroupsCascadedly(new List<PersistentObject>());
        }

        /// <summary>
        /// Gets all fields with child objects with cascaded removal
        /// behaviour set to RemoveValuesIfTheyAreNotReferenced or
        /// RemoveValuesForcibly having different allowed groups than
        /// their parent cascadedly.
        /// </summary>
        /// <param name="processedObjects">list of objects that have
        /// been processed before</param>
        /// <returns>all fields with child objects with cascaded
        /// removal behaviour set to RemoveValuesIfTheyAreNotReferenced
        /// or RemoveValuesForcibly having different allowed groups
        /// than their parent cascadedly</returns>
        private IEnumerable<PersistentField> GetPersistentFieldsWithDifferentAllowedGroupsCascadedly(IList<PersistentObject> processedObjects) {
            foreach (var persistentFieldForPersistentObject in this.PersistentFieldsForPersistentObjects) {
                if (persistentFieldForPersistentObject.IsComposition) {
                    var value = persistentFieldForPersistentObject.ValueAsPersistentObject;
                    if (null != value && value.Type != TypeOf.AllowedGroups && !processedObjects.Contains(value)) {
                        processedObjects.Add(value);
                        if (this.AllowedGroups != value.AllowedGroups) {
                            yield return persistentFieldForPersistentObject;
                        }
                        foreach (var persistentFieldWithDifferentAllowedGroups in value.GetPersistentFieldsWithDifferentAllowedGroupsCascadedly(processedObjects)) {
                            yield return persistentFieldWithDifferentAllowedGroups;
                        }
                    }
                }
            }
            foreach (var persistentFieldForCollectionOfPersistentObjects in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                if (persistentFieldForCollectionOfPersistentObjects.IsComposition) {
                    foreach (var value in persistentFieldForCollectionOfPersistentObjects.GetValuesAsPersistentObject()) {
                        if (null != value && !processedObjects.Contains(value)) {
                            processedObjects.Add(value);
                            if (this.AllowedGroups != value.AllowedGroups) {
                                yield return persistentFieldForCollectionOfPersistentObjects;
                            }
                            foreach (var persistentFieldWithDifferentAllowedGroups in value.GetPersistentFieldsWithDifferentAllowedGroupsCascadedly(processedObjects)) {
                                yield return persistentFieldWithDifferentAllowedGroups;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all presentable fields.
        /// </summary>
        /// <returns>enumerable of presentable fields</returns>
        protected virtual IEnumerable<IPresentableField> GetPresentableFields() {
            foreach (var persistentField in this.GetPersistentFields()) {
                yield return persistentField;
            }
        }

        /// <summary>
        /// Gets all references to persistent object. This method
        /// should be used with care because it might run for a long
        /// time.
        /// </summary>
        /// <returns>all references to persistent object</returns>
        public ReadOnlyCollection<PersistentObject> GetReferencingPersistentObjects() {
            ReadOnlyCollection<PersistentObject> references;
            if (this.IsNew) {
                references = new List<PersistentObject>(0).AsReadOnly();
            } else {
                references = this.ParentPersistentContainer.ParentPersistenceMechanism.GetReferencingPersistentObjectsTo(this);
            }
            return references;
        }

        /// <summary>
        /// Gets all distinct topmost objects referencing persistent
        /// object.
        /// </summary>
        /// <param name="typesOfTopmostObjects">types of topmost
        /// objects to list as references</param>
        /// <returns>distinct topmost objects referencing persistent
        /// object</returns>
        public IList<IProvidableObject> GetReferencingPersistentObjects(IEnumerable<Type> typesOfTopmostObjects) {
            return this.GetReferencingPersistentObjects(typesOfTopmostObjects, new Type[0]);
        }

        /// <summary>
        /// Gets all distinct topmost objects referencing persistent
        /// object.
        /// </summary>
        /// <param name="typesOfTopmostObjects">types of topmost
        /// objects to list as references</param>
        /// <param name="typesOfBarrierObjects">types of presentable
        /// objects to act as barrier when resolving topmost
        /// presentable objects - usually barrier types are needed to
        /// adjust the traversal behaviour on object tree if child
        /// instances can be accessed via multiple paths</param>
        /// <returns>distinct topmost objects referencing persistent
        /// object</returns>
        public IList<IProvidableObject> GetReferencingPersistentObjects(IEnumerable<Type> typesOfTopmostObjects, IEnumerable<Type> typesOfBarrierObjects) {
            return this.GetReferencingPersistentObjects(typesOfTopmostObjects, typesOfBarrierObjects, int.MaxValue);
        }

        /// <summary>
        /// Gets all distinct topmost objects referencing persistent
        /// object.
        /// </summary>
        /// <param name="typesOfTopmostObjects">types of topmost
        /// objects to list as references</param>
        /// <param name="typesOfBarrierObjects">types of presentable
        /// objects to act as barrier when resolving topmost
        /// presentable objects - usually barrier types are needed to
        /// adjust the traversal behaviour on object tree if child
        /// instances can be accessed via multiple paths</param>
        /// <param name="maxResults">maximum number of results to get</param>
        /// <returns>distinct topmost objects referencing persistent
        /// object</returns>
        private IList<IProvidableObject> GetReferencingPersistentObjects(IEnumerable<Type> typesOfTopmostObjects, IEnumerable<Type> typesOfBarrierObjects, int maxResults) {
            var references = new List<IProvidableObject>();
            var elements = new Stack<PersistentObject>();
            var element = this;
            while (references.Count < maxResults) {
                foreach (var referencingObject in element.GetReferencingPersistentObjects()) {
                    var isTypeOfReferencingObjectMatching = false;
                    foreach (var typeOfTopmostObject in typesOfTopmostObjects) {
                        if (typeOfTopmostObject.IsAssignableFrom(referencingObject.Type)) {
                            isTypeOfReferencingObjectMatching = true;
                            break;
                        }
                    }
                    if (isTypeOfReferencingObjectMatching && referencingObject is IProvidableObject referencingProvidableObject) {
                        if (!references.Contains(referencingProvidableObject)) {
                            references.Add(referencingProvidableObject);
                        }
                    } else {
                        var isTypeOfReferencingObjectBarrier = false;
                        foreach (var typeOfBarrierObject in typesOfBarrierObjects) {
                            if (typeOfBarrierObject.IsAssignableFrom(referencingObject.Type)) {
                                isTypeOfReferencingObjectBarrier = true;
                                break;
                            }
                        }
                        if (!isTypeOfReferencingObjectBarrier) {
                            elements.Push(referencingObject);
                        }
                    }
                }
                if (elements.Count > 0) {
                    element = elements.Pop();
                } else {
                    break;
                }
            }
            return references;
        }

        /// <summary>
        /// Gets the version of this persistent object for a specific
        /// date.
        /// </summary>
        /// <param name="modificationDate">date to get version for</param>
        /// <returns>version of this persistent object if existing,
        /// null otherwise</returns>
        internal PersistentObject GetVersionValue(DateTime? modificationDate) {
            PersistentObject versionValue = null;
            if (modificationDate.HasValue) {
                if (modificationDate.Value >= this.ModifiedAt) {
                    versionValue = this;
                } else {
                    var version = this.ParentPersistentContainer.FindOneVersion(this, modificationDate.Value);
                    if (null != version) {
                        versionValue = version.Value;
                    }
                }
            }
            return versionValue;
        }

        /// <summary>
        /// Gets this element with elevated privileges.
        /// </summary>
        /// <returns>either this element or a copy of this element
        /// with elevated privileges</returns>
        internal PersistentObject GetWithElevatedPrivileges() {
            PersistentObject elevatedElement;
            if (!this.IsNew && this.ParentPersistentContainer.ParentPersistenceMechanism.SecurityModel != SecurityModel.IgnorePermissions) {
                var elevatedPersistenceMechnism = this.ParentPersistentContainer.ParentPersistenceMechanism.CopyWithElevatedPrivileges();
                elevatedElement = elevatedPersistenceMechnism.FindContainer(this.Type).FindOnePersistentObject(this.Id);
            } else {
                elevatedElement = this;
            }
            return elevatedElement;
        }

        /// <summary>
        /// Sets the allowed groups of this persistent object to all
        /// persistent child objects, but not to their child objects.
        /// </summary>
        internal void HandDownAllowedGroups() {
            if (null != this.AllowedGroups) {
                foreach (var persistentField in this.PersistentFieldsForPersistentObjects) {
                    this.HandDownAllowedGroups(persistentField.ValueAsPersistentObject);
                }
                foreach (var persistentField in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                    foreach (var persistentChildObject in persistentField.GetValuesAsPersistentObject()) {
                        this.HandDownAllowedGroups(persistentChildObject);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Sets the allowed groups of this persistent object to a
        /// persistent child object, but not to its child objects.
        /// </summary>
        /// <param name="persistentObject">persistent child object</param>
        private void HandDownAllowedGroups(PersistentObject persistentObject) {
            if (null != persistentObject && null == persistentObject.AllowedGroups) {
                persistentObject.AllowedGroups = this.AllowedGroups;
            }
            return;
        }

        /// <summary>
        /// Checks the existence of references to persistent object.
        /// </summary>
        /// <returns>true if references to persistent object exist,
        /// false otherwise</returns>
        public bool HasReferencingPersistentObjects() {
            bool hasReferences;
            if (this.IsNew) {
                hasReferences = false;
            } else {
                hasReferences = this.ParentPersistentContainer.ParentPersistenceMechanism.ContainsReferencingPersistentObjectsTo(this);
            }
            return hasReferences;
        }

        /// <summary>
        /// Checks the existence of references to persistent object
        /// by specific topmost objects cascadedly.
        /// </summary>
        /// <param name="typesOfTopmostObjects">types of topmost
        /// objects to check existence of references for</param>
        /// <returns>true if references to persistent object by
        /// topmost objects exist, false otherwise</returns>
        public bool HasReferencingPersistentObjects(IEnumerable<Type> typesOfTopmostObjects) {
            return this.GetWithElevatedPrivileges().GetReferencingPersistentObjects(typesOfTopmostObjects, new Type[0], 1).Count > 0;
        }

        /// <summary>
        /// Checks the existence of references to persistent object
        /// by specific topmost objects cascadedly.
        /// </summary>
        /// <param name="typesOfTopmostObjects">types of topmost
        /// objects to check existence of references for</param>
        /// <param name="typesOfBarrierObjects">types of presentable
        /// objects to act as barrier when resolving topmost
        /// presentable objects - usually barrier types are needed to
        /// adjust the traversal behaviour on object tree if child
        /// instances can be accessed via multiple paths</param>
        /// <returns>true if references to persistent object by
        /// topmost objects exist, false otherwise</returns>
        public bool HasReferencingPersistentObjects(IEnumerable<Type> typesOfTopmostObjects, IEnumerable<Type> typesOfBarrierObjects) {
            return this.GetWithElevatedPrivileges().GetReferencingPersistentObjects(typesOfTopmostObjects, typesOfBarrierObjects, 1).Count > 0;
        }

        /// <summary>
        /// Indicates whether all child objects with cascaded removal
        /// behaviour set to RemoveValuesIfTheyAreNotReferenced or
        /// RemoveValuesForcibly have the same allowed groups as
        /// parent cascadedly.
        /// </summary>
        /// <returns>true if all child objects with cascaded removal
        /// behaviour set to RemoveValuesIfTheyAreNotReferenced or
        /// RemoveValuesForcibly have the same allowed groups as
        /// parent cascadedly, false otherwise</returns>
        public bool HasSameAllowedGroupsCascadedly() {
            var hasSameAllowedGroupsCascadedly = true;
            foreach (var persistentFieldWithDifferentAllowedGroups in this.GetPersistentFieldsWithDifferentAllowedGroupsCascadedly()) {
                hasSameAllowedGroupsCascadedly = false;
                break;
            }
            return hasSameAllowedGroupsCascadedly;
        }

        /// <summary>
        /// Prepares a persistent field for registration.
        /// </summary>
        /// <param name="persistentField">persistent field to prepare
        /// for registration</param>
        private void PreparePersistentFieldForRegistration(PersistentField persistentField) {
            if (this.IsRetrievedPartially) {
                throw new PersistenceException("Persistent field with key \"" + persistentField.Key
                    + "\" cannot be registered, because the parent object of type " + this.Type.FullName
                    + " is retrieved already. Please make sure not to get or set any persistent values before registering persistent fields.");
            }
#if DEBUG
            foreach (var existingPersistentField in this.GetPersistentFields()) {
                if (persistentField.Key == existingPersistentField.Key) {
                    throw new PersistenceException("Persistent field with key \"" + persistentField.Key
                        + "\" cannot be registered because a persistent field with this key exists already.");
                }
            }
#endif
            persistentField.ParentPersistentObject = this;
            return;
        }

        /// <summary>
        /// Registeres a persistent field to be saved to and loaded
        /// from persistence mechanism. Do not call this method
        /// outside of the constructor!
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// register</param>
        protected void RegisterPersistentField(PersistentFieldForCollection persistentField) {
            this.PreparePersistentFieldForRegistration(persistentField);
            this.persistentFieldsForCollectionsOfElements.Add(persistentField);
            return;
        }

        /// <summary>
        /// Registeres a persistent field to be saved to and loaded
        /// from persistence mechanism. Do not call this method
        /// outside of the constructor!
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// register</param>
        protected void RegisterPersistentField(PersistentFieldForPersistentObjectCollection persistentField) {
            this.PreparePersistentFieldForRegistration(persistentField);
            this.persistentFieldsForCollectionsOfPersistentObjects.Add(persistentField);
            return;
        }

        /// <summary>
        /// Registeres a persistent field to be saved to and loaded
        /// from persistence mechanism. Do not call this method
        /// outside of the constructor!
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// register</param>
        protected void RegisterPersistentField(PersistentFieldForElement persistentField) {
            this.PreparePersistentFieldForRegistration(persistentField);
            this.persistentFieldsForElements.Add(persistentField);
            return;
        }

        /// <summary>
        /// Registeres a persistent field to be saved to and loaded
        /// from persistence mechanism. Do not call this method
        /// outside of the constructor!
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// register</param>
        protected void RegisterPersistentField(PersistentFieldForPersistentObject persistentField) {
            this.PreparePersistentFieldForRegistration(persistentField);
            this.persistentFieldsForPersistentObjects.Add(persistentField);
            return;
        }

        /// <summary>
        /// Removes this object from persistence mechanism
        /// immediately.
        /// </summary>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        public bool Remove() {
            bool success;
            if (this.IsNew) {
                throw new ObjectNotPersistentException("Persistent object cannot be removed because it is not connected to a persistence mechanism.");
            } else {
                success = true == this.ParentPersistentContainer.ParentPersistenceMechanism.RemoveObject(this, false);
            }
            return success;
        }

        /// <summary>
        /// Removes this object from persistence mechanism
        /// immediately if it is not referenced by other objects.
        /// </summary>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, null if it was referenced by other
        /// objects, false otherwise or if object was not contained
        /// in persistence mechanism</returns>
        public bool? RemoveIfNotReferenced() {
            bool? success;
            if (this.IsNew) {
                throw new ObjectNotPersistentException("Persistent object cannot be removed because it is not connected to a persistence mechanism.");
            } else {
                success = this.ParentPersistentContainer.ParentPersistenceMechanism.RemoveObject(this, true);
            }
            return success;
        }

        /// <summary>
        /// Removes this object and all child objects to be removed
        /// cascadedly from persistence mechanism immediately.
        /// </summary>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        public bool RemoveCascadedly() {
            bool success;
            if (this.IsNew) {
                throw new ObjectNotPersistentException("Persistent object cannot be removed because it is not connected to a persistence mechanism.");
            } else {
                success = true == this.ParentPersistentContainer.ParentPersistenceMechanism.RemoveObjectCascadedly(this, false);
            }
            return success;
        }

        /// <summary>
        /// Removes this object and all child objects to be removed
        /// cascadedly from persistence mechanism immediately if it
        /// is not referenced by other objects.
        /// </summary>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, null if it was referenced by other
        /// objects, false otherwise or if object was not contained
        /// in persistence mechanism</returns>
        public bool? RemoveCascadedlyIfNotReferenced() {
            bool? success;
            if (this.IsNew) {
                throw new ObjectNotPersistentException("Persistent object cannot be removed because it is not connected to a persistence mechanism.");
            } else {
                success = this.ParentPersistentContainer.ParentPersistenceMechanism.RemoveObjectCascadedly(this, true);
            }
            return success;
        }

        /// <summary>
        /// Retrieves all values of this object from persistence
        /// mechanism. This can be used to refresh the values of any
        /// persistent object.
        /// </summary>
        public void Retrieve() {
            if (this.IsNew) {
                throw new ObjectNotPersistentException("Persistent object cannot be retrieved because it is not connected to a persistence mechanism.");
            } else {
                this.ParentPersistentContainer.Retrieve(this);
            }
            return;
        }

        /// <summary>
        /// Retrieves all values of this object from persistence
        /// mechanism and all of its persistent child objects
        /// cascadedly. This can be used to refresh the values of any
        /// persistent object cascadedly.
        /// </summary>
        public void RetrieveCascadedly() {
            this.RetrieveCascadedly(new List<Guid>());
            return;
        }

        /// <summary>
        /// Retrieves all values of this object from persistence
        /// mechanism and all of its persistent child objects
        /// cascadedly. This can be used to refresh the values of any
        /// persistent object cascadedly.
        /// </summary>
        /// <param name="idsOfCheckedObjects">IDs of objects that
        /// have already been checked during this recursion</param>
        private void RetrieveCascadedly(IList<Guid> idsOfCheckedObjects) {
            if (!idsOfCheckedObjects.Contains(this.Id)) {
                idsOfCheckedObjects.Add(this.Id);
                foreach (var persistentField in this.PersistentFieldsForElements) {
                    if (persistentField.IsRetrieved && nameof(PersistentObject.Id) != persistentField.Key) {
                        persistentField.Retrieve();
                        break; // all other persistent fields for elements will be retrieved together with the first one
                    }
                }
                foreach (var persistentField in this.PersistentFieldsForCollectionsOfElements) {
                    if (persistentField.IsRetrieved) {
                        persistentField.Retrieve();
                    }
                }
                foreach (var persistentField in this.PersistentFieldsForPersistentObjects) {
                    if (persistentField.IsRetrieved || persistentField.IsComposition) {
                        persistentField.Retrieve();
                        var value = persistentField.ValueAsPersistentObject;
                        if (null != value) {
                            value.RetrieveCascadedly(idsOfCheckedObjects);
                        }
                    }
                }
                foreach (var persistentField in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                    if (persistentField.IsRetrieved || persistentField.IsComposition) {
                        persistentField.Retrieve();
                        foreach (var persistentChildObject in persistentField.GetValuesAsPersistentObject()) {
                            persistentChildObject?.RetrieveCascadedly(idsOfCheckedObjects);
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Sets the supplied allowed groups to object and all child
        /// objects with cascaded removal behaviour set to
        /// RemoveValuesIfTheyAreNotReferenced or
        /// RemoveValuesForcibly cascadedly.
        /// WARNING: Be aware that using this method might lead to
        /// unwanted results if child objects are supposed to have
        /// different allowed groups than their parent objects.
        /// </summary>
        /// <param name="allowedGroups">allowed groups to be set
        /// cascadedly</param>
        public void SetAllowedGroupsCascadedly(AllowedGroups allowedGroups) {
            this.SetAllowedGroupsCascadedly(allowedGroups, new List<PersistentObject>());
            return;
        }

        /// <summary>
        /// Sets the supplied allowed groups to object and all child
        /// objects with cascaded removal behaviour set to
        /// RemoveValuesIfTheyAreNotReferenced or
        /// RemoveValuesForcibly cascadedly.
        /// WARNING: Be aware that using this method might lead to
        /// unwanted results if child objects are supposed to have
        /// different allowed groups than their parent objects.
        /// </summary>
        /// <param name="allowedGroups">allowed groups to be set
        /// cascadedly</param>
        /// <param name="processedObjects">list of objects that have
        /// been processed before</param>
        private void SetAllowedGroupsCascadedly(AllowedGroups allowedGroups, IList<PersistentObject> processedObjects) {
            this.AllowedGroups = allowedGroups;
            foreach (var persistentFieldForPersistentObject in this.PersistentFieldsForPersistentObjects) {
                if (persistentFieldForPersistentObject.IsComposition) {
                    var value = persistentFieldForPersistentObject.ValueAsPersistentObject;
                    if (null != value && value.Type != TypeOf.AllowedGroups && !processedObjects.Contains(value)) {
                        processedObjects.Add(value);
                        value.SetAllowedGroupsCascadedly(allowedGroups, processedObjects);
                    }
                }
            }
            foreach (var persistentFieldForCollectionOfPersistentObjects in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                if (persistentFieldForCollectionOfPersistentObjects.IsComposition) {
                    foreach (var value in persistentFieldForCollectionOfPersistentObjects.GetValuesAsPersistentObject()) {
                        if (null != value && !processedObjects.Contains(value)) {
                            processedObjects.Add(value);
                            value.SetAllowedGroupsCascadedly(allowedGroups, processedObjects);
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Sets the change state of all fields to not changed.
        /// </summary>
        internal void SetIsChangedToFalse() {
            foreach (var persistentField in this.GetPersistentFields()) {
                persistentField.SetIsChangedToFalse();
            }
            return;
        }

        /// <summary>
        /// Sync copies the state of all fields for collections of
        /// elements of another instance of this type into this
        /// instance.
        /// </summary>
        /// <param name="source">source instance to sync copy state
        /// from</param>
        internal void SyncCopyFieldsForCollectionsOfElementsFrom(PersistentObject source) {
            foreach (var targetField in this.PersistentFieldsForCollectionsOfElements) {
                var sourceField = source.GetPersistentFieldForCollectionOfElements(targetField.Key);
                if (null != sourceField && targetField.ContentBaseType == sourceField.ContentBaseType) {
                    var areValuesEqual = targetField.Count == sourceField.Count;
                    if (areValuesEqual) {
                        foreach (var targetValue in targetField.GetValuesAsObject()) {
                            var isValueFound = false;
                            foreach (var sourceValue in sourceField.GetValuesAsObject()) {
                                if (targetValue == sourceValue) {
                                    isValueFound = true;
                                    break;
                                }
                            }
                            if (!isValueFound) {
                                areValuesEqual = false;
                                break;
                            }
                        }
                    }
                    if (!areValuesEqual) {
                        targetField.Clear();
                        foreach (var value in sourceField.CopyValues()) {
                            targetField.AddObject(value);
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Sync copies the state of all fields for collections of
        /// persistent objects of another instance of this type into
        /// this instance.
        /// </summary>
        /// <param name="source">source instance to sync copy state
        /// from</param>
        /// <param name="targetPersistenceMechanism">persistent
        /// mechnaism of target instance</param>
        /// <param name="processedSourceObjects">list of processed
        /// source objects</param>
        /// <param name="processedTargetObjects">list of processed
        /// target objects</param>
        internal void SyncCopyFieldsForCollectionsOfPersistentObjectsFrom(PersistentObject source, PersistenceMechanism targetPersistenceMechanism, PersistentObjectCache processedSourceObjects, PersistentObjectCache processedTargetObjects) {
            foreach (var targetField in this.PersistentFieldsForCollectionsOfPersistentObjects) {
                var sourceField = source.GetPersistentFieldForCollectionOfPersistentObjects(targetField.Key);
                if (null != sourceField) {
                    var areValuesEqual = targetField.Count == sourceField.Count;
                    if (areValuesEqual) {
                        foreach (var targetFieldValue in targetField.GetValuesAsPersistentObject()) {
                            var isValueFound = false;
                            foreach (var sourceFieldValue in sourceField.GetValuesAsPersistentObject()) {
                                if (targetFieldValue.Id == sourceFieldValue.Id) {
                                    isValueFound = true;
                                    break;
                                }
                                if (!isValueFound) {
                                    areValuesEqual = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!areValuesEqual) {
                        targetField.Clear();
                        foreach (var sourceObject in sourceField.GetValuesAsPersistentObject()) {
                            if (null == sourceObject) {
                                targetField.AddObject(null);
                            } else if (processedTargetObjects.Contains(sourceObject.Id)) {
                                targetField.AddObject(processedTargetObjects[sourceObject.Id]);
                            } else {
                                var targetPersistentContainer = targetPersistenceMechanism.FindContainer(sourceObject.Type);
                                if (sourceField.IsComposition || sourceField.IsChanged || !targetPersistentContainer.Contains(sourceObject.Id)) {
                                    var persistentObject = targetPersistentContainer.Synchronize(sourceObject, processedSourceObjects, processedTargetObjects);
                                    if (null != persistentObject) {
                                        targetField.AddObject(persistentObject);
                                    }
                                } else {
                                    var targetObject = targetPersistentContainer.FindOnePersistentObject(sourceObject.Id);
                                    if (null != targetObject) {
                                        targetField.AddObject(targetObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Sync copies the state of all fields for persistent
        /// objects of another instance of this type into this
        /// instance.
        /// </summary>
        /// <param name="source">source instance to sync copy state
        /// from</param>
        /// <param name="targetPersistenceMechanism">persistent
        /// mechnaism of target instance</param>
        /// <param name="processedSourceObjects">list of processed
        /// source objects</param>
        /// <param name="processedTargetObjects">list of processed
        /// target objects</param>
        internal void SyncCopyFieldsForPersistentObjectsFrom(PersistentObject source, PersistenceMechanism targetPersistenceMechanism, PersistentObjectCache processedSourceObjects, PersistentObjectCache processedTargetObjects) {
            foreach (var targetField in this.PersistentFieldsForPersistentObjects) {
                var sourceField = source.GetPersistentFieldForPersistentObject(targetField.Key);
                if (null != sourceField) {
                    var sourceObject = sourceField.ValueAsPersistentObject;
                    if (null == sourceObject) {
                        targetField.ValueAsObject = null;
                    } else if (processedTargetObjects.Contains(sourceObject.Id)) {
                        targetField.ValueAsObject = processedTargetObjects[sourceObject.Id];
                    } else {
                        var targetPersistentContainer = targetPersistenceMechanism.FindContainer(sourceObject.Type);
                        if (sourceField.IsComposition || sourceField.IsChanged || !targetPersistentContainer.Contains(sourceObject.Id)) {
                            targetField.ValueAsObject = targetPersistentContainer.Synchronize(sourceObject, processedSourceObjects, processedTargetObjects);
                        } else {
                            targetField.ValueAsObject = targetPersistentContainer.FindOnePersistentObject(sourceObject.Id);
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Updates this object in persistence mechanism.
        /// </summary>
        /// <returns>true if object was updated successfully in
        /// persistence mechanism, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        public bool Update() {
            if (this.IsNew) {
                throw new ObjectNotPersistentException("Persistent object cannot be updated because it is not connected to a persistence mechanism.");
            }
            return this.ParentPersistentContainer.ParentPersistenceMechanism.UpdateObject(this, this.ParentPersistentContainer);
        }

        /// <summary>
        /// Updates this object in persistence mechanism and all of
        /// its persistent child objects cascadedly. Missing
        /// persistent child objects are added cascadedly.
        /// </summary>
        public void UpdateCascadedly() {
            if (this.IsNew) {
                throw new ObjectNotPersistentException("Persistent object cannot be updated because it is not connected to a persistence mechanism.");
            }
            this.ParentPersistentContainer.ParentPersistenceMechanism.UpdateObjectCascadedly(this, this.ParentPersistentContainer);
            return;
        }

        /// <summary>
        /// Updates the value for created at.
        /// </summary>
        /// <param name="createdAt">new date/time value to be set for
        /// created at</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        public bool UpdateCreatedAt(DateTime createdAt) {
            if (this.IsNew) {
                throw new ObjectNotPersistentException("Persistent object cannot be updated because it is not connected to a persistence mechanism.");
            }
            return this.ParentPersistentContainer.ParentPersistenceMechanism.UpdateObjectCreatedAt(this, createdAt);
        }

        /// <summary>
        /// Updates the value for created by.
        /// </summary>
        /// <param name="createdBy">new user value to be set for
        /// created by</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        public bool UpdateCreatedBy(IUser createdBy) {
            if (this.IsNew) {
                throw new ObjectNotPersistentException("Persistent object cannot be updated because it is not connected to a persistence mechanism.");
            }
            return this.ParentPersistentContainer.ParentPersistenceMechanism.UpdateObjectCreatedBy(this, createdBy);
        }

        /// <summary>
        /// Gets called after change of any persistent field.
        /// </summary>
        /// <param name="senderTrace">list of objects that were
        /// involved in this event already</param>
        /// <param name="keyChain">key chain of the affected property</param>
        internal void ValueChanged(string[] keyChain, IList<object> senderTrace) {
            var eventHandler = this.PropertyChanged;
            if (this.IsEventFiringEnabled && null != eventHandler) {
                eventHandler(senderTrace[0], new TraceablePropertyChangedEventArgs(keyChain, senderTrace));
            }
            return;
        }

    }

}