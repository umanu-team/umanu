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

namespace Framework.Persistence {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of IComparer&lt;T&gt; that uses an delagate of
    /// type Comparison&lt;T&gt; for comparison of two values.
    /// </summary>
    /// <typeparam name="T">type of elements to be compared</typeparam>
    internal sealed class ComparisonComparer<T> : IComparer<T> {

        /// <summary>
        /// Comparison delegate to use when comparing two elements.
        /// </summary>
        private Comparison<T> comparison;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        public ComparisonComparer(Comparison<T> comparison) {
            if (null == comparison) {
                throw new ArgumentNullException(nameof(comparison));
            }
            this.comparison = comparison;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        public ComparisonComparer(Comparison<object> comparison)
            : this(delegate (T x, T y) {
                return comparison.Invoke(x, y);
            }) {
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
        public int Compare(T x, T y) {
            return this.comparison.Invoke(x, y);
        }

    }

}