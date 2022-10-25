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

    using System.Collections.Generic;

    /// <summary>
    /// Sort criteria used to order the results of actions on
    /// containers of persistent objects. The order of criteria
    /// added to this list affects the way the results become
    /// sorted: First-come, first-serve.
    /// </summary>
    public sealed class SortCriterionCollection : List<SortCriterion> {

        /// <summary>
        /// Empty sort criteria that will skip sorting.
        /// </summary>
        public static readonly SortCriterionCollection Empty = new SortCriterionCollection();

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public SortCriterionCollection()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="sortCriteria">sort criteria to initialize
        /// sort criterion collection with</param>
        public SortCriterionCollection(IEnumerable<SortCriterion> sortCriteria)
            : this() {
            this.AddRange(sortCriteria);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="memberName">name of field to order by</param>
        /// <param name="sortDirection">direction to use for sorting</param>
        public SortCriterionCollection(string memberName, SortDirection sortDirection)
            : this() {
            this.Add(memberName, sortDirection);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="memberNameChain">name chain of fields to
        /// order by</param>
        /// <param name="sortDirection">direction to use for sorting</param>
        public SortCriterionCollection(string[] memberNameChain, SortDirection sortDirection)
            : this() {
            this.Add(memberNameChain, sortDirection);
        }

        /// <summary>
        /// Adds a new sort criterion to these sort criteria.
        /// </summary>
        /// <param name="memberName">name of class field/property to
        /// sort by</param>
        /// <param name="sortDirection">direction of sort criterion</param>
        public void Add(string memberName, SortDirection sortDirection) {
            var sortCriterion = new SortCriterion(memberName, sortDirection);
            this.Add(sortCriterion);
            return;
        }

        /// <summary>
        /// Adds a new sort criterion to these sort criteria.
        /// </summary>
        /// <param name="memberNameChain">name chain of class
        /// field/property to sort by</param>
        /// <param name="sortDirection">direction of sort criterion</param>
        public void Add(string[] memberNameChain, SortDirection sortDirection) {
            var sortCriterion = new SortCriterion(memberNameChain, sortDirection);
            this.Add(sortCriterion);
            return;
        }

        /// <summary>
        /// Concatenates two sort criterion collections.
        /// </summary>
        /// <param name="first">first sort criterion collection to
        /// concatenate</param>
        /// <param name="second">second sort criterion collection to
        /// concatenate</param>
        /// <returns>concatenation of provided sort criterion
        /// collections</returns>
        public static SortCriterionCollection Concat(SortCriterionCollection first, SortCriterionCollection second) {
            SortCriterionCollection concated;
            if (null == second || second.Count < 1) {
                concated = first;
            } else if (null == first || first.Count < 1) {
                concated = second;
            } else {
                concated = new SortCriterionCollection(first);
                concated.AddRange(second);
            }
            return concated;
        }

        /// <summary>
        /// Reverses the sort directions of all sort criteria.
        /// </summary>
        public void ReverseSortDirections() {
            foreach (var sortCriterion in this) {
                sortCriterion.ReverseSortDirection();
            }
            return;
        }

        /// <summary>
        /// Translates the field names of sort criteria.
        /// </summary>
        /// <param name="dictionary">dictionary of translations to
        /// apply</param>
        /// <returns>copy of sort criteria with translated field
        /// names</returns>
        public SortCriterionCollection TranslateFieldNames(IDictionary<string, string> dictionary) {
            var sortCriteria = new SortCriterionCollection();
            foreach (var sortCriterion in this) {
                sortCriteria.Add(sortCriterion.TranslateFieldName(dictionary));
            }
            return sortCriteria;
        }

    }

}