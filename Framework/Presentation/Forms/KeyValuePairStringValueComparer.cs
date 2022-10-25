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

    using System.Collections.Generic;

    /// <summary>
    /// Compares key/value pairs of type string/string by value.
    /// </summary>
    internal sealed class KeyValuePairStringValueComparer : IComparer<KeyValuePair<string, string>> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public KeyValuePairStringValueComparer() {
            // nothing to do
        }

        /// <summary>
        /// Compares two objects and returns a value indicating
        /// whether one is less than, equal to, or greater than the
        /// other.
        /// </summary>
        /// <param name="x">first object to compare</param>
        /// <param name="y">second object to compare</param>
        /// <returns>A signed integer that indicates the relative
        /// order of the comparands: Less than zero if x is less than
        /// y. Equal to zero if x is equal to y. Greater than zero if
        /// x is greater than y.</returns>
        public int Compare(KeyValuePair<string, string> x, KeyValuePair<string, string> y) {
            return x.Value.CompareTo(y.Value);
        }

    }

}