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
    using Framework.Persistence.Exceptions;
    using Framework.Presentation;
    using Framework.Presentation.Forms;

    /// <summary>
    /// Implementation of IComparer&lt;T&gt; for presentable objects
    /// that uses sort criteria one after another for comparison of
    /// two values.
    /// </summary>
    public class SortCriteriaComparer : IComparer<IPresentableObject> {

        /// <summary>
        /// Sort criteria to use when comparing two elements.
        /// </summary>
        private SortCriterionCollection sortCriteria;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="sortCriteria">sort criteria to use when
        /// comparing two elements</param>
        public SortCriteriaComparer(SortCriterionCollection sortCriteria) {
            if (null == sortCriteria) {
                throw new ArgumentNullException(nameof(sortCriteria));
            }
            this.sortCriteria = sortCriteria;
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
        public int Compare(IPresentableObject x, IPresentableObject y) {
            int result = 0;
            using (var sortCriteriaEnumerator = this.sortCriteria.GetEnumerator()) {
                while (0 == result && sortCriteriaEnumerator.MoveNext()) {
                    SortCriterion sortCriterion = sortCriteriaEnumerator.Current;
                    var xField = x.FindPresentableField(sortCriterion.FieldName) as IPresentableFieldForElement;
                    var yField = y.FindPresentableField(sortCriterion.FieldName) as IPresentableFieldForElement;
                    if (null == xField || null == yField) {
                        throw new FieldTypeException("Fields for multiple elements cannot be compared to each other.");
                    } else {
                        result = xField.ValueAsString.CompareTo(yField.ValueAsString);
                        if (SortDirection.Descending == sortCriterion.SortDirection) {
                            result *= -1;
                        }
                    }
                }
            }
            return result;
        }

    }

}