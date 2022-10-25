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
    using System.Collections.Generic;

    /// <summary>
    /// Base class for any provider class of options to be pulled in
    /// fields of views.
    /// </summary>
    public class OptionProvider : PersistentObject {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public OptionProvider()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Determines whether the options contain an element with
        /// the specified key.
        /// </summary>
        /// <param name="key">key to look for</param>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>true if an element for the given key exists,
        /// false otherwise</returns>
        public bool ContainsKey(string key, IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            return null != this.FindValueForKey(key, parentPresentableObject, topmostPresentableObject, optionDataProvider);
        }

        /// <summary>
        /// Counts the number of occurrences of elements with a
        /// specified key with different values in the options.
        /// </summary>
        /// <param name="key">key to look for</param>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>number of occurrences of elements with specified
        /// key with different values in the options</returns>
        public int CountKey(string key, IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            var values = new List<string>(1);
            var options = this.GetOptions(parentPresentableObject, topmostPresentableObject, optionDataProvider);
            if (null != options) {
                foreach (var option in options) {
                    if (option.Key == key && !values.Contains(option.Value)) {
                        values.Add(option.Value);
                    }
                }
            }
            return values.Count;
        }

        /// <summary>
        /// Finds the key for a value.
        /// </summary>
        /// <param name="value">value to get key for</param>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>key for value if value is contained or null if
        /// value is not contained</returns>
        public string FindKeyForValue(string value, IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            string key = null;
            var options = this.GetOptions(parentPresentableObject, topmostPresentableObject, optionDataProvider);
            if (null != options) {
                foreach (var option in options) {
                    if (option.Value == value) {
                        key = option.Key;
                        break;
                    }
                }
            }
            return key;
        }

        /// <summary>
        /// Finds the read-only options for keys. 
        /// </summary>
        /// <param name="keys">keys to get options for</param>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost presentable 
        /// object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>pairs of key and value for keys which are 
        /// contained</returns>
        public IEnumerable<KeyValuePair<string, string>> FindReadOnlyOptionsForKeys(IEnumerable<string> keys, IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            var options = this.GetOptions(parentPresentableObject, topmostPresentableObject, optionDataProvider);
            foreach (var key in keys) {
                if (!string.IsNullOrEmpty(key)) {
                    var readOnlyValueForKey = this.FindReadOnlyValueForKey(key, options, optionDataProvider);
                    if (!string.IsNullOrEmpty(readOnlyValueForKey)) {
                        yield return new KeyValuePair<string, string>(key, readOnlyValueForKey);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the read-only value for a key.
        /// </summary>
        /// <param name="key">key to get value for</param>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>value for key if key is contained or null if key
        /// is not contained</returns>
        public string FindReadOnlyValueForKey(string key, IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            string value = null;
            foreach (var readOnlyOptionForKey in this.FindReadOnlyOptionsForKeys(new string[] { key }, parentPresentableObject, topmostPresentableObject, optionDataProvider)) {
                value = readOnlyOptionForKey.Value;
                break;
            }
            return value;
        }

        /// <summary>
        /// Finds the read-only value for a key.
        /// </summary>
        /// <param name="key">key to get value for</param>
        /// <param name="options">options to find value for key in</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>value for key if key is contained or null if key
        /// is not contained</returns>
        protected virtual string FindReadOnlyValueForKey(string key, IEnumerable<KeyValuePair<string, string>> options, IOptionDataProvider optionDataProvider) {
            string value = null;
            foreach (var option in options) {
                if (option.Key == key) {
                    value = option.Value;
                    break;
                }
            }
            return value;
        }

        /// <summary>
        /// Finds the value for a key.
        /// </summary>
        /// <param name="key">key to get value for</param>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>value for key if key is contained or null if key
        /// is not contained</returns>
        public string FindValueForKey(string key, IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            string value = null;
            if (!string.IsNullOrEmpty(key)) {
                foreach (var option in this.GetOptions(parentPresentableObject, topmostPresentableObject, optionDataProvider)) {
                    if (option.Key == key) {
                        value = option.Value;
                        break;
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Gets the display value to show for null.
        /// </summary>
        /// <returns>display value to show for null</returns>
        public virtual string GetDisplayValueForNull() {
            return string.Empty;
        }

        /// <summary>
        /// Gets the icon URL for a key of this option provider.
        /// </summary>
        /// <param name="key">key to get icon URL for</param>
        /// <param name="presentableObject">presentable object to get
        /// icon URL for</param>
        /// <returns>icon URLs for key of this option provider</returns>
        public string GetIconUrlFor(string key, IPresentableObject presentableObject) {
            string iconUrl = null;
            foreach (var keyValuePair in this.GetIconUrls(presentableObject)) {
                if (keyValuePair.Key == key) {
                    iconUrl = keyValuePair.Value;
                    break;
                }
            }
            return iconUrl;
        }

        /// <summary>
        /// Gets the icon URLs for all option of this option
        /// provider. (IDs of objects or user names of users need to
        /// be set as keys).
        /// </summary>
        /// <param name="presentableObject">presentable object to get
        /// icon URLs for</param>
        /// <returns>icon URLs for all options of this option
        /// provider (IDs of objects or user names of users need to
        /// be set as keys)</returns>
        protected internal virtual IEnumerable<KeyValuePair<string, string>> GetIconUrls(IPresentableObject presentableObject) {
            yield break;
        }

        /// <summary>
        /// Gets a dictionary of all unique options of this option
        /// provider (IDs of objects or user names of users need to
        /// be set as keys).
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>dictionary of all unique options of this option
        /// provider (IDs of objects or user names of users need to
        /// be set as keys)</returns>
        public IDictionary<string, string> GetOptionDictionary(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            var dictionary = new Dictionary<string, string>();
            foreach (var option in this.GetOptions(parentPresentableObject, topmostPresentableObject, optionDataProvider)) {
                dictionary[option.Key] = option.Value;
            }
            return dictionary;
        }

        /// <summary>
        /// Gets all options of this option provider (IDs of
        /// objects or user names of users need to be set as keys).
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>all options of this option provider (IDs of
        /// objects or user names of users need to be set as keys)</returns>
        public virtual IEnumerable<KeyValuePair<string, string>> GetOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            yield break;
        }

        /// <summary>
        /// Indicates whether option provider has options.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>true if option provider has options, false
        /// otherwise</returns>
        public bool HasOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            var hasOptions = false;
            foreach (var option in this.GetOptions(parentPresentableObject, topmostPresentableObject, optionDataProvider)) {
                hasOptions = true;
                break;
            }
            return hasOptions;
        }

    }

}