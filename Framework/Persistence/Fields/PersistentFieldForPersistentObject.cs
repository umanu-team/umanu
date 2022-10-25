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

namespace Framework.Persistence.Fields {

    using Framework.Model;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using System;
    using System.ComponentModel;
    using System.Data.Common;

    /// <summary>
    /// Wrapper class for a field of type PersistentObject to be
    /// stored in persistence mechanism.
    /// </summary>
    public abstract class PersistentFieldForPersistentObject : PersistentFieldForElement, IPresentableFieldWithOptionDataProvider {

        /// <summary>
        /// Specifies the behavior to apply on cascaded removal of parent
        /// object.
        /// </summary>
        public CascadedRemovalBehavior CascadedRemovalBehavior { get; internal set; }

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public sealed override Type ContentBaseType {
            get {
                return TypeOf.PersistentObject;
            }
        }

        /// <summary>
        /// Initial value of this persistent field.
        /// </summary>
        internal PersistentObject InitialValue { get; set; }

        /// <summary>
        /// True if this field "owns" its value(s), false otherwise.
        /// Doing so means that the field value(s) is/are to be
        /// removed if the parent object of this field is removed.
        /// </summary>
        internal bool IsComposition {
            get {
                return CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced == this.CascadedRemovalBehavior
                    || CascadedRemovalBehavior.RemoveValuesForcibly == this.CascadedRemovalBehavior;
            }
        }

        /// <summary>
        /// Option data provider to use for ID resolval.
        /// </summary>
        public IOptionDataProvider OptionDataProvider { get; set; }

        /// <summary>
        /// Sortable value of this presentable field.
        /// </summary>
        public override string SortableValue {
            get { return null; }
        }

        /// <summary>
        /// Value of this persistent field as persistent object.
        /// </summary>
        internal abstract PersistentObject ValueAsPersistentObject { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        /// <param name="cascadedRemovalBehavior">specifies the
        /// behavior to apply on cascaded removal of parent object</param>
        public PersistentFieldForPersistentObject(string key, CascadedRemovalBehavior cascadedRemovalBehavior)
            : base(key) {
            this.CascadedRemovalBehavior = cascadedRemovalBehavior;
            this.IsFullTextIndexed = true;
        }

        /// <summary>
        /// Copies the value of persistent field.
        /// </summary>
        /// <returns>copy of value of persistent field</returns>
        internal override object CopyValue() {
            throw new InvalidOperationException("Value of persistent field for persistent object may not be copied this way.");
        }

        /// <summary>
        /// Indicates whether initial value of field is supposed to
        /// be removed if not referenced on removal of parent
        /// persistent object.
        /// </summary>
        /// <returns>initial value to be removed if not referenced on
        /// removal of parent persistent object</returns>
        internal PersistentObject GetInitialValueToBeRemovedIfNotReferencedOnRemovalOfParent() {
            PersistentObject initialValueToBeRemovedIfNotReferencedOfRemovalOfParent;
            if (null != this.InitialValue
                && !this.InitialValue.IsRemoved
                && (null == this.ValueAsPersistentObject || this.InitialValue.Id != this.ValueAsPersistentObject.Id)
                && (CascadedRemovalBehavior.RemoveValuesForcibly == this.CascadedRemovalBehavior || CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced == this.CascadedRemovalBehavior)) {
                initialValueToBeRemovedIfNotReferencedOfRemovalOfParent = this.InitialValue;
            } else {
                initialValueToBeRemovedIfNotReferencedOfRemovalOfParent = null;
            }
            return initialValueToBeRemovedIfNotReferencedOfRemovalOfParent;
        }

        /// <summary>
        /// Gets the versioned field for a specific date.
        /// </summary>
        /// <param name="modificationDate">date to get versioned
        /// field for</param>
        /// <returns>versioned field if existing, null otherwise</returns>
        public sealed override IPresentableFieldForElement GetVersionedField(DateTime? modificationDate) {
            IPresentableFieldForElement versionedField;
            var versionValue = this.ParentPersistentObject.GetVersionValue(modificationDate);
            if (null == versionValue) {
                versionedField = null;
            } else {
                versionedField = versionValue.GetPersistentFieldForPersistentObject(this.Key);
            }
            return versionedField;
        }

        /// <summary>
        /// Gets a value indicating whether the value/collection is
        /// read-only.
        /// </summary>
        public sealed override bool IsReadOnly {
            get {
                bool isReadOnly;
                var valueAsPersistentObject = this.ValueAsPersistentObject;
                if (this.IsComposition && null != valueAsPersistentObject) {
                    isReadOnly = valueAsPersistentObject.IsWriteProtected;
                } else {
                    isReadOnly = base.IsReadOnly;
                }
                return isReadOnly;
            }
        }

        /// <summary>
        /// Loads a new value for persistent field from a specified
        /// index of a DbDataReader, but does not set the value yet.
        /// </summary>
        /// <param name="dataReader">data reader to load value from</param>
        /// <param name="ordinal">index of data reader to load value
        /// from</param>
        internal sealed override void LoadValueFromDbDataReader(DbDataReader dataReader, int ordinal) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the value of this field from persistence
        /// mechanism. This can be used to refresh the value of this
        /// field.
        /// </summary>
        public sealed override void Retrieve() {
            if (null == this.ParentPersistentObject || this.ParentPersistentObject.IsNew) {
                throw new ObjectNotPersistentException("Persistent field cannot be retrieved because it is not connected to a persistence mechanism.");
            } else {
                this.ParentPersistentObject.ParentPersistentContainer.RetrieveField(this);
            }
            return;
        }

        /// <summary>
        /// Sets the new value for persistent field which has been
        /// loaded from DbDataReader before.
        /// </summary>
        internal sealed override void SetValueFromDbDataReader() {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// Wrapper class for a field of type PersistentObject to be
    /// stored in persistence mechanism.
    /// </summary>
    /// <typeparam name="T">type of persistent object</typeparam>
    public class PersistentFieldForPersistentObject<T> : PersistentFieldForPersistentObject, IPresentableFieldForElement<T>
        where T : PersistentObject, new() {

        /// <summary>
        /// Event handler called after change of persistent value.
        /// </summary>
        private event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Value of this persistent field.
        /// </summary>
        public T Value {
            get {
                this.EnsureRetrieval();
                return this.value;
            }
            set {
                if (null != this.Value) {
                    this.value.PropertyChanged -= this.PropertyChanged;
                }
                if (null != value) {
                    value.PropertyChanged += this.PropertyChanged;
                }
                bool valuesAreDifferent = this.value?.Id != value?.Id;
                this.value = value;
                if (valuesAreDifferent) {
                    this.HasChanged();
                }
            }
        }
        private T value;

        /// <summary>
        /// Value of this persistent field as object.
        /// </summary>
        public sealed override object ValueAsObject {
            get {
                return this.Value;
            }
            set {
                this.Value = (T)value;
            }
        }

        /// <summary>
        /// Value of this persistent field as persistent object.
        /// </summary>
        internal sealed override PersistentObject ValueAsPersistentObject {
            get {
                return this.Value;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        /// <param name="cascadedRemovalBehavior">specifies the
        /// behavior to apply on cascaded removal of parent object</param>
        public PersistentFieldForPersistentObject(string key, CascadedRemovalBehavior cascadedRemovalBehavior)
            : base(key, cascadedRemovalBehavior) {
            this.PropertyChanged += this.PropertyChangedHandler;
        }

        /// <summary>
        /// Converts the value of this field to a string value.
        /// </summary>
        /// <returns>value of this field as string</returns>
        protected internal sealed override string GetValueAsString() {
            string value;
            if (null == this.Value) {
                value = string.Empty;
            } else {
                value = this.Value.ToString();
                if (this.Value.Type.ToString() == value) {
                    value = this.Value.Id.ToString("N");
                }
            }
            return value;
        }

        /// <summary>
        /// Creates a new item that could be set to this field.
        /// </summary>
        /// <returns>new item that could be set to this field</returns>
        public sealed override object NewItemAsObject() {
            return new T();
        }

        /// <summary>
        /// Event handler for property change of value.
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e) {
            var traceableEventArgs = (TraceablePropertyChangedEventArgs)e;
            var senderTrace = traceableEventArgs.SenderTrace;
            if (null != this.ParentPersistentObject && !senderTrace.Contains(this.ParentPersistentObject)) {
                var keyChain = KeyChain.Concat(this.Key, traceableEventArgs.KeyChain);
                senderTrace.Add(this.ParentPersistentObject);
                this.ParentPersistentObject.ValueChanged(keyChain, senderTrace);
            }
            return;
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of value of this field and sets it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">ID of persistent object to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public sealed override bool TrySetValueAsString(string value) {
            bool success;
            if (string.IsNullOrEmpty(value)) {
                this.Value = null;
                success = true;
            } else if (Guid.TryParse(value, out Guid id)) {
                if (null == this.OptionDataProvider) {
                    throw new PersistenceException("ID of persistent object cannot be resolved because property \"" + nameof(this.OptionDataProvider) + "\" is not set.  In case you are working with view fields this might be because an incompatible type of view field is used (e.g. " + nameof(ViewFieldForStringChoice) + " instead of " + nameof(ViewFieldForPresentableObjectChoice) + ").");
                }
                var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, id);
                var items = this.OptionDataProvider.Find<T>(filterCriteria, SortCriterionCollection.Empty);
                if (1 == items.Count) {
                    this.Value = items[0];
                    success = true;
                } else {
                    success = false;
                }
            } else {
                success = false;
            }
            return success;
        }

    }

}