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

namespace Framework.BusinessApplications.Interchange.JsonRpc {

    using System.Globalization;
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;

    /// <summary>
    /// JSON-RPC parameters for filter criteria and sort criteria.
    /// </summary>
    internal class ParametersForFilterCriteriaAndSortCriteria : Parameter {

        /// <summary>
        /// Filter criteria.
        /// </summary>
        public FilterCriteria FilterCriteria {
            get {
                return FilterCriteria.FromStringFilter(this.filterCriteria.Value);
            }
            set {
                this.filterCriteria.Value = value.ToString();
            }
        }
        private readonly PersistentFieldForString filterCriteria =
            new PersistentFieldForString(nameof(FilterCriteria));

        /// <summary>
        /// Sort criteria.
        /// </summary>
        public SortCriterionCollection SortCriteria {
            get {
                SortCriterionCollection sortCriteria = new SortCriterionCollection();
                foreach (KeyValuePair sortCriterion in this.sortCriteria) {
                    sortCriteria.Add(sortCriterion.KeyField, (SortDirection)((int.Parse(sortCriterion.Value))));
                }
                return sortCriteria;
            }
            set {
                this.sortCriteria.Clear();
                foreach (SortCriterion sortCriterion in value) {
                    this.sortCriteria.Add(new KeyValuePair(sortCriterion.FieldName, ((int)sortCriterion.SortDirection).ToString(CultureInfo.InvariantCulture)));
                }
            }
        }
        private readonly PersistentFieldForPersistentObjectCollection<KeyValuePair> sortCriteria =
            new PersistentFieldForPersistentObjectCollection<KeyValuePair>(nameof(SortCriteria), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForFilterCriteriaAndSortCriteria()
            : base() {
            this.RegisterPersistentField(this.filterCriteria);
            this.RegisterPersistentField(this.sortCriteria);
        }

    }

}