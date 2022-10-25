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

    using Framework.Model;
    using System;
    using System.Globalization;

    /// <summary>
    /// Dalegate to be executed for calculation of value.
    /// </summary>
    /// <typeparam name="T">type of value</typeparam>
    public delegate T CalculateValueDelegate<T>();

    /// <summary>
    /// Dalegate to be executed for pass-through of value.
    /// </summary>
    /// <param name="value">value to pass through</param>
    /// <typeparam name="T">type of value</typeparam>
    public delegate void PassThroughValueDelegate<T>(T value);

    /// <summary>
    /// Field for calculated value to be presented.
    /// </summary>
    /// <typeparam name="T">type of value</typeparam>
    public class PresentableFieldForCalculatedValue<T> : IPresentableFieldForElement<T> {

        /// <summary>
        /// Delegate for calculation of value.
        /// </summary>
        private readonly CalculateValueDelegate<T> CalculateValueDelegate;

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public Type ContentBaseType {
            get { return typeof(T); }
        }

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
        public bool IsReadOnly {
            get { return null == this.PassThroughValueDelegate || (this.ParentPresentableObject is Persistence.PersistentObject persistentObject && persistentObject.IsWriteProtected); }
        }

        /// <summary>
        /// Indicates whether value was set by calling delegate
        /// already. This information has to be stored in a separate
        /// field and cannot be solved via null-check on value. Null-
        /// checks would not work if T is not nullable.
        /// </summary>
        private bool isValueSet;

        /// <summary>
        /// Internal key of this presentable field.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Parent presentable object of this field.
        /// </summary>
        public IPresentableObject ParentPresentableObject { get; private set; }

        /// <summary>
        /// Delegate for pass-through of value.
        /// </summary>
        private readonly PassThroughValueDelegate<T> PassThroughValueDelegate;

        /// <summary>
        /// Sortable value of this presentable field.
        /// </summary>
        public string SortableValue {
            get { return this.ValueAsString; }
        }

        /// <summary>
        /// Value of this presentable field.
        /// </summary>
        public T Value {
            get {
                if (!this.isValueSet) {
                    this.isValueSet = true;
                    this.value = this.CalculateValueDelegate();
                    if (null == this.value && !(this.ParentPresentableObject is Persistence.PersistentObject)) {
                        this.isValueSet = false; // calculated value would always be null otherwise
                    }
                }
                return this.value;
            }
            set {
                if (null == this.PassThroughValueDelegate) {
                    throw new InvalidOperationException("Setting values is not allowed for presentable fields of type " + nameof(PresentableFieldForCalculatedValue<T>) + " if no pass-through value delegate is set.");
                } else {
                    this.PassThroughValueDelegate(value);
                    this.value = value;
                }
            }
        }
        private T value;

        /// <summary>
        /// Value of this presentable field as object.
        /// </summary>
        public object ValueAsObject {
            get { return this.Value; }
            set { this.Value = (T)value; }
        }

        /// <summary>
        /// Value of this presentable field as string.
        /// </summary>
        public virtual string ValueAsString {
            get {
                string valueAsString = this.Value?.ToString();
                if (long.TryParse(valueAsString, out long longValue)) {
                    valueAsString = longValue.ToString(CultureInfo.InvariantCulture);
                } else if (decimal.TryParse(valueAsString, out decimal decimalValue)) {
                    valueAsString = decimalValue.ToString(CultureInfo.InvariantCulture);
                }
                return valueAsString;
            }
            set {
                if (typeof(T) == Persistence.TypeOf.DateTime || typeof(T) == Persistence.TypeOf.NullableDateTime) {
                    if (UtcDateTime.TryParse(value, out DateTime dateTime)) {
                        this.ValueAsObject = dateTime;
                    } else {
                        this.ValueAsObject = default(T);
                    }
                } else {
                    this.ValueAsObject = value;
                }
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        /// <param name="calculateValueDelegate">delegate for
        /// calculation of value</param>
        public PresentableFieldForCalculatedValue(IPresentableObject parentPresentableObject, string key, CalculateValueDelegate<T> calculateValueDelegate) {
            this.CalculateValueDelegate = calculateValueDelegate;
            this.Key = key;
            this.isValueSet = false;
            this.ParentPresentableObject = parentPresentableObject;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        /// <param name="calculateValueDelegate">delegate for
        /// calculation of value</param>
        /// <param name="passThroughValueDelegate">delegate for
        /// pass-through of value</param>
        public PresentableFieldForCalculatedValue(IPresentableObject parentPresentableObject, string key, CalculateValueDelegate<T> calculateValueDelegate, PassThroughValueDelegate<T> passThroughValueDelegate)
            : this(parentPresentableObject, key, calculateValueDelegate) {
            this.PassThroughValueDelegate = passThroughValueDelegate;
        }

        /// <summary>
        /// Gets the value of this presentable field as plain text.
        /// As an example, this is used for full-text indexing.
        /// </summary>
        /// <returns>value of this presentable field as plain text</returns>
        public string GetValueAsPlainText() {
            string value = this.ValueAsString;
            if (!string.IsNullOrEmpty(value)) {
                value = XmlUtility.RemoveTags(value);
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
        public object NewItemAsObject() {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of value of this field and sets it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">new value to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public virtual bool TrySetValueAsString(string value) {
            bool success = !this.IsReadOnly;
            if (success) {
                this.ValueAsString = value;
            }
            return success;
        }

    }

}