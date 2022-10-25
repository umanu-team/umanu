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

    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using System;

    /// <summary>
    /// JSON-RPC parameters for internal name of container, generic
    /// type and filter criteria (not thread-safe).
    /// </summary>
    internal class ParametersForInternalContainerNameAndGenericTypeAndFilterCriteria : ParametersForInternalContainerName {

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
        /// Generic type parameter.
        /// </summary>
        public Type GenericType {
            get { return Type.GetType(this.gernericType.Value); }
            set { this.gernericType.Value = value.AssemblyQualifiedName; }
        }
        private readonly PersistentFieldForString gernericType =
            new PersistentFieldForString(nameof(GenericType));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForInternalContainerNameAndGenericTypeAndFilterCriteria()
            : base() {
            this.RegisterPersistentField(this.filterCriteria);
            this.RegisterPersistentField(this.gernericType);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="internalContainerName">internal name of
        /// container</param>
        /// <param name="genericType">generic type parameter</param>
        /// <param name="filterCriteria">filter criteria</param>
        public ParametersForInternalContainerNameAndGenericTypeAndFilterCriteria(string internalContainerName, Type genericType, FilterCriteria filterCriteria)
            : this() {
            this.InternalContainerName = internalContainerName;
            this.GenericType = genericType;
            this.FilterCriteria = filterCriteria;
        }

    }

}