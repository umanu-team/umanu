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

    // Partial class for container operations of persistence mechanism.
    public partial class DummyPersistenceMechanism {

        /// <summary>
        /// Adds a container for storing persistent objects of a
        /// specific type to persistence mechanism.
        /// </summary>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to create container for</param>
        protected override void AddContainer(PersistentObject sampleInstance) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the full .NET type name of a container in this
        /// persistence mechanism.
        /// </summary>
        /// <param name="internalContainerName">internal name of a
        /// container in this persistence mechanism</param>
        /// <returns>full .NET type name of a container in this
        /// persistence mechanism or an empty string if container
        /// does not exist</returns>
        protected internal override string GetAssemblyQualifiedTypeNameOfContainer(string internalContainerName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets info objects for all containers.
        /// </summary>
        /// <returns>info objects for all containers</returns>
        protected override ICollection<ContainerInfo> GetContainerInfos() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the internal name of a container in this
        /// persistence mechanism (e.g. the name of the related SQL
        /// database table).
        /// </summary>
        /// <param name="assemblyQualifiedTypeName">assembly qualifoed type name
        /// of persistent object to get internal name for</param>
        /// <returns>internal name of a container in this persistence
        /// mechanism or an empty string if container does not exist</returns>
        protected internal override string GetInternalNameOfContainer(string assemblyQualifiedTypeName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates all required system containers in an empty
        /// persistence mechanism - this needs to be called from
        /// "AddContainer()", "MigrateAllContainers()",
        /// "RemoveAllContainers()" and "RenameAllContainers()" if
        /// necessary.
        /// </summary>
        protected override void InitializePersistenceMechanism() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the container for storing persistent objects of
        /// a type and all its persistent objects in persistence
        /// mechanism. Containers of subclasses are not affected.
        /// </summary>
        /// <param name="internalContainerName">internal name of
        /// container in this persistence mechanism to remove</param>
        protected override void RemoveContainer(string internalContainerName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renames a container for storing persistent objects of a
        /// type.
        /// </summary>
        /// <param name="oldName">start of old name of container</param>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to rename container for</param>
        /// <returns>true if container could be renamed successfully,
        /// false otherwise or if it did not exist in persistence
        /// mechanism</returns>
        protected override bool RenameContainer(string oldName, PersistentObject sampleInstance) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the container for storing persistent objects of a
        /// specific type in persistence mechanism. This needs to be
        /// called whenever there is a change in the fields to
        /// persist of the related persistent objects.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// machnism!
        /// </summary>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to update container for</param>
        protected override void UpdateContainer(PersistentObject sampleInstance) {
            throw new NotImplementedException();
        }

    }

}