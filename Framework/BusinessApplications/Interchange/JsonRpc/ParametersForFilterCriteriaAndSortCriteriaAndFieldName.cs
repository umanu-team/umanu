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

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;

    /// <summary>
    /// JSON-RPC parameters for filter criteria, sort criteria and
    /// field name.
    /// </summary>
    internal sealed class ParametersForFilterCriteriaAndSortCriteriaAndFieldName : ParametersForFilterCriteriaAndSortCriteria {

        /// <summary>
        /// Field name.
        /// </summary>
        public string FieldName {
            get { return this.fieldName.Value; }
            set { this.fieldName.Value = value; }
        }
        private readonly PersistentFieldForString fieldName =
            new PersistentFieldForString(nameof(FieldName));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForFilterCriteriaAndSortCriteriaAndFieldName()
            : base() {
            this.RegisterPersistentField(this.fieldName);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="filterCriteria">filter criteria</param>
        /// <param name="sortCriteria">sort criteria</param>
        /// <param name="fieldName">field name</param>
        public ParametersForFilterCriteriaAndSortCriteriaAndFieldName(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string fieldName)
            : this() {
            this.FilterCriteria = filterCriteria;
            this.SortCriteria = sortCriteria;
            this.FieldName = fieldName;
        }

    }

}