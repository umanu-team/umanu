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

    using System;
    using System.Collections.Generic;
    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;

    /// <summary>
    /// JSON-RPC parameters for internal name of container, filter
    /// criteria, sort criteria and field names (not thread-safe).
    /// </summary>
    internal sealed class ParametersForInternalContainerNameAndGenericTypeAndFilterCriteriaAndSortCriteriaAndFieldNames : ParametersForInternalContainerNameAndGenericTypeAndFilterCriteriaAndSortCriteria {

        /// <summary>
        /// Field names.
        /// </summary>
        public PersistentFieldForStringCollection FieldNames { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForInternalContainerNameAndGenericTypeAndFilterCriteriaAndSortCriteriaAndFieldNames()
            : base() {
            this.FieldNames = new PersistentFieldForStringCollection(nameof(FieldNames));
            this.RegisterPersistentField(this.FieldNames);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="internalContainerName">internal name of
        /// container</param>
        /// <param name="genericType">generic type parameter</param>
        /// <param name="filterCriteria">filter criteria</param>
        /// <param name="sortCriteria">sort criteria</param>
        /// <param name="fieldNames">field names</param>
        public ParametersForInternalContainerNameAndGenericTypeAndFilterCriteriaAndSortCriteriaAndFieldNames(string internalContainerName, Type genericType, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, IEnumerable<string> fieldNames)
            : this() {
            this.InternalContainerName = internalContainerName;
            this.GenericType = genericType;
            this.FilterCriteria = filterCriteria;
            this.SortCriteria = sortCriteria;
            this.FieldNames.AddRange(fieldNames);
        }

    }

}
