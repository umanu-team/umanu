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
    using Framework.Persistence.Fields;
    using System.Collections.Generic;

    /// <summary>
    /// Provider class for options of type string to be pulled in
    /// fields of views.
    /// </summary>
    public class StringOptionDictionary : OptionProvider, IDictionary<string, string> {

        /// <summary>
        /// Gets the number of elements contained.
        /// </summary>
        public int Count {
            get {
                return this.Options.Count;
            }
        }

        /// <summary>
        /// Display value to show for null.
        /// </summary>
        public string DisplayValueForNull {
            get { return this.displayValueForNull.Value; }
            set { this.displayValueForNull.Value = value; }
        }
        private readonly PersistentFieldForString displayValueForNull =
            new PersistentFieldForString(nameof(DisplayValueForNull));

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
                return this.Options.Keys;
            }
        }

        /// <summary>
        /// Dictionary of options.
        /// </summary>
        protected StringDictionary Options {
            get { return this.options.Value; }
        }
        private readonly PersistentFieldForPersistentObject<StringDictionary> options =
            new PersistentFieldForPersistentObject<StringDictionary>(nameof(Options), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Gets a collection containing all values.
        /// </summary>
        public ICollection<string> Values {
            get {
                return this.Options.Values;
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">key of the element to get or set</param>
        /// <returns>value of element with the specified key</returns>
        public string this[string key] {
            get {
                return this.Options[key];
            }
            set {
                this.Options[key] = value;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public StringOptionDictionary()
            : base() {
            this.RegisterPersistentField(this.displayValueForNull);
            this.options.Value = new StringDictionary();
            this.RegisterPersistentField(this.options);
        }

        /// <summary>
        /// Adds a new element to the dictionary.
        /// </summary>
        /// <param name="item">element to add to dictionary</param>
        public void Add(KeyValuePair item) {
            this.Options.Add(item);
            return;
        }

        /// <summary>
        /// Adds a new element to the dictionary.
        /// </summary>
        /// <param name="item">element to add to dictionary</param>
        public void Add(KeyValuePair<string, string> item) {
            this.Options.Add(item);
            return;
        }

        /// <summary>
        /// Adds a new element to the dictionary.
        /// </summary>
        /// <param name="key">key of the element to add</param>
        /// <param name="value">value of the element to add</param>
        public void Add(string key, string value) {
            this.Options.Add(key, value);
            return;
        }

        /// <summary>
        /// Adds a range of elements to this dictionary.
        /// </summary>
        /// <param name="items">range of elements to add</param>
        public void AddRange(IEnumerable<KeyValuePair> items) {
            this.Options.AddRange(items);
            return;
        }

        /// <summary>
        /// Adds a range of elements to this dictionary.
        /// </summary>
        /// <param name="items">range of elements to add</param>
        public void AddRange(IEnumerable<KeyValuePair<string, string>> items) {
            this.Options.AddRange(items);
            return;
        }

        /// <summary>
        /// Adds a range of pairs of key and value to the list.
        /// </summary>
        /// <param name="items">enumerable of arrays of objects
        /// whereas either the key to add is at index 0 and the value
        /// is at index 1 or key and value are equal and at index 0</param>
        public void AddRange(IEnumerable<object[]> items) {
            foreach (object[] item in items) {
                if (null != item || item.LongLength > 0) {
                    string key = item[0] as string;
                    string value;
                    if (item.LongLength > 1) {
                        value = item[1] as string;
                    } else {
                        value = key;
                    }
                    if (null != key && !this.ContainsKey(key)) {
                        this.Add(key, value);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Removes all elements from dictionary.
        /// </summary>
        public void Clear() {
            this.Options.Clear();
            return;
        }

        /// <summary>
        /// Determines whether the dictionary contains a specific
        /// element.
        /// </summary>
        /// <param name="item">element to look for</param>
        /// <returns>true if the element exists, false otherwise</returns>
        public bool Contains(KeyValuePair<string, string> item) {
            return this.Options.Contains(item);
        }

        /// <summary>
        /// Determines whether the dictionary contains an element
        /// with the specified key.
        /// </summary>
        /// <param name="key">key to look for</param>
        /// <returns>true if an element for the given key exists,
        /// false otherwise</returns>
        public bool ContainsKey(string key) {
            return this.Options.ContainsKey(key);
        }

        /// <summary>
        /// Copies the dictionary to an Array, starting at a
        /// particular Array index.
        /// </summary>
        /// <param name="array">zero-based array to copy elements
        /// into</param>
        /// <param name="arrayIndex">Array index to start at</param>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
            this.Options.CopyTo(array, arrayIndex);
            return;
        }

        /// <summary>
        /// Gets the display value to show for null.
        /// </summary>
        /// <returns>display value to show for null</returns>
        public sealed override string GetDisplayValueForNull() {
            return this.DisplayValueForNull;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// dictionary.
        /// </summary>
        /// <returns>enumerator that iterates through the dictionary</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            return this.Options.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// dictionary.
        /// </summary>
        /// <returns>enumerator that iterates through the dictionary</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            foreach (var option in this.Options) {
                yield return option;
            }
        }

        /// <summary>
        /// Gets all options of this option prvider.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>all options of this option prvider</returns>
        public override IEnumerable<KeyValuePair<string, string>> GetOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            return this.Options;
        }

        /// <summary>
        /// Removes the element for a specific key.
        /// </summary>
        /// <param name="key">key to remove element for</param>
        /// <returns>true if the element is successfully removed,
        /// false otherwise - e.g. if no element for the given key
        /// exists</returns>
        public bool Remove(string key) {
            return this.Options.Remove(key);
        }

        /// <summary>
        /// Removes the element with the specified key from the
        /// dictionary.
        /// </summary>
        /// <param name="item">element to remove key for</param>
        /// <returns>true if the element is successfully removed,
        /// false otherwise - e.g. if it does not exist</returns>
        public bool Remove(KeyValuePair<string, string> item) {
            return this.Options.Remove(item);
        }

        /// <summary>
        /// Sorts the dictionary by key.
        /// </summary>
        public virtual void SortByKey() {
            this.Options.Sort(new KeyValuePairStringKeyComparer());
            return;
        }

        /// <summary>
        /// Sorts the dictionary by value.
        /// </summary>
        public virtual void SortByValue() {
            this.Options.Sort(new KeyValuePairStringValueComparer());
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
            return this.Options.TryGetValue(key, out value);
        }

    }

}