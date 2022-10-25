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

    using Framework.Persistence;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Dalegate to be executed for calculation of values.
    /// </summary>
    /// <typeparam name="T">type of values</typeparam>
    public delegate IEnumerable<T> CalculateValuesDelegate<T>();

    /// <summary>
    /// Dalegate to be executed for pass-through of values.
    /// </summary>
    /// <param name="values">values to pass through</param>
    /// <typeparam name="T">type of values</typeparam>
    public delegate void PassThroughValuesDelegate<T>(IEnumerable<T> values);

    /// <summary>
    /// Field for calculated value to be presented.
    /// </summary>
    /// <typeparam name="T">type of values</typeparam>
    public class PresentableFieldForCalculatedValueCollection<T> : IPresentableFieldForCollection<T> {

        /// <summary>
        /// Gets or sets the value at a specific index.
        /// </summary>
        /// <param name="index">index to get or set value for</param>
        /// <returns>value at the specific index</returns>
        public T this[int index] {
            get {
                return this.GetValues()[index];
            }
            set {
                var values = this.GetValues();
                values[index] = value;
                this.SetValues(values);
            }
        }

        /// <summary>
        /// Delegate for calculation of value.
        /// </summary>
        private readonly CalculateValuesDelegate<T> CalculateValuesDelegate;

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public Type ContentBaseType {
            get { return typeof(T); }
        }

        /// <summary>
        /// Gets the number of objects contained.
        /// </summary>
        public int Count {
            get { return this.GetValues().Count; }
        }

        /// <summary>
        /// True if field is for a single element, false if it is for
        /// a collection of elements.
        /// </summary>
        public bool IsForSingleElement {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this field is read-only.
        /// </summary>
        public bool IsReadOnly {
            get { return null == this.PassThroughValuesDelegate || (this.ParentPresentableObject is Persistence.PersistentObject persistentObject && persistentObject.IsWriteProtected); }
        }

        /// <summary>
        /// Internal key of this presentable field.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Parent presentable object of this field.
        /// </summary>
        public IPresentableObject ParentPresentableObject { get; private set; }

        /// <summary>
        /// Delegate for pass-through of values.
        /// </summary>
        private readonly PassThroughValuesDelegate<T> PassThroughValuesDelegate;

        /// <summary>
        /// Cached calculated values.
        /// </summary>
        private IList<T> values;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        /// <param name="calculateValuesDelegate">delegate for
        /// calculation of values</param>
        public PresentableFieldForCalculatedValueCollection(IPresentableObject parentPresentableObject, string key, CalculateValuesDelegate<T> calculateValuesDelegate) {
            this.CalculateValuesDelegate = calculateValuesDelegate;
            this.Key = key;
            this.ParentPresentableObject = parentPresentableObject;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        /// <param name="calculateValuesDelegate">delegate for
        /// calculation of values</param>
        /// <param name="passThroughValuesDelegate">delegate for
        /// pass-through of values</param>
        public PresentableFieldForCalculatedValueCollection(IPresentableObject parentPresentableObject, string key, CalculateValuesDelegate<T> calculateValuesDelegate, PassThroughValuesDelegate<T> passThroughValuesDelegate)
            : this(parentPresentableObject, key, calculateValuesDelegate) {
            this.PassThroughValuesDelegate = passThroughValuesDelegate;
        }

        /// <summary>
        /// Adds an item to the collection. 
        /// </summary>
        /// <param name="item">item to add to the collection</param>
        public void Add(T item) {
            var values = this.GetValues();
            values.Add(item);
            this.SetValues(values);
            return;
        }

        /// <summary>
        /// Adds an object to the collection. 
        /// </summary>
        /// <param name="item">object to add to the collection</param>
        public void AddObject(object item) {
            this.Add((T)item);
            return;
        }

        /// <summary>
        /// Adds a string value to the list. 
        /// </summary>
        /// <param name="item">string value to add to the collection</param>
        public void AddString(string item) {
            this.AddObject(item);
            return;
        }

        /// <summary>
        /// Removes all objects from the collection.
        /// </summary>
        public void Clear() {
            var values = this.GetValues();
            values.Clear();
            this.SetValues(values);
            return;
        }

        /// <summary>
        /// Determines whether the collection contains a specific
        /// item.
        /// </summary>
        /// <param name="item">item to locate in collection - can
        /// be null for reference types</param>
        /// <returns>true if specific item is contained, false
        /// otherwise</returns>
        public bool Contains(T item) {
            return this.GetValues().Contains(item);
        }

        /// <summary>
        /// Copies the objects of the collection to an array,
        /// starting at a particular array index. 
        /// </summary>
        /// <param name="array">array to copy the collection into</param>
        /// <param name="arrayIndex">array index to start at</param>
        public void CopyTo(T[] array, int arrayIndex) {
            this.GetValues().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public IEnumerator<T> GetEnumerator() {
            return this.GetValues().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            foreach (var value in this.GetValues()) {
                yield return value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public IEnumerable<string> GetSortableValues() {
            return this.GetValuesAsString();
        }

        /// <summary>
        /// Gets the values of this presentable field.
        /// </summary>
        protected IList<T> GetValues() {
            if (null == this.values) {
                var calculatedValues = this.CalculateValuesDelegate();
                if (null == calculatedValues) {
                    this.values = new List<T>(0);
                } else {
                    this.values = new List<T>(calculatedValues);
                }
            }
            return this.values;
        }

        /// <summary>
        /// Gets the values of presentable field as object.
        /// </summary>
        /// <returns>values of presentable field as object</returns>
        public IEnumerable<object> GetValuesAsObject() {
            foreach (var value in this) {
                yield return value;
            }
        }

        /// <summary>
        /// Gets the values of presentable field as plain text.
        /// As an example, this is used for full-text indexing.
        /// </summary>
        /// <returns>values of presentable field as plain text</returns>
        public IEnumerable<string> GetValuesAsPlainText() {
            return this.GetValuesAsString();
        }

        /// <summary>
        /// Gets the values of presentable field as string.
        /// </summary>
        /// <returns>values of presentable field as string</returns>
        public virtual IEnumerable<string> GetValuesAsString() {
            foreach (var value in this) {
                string valueAsString = value?.ToString();
                if (long.TryParse(valueAsString, out long longValue)) {
                    valueAsString = longValue.ToString(CultureInfo.InvariantCulture);
                } else if (decimal.TryParse(valueAsString, out decimal decimalValue)) {
                    valueAsString = decimalValue.ToString(CultureInfo.InvariantCulture);
                }
                yield return valueAsString;
            }
        }

        /// <summary>
        /// Gets the versioned field for a specific date.
        /// </summary>
        /// <param name="modificationDate">date to get versioned
        /// field for</param>
        /// <returns>versioned field if existing, null otherwise</returns>
        public IPresentableFieldForCollection GetVersionedField(DateTime? modificationDate) {
            return null;
        }

        /// <summary>
        /// Creates a new item that could be added to this
        /// collection.
        /// </summary>
        /// <returns>new item that could be added to this collection</returns>
        public object NewItemAsObject() {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Removes a specific object from the collection.
        /// </summary>
        /// <param name="item">specific object to remove from
        /// collection</param>
        /// <returns>true if object was successfully removed from
        /// collection, false otherwise or if object was not
        /// contained in collection</returns>
        public bool Remove(T item) {
            var values = this.GetValues();
            bool isRemoved = values.Remove(item);
            this.SetValues(values);
            return isRemoved;
        }

        /// <summary>
        /// Sets the values of this presentable field.
        /// </summary>
        /// <param name="values">new values to be set for field</param>
        protected void SetValues(IList<T> values) {
            if (null == this.PassThroughValuesDelegate) {
                throw new InvalidOperationException("Setting values is not allowed for presentable fields of type " + nameof(PresentableFieldForCalculatedValueCollection<T>) + " if no pass-through value delegate is set.");
            } else {
                this.PassThroughValuesDelegate(values);
                this.values = values;
            }
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the default
        /// comparer.
        /// </summary>
        public void Sort() {
            var values = this.GetValues() as List<T>;
            values.Sort();
            this.SetValues(values);
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the default
        /// comparer.
        /// </summary>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        public void SortObjects(Comparison<object> comparison) {
            var values = this.GetValues() as List<T>;
            values.Sort(new ComparisonComparer<T>(comparison));
            this.SetValues(values);
            return;
        }

        /// <summary>
        /// Swaps the items of two indexes.
        /// </summary>
        /// <param name="firstIndex">index to assign item of second
        /// index to</param>
        /// <param name="secondIndex">index to assign item of first
        /// index to</param>
        public void Swap(int firstIndex, int secondIndex) {
            var values = this.GetValues();
            var element = values[firstIndex];
            values[firstIndex] = values[secondIndex];
            values[secondIndex] = element;
            this.SetValues(values);
            return;
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of values of this field and adds it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="item">new value to add</param>
        /// <returns>true if value could be added successfully, false
        /// otherwise</returns>
        public virtual bool TryAddString(string item) {
            bool success = !this.IsReadOnly;
            if (success) {
                this.AddString(item);
            }
            return success;
        }

    }

}