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
    using Framework.Presentation.Forms;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common;
    using System.Text;

    /// <summary>
    /// List of objects that can be stored in persistence mechanism.
    /// </summary>
    public abstract class PersistentFieldForCollection : PersistentField, IPresentableFieldForCollection {

        /// <summary>
        /// Gets or sets the total number of elements the internal
        /// data structure can hold without resizing.
        /// </summary>
        public abstract int Capacity { get; set; }

        /// <summary>
        /// Gets the number of objects contained.
        /// </summary>
        public int Count {
            get {
                this.EnsureRetrieval();
                return this.count;
            }
            protected set {
                this.EnsureRetrieval();
                if (value > this.Capacity) {
                    if (value > int.MaxValue / 2) {
                        this.Capacity = value;
                    } else {
                        this.Capacity = value * 2;
                    }
                }
                this.count = value;
            }
        }
        private int count;

        /// <summary>
        /// True if field is for a single element, false if it is for
        /// a collection of elements.
        /// </summary>
        public sealed override bool IsForSingleElement {
            get {
                return false;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        internal PersistentFieldForCollection(string key)
            : base(key) {
            this.count = 0;
        }

        /// <summary>
        /// Adds an object to the collection. 
        /// </summary>
        /// <param name="item">object to add to the collection</param>
        public abstract void AddObject(object item);

        /// <summary>
        /// Adds a string value to the list. 
        /// </summary>
        /// <param name="item">string value to add to the list</param>
        public void AddString(string item) {
            if (!this.TryAddString(item)) {
                throw new FormatException("String value \"" + item + "\" cannot be converted to type " + this.ContentBaseType + ".");
            }
            return;
        }

        /// <summary>
        /// Searches the sorted list for an item. The list must
        /// already be sorted according to the comparer
        /// implementation; otherwise, the result is incorrect.
        /// </summary>
        /// <param name="persistentFields">array of persistent fields
        /// for all values</param>
        /// <param name="leftIndex">zero-based starting index of the
        /// range to search</param>
        /// <param name="rightIndex">zero-based ending index of the
        /// range to search</param>
        /// <param name="item">item to locate - can be null for
        /// reference types</param>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        /// <returns>zero-based index of item in the sorted list if
        /// item was found, otherwise a negative number that is the
        /// bitwise complement of the index of the next element that
        /// is larger than item or, if there is no larger element,
        /// the bitwise complement of Count</returns>
        /// <typeparam name="TValue">type of values</typeparam>
        /// <typeparam name="TField">type of persistent field for values</typeparam>
        protected static int BinarySearch<TValue, TField>(TField[] persistentFields, TValue item, int leftIndex, int rightIndex, IComparer<TValue> comparer) where TField : IPresentableFieldForElement<TValue> {
            bool itemFound = false;
            int middleIndex = 0;
            while (rightIndex >= leftIndex) {
                middleIndex = leftIndex + (rightIndex - leftIndex) / 2;
                int comparisonResult = comparer.Compare(persistentFields[middleIndex].Value, item);
                if (comparisonResult < 0) {
                    leftIndex = middleIndex + 1;
                } else if (comparisonResult > 0) {
                    rightIndex = middleIndex - 1;
                } else {
                    itemFound = true;
                    break;
                }
            }
            if (!itemFound) {
                middleIndex = ~(leftIndex + (rightIndex - leftIndex) / 2);
            }
            return middleIndex;
        }

        /// <summary>
        /// Removes all objects from the collection.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Copies the values of persistent field.
        /// </summary>
        /// <returns>copies of values of persistent field</returns>
        internal virtual IEnumerable<object> CopyValues() {
            return this.GetValuesAsObject();
        }

        /// <summary>
        /// Calculates the ending index and validates both, starting
        /// index and ending index.
        /// </summary>
        /// <param name="startIndex">zero-based starting index</param>
        /// <param name="count">number of elements in the section</param>
        /// <returns>ending index</returns>
        protected int GetEndIndexAndValidateIndexes(int startIndex, int count) {
            if (0 != startIndex) {
                this.ValidateIndex(startIndex);
            }
            if (count < 0) {
                throw new ArgumentException("Count may not be < 0.");
            }
            int endIndex = startIndex + count - 1;
            if (endIndex >= this.Count) {
                throw new ArgumentOutOfRangeException(nameof(endIndex));
            }
            return endIndex;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection of sortable values or null. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection
        /// of sortable values or null</returns>
        public virtual IEnumerable<string> GetSortableValues() {
            return this.GetValuesAsString();
        }

        /// <summary>
        /// Gets the values of presentable field as object.
        /// </summary>
        /// <returns>values of presentable field as object</returns>
        public abstract IEnumerable<object> GetValuesAsObject();

        /// <summary>
        /// Gets the values of presentable field as plain text. As an
        /// example, this is used for full-text indexing.
        /// </summary>
        /// <returns>values of this presentable field as plain text</returns>
        public virtual IEnumerable<string> GetValuesAsPlainText() {
            return this.GetValuesAsString();
        }

        /// <summary>
        /// Gets the values of presentable field as string.
        /// </summary>
        /// <returns>values of presentable field as string</returns>
        public abstract IEnumerable<string> GetValuesAsString();

        /// <summary>
        /// Gets the versioned field for a specific date.
        /// </summary>
        /// <param name="modificationDate">date to get versioned
        /// field for</param>
        /// <returns>versioned field if existing, null otherwise</returns>
        public virtual IPresentableFieldForCollection GetVersionedField(DateTime? modificationDate) {
            IPresentableFieldForCollection versionedField;
            var versionValue = this.ParentPersistentObject.GetVersionValue(modificationDate);
            if (null == versionValue) {
                versionedField = null;
            } else {
                versionedField = versionValue.GetPersistentFieldForCollectionOfElements(this.Key);
            }
            return versionedField;
        }

        /// <summary>
        /// Initializes the temporary collection for DbDataReader.
        /// </summary>
        internal abstract void InitializeTemporaryCollectionForDbDataReader();

        /// <summary>
        /// Joins all values of collection into a single string value
        /// with a separator.
        /// </summary>
        /// <param name="separator">separator to add between values</param>
        /// <returns>all values of collection as single string value
        /// with separator</returns>
        public string Join(string separator) {
            var stringBuilder = new StringBuilder();
            foreach (var stringValue in this.GetValuesAsString()) {
                if (stringBuilder.Length > 0) {
                    stringBuilder.Append(separator);
                }
                stringBuilder.Append(stringValue);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Loads the current value of a DbDataReader into a
        /// temporary collection but does not set it yet.
        /// </summary>
        /// <param name="dataReader">data reader to load value from</param>
        internal abstract void LoadValueFromDbDataReader(DbDataReader dataReader);

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="persistentFields">array of persistent fields
        /// for all values</param>
        /// <param name="leftIndex">start index of elements to sort</param>
        /// <param name="rightIndex">end index of elements to sort</param>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements</param>
        /// <typeparam name="TValue">type of values</typeparam>
        protected static void QuickSort<TValue>(IPresentableFieldForCollection<TValue> persistentFields, int leftIndex, int rightIndex, IComparer<TValue> comparer) {
            if (leftIndex < rightIndex) {
                int pivotIndex = leftIndex + (rightIndex - leftIndex) / 2;
                var pivotValue = persistentFields[pivotIndex];
                persistentFields.Swap(pivotIndex, rightIndex);
                pivotIndex = leftIndex;
                for (int i = leftIndex; i < rightIndex; i++) {
                    if (comparer.Compare(persistentFields[i], pivotValue) < 0) {
                        persistentFields.Swap(i, pivotIndex);
                        pivotIndex++;
                    }
                }
                persistentFields.Swap(pivotIndex, rightIndex);
                PersistentFieldForCollection.QuickSort(persistentFields, leftIndex, pivotIndex - 1, comparer);
                PersistentFieldForCollection.QuickSort(persistentFields, pivotIndex + 1, rightIndex, comparer);
            }
            return;
        }

        /// <summary>
        /// Retrieves the values of this field from persistence
        /// mechanism. This can be used to refresh the values of this
        /// field.
        /// </summary>
        public override void Retrieve() {
            if (null == this.ParentPersistentObject || this.ParentPersistentObject.IsNew) {
                throw new ObjectNotPersistentException("Persistent field cannot be retrieved because it is not connected to a persistence mechanism.");
            } else {
                this.ParentPersistentObject.ParentPersistentContainer.RetrieveField(this);
            }
            return;
        }

        /// <summary>
        /// Sets the new values for persistent field which have been
        /// loaded from DbDataReader before.
        /// </summary>
        internal abstract void SetValuesFromDbDataReader();

        /// <summary>
        /// Sorts the elements in the list using the default
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        public abstract void Sort();

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        public abstract void SortObjects(Comparison<object> comparison);

        /// <summary>
        /// Swaps the items of two indexes.
        /// </summary>
        /// <param name="firstIndex">index to assign item of second
        /// index to</param>
        /// <param name="secondIndex">index to assign item of first
        /// index to</param>
        public abstract void Swap(int firstIndex, int secondIndex);

        /// <summary>
        /// Sets the capacity to the actual number of elements in the
        /// list.
        /// </summary>
        public void TrimExcess() {
            this.Capacity = this.Count;
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

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if index is out of
        /// range.
        /// </summary>
        /// <param name="index">index to validate</param>
        /// <returns>index to validate</returns>
        protected int ValidateIndex(int index) {
            if (index < 0 || index >= this.Count) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return index;
        }

    }

    /// <summary>
    /// List of objects that can be stored in persistence mechanism.
    /// </summary>
    /// <typeparam name="T">type of values to be stored in list</typeparam>
    public abstract class PersistentFieldForCollection<T> : PersistentFieldForCollection, IPresentableFieldForCollection<T>, IConvenientList<T> {

        /// <summary>
        /// Gets or sets the total number of elements the internal
        /// data structure can hold without resizing.
        /// </summary>
        public sealed override int Capacity {
            get {
                return this.PersistentFields.Length;
            }
            set {
                if (value < 0 || value < this.Count) {
                    throw new ArgumentOutOfRangeException(nameof(value));
                } else if (value != this.Capacity) {
                    var newPersistentFields = new PersistentFieldForElement<T>[value];
                    if (value > 0) {
                        this.PersistentFields.CopyTo(newPersistentFields, 0);
                    }
                    for (int i = this.PersistentFields.Length; i < value; i++) {
                        var newPersistentField = this.GetNewPersistentFieldForElement();
                        newPersistentField.IsRetrieved = true;
                        newPersistentField.ParentPersistentObject = this.ParentPersistentObject;
                        newPersistentFields[i] = newPersistentField;
                    }
                    this.PersistentFields = newPersistentFields;
                }
            }
        }

        /// <summary>
        /// Parent persistent object of this persistent field/list.
        /// </summary>
        public sealed override PersistentObject ParentPersistentObject {
            get {
                return base.ParentPersistentObject;
            }
            internal set {
                base.ParentPersistentObject = value;
                for (int i = 0; i < this.Count; i++) {
                    this.PersistentFields[i].ParentPersistentObject = value;
                }
            }
        }

        /// <summary>
        /// Protected array of persistent fields for all values.
        /// </summary>
        private PersistentFieldForElement<T>[] PersistentFields {
            get {
                this.EnsureRetrieval();
                return this.persistentFields;
            }
            set {
                this.EnsureRetrieval();
                this.persistentFields = value;
            }
        }
        private PersistentFieldForElement<T>[] persistentFields;

        /// <summary>
        /// Gets the value at a specific index.
        /// </summary>
        /// <param name="index">index to get value for</param>
        /// <returns>value at the specific index</returns>
        public T this[int index] {
            get {
                this.ValidateIndex(index);
                return this.PersistentFields[index].Value;
            }
            set {
                this.ValidateIndex(index);
                this.PersistentFields[index].Value = value;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent list</param>
        internal PersistentFieldForCollection(string key)
            : base(key) {
            this.persistentFields = new PersistentFieldForElement<T>[0];
        }

        /// <summary>
        /// Adds an object to the list. 
        /// </summary>
        /// <param name="item">object to add to the list</param>
        public void Add(T item) {
            this.Insert(this.Count, item);
            return;
        }

        /// <summary>
        /// Adds an object to the collection. 
        /// </summary>
        /// <param name="item">object to add to the collection</param>
        public sealed override void AddObject(object item) {
            this.Add((T)item);
            return;
        }

        /// <summary>
        /// Adds a collection of objects to the list. 
        /// </summary>
        /// <param name="collection">collection of objects to add to
        /// the list</param>
        public void AddRange(IEnumerable<T> collection) {
            this.InsertRange(this.Count, collection);
            return;
        }

        /// <summary>
        /// Returns a read-only wrapper for the list.
        /// </summary>
        /// <returns>read-only wrapper for list</returns>
        public ReadOnlyCollection<T> AsReadOnly() {
            return new ReadOnlyCollection<T>(this);
        }

        /// <summary>
        /// Searches the sorted list for an item. The list must
        /// already be sorted according to the default comparer
        /// implementation; otherwise, the result is incorrect.
        /// </summary>
        /// <param name="item">item to locate - can be null for
        /// reference types</param>
        public int BinarySearch(T item) {
            return this.BinarySearch(0, this.Count, item, null);
        }

        /// <summary>
        /// Searches the sorted list for an item. The list must
        /// already be sorted according to the comparison
        /// implementation; otherwise, the result is incorrect.
        /// </summary>
        /// <param name="item">item to locate - can be null for
        /// reference types</param>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        /// <returns>zero-based index of item in the sorted list if
        /// item was found, otherwise a negative number that is the
        /// bitwise complement of the index of the next element that
        /// is larger than item or, if there is no larger element,
        /// the bitwise complement of Count</returns>
        public int BinarySearch(T item, Comparison<T> comparison) {
            return this.BinarySearch(item, new ComparisonComparer<T>(comparison));
        }

        /// <summary>
        /// Searches the sorted list for an item. The list must
        /// already be sorted according to the comparer
        /// implementation; otherwise, the result is incorrect.
        /// </summary>
        /// <param name="item">item to locate - can be null for
        /// reference types</param>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        /// <returns>zero-based index of item in the sorted list if
        /// item was found, otherwise a negative number that is the
        /// bitwise complement of the index of the next element that
        /// is larger than item or, if there is no larger element,
        /// the bitwise complement of Count</returns>
        public int BinarySearch(T item, IComparer<T> comparer) {
            return this.BinarySearch(0, this.Count, item, comparer);
        }

        /// <summary>
        /// Searches the sorted list for an item. The list must
        /// already be sorted according to the comparer
        /// implementation; otherwise, the result is incorrect.
        /// </summary>
        /// <param name="index">zero-based starting index of the
        /// range to search</param>
        /// <param name="count">length of the range to search</param>
        /// <param name="item">item to locate - can be null for
        /// reference types</param>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        /// <returns>zero-based index of item in the sorted list if
        /// item was found, otherwise a negative number that is the
        /// bitwise complement of the index of the next element that
        /// is larger than item or, if there is no larger element,
        /// the bitwise complement of Count</returns>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) {
            IComparer<T> searchComparer;
            if (null == comparer) {
                searchComparer = Comparer<T>.Default;
            } else {
                searchComparer = comparer;
            }
            int endIndex = this.GetEndIndexAndValidateIndexes(index, count);
            return PersistentFieldForCollection.BinarySearch(this.PersistentFields, item, index, endIndex, searchComparer);
        }

        /// <summary>
        /// Removes all objects from the list.
        /// </summary>
        public sealed override void Clear() {
            this.EnsureRetrieval();
            this.ClearUnsafe();
            this.HasChanged();
            return;
        }

        /// <summary>
        /// Removes all objects from the list by bypassing the change
        /// cycle.
        /// </summary>
        private void ClearUnsafe() {
            this.Count = 0;
            this.PersistentFields = new PersistentFieldForElement<T>[0];
            return;
        }

        /// <summary>
        /// Determines whether the list contains a specific object.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <returns>true if specific object is contained, false
        /// otherwise</returns>
        public bool Contains(T item) {
            return (this.IndexOf(item) > -1);
        }

        /// <summary>
        /// Converts the elements of the list to another type, and
        /// returns a list containing the converted elements.
        /// </summary>
        /// <typeparam name="TOutput">type of the elements of the
        /// target array</typeparam>
        /// <param name="converter">delegate that converts each
        /// element from one type to another type</param>
        /// <returns>list containing the converted elements</returns>
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
            if (null == converter) {
                throw new ArgumentNullException(nameof(converter));
            }
            var convertedList = new List<TOutput>(this.Count);
            foreach (var value in this) {
                convertedList.Add(converter.Invoke(value));
            }
            return convertedList;
        }

        /// <summary>
        /// Copies the objects of the list to an array, starting at
        /// the beginning of the array.
        /// </summary>
        /// <param name="array">array to copy the list into</param>
        public void CopyTo(T[] array) {
            this.CopyTo(array, 0);
            return;
        }

        /// <summary>
        /// Copies the objects of the list to an array, starting at a
        /// particular array index. 
        /// </summary>
        /// <param name="array">array to copy the list into</param>
        /// <param name="arrayIndex">array index to start at</param>
        public void CopyTo(T[] array, int arrayIndex) {
            this.CopyTo(0, array, arrayIndex, this.Count);
            return;
        }

        /// <summary>
        /// Copies a range of objects of the list to an array,
        /// starting at a particular array index. 
        /// </summary>
        /// <param name="listIndex">list index to start at</param>
        /// <param name="array">array to copy the list into</param>
        /// <param name="arrayIndex">array index to start at</param>
        /// <param name="count">number of list objects to copy</param>
        public void CopyTo(int listIndex, T[] array, int arrayIndex, int count) {
            if (null == array) {
                throw new ArgumentNullException(nameof(array));
            } else if (listIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(listIndex));
            } else if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count));
            } else if (array.LongLength < arrayIndex + count) {
                throw new ArgumentException("The number of elements from " + nameof(listIndex) + " to the end of the source list is greater than the available space from arrayIndex to the end of the destination array.");
            } else if (this.Count > 0) {
                this.ValidateIndex(listIndex);
                int arrayPosition = arrayIndex;
                int listPosition = listIndex;
                int copiedElements = 0;
                while (copiedElements < count) {
                    array[arrayPosition] = this.PersistentFields[listPosition].Value;
                    arrayPosition++;
                    listPosition++;
                    copiedElements++;
                }
            }
            return;
        }

        /// <summary>
        /// Determines whether the list contains elements that match
        /// the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the elements to search for</param>
        /// <returns>true if the list contains one or more elements
        /// that match the conditions defined by the specified
        /// predicate, false otherwise</returns>
        public bool Exists(Predicate<T> match) {
            if (null == match) {
                throw new ArgumentNullException(nameof(match));
            }
            bool foundMatch = false;
            foreach (var value in this) {
                if (match.Invoke(value)) {
                    foundMatch = true;
                    break;
                }
            }
            return foundMatch;
        }

        /// <summary>
        /// Finds the the first element that matches the conditions
        /// defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>first element that matches the conditions
        /// defined by the specified predicate, default value of type
        /// T otherwise</returns>
        public T Find(Predicate<T> match) {
            T result = default(T);
            int index = this.FindIndex(match);
            if (index > -1) {
                result = this.PersistentFields[index].Value;
            }
            return result;
        }

        /// <summary>
        /// Finds all elements that match the conditions defined by
        /// the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>all elements that match the conditions defined
        /// by the specified predicate</returns>
        public List<T> FindAll(Predicate<T> match) {
            List<int> indexes = this.FindAllIndexes(match);
            List<T> values = new List<T>(indexes.Count);
            foreach (int index in indexes) {
                values.Add(this.PersistentFields[index].Value);
            }
            return values;
        }

        /// <summary>
        /// Finds the index of the first element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int FindIndex(Predicate<T> match) {
            return this.FindIndex(0, match);
        }

        /// <summary>
        /// Finds the index of the first element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int FindIndex(int startIndex, Predicate<T> match) {
            return this.FindIndex(startIndex, this.Count - startIndex, match);
        }

        /// <summary>
        /// Finds the index of the first element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int FindIndex(int startIndex, int count, Predicate<T> match) {
            int index = -1;
            List<int> indexes = this.FindAllIndexes(startIndex, count, match, true);
            if (indexes.Count > 0) {
                index = indexes[0];
            }
            return index;
        }

        /// <summary>
        /// Finds the indexes of all elements that match the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>list of zero-based indexes of all occurrences of
        /// matching objects within the entire list</returns>
        public List<int> FindAllIndexes(Predicate<T> match) {
            return this.FindAllIndexes(0, match);
        }

        /// <summary>
        /// Finds the indexes of all elements that match the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>list of zero-based indexes of all occurrences of
        /// matching objects within the entire list</returns>
        public List<int> FindAllIndexes(int startIndex, Predicate<T> match) {
            return this.FindAllIndexes(startIndex, this.Count - startIndex, match);
        }

        /// <summary>
        /// Finds the indexes of all elements that match the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>list of zero-based indexes of all occurrences of
        /// matching objects within the entire list</returns>
        public List<int> FindAllIndexes(int startIndex, int count, Predicate<T> match) {
            return this.FindAllIndexes(startIndex, count, match, false);
        }

        /// <summary>
        /// Finds the indexes of all elements that match the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <param name="onlyFindFirstMatch">true to find the first
        /// match only, false otherwise</param>
        /// <returns>list of zero-based indexes of all occurrences of
        /// matching objects within the entire list</returns>
        private List<int> FindAllIndexes(int startIndex, int count, Predicate<T> match, bool onlyFindFirstMatch) {
            int endIndex = this.GetEndIndexAndValidateIndexes(startIndex, count);
            if (null == match) {
                throw new ArgumentNullException(nameof(match));
            }
            List<int> indexes = new List<int>();
            for (int i = startIndex; i <= endIndex; i++) {
                if (match.Invoke(this.PersistentFields[i].Value)) {
                    indexes.Add(i);
                    if (onlyFindFirstMatch) {
                        break;
                    }
                }
            }
            return indexes;
        }

        /// <summary>
        /// Finds the the last element that matches the conditions
        /// defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>last element that matches the conditions
        /// defined by the specified predicate, default value of type
        /// T otherwise</returns>
        public T FindLast(Predicate<T> match) {
            T result = default(T);
            int index = this.FindLastIndex(match);
            if (index > -1) {
                result = this.PersistentFields[index].Value;
            }
            return result;
        }

        /// <summary>
        /// Finds the index of the last element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int FindLastIndex(Predicate<T> match) {
            return this.FindLastIndex(0, match);
        }

        /// <summary>
        /// Finds the index of the last element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int FindLastIndex(int startIndex, Predicate<T> match) {
            return this.FindLastIndex(startIndex, this.Count - startIndex, match);
        }

        /// <summary>
        /// Finds the index of the last element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int FindLastIndex(int startIndex, int count, Predicate<T> match) {
            int endIndex = this.GetEndIndexAndValidateIndexes(startIndex, count);
            if (null == match) {
                throw new ArgumentNullException(nameof(match));
            }
            int index = -1;
            for (int i = endIndex; i >= startIndex; i--) {
                if (match.Invoke(this.PersistentFields[i].Value)) {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// Performs the specified action on each element of the
        /// list.
        /// </summary>
        /// <param name="action">delegate to perform on each element
        /// of the list</param>
        public void ForEach(Action<T> action) {
            if (null == action) {
                throw new ArgumentNullException(nameof(action));
            }
            foreach (var value in this) {
                action.Invoke(value);
            }
            return;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>enumerator that iterates through the list</returns>
        public IEnumerator<T> GetEnumerator() {
            for (var i = 0; i < this.Count; i++) {
                yield return this.PersistentFields[i].Value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            for (var i = 0; i < this.Count; i++) {
                yield return this.PersistentFields[i].Value;
            }
        }

        /// <summary>
        /// Gets a new instance of
        /// PersistentFieldForElement&lt;TValue&gt;.
        /// </summary>
        /// <returns>new instance of
        /// PersistentFieldForElement&lt;TValue&gt;</returns>
        protected abstract PersistentFieldForElement<T> GetNewPersistentFieldForElement();

        /// <summary>
        /// Creates a shallow copy of a range of elements in the
        /// list.
        /// </summary>
        /// <param name="startIndex">zero-based list index at which
        /// the range starts</param>
        /// <param name="count">number of elements in the range</param>
        /// <returns>shallow copy of a range of elements in the list</returns>
        public List<T> GetRange(int startIndex, int count) {
            int endIndex = this.GetEndIndexAndValidateIndexes(startIndex, count);
            List<T> rangeList = new List<T>(count);
            for (int i = startIndex; i <= endIndex; i++) {
                rangeList.Add(this.PersistentFields[i].Value);
            }
            return rangeList;
        }

        /// <summary>
        /// Gets the values of presentable field as object.
        /// </summary>
        /// <returns>values of presentable field as object</returns>
        public sealed override IEnumerable<object> GetValuesAsObject() {
            foreach (var value in this) {
                yield return value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public override IEnumerable<string> GetValuesAsString() {
            foreach (var value in this) {
                yield return value.ToString();
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the first occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int IndexOf(T item) {
            return this.IndexOf(item, 0);
        }

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the first occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int IndexOf(T item, int startIndex) {
            return this.IndexOf(item, startIndex, this.Count - startIndex);
        }

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the first occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int IndexOf(T item, int startIndex, int count) {
            int endIndex = this.GetEndIndexAndValidateIndexes(startIndex, count);
            int index = -1;
            for (int i = startIndex; i <= endIndex; i++) {
                if (null == this.PersistentFields[i].Value) {
                    if (null == item) {
                        index = i;
                        break;
                    }
                } else if (this.PersistentFields[i].Value.Equals(item)) {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// Inserts an object into the list at a specific index.
        /// </summary>
        /// <param name="index">index to insert item at</param>
        /// <param name="item">object to insert into the list</param>
        public void Insert(int index, T item) {
            this.InsertRange(index, new T[] { item });
            return;
        }

        /// <summary>
        /// Inserts a collection of objects into the list at a
        /// specific index.
        /// </summary>
        /// <param name="startIndex">index to insert item at</param>
        /// <param name="collection">collection of objects to insert
        /// into the list</param>
        public void InsertRange(int startIndex, IEnumerable<T> collection) {
            if (0 != startIndex && this.Count != startIndex) {
                this.ValidateIndex(startIndex);
            }
            if (null == collection) {
                throw new ArgumentNullException(nameof(collection));
            }
            this.EnsureRetrieval();
            this.InsertRangeUnsafe(startIndex, collection);
            this.HasChanged();
            return;
        }

        /// <summary>
        /// Inserts a collection of objects into the list at a
        /// specific index unsafely and by bypassing the change
        /// cycle.
        /// </summary>
        /// <param name="startIndex">index to insert item at</param>
        /// <param name="collection">collection of objects to insert
        /// into the list</param>
        private void InsertRangeUnsafe(int startIndex, IEnumerable<T> collection) {
            int collectionCount = 0;
            foreach (var value in collection) {
                collectionCount++;
            }
            this.Count += collectionCount;
            int moveIndex = collectionCount + startIndex;
            for (int i = this.Count - 1; i >= moveIndex; i--) {
                this.PersistentFields[i] = this.PersistentFields[i - collectionCount];
            }
            int j = startIndex;
            foreach (T value in collection) {
                var newPersistentField = this.GetNewPersistentFieldForElement();
                newPersistentField.IsRetrieved = true;
                newPersistentField.Value = value;
                newPersistentField.ParentPersistentObject = this.ParentPersistentObject;
                this.PersistentFields[j] = newPersistentField;
                j++;
            }
            return;
        }

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the last occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int LastIndexOf(T item) {
            return this.LastIndexOf(item, 0);
        }

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the last occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int LastIndexOf(T item, int startIndex) {
            return this.LastIndexOf(item, startIndex, this.Count - startIndex);
        }

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the last occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public int LastIndexOf(T item, int startIndex, int count) {
            int endIndex = this.GetEndIndexAndValidateIndexes(startIndex, count);
            int index = -1;
            for (int i = endIndex; i >= startIndex; i--) {
                if (null == this.PersistentFields[i].Value) {
                    if (null == item) {
                        index = i;
                        break;
                    }
                } else if (this.PersistentFields[i].Value.Equals(item)) {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// Creates a new item that could be added to this
        /// collection.
        /// </summary>
        /// <returns>new item that could be added to this collection</returns>
        public sealed override object NewItemAsObject() {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Removes a specific object from the list.
        /// </summary>
        /// <param name="item">specific object to remove from list</param>
        /// <returns>true if object was successfully removed from
        /// list, false otherwise or if object was not contained in
        /// list</returns>
        public virtual bool Remove(T item) {
            bool success = false;
            int index = this.IndexOf(item);
            if (index > -1) {
                this.RemoveAt(index);
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Removes all elements that match the conditions defined by
        /// the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the elements to remove</param>
        /// <returns>number of elements removed from list</returns>
        public int RemoveAll(Predicate<T> match) {
            List<int> indexes = this.FindAllIndexes(match);
            for (int i = indexes.Count - 1; i > -1; i--) {
                this.RemoveAt(indexes[i]);
            }
            return indexes.Count;
        }

        /// <summary>
        /// Removes the object at a specific index from list.
        /// </summary>
        /// <param name="index">specific index to remove object at</param>
        public void RemoveAt(int index) {
            this.RemoveRange(index, 1);
            return;
        }

        /// <summary>
        /// Removes the object at a specific index from list.
        /// </summary>
        /// <param name="startIndex">index to start removing objects
        /// at</param>
        /// <param name="count">number of objects to remove from list</param>
        public void RemoveRange(int startIndex, int count) {
            this.EnsureRetrieval();
            this.GetEndIndexAndValidateIndexes(startIndex, count);
            int oldCount = this.Count;
            this.Count -= count;
            for (int i = startIndex; i < this.Count; i++) {
                this.PersistentFields[i] = this.PersistentFields[i + count];
            }
            for (int i = this.Count; i < oldCount; i++) {
                var newPersistentField = this.GetNewPersistentFieldForElement();
                newPersistentField.IsRetrieved = true;
                newPersistentField.ParentPersistentObject = this.ParentPersistentObject;
                this.PersistentFields[i] = newPersistentField;
            }
            this.HasChanged();
            return;
        }

        /// <summary>
        /// Clears this list and adds a new object afterwards.
        /// </summary>
        /// <param name="value">new object to add to the list</param>
        public void ReplaceBy(T value) {
            this.Clear();
            this.Add(value);
            return;
        }

        /// <summary>
        /// Clears this list and adds a new collection of objects
        /// afterwards.
        /// </summary>
        /// <param name="collection">new collection of objects to add
        /// to the list</param>
        public void ReplaceBy(IEnumerable<T> collection) {
            this.Clear();
            this.AddRange(collection);
            return;
        }

        /// <summary>
        /// Reverses the order of the elements in the list.
        /// </summary>
        public void Reverse() {
            this.Reverse(0, this.Count);
            return;
        }

        /// <summary>
        /// Reverses the order of the elements in the list.
        /// </summary>
        /// <param name="index">zero-based starting index of the
        /// range to reverse</param>
        /// <param name="count">number of elements in the range to
        /// reverse</param>
        public void Reverse(int index, int count) {
            this.GetEndIndexAndValidateIndexes(index, count);
            Array.Reverse(this.PersistentFields, index, count);
            return;
        }

        /// <summary>
        /// Sets the values of persistent field by bypassing the
        /// change cycle.
        /// </summary>
        /// <param name="values">values to be set</param>
        protected void SetValuesUnsafe(IEnumerable<T> values) {
            this.ClearUnsafe();
            this.InsertRangeUnsafe(0, values);
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the default
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        public override void Sort() {
            this.Sort(0, this.Count, null);
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the default
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        public void Sort(Comparison<T> comparison) {
            this.Sort(new ComparisonComparer<T>(comparison));
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        public void Sort(IComparer<T> comparer) {
            this.Sort(0, this.Count, comparer);
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="index">zero-based starting index of the
        /// range to sort</param>
        /// <param name="count">length of the range to sort</param>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        public void Sort(int index, int count, IComparer<T> comparer) {
            IComparer<T> sortComparer;
            if (null == comparer) {
                sortComparer = Comparer<T>.Default;
            } else {
                sortComparer = comparer;
            }
            int endIndex = this.GetEndIndexAndValidateIndexes(index, count);
            PersistentFieldForCollection.QuickSort(this, index, endIndex, sortComparer);
            return;
        }

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        public override void SortObjects(Comparison<object> comparison) {
            this.Sort(new ComparisonComparer<T>(comparison));
            return;
        }

        /// <summary>
        /// Swaps the elements of two indexes.
        /// </summary>
        /// <param name="firstIndex">index to assign element of
        /// second index to</param>
        /// <param name="secondIndex">index to assign element of
        /// first index to</param>
        public sealed override void Swap(int firstIndex, int secondIndex) {
            this.ValidateIndex(firstIndex);
            this.ValidateIndex(secondIndex);
            T element = this.persistentFields[firstIndex].Value;
            this.persistentFields[firstIndex].Value = this.persistentFields[secondIndex].Value;
            this.persistentFields[secondIndex].Value = element;
            this.HasChanged();
            return;
        }

        /// <summary>
        /// Copies the elements of the list to a new array.
        /// </summary>
        /// <returns>array containing copies of the elements of the
        /// list</returns>
        public T[] ToArray() {
            T[] array = new T[this.Count];
            this.CopyTo(array);
            return array;
        }

        /// <summary>
        /// Determines whether every element in the list matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions</param>
        /// <returns>true if list is empty or if all elements in the
        /// list match the condition, false otherwise</returns>
        public bool TrueForAll(Predicate<T> match) {
            return this.FindAllIndexes(match).Count == this.Count;
        }

    }

}