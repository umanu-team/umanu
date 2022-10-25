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

    using Model;
    using System.Collections.Generic;

    /// <summary>
    /// Sort criterion used to order the results of actions on
    /// containers of persistent objects.
    /// </summary>
    public sealed class SortCriterion {

        /// <summary>
        /// Name of field to order by.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Name chain of fields to order by.
        /// </summary>
        public string[] FieldNameChain {
            get { return KeyChain.FromKey(this.FieldName); }
            set { this.FieldName = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Direction to use for sorting, either ascending order or
        /// descending order.
        /// </summary>
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// Instantiates a new instance with name of member and
        /// direction for sorting.
        /// </summary>
        /// <param name="memberName">name of field to order by</param>
        /// <param name="sortDirection">direction to use for sorting</param>
        public SortCriterion(string memberName, SortDirection sortDirection) {
            this.FieldName = memberName;
            this.SortDirection = sortDirection;
        }

        /// <summary>
        /// Instantiates a new instance with name of member and
        /// direction for sorting.
        /// </summary>
        /// <param name="memberNameChain">name chain of fields to
        /// order by</param>
        /// <param name="sortDirection">direction to use for sorting</param>
        public SortCriterion(string[] memberNameChain, SortDirection sortDirection) {
            this.FieldNameChain = memberNameChain;
            this.SortDirection = sortDirection;
        }

        /// <summary>
        /// Reverses the sort direction.
        /// </summary>
        public void ReverseSortDirection() {
            if (SortDirection.Ascending == this.SortDirection) {
                this.SortDirection = SortDirection.Descending;
            } else {
                this.SortDirection = SortDirection.Ascending;
            }
            return;
        }

        /// <summary>
        /// Translates the field name of sort criterion.
        /// </summary>
        /// <param name="dictionary">dictionary of translations to
        /// apply</param>
        /// <returns>copy of sort criterion with translated field
        /// name</returns>
        public SortCriterion TranslateFieldName(IDictionary<string, string> dictionary) {
            if (!dictionary.TryGetValue(this.FieldName, out string translatedFieldName)) {
                translatedFieldName = this.FieldName;
            }
            return new SortCriterion(translatedFieldName, this.SortDirection);
        }

    }

}