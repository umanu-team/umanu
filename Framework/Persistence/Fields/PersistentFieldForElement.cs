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

    using Framework.Persistence.Exceptions;
    using Framework.Presentation.Forms;
    using System;
    using System.Data.Common;

    /// <summary>
    /// Wrapper class for a field of any element type to be stored in
    /// persistence mechanism.
    /// </summary>
    public abstract class PersistentFieldForElement : PersistentField, IPresentableFieldForElement {

        /// <summary>
        /// True if field is for a single element, false if it is for
        /// a collection of elements.
        /// </summary>
        public sealed override bool IsForSingleElement {
            get { return true; }
        }

        /// <summary>
        /// Indicates whether this field is supposed to be indexed
        /// for performance improvements on conditional queries based
        /// on this field.
        /// WARNING: The value of this property will be ignored for
        /// database migrations. Once an index was created, it won't
        /// be removed any more. No indexed will be created for
        /// fields which were added after the initial creation of the
        /// parent container.
        /// </summary>
        internal bool IsIndexed { get; set; }

        /// <summary>
        /// Sortable value of this presentable field.
        /// </summary>
        public virtual string SortableValue {
            get { return this.ValueAsString; }
        }

        /// <summary>
        /// Value of this persistent field as object.
        /// </summary>
        public abstract object ValueAsObject { get; set; }

        /// <summary>
        /// Value of this presentable field as string.
        /// </summary>
        public string ValueAsString {
            get {
                return this.GetValueAsString();
            }
            set {
                if (!this.TrySetValueAsString(value)) {
                    throw new FormatException("String value \"" + value + "\" cannot be converted to type " + this.ContentBaseType + ".");
                }
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        internal PersistentFieldForElement(string key)
            : base(key) {
            this.IsIndexed = false;
        }

        /// <summary>
        /// Copies the value of persistent field.
        /// </summary>
        /// <returns>copy of value of persistent field</returns>
        internal virtual object CopyValue() {
            return this.ValueAsObject;
        }

        /// <summary>
        /// Gets the value of this presentable field as plain text.
        /// As an example, this is used for full-text indexing.
        /// </summary>
        /// <returns>value of this presentable field as plain text</returns>
        public virtual string GetValueAsPlainText() {
            return this.ValueAsString;
        }

        /// <summary>
        /// Converts the value of this field to a string value.
        /// </summary>
        /// <returns>value of this field as string</returns>
        protected internal abstract string GetValueAsString();

        /// <summary>
        /// Gets the versioned field for a specific date.
        /// </summary>
        /// <param name="modificationDate">date to get versioned
        /// field for</param>
        /// <returns>versioned field if existing, null otherwise</returns>
        public virtual IPresentableFieldForElement GetVersionedField(DateTime? modificationDate) {
            IPresentableFieldForElement versionedField;
            var versionValue = this.ParentPersistentObject.GetVersionValue(modificationDate);
            if (null == versionValue) {
                versionedField = null;
            } else {
                versionedField = versionValue.GetPersistentFieldForElement(this.Key);
            }
            return versionedField;
        }

        /// <summary>
        /// Loads a new value for persistent field from a specified
        /// index of a DbDataReader, but does not set the value yet.
        /// </summary>
        /// <param name="dataReader">data reader to load value from</param>
        /// <param name="ordinal">index of data reader to load value
        /// from</param>
        internal abstract void LoadValueFromDbDataReader(DbDataReader dataReader, int ordinal);

        /// <summary>
        /// Retrieves the value of this field from persistence
        /// mechanism. This can be used to refresh the value of this
        /// field.
        /// </summary>
        public override void Retrieve() {
            if (null == this.ParentPersistentObject || this.ParentPersistentObject.IsNew) {
                throw new ObjectNotPersistentException("Persistent field cannot be retrieved because it is not connected to a persistence mechanism.");
            } else {
                this.ParentPersistentObject.ParentPersistentContainer.RetrieveFieldsForElements(this.ParentPersistentObject);
            }
            return;
        }

        /// <summary>
        /// Sets the new value for persistent field which has been
        /// loaded from DbDataReader before.
        /// </summary>
        internal abstract void SetValueFromDbDataReader();

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of value of this field and sets it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">new value to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public abstract bool TrySetValueAsString(string value);

    }

    /// <summary>
    /// Wrapper class for a field of any element type to be stored in
    /// persistence mechanism.
    /// </summary>
    /// <typeparam name="T">type of value</typeparam>
    public abstract class PersistentFieldForElement<T> : PersistentFieldForElement, IPresentableFieldForElement<T> {

        /// <summary>
        /// Value of this persistent field.
        /// </summary>
        public T Value {
            get {
                this.EnsureRetrieval();
                return this.value;
            }
            set {
                bool valuesAreDifferent = this.ValuesAreDifferent(this.Value, value);
                this.value = value; // assignment is necessary for reference types like IUser
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
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        internal PersistentFieldForElement(string key)
            : base(key) {
            // nothing to do
        }

        /// <summary>
        /// Converts the value of this field to a string value.
        /// </summary>
        /// <returns>value of this field as string</returns>
        protected internal override string GetValueAsString() {
            string value;
            if (null == this.Value) {
                value = string.Empty;
            } else {
                value = this.Value.ToString();
            }
            return value;
        }

        /// <summary>
        /// Creates a new item that could be set to this field.
        /// </summary>
        /// <returns>new item that could be set to this field</returns>
        public sealed override object NewItemAsObject() {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Sets the value of persistent field by bypassing the
        /// change cycle.
        /// </summary>
        /// <param name="value">value to be set</param>
        protected void SetValueUnsafe(T value) {
            this.value = value;
            return;
        }

        /// <summary>
        /// Determines whether two value are different.
        /// </summary>
        /// <param name="x">first value to compare</param>
        /// <param name="y">second value to compare</param>
        /// <returns>true if the specified values are different,
        /// false otherwise</returns>
        protected abstract bool ValuesAreDifferent(T x, T y);

    }

}