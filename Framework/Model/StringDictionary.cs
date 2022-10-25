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

namespace Framework.Model {

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Persistent dictionary for key/value pairs of type string.
    /// </summary>
    public class StringDictionary : PersistentObject, IDictionary<string, string> {

        /// <summary>
        /// Gets the number of elements contained.
        /// </summary>
        public int Count {
            get {
                return this.Options.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dictionary is
        /// read-only.
        /// </summary>
        public bool IsReadOnly {
            get {
                return this.Options.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets a collection containing all keys.
        /// </summary>
        public ICollection<string> Keys {
            get {
                var keys = new List<string>(this.Options.Count);
                foreach (var option in this.Options) {
                    keys.Add(option.KeyField);
                }
                return keys;
            }
        }

        /// <summary>
        /// List of options.
        /// </summary>
        internal PersistentFieldForPersistentObjectCollection<KeyValuePair> Options { get; private set; }

        /// <summary>
        /// Gets a collection containing all values.
        /// </summary>
        public ICollection<string> Values {
            get {
                var values = new List<string>(this.Options.Count);
                foreach (var option in this.Options) {
                    values.Add(option.Value);
                }
                return values;
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">key of the element to get or set</param>
        /// <returns>value of element with the specified key</returns>
        public string this[string key] {
            get {
                string value;
                if (null == key) {
                    throw new ArgumentNullException(nameof(key));
                } else if (!this.TryGetValue(key, out value)) {
                    throw new KeyNotFoundException("An element with key \""
                        + key + "\" could not be found.");
                }
                return value;
            }
            set {
                if (this.IsReadOnly) {
                    throw new NotSupportedException("The dictionary is read-only.");
                } else {
                    bool keyIsContained = false;
                    foreach (var option in this.Options) {
                        if (option.KeyField == key) {
                            keyIsContained = true;
                            option.Value = value;
                            break;
                        }
                    }
                    if (!keyIsContained) {
                        this.Add(key, value);
                    }
                }
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public StringDictionary()
            : base() {
            this.Options = new PersistentFieldForPersistentObjectCollection<KeyValuePair>(nameof(this.Options), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.Options);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="items">range of elements to add</param>
        public StringDictionary(IEnumerable<KeyValuePair> items)
            : this() {
            this.AddRange(items);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="items">range of elements to add</param>
        public StringDictionary(IEnumerable<KeyValuePair<string, string>> items)
            : this() {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds a new element to the dictionary.
        /// </summary>
        /// <param name="item">element to add to dictionary</param>
        public void Add(KeyValuePair item) {
            this.Add(item.KeyField, item.Value);
            return;
        }

        /// <summary>
        /// Adds a new element to the dictionary.
        /// </summary>
        /// <param name="item">element to add to dictionary</param>
        public void Add(KeyValuePair<string, string> item) {
            this.Add(item.Key, item.Value);
            return;
        }

        /// <summary>
        /// Adds a new element to the dictionary.
        /// </summary>
        /// <param name="key">key of the element to add</param>
        /// <param name="value">value of the element to add</param>
        public void Add(string key, string value) {
            if (null == key) {
                throw new ArgumentNullException(nameof(value));
            } else if (this.ContainsKey(key)) {
                throw new ArgumentException("An element with key \""
                    + key + "\" already exists in the dictionary.", nameof(key));
            } else if (this.IsReadOnly) {
                throw new NotSupportedException("The dictionary is read-only.");
            } else {
                this.Options.Add(new KeyValuePair() { KeyField = key, Value = value });
            }
            return;
        }

        /// <summary>
        /// Adds a range of elements to this dictionary.
        /// </summary>
        /// <param name="items">range of elements to add</param>
        public void AddRange(IEnumerable<KeyValuePair> items) {
            foreach (var item in items) {
                this.Add(item);
            }
            return;
        }

        /// <summary>
        /// Adds a range of elements to this dictionary.
        /// </summary>
        /// <param name="items">range of elements to add</param>
        public void AddRange(IEnumerable<KeyValuePair<string, string>> items) {
            foreach (var item in items) {
                this.Add(item);
            }
            return;
        }

        /// <summary>
        /// Removes all elements from dictionary.
        /// </summary>
        public void Clear() {
            if (this.IsReadOnly) {
                throw new NotSupportedException("The dictionary is read-only.");
            } else {
                this.Options.Clear();
            }
            return;
        }

        /// <summary>
        /// Determines whether the dictionary contains a specific
        /// element.
        /// </summary>
        /// <param name="item">element to look for</param>
        /// <returns>true if the element exists, false otherwise</returns>
        public bool Contains(KeyValuePair item) {
            return this.Contains(new KeyValuePair<string, string>(item.KeyField, item.Value));
        }

        /// <summary>
        /// Determines whether the dictionary contains a specific
        /// element.
        /// </summary>
        /// <param name="item">element to look for</param>
        /// <returns>true if the element exists, false otherwise</returns>
        public bool Contains(KeyValuePair<string, string> item) {
            bool isContained = false;
            if (this.TryGetValue(item.Key, out string value)) {
                isContained = item.Value == value;
            }
            return isContained;
        }

        /// <summary>
        /// Determines whether the dictionary contains an element
        /// with the specified key.
        /// </summary>
        /// <param name="key">key to look for</param>
        /// <returns>true if an element for the given key exists,
        /// false otherwise</returns>
        public bool ContainsKey(string key) {
            return this.TryGetValue(key, out _);
        }

        /// <summary>
        /// Copies the dictionary to an array, starting at a
        /// particular array index.
        /// </summary>
        /// <param name="array">zero-based array to copy elements
        /// into</param>
        /// <param name="arrayIndex">array index to start at</param>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
            long longArrayIndex = arrayIndex;
            if (null == array) {
                throw new ArgumentNullException(nameof(array));
            } else if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            } else if (this.Count > array.LongLength - longArrayIndex) {
                throw new ArgumentException("The number of elements in the source dictionary is greater than the available space from " + nameof(arrayIndex) + " to the end of the destination array.");
            } else {
                foreach (var option in this.Options) {
                    array[longArrayIndex] = new KeyValuePair<string, string>(option.KeyField, option.Value);
                    longArrayIndex++;
                }
            }
            return;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// dictionary.
        /// </summary>
        /// <returns>enumerator that iterates through the dictionary</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            foreach (var option in this.Options) {
                yield return new KeyValuePair<string, string>(option.KeyField, option.Value);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// dictionary.
        /// </summary>
        /// <returns>enumerator that iterates through the dictionary</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            foreach (var option in this.Options) {
                yield return new KeyValuePair<string, string>(option.KeyField, option.Value);
            }
        }

        /// <summary>
        /// Gets the index of a specific key.
        /// </summary>
        /// <param name="key">key to get index for</param>
        /// <returns>index of given key or -1</returns>
        protected int GetIndexOfKey(string key) {
            if (null == key) {
                throw new ArgumentNullException(nameof(key));
            }
            int index = -1;
            for (int i = 0; i < this.Options.Count; i++) {
                if (this.Options[i].KeyField == key) {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// Sorts the elements in the dictionary using the specified
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="leftIndex">start index of elements to sort</param>
        /// <param name="rightIndex">end index of elements to sort</param>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements</param>
        private void QuickSort(int leftIndex, int rightIndex, IComparer<KeyValuePair<string, string>> comparer) {
            if (leftIndex < rightIndex) {
                int pivotIndex = leftIndex + (rightIndex - leftIndex) / 2;
                var pivotOption = this.Options[pivotIndex];
                var pivotKeyValuePair = new KeyValuePair<string, string>(pivotOption.KeyField, pivotOption.Value);
                this.SwapUnsafe(pivotIndex, rightIndex);
                pivotIndex = leftIndex;
                for (int i = leftIndex; i < rightIndex; i++) {
                    var option = this.Options[i];
                    var keyValuePair = new KeyValuePair<string, string>(option.KeyField, option.Value);
                    if (comparer.Compare(keyValuePair, pivotKeyValuePair) < 0) {
                        this.SwapUnsafe(i, pivotIndex);
                        pivotIndex++;
                    }
                }
                this.SwapUnsafe(pivotIndex, rightIndex);
                this.QuickSort(leftIndex, pivotIndex - 1, comparer);
                this.QuickSort(pivotIndex + 1, rightIndex, comparer);
            }
            return;
        }

        /// <summary>
        /// Removes the element for a specific key.
        /// </summary>
        /// <param name="key">key to remove element for</param>
        /// <returns>true if the element is successfully removed,
        /// false otherwise - e.g. if no element for the given key
        /// exists</returns>
        public bool Remove(string key) {
            int index = this.GetIndexOfKey(key);
            bool success = index > -1;
            if (success) {
                this.RemoveAt(index);
            }
            return success;
        }

        /// <summary>
        /// Removes the element with the specified key from the
        /// dictionary.
        /// </summary>
        /// <param name="item">element to remove key for</param>
        /// <returns>true if the element is successfully removed,
        /// false otherwise - e.g. if it does not exist</returns>
        public bool Remove(KeyValuePair item) {
            return this.Remove(item.KeyField);
        }

        /// <summary>
        /// Removes the element with the specified key from the
        /// dictionary.
        /// </summary>
        /// <param name="item">element to remove key for</param>
        /// <returns>true if the element is successfully removed,
        /// false otherwise - e.g. if it does not exist</returns>
        public bool Remove(KeyValuePair<string, string> item) {
            return this.Remove(item.Key);
        }

        /// <summary>
        /// Removes the value at a specific index.
        /// </summary>
        /// <param name="index">index to remove value at</param>
        protected void RemoveAt(int index) {
            if (this.IsReadOnly) {
                throw new NotSupportedException("The dictionary is read-only.");
            } else {
                this.Options.RemoveAt(index);
            }
            return;
        }

        /// <summary>
        /// Sorts the elements in the dictionary using the default
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        public void Sort(Comparison<KeyValuePair<string, string>> comparison) {
            this.Sort(new ComparisonComparer<KeyValuePair<string, string>>(comparison));
            return;
        }

        /// <summary>
        /// Sorts the elements in the dictionary using the specified
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        public void Sort(IComparer<KeyValuePair<string, string>> comparer) {
            this.QuickSort(0, this.Count - 1, comparer);
            return;
        }

        /// <summary>
        /// Swaps the elements of two indexes whithout validation of
        /// the indexes.
        /// </summary>
        /// <param name="firstIndex">index to assign element of
        /// second index to</param>
        /// <param name="secondIndex">index to assign element of
        /// first index to</param>
        private void SwapUnsafe(int firstIndex, int secondIndex) {
            var option = this.Options[firstIndex];
            this.Options[firstIndex] = this.Options[secondIndex];
            this.Options[secondIndex] = option;
            return;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">key to get value for</param>
        /// <param name="value">value associated with the specified
        /// key</param>
        /// <returns>true on success, false otherwise - e.g. if no
        /// value for the given key exists</returns>
        public bool TryGetValue(string key, out string value) {
            bool success = false;
            value = null;
            foreach (var option in this.Options) {
                if (option.KeyField == key) {
                    success = true;
                    value = option.Value;
                    break;
                }
            }
            return success;
        }

    }

}