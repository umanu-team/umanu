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

    /// <summary>
    /// Info about a persistent container.
    /// </summary>
    public sealed class ContainerInfo {

        /// <summary>
        /// Assembly qualified type name of elements in container.
        /// </summary>
        public string AssemblyQualifiedTypeName { get; private set; }

        /// <summary>
        /// Insternal name of container.
        /// </summary>
        public string InternalName { get; private set; }

        /// <summary>
        /// Indicates whether type property is initialized already.
        /// A null-check would not be sufficient because a value of
        /// null is a possible result of initialization.
        /// </summary>
        private bool isTypeInitialized = false;

        /// <summary>
        /// .NET type of elements in container or null.
        /// </summary>
        public Type Type {
            get {
                if (!this.isTypeInitialized) {
                    this.isTypeInitialized = true;
                    this.type = Type.GetType(this.AssemblyQualifiedTypeName, false);
                }
                return this.type;
            }
        }
        private Type type;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="internalName">internal name of container</param>
        /// <param name="assemblyQualifiedTypeName">assembly
        /// qualified type name of container</param>
        public ContainerInfo(string internalName, string assemblyQualifiedTypeName) {
            this.InternalName = internalName;
            this.AssemblyQualifiedTypeName = assemblyQualifiedTypeName;
        }

    }

}