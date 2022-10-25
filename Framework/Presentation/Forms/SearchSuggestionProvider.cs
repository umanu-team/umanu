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

    /// <summary>
    /// Base class for any provider class of lookup values with
    /// string keys to be pulled in fields of views.
    /// </summary>
    public class SearchSuggestionProvider : StringLookupProvider {

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
        public override string FindKeyForValue(string value, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            return value; // this is applied to full text part of search query
        }

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
        public sealed override string FindValueForKey(string key, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            return key; // this is not used for search suggestions
        }

    }

}
