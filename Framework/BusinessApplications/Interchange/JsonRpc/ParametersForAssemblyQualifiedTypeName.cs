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
    /// JSON-RPC parameters for assembly qualified type name.
    /// </summary>
    internal sealed class ParametersForAssemblyQualifiedTypeName : Parameter {

        /// <summary>
        /// Assembly qualified type name.
        /// </summary>
        public string AssemblyQualifiedTypeName {
            get { return this.assemblyQualifiedTypeName.Value; }
            set { this.assemblyQualifiedTypeName.Value = value; }
        }
        private readonly PersistentFieldForString assemblyQualifiedTypeName =
            new PersistentFieldForString(nameof(AssemblyQualifiedTypeName));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForAssemblyQualifiedTypeName()
            : base() {
            this.RegisterPersistentField(this.assemblyQualifiedTypeName);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="assemblyQualifiedTypeName">assembly
        /// qualified type name</param>
        public ParametersForAssemblyQualifiedTypeName(string assemblyQualifiedTypeName)
            : this() {
            this.AssemblyQualifiedTypeName = assemblyQualifiedTypeName;
        }

    }

}