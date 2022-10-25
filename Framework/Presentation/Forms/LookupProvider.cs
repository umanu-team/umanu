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

    using Framework.Presentation.Exceptions;
    using Persistence;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for any provider class of lookup values to be
    /// pulled in fields of views.
    /// </summary>
    public abstract class LookupProvider : PersistentObject {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public LookupProvider()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Finds values by a vague search term containing at least
        /// parts of the value.
        /// </summary>
        /// <param name="vagueTerm">search term for value</param>
        /// <param name="presentableObject">presentable object to get
        /// lookup values for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// lookup providers</param>
        /// <returns>matching values</returns>
        public virtual IEnumerable<string> FindValuesByVagueTerm(string vagueTerm, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            yield break;
        }

        /// <summary>
        /// Finds a unique value by a vague search term containing at
        /// least parts of the value. If no unique match can be
        /// found, null will be returned.
        /// </summary>
        /// <param name="vagueTerm">search term for value</param>
        /// <param name="presentableObject">presentable object to get
        /// lookup values for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// lookup providers</param>
        /// <returns>matching unique value or null</returns>
        public string FindUniqueValueByVagueTerm(string vagueTerm, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string uniqueValue = null;
            if (!string.IsNullOrEmpty(vagueTerm)) {
                var values = this.FindValuesByVagueTerm(vagueTerm, presentableObject, optionDataProvider);
                foreach (var value in values) {
                    if (!string.IsNullOrEmpty(uniqueValue)) {
                        uniqueValue = null;
                        foreach (var v in values) {
                            if (vagueTerm.Equals(v, StringComparison.Ordinal)) {
                                uniqueValue = v;
                                break;
                            }
                        }
                        break;
                    }
                    uniqueValue = value;
                }
            }
            return uniqueValue;
        }

        /// <summary>
        /// Gets the comparison to be used for sorting lists of
        /// string values alphabetically with values starting with
        /// vague term on top.
        /// </summary>
        /// <param name="vagueTerm">search term for value</param>
        /// <returns>comparison to be used for sorting lists of
        /// string values alphabetically with values starting with
        /// vague term on top</returns>
        protected static Comparison<string> GetComparisonFor(string vagueTerm) {
            return delegate (string a, string b) {
                int result;
                if (a.StartsWith(vagueTerm) && !b.StartsWith(vagueTerm)) {
                    result = -1;
                } else if (!a.StartsWith(vagueTerm) && b.StartsWith(vagueTerm)) {
                    result = 1;
                } else {
                    result = a.CompareTo(b);
                }
                return result;
            };
        }

        /// <summary>
        /// Tries to split a value with paranthesis in format
        /// "valueLeftToParanthesis (valueInParanthesis)".
        /// </summary>
        /// <param name="value">value with paranthesis</param>
        /// <param name="valueLeftToParanthesis">value left to
        /// paranthesis</param>
        /// <param name="valueInParanthesis">value in paranthesis</param>
        /// <returns>true if values left to as well as inside of
        /// paranthesis could be found and split, false otherwise</returns>
        protected static bool TrySplitValueWithParanthesis(string value, out string valueLeftToParanthesis, out string valueInParanthesis) {
            bool isSplitSuccessfully = false;
            valueLeftToParanthesis = null;
            valueInParanthesis = null;
            if (!string.IsNullOrEmpty(value)) {
                var leftParenthesisIndex = value.LastIndexOf('(') + 1;
                if (leftParenthesisIndex > 1 && value.EndsWith(")")) {
                    valueLeftToParanthesis = value.Substring(0, leftParenthesisIndex - 2).Trim();
                    var rightParenthesisIndex = value.Length - leftParenthesisIndex - 1;
                    valueInParanthesis = value.Substring(leftParenthesisIndex, rightParenthesisIndex).Trim();
                    if (!string.IsNullOrEmpty(valueLeftToParanthesis) && !string.IsNullOrEmpty(valueInParanthesis)) {
                        isSplitSuccessfully = true;
                    }
                }
            }
            return isSplitSuccessfully;
        }

    }

    /// <summary>
    /// Base class for any provider class of lookup values to be
    /// pulled in fields of views.
    /// </summary>
    /// <typeparam name="T">type of key</typeparam>
    public abstract class LookupProvider<T> : LookupProvider {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public LookupProvider()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Determines whether the lookup values contain a specific
        /// key.
        /// </summary>
        /// <param name="key">key to look for</param>
        /// <param name="presentableObject">presentable object to get
        /// lookup values for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// lookup providers</param>
        /// <returns>true if the specific key exists, false otherwise</returns>
        public bool ContainsKey(T key, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            var value = this.FindValueForKey(key, presentableObject, optionDataProvider);
            if (string.Empty == value) {
                throw new PresentationException("Empty string is not a valid value for lookup providers.");
            }
            return null != value;
        }

        /// <summary>
        /// Finds the key for a value.
        /// </summary>
        /// <param name="value">value to get key for</param>
        /// <param name="presentableObject">presentable object to get
        /// lookup values for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// lookup providers</param>
        /// <returns>key for value if value is contained or null if
        /// value is not contained</returns>
        public abstract T FindKeyForValue(string value, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider);

        /// <summary>
        /// Finds the value for a key.
        /// </summary>
        /// <param name="key">key to get value for</param>
        /// <param name="presentableObject">presentable object to get
        /// lookup values for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// lookup providers</param>
        /// <returns>value for key if key is contained or null if key
        /// is not contained</returns>
        public abstract string FindValueForKey(T key, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider);

    }

}