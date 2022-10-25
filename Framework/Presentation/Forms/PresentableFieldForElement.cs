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

namespace Framework.Presentation.Forms {

    using System;

    /// <summary>
    /// Field for element to be presented.
    /// </summary>
    /// <typeparam name="TValue">type of value</typeparam>
    public abstract class PresentableFieldForElement<TValue> : IPresentableFieldForElement<TValue> {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public abstract Type ContentBaseType { get; }

        /// <summary>
        /// True if field is for a single element, false if it is for
        /// a collection of elements.
        /// </summary>
        public bool IsForSingleElement {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this field is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Internal key of this presentable field.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Parent presentable object of this field.
        /// </summary>
        public IPresentableObject ParentPresentableObject { get; private set; }

        /// <summary>
        /// Sortable value of this presentable field.
        /// </summary>
        public virtual string SortableValue {
            get { return this.ValueAsString; }
        }

        /// <summary>
        /// Value of this presentable field.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Value of this presentable field as object.
        /// </summary>
        public object ValueAsObject {
            get {
                return this.Value;
            }
            set {
                this.Value = (TValue)value;
            }
        }

        /// <summary>
        /// Value of this presentable field as string.
        /// </summary>
        public string ValueAsString {
            get {
                return this.GetValueAsString();
            }
            set {
                if (!this.TrySetValueAsString(value)) {
                    throw new FormatException("String value \"" + value + "\" cannot be converted to type " + typeof(TValue) + ".");
                }
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        internal PresentableFieldForElement(IPresentableObject parentPresentableObject, string key) {
            this.IsReadOnly = false;
            this.Key = key;
            this.ParentPresentableObject = parentPresentableObject;
            this.Value = default(TValue);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        /// <param name="value">value of this presentable field</param>
        internal PresentableFieldForElement(IPresentableObject parentPresentableObject, string key, TValue value)
            : this(parentPresentableObject, key) {
            this.Value = value;
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
        protected virtual string GetValueAsString() {
            string value;
            if (null == this.Value) {
                value = string.Empty;
            } else {
                value = this.Value.ToString();
            }
            return value;
        }

        /// <summary>
        /// Gets the versioned field for a specific date.
        /// </summary>
        /// <param name="modificationDate">date to get versioned
        /// field for</param>
        /// <returns>versioned field if existing, null otherwise</returns>
        public IPresentableFieldForElement GetVersionedField(DateTime? modificationDate) {
            return null;
        }

        /// <summary>
        /// Creates a new item that could be set to this field.
        /// </summary>
        /// <returns>new item that could be set to this field</returns>
        public virtual object NewItemAsObject() {
            return Activator.CreateInstance<TValue>();
        }

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

}
