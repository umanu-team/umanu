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
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Wrapper class for fields to be stored in persistence
    /// mechanism.
    /// </summary>
    public abstract class PersistentField : IPresentableField, IEquatable<PersistentField>, IComparable<PersistentField> {

        /// <summary>
        /// Base type of value of this persistent field.
        /// </summary>
        public abstract Type ContentBaseType { get; }

        /// <summary>
        /// Actual type of value of persistent field.
        /// </summary>
        private Type contentType;

        /// <summary>
        /// Persistent field type of this persistent field/list.
        /// </summary>
        public PersistentFieldType FieldType {
            get {
                return new PersistentFieldType(this.Key, this.IsForSingleElement, this.ContentBaseType);
            }
        }

        /// <summary>
        /// True if the values of this object were changed since last
        /// retrieval, false otherwise.
        /// </summary>
        public bool IsChanged {
            get {
                return this.isChanged && (null == this.ParentPersistentObject || this.ParentPersistentObject.IsNew || this.IsRetrieved);
            }
        }
        private bool isChanged;

        /// <summary>
        /// True if field is for a single element, false if it is for
        /// a collection of elements.
        /// </summary>
        public abstract bool IsForSingleElement { get; }

        /// <summary>
        /// Indicates whether field is accessible via full-text
        /// search.
        /// </summary>
        public bool IsFullTextIndexed { get; set; }

        /// <summary>
        /// Gets a value indicating whether the value/collection is
        /// read-only.
        /// </summary>
        public virtual bool IsReadOnly {
            get {
                return this.ParentPersistentObject.IsWriteProtected;
            }
        }

        /// <summary>
        /// True if this field is persistent and initialized with
        /// values from persistence mechanism, false otherwise.
        /// </summary>
        public bool IsRetrieved { get; internal set; }

        /// <summary>
        /// Key of the persistent field.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Parent persistent object of this persistent field/list.
        /// </summary>
        public virtual PersistentObject ParentPersistentObject { get; internal set; }

        /// <summary>
        /// Parent presentable object of this field.
        /// </summary>
        public IPresentableObject ParentPresentableObject {
            get {
                return this.ParentPersistentObject;
            }
        }

        /// <summary>
        /// List of previous keys for automatic migrations of
        /// persistence mechanisms.
        /// </summary>
        public IList<string> PreviousKeys {
            get {
                if (null == this.previousKeys) {
                    this.previousKeys = new List<string>(0);
                }
                return this.previousKeys;
            }
        }
        private IList<string> previousKeys;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        internal PersistentField(string key) {
            if (null == key) {
                throw new ArgumentNullException(nameof(key));
            }
            this.isChanged = false;
            this.IsFullTextIndexed = false;
            this.IsRetrieved = nameof(PersistentObject.Id) == key;
            this.Key = PersistentField.ValidateKey(key);
        }

        /// <summary>
        /// Compares the current instance with another object of the
        /// same type by key.
        /// </summary>
        /// <param name="other">An object to compare with this
        /// instance.</param>
        /// <returns>A signed integer that indicates the relative
        /// order of the comparands: Less than zero if this instance
        /// is less than the other instance. Equal to zero if this
        /// instance is equal to the other instance. Greater than
        /// zero if this instance is greater than the other instance.</returns>
        public int CompareTo(PersistentField other) {
            return this.Key.CompareTo(other.Key);
        }

        /// <summary>
        /// Ensures that this field was retrieved from persistence at
        /// least once.
        /// </summary>
        protected virtual void EnsureRetrieval() {
            if (!this.IsRetrieved && null != this.ParentPersistentObject && this.ParentPersistentObject.IsAutoRetrievalEnabled && !this.ParentPersistentObject.IsNew && nameof(PersistentObject.Id) != this.Key) {
                this.Retrieve();
            }
            return;
        }

        /// <summary>
        /// Determines whether the current object is equal to another
        /// object of the same type.
        /// </summary>
        /// <param name="other">object to compare this object to</param>
        /// <returns>true if both objects are equal, false otherwise</returns>
        public bool Equals(PersistentField other) {
            return this.FieldType.Equals(other.FieldType)
                && this.IsReadOnly.Equals(other.IsReadOnly);
        }

        /// <summary>
        /// Gets the actual type of value of persistent field.
        /// </summary>
        /// <returns>actual type of value of persistent field</returns>
        public Type GetContentType() {
            if (null == this.contentType) {
                this.contentType = this.NewItemAsObject().GetType();
            }
            return this.contentType;
        }

        /// <summary>
        /// This has to be called whenever the persisted state of
        /// this field has changed.
        /// </summary>
        protected void HasChanged() {
            if (nameof(PersistentObject.Id) != this.Key) {
                this.isChanged = true;
                if (null != this.ParentPersistentObject) {
                    var keyChain = KeyChain.FromKey(this.Key);
                    var senderTrace = new List<object> {
                        this.ParentPersistentObject
                    };
                    this.ParentPersistentObject.ValueChanged(keyChain, senderTrace);
                }
            }
            return;
        }

        /// <summary>
        /// Creates a new item that could be set to this field.
        /// </summary>
        /// <returns>new item that could be set to this field</returns>
        public abstract object NewItemAsObject();

        /// <summary>
        /// Retrieves the value of this field from persistence
        /// mechanism. This can be used to refresh the value of this
        /// field.
        /// </summary>
        public abstract void Retrieve();

        /// <summary>
        /// Sets the change state of all fields to not changed.
        /// </summary>
        internal void SetIsChangedToFalse() {
            this.isChanged = false;
            return;
        }

        /// <summary>
        /// Validates a field name - throws a "FieldNameException"
        /// if field name is invalid.
        /// </summary>
        /// <param name="fieldName">field name to validate</param>
        /// <returns>field name to validate</returns>
        public static string ValidateKey(string fieldName) {
            foreach (char c in fieldName) {
                if ((c >= 'a' && c <= 'z')
                    || (c >= 'A' && c <= 'Z')
                    || (c >= '1' && c <= '9')
                    || '0' == c
                    || '_' == c) {
                    continue;
                } else {
                    throw new FieldNameException("Field name \""
                        + fieldName + "\" contains invalid character \""
                        + c + "\".");
                }
            }
            return fieldName;
        }

    }

}
