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

    /// <summary>
    /// JSON-RPC parameters for internal name of container.
    /// </summary>
    internal class ParametersForInternalContainerName : Parameter {

        /// <summary>
        /// Internal name of container.
        /// </summary>
        public string InternalContainerName {
            get { return this.internalContainerName.Value; }
            set { this.internalContainerName.Value = value; }
        }
        private readonly PersistentFieldForString internalContainerName =
            new PersistentFieldForString(nameof(InternalContainerName));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForInternalContainerName()
            : base() {
            this.RegisterPersistentField(this.internalContainerName);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="internalContainerName">internal name of
        /// container</param>
        public ParametersForInternalContainerName(string internalContainerName)
            : this() {
            this.InternalContainerName = internalContainerName;
        }

    }

}