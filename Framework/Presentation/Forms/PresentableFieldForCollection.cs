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

    /// <summary>
    /// Field of multiple elements to be presented.
    /// </summary>
    public class PresentableFieldForCollection : PresentableFieldForCollection<object> {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.Object;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        public PresentableFieldForCollection(IPresentableObject parentPresentableObject, string key)
            : base(parentPresentableObject, key) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        /// <param name="values">collection of objects to add to
        /// the collection</param>
        public PresentableFieldForCollection(IPresentableObject parentPresentableObject, string key, IEnumerable<object> values)
            : this(parentPresentableObject, key) {
            this.AddRange(values);
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of values of this field and adds it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="item">new value to add</param>
        /// <returns>true if value could be added successfully, false
        /// otherwise</returns>
        public override bool TryAddString(string item) {
            this.Add(item);
            return true;
        }

    }

    /// <summary>
    /// Field of multiple elements to be presented.
    /// </summary>
    /// <typeparam name="TValue">type of values</typeparam>
    public abstract class PresentableFieldForCollection<TValue> : IPresentableFieldForCollection<TValue>, IEnumerable {

        /// <summary>
        /// Gets the value at a specific index.
        /// </summary>
        /// <param name="index">index to get value for</param>
        /// <returns>value at the specific index</returns>
        public TValue this[int index] {
            get { return this.Items[index]; }
            set { this.Items[index] = value; }
        }

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public abstract Type ContentBaseType { get; }

        /// <summary>
        /// Gets the number of objects contained.
        /// </summary>
        public int Count {
            get {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// True if field is for a single element, false if it is for
        /// a collection of elements.
        /// </summary>
        public bool IsForSingleElement {
            get {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this field is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Internal list for elements.
        /// </summary>
        protected virtual List<TValue> Items { get; private set; }

        /// <summary>
        /// Internal key of this presentable field.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Parent presentable object of this field.
        /// </summary>
        public IPresentableObject ParentPresentableObject { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        public PresentableFieldForCollection(IPresentableObject parentPresentableObject, string key) {
            this.IsReadOnly = false;
            this.Items = new List<TValue>();
            this.Key = key;
            this.ParentPresentableObject = parentPresentableObject;
        }

        /// <summary>
        /// Adds an item to the collection. 
        /// </summary>
        /// <param name="item">item to add to the collection</param>
        public void Add(TValue item) {
            this.Items.Add(item);
        }

        /// <summary>
        /// Adds an object to the collection. 
        /// </summary>
        /// <param name="item">object to add to the collection</param>
        public void AddObject(object item) {
            this.Items.Add((TValue)item);
            return;
        }

        /// <summary>
        /// Adds a collection of objects to the list. 
        /// </summary>
        /// <param name="collection">collection of objects to add to
        /// the collection</param>
        public void AddRange(IEnumerable<TValue> collection) {
            this.Items.AddRange(collection);
            return;
        }

        /// <summary>
        /// Adds a string value to the list. 
        /// </summary>
        /// <param name="item">string value to add to the collection</param>
        public void AddString(string item) {
            if (!this.TryAddString(item)) {
                throw new FormatException("String value \"" + item + "\" cannot be converted to type " + typeof(TValue) + ".");
            }
            return;
        }

        /// <summary>
        /// Removes all objects from the collection.
        /// </summary>
        public void Clear() {
            this.Items.Clear();
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
        public bool Contains(TValue item) {
            return this.Items.Contains(item);
        }

        /// <summary>
        /// Copies the objects of the collection to an array,
        /// starting at a particular array index. 
        /// </summary>
        /// <param name="array">array to copy the collection into</param>
        /// <param name="arrayIndex">array index to start at</param>
        public void CopyTo(TValue[] array, int arrayIndex) {
            this.Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public IEnumerator<TValue> GetEnumerator() {
            return this.Items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            foreach (var item in this.Items) {
                yield return item;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public virtual IEnumerable<string> GetSortableValues() {
            return this.GetValuesAsString();
        }

        /// <summary>
        /// Gets the values of presentable field as object.
        /// </summary>
        /// <returns>values of presentable field as object</returns>
        public IEnumerable<object> GetValuesAsObject() {
            foreach (var item in this) {
                yield return item;
            }
        }

        /// <summary>
        /// Gets the values of this presentable field as plain text.
        /// As an example, this is used for full-text indexing.
        /// </summary>
        /// <returns>values of this presentable field as plain text</returns>
        public virtual IEnumerable<string> GetValuesAsPlainText() {
            return this.GetValuesAsString();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public virtual IEnumerable<string> GetValuesAsString() {
            foreach (var item in this) {
                yield return item.ToString();
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
        public virtual object NewItemAsObject() {
            return Activator.CreateInstance<TValue>();
        }

        /// <summary>
        /// Removes a specific object from the collection.
        /// </summary>
        /// <param name="item">specific object to remove from
        /// collection</param>
        /// <returns>true if object was successfully removed from
        /// collection, false otherwise or if object was not
        /// contained in collection</returns>
        public virtual bool Remove(TValue item) {
            return this.Items.Remove(item);
        }

        /// <summary>
        /// Sorts the elements in the list using the default
        /// comparer.
        /// </summary>
        public void Sort() {
            this.Items.Sort();
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the default
        /// comparer.
        /// </summary>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        public void Sort(Comparison<TValue> comparison) {
            this.Items.Sort(comparison);
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer.
        /// </summary>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        public void Sort(IComparer<TValue> comparer) {
            this.Items.Sort(comparer);
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer.
        /// </summary>
        /// <param name="index">zero-based starting index of the
        /// range to sort</param>
        /// <param name="count">length of the range to sort</param>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        public void Sort(int index, int count, IComparer<TValue> comparer) {
            this.Items.Sort(index, count, comparer);
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer.
        /// </summary>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        public void SortObjects(Comparison<object> comparison) {
            this.Items.Sort(new ComparisonComparer<TValue>(comparison));
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
            TValue element = this.Items[firstIndex];
            this.Items[firstIndex] = this.Items[secondIndex];
            this.Items[secondIndex] = element;
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
        public abstract bool TryAddString(string item);

    }

}