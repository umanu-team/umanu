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

    using Framework.Diagnostics;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Transactions;

    // Partial class for container operations of persistence
    // mechanism.
    public partial class PersistenceMechanism {

        /// <summary>
        /// Cache for persistent containers.
        /// </summary>
        protected KeyedCollection<string, PersistentContainer> ContainerCache {
            get {
                return this.containerCache;
            }
        }
        private readonly PersistentContainerCache containerCache = new PersistentContainerCache();

        /// <summary>
        /// Indicates whether persistence mechanism has containers.
        /// </summary>
        public bool HasContainers {
            get {
                bool hasContainers = false;
                try {
                    hasContainers = this.GetContainerInfos().Count > 0;
                } catch (PersistenceMechanismNotInitializedException) {
                    // ignore persistence mechanism not initialized exceptions
                }
                return hasContainers;
            }
        }

        /// <summary>
        /// Adds a container for storing persistent objects of type
        /// T to persistence mechanism.
        /// </summary>
        /// <typeparam name="T">type of persistent objects to be
        /// stored in container</typeparam>
        public void AddContainer<T>() where T : PersistentObject, new() {
            this.AddContainer(this.CreateInstance<T>());
            if (this.HasVersioningEnabled) {
                this.VersioningRepository.AddContainer<T>();
            }
            return;
        }

        /// <summary>
        /// Adds a container for storing persistent objects of a
        /// specific type to persistence mechanism.
        /// </summary>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to create container for</param>
        protected abstract void AddContainer(PersistentObject sampleInstance);

        /// <summary>
        /// Checks whether a container for storing persistent
        /// objects of type T exists in persistence mechanism.
        /// </summary>
        /// <typeparam name="T">type of persistent objects to be
        /// stored in container</typeparam>
        /// <returns>true if container exists, false otherwise</returns>
        public bool ContainsContainer<T>()
            where T : PersistentObject, new() {
            string assemblyQualifiedName = typeof(T).AssemblyQualifiedName;
            return this.ContainsContainer(assemblyQualifiedName);
        }

        /// <summary>
        /// Checks whether a container for storing persistent
        /// objects of a specific type exists in persistence mechanism.
        /// </summary>
        /// <param name="assemblyQualifiedName">assembly qualified
        /// type name of container to check</param>
        /// <returns>true if container exists, false otherwise</returns>
        protected bool ContainsContainer(string assemblyQualifiedName) {
            string internalName = this.GetInternalNameOfContainer(assemblyQualifiedName);
            return !string.IsNullOrEmpty(internalName);
        }

        /// <summary>
        /// Checks whether a container for storing persistent
        /// objects exists in persistence mechanism for a sample
        /// object.
        /// </summary>
        /// <param name="sampleInstance">sample instance of
        /// persistent object to check existence of container for</param>
        /// <returns>true if container exists, false otherwise</returns>
        public bool ContainsContainer(PersistentObject sampleInstance) {
            string assemblyQualifiedName = sampleInstance.Type.AssemblyQualifiedName;
            return this.ContainsContainer(assemblyQualifiedName);
        }

        /// <summary>
        /// Ensures that containers for storing all types of
        /// persistent objects contained in a given enumerable of
        /// types exist in persistence mechanism and that they are up
        /// to date.
        /// </summary>
        /// <param name="types">types to ensure containers for</param>
        private void EnsureAllContainers(IEnumerable<Type> types) {
            var persistentTypes = PersistenceMechanism.GetPersistentTypesCascadedly(types);
            while (persistentTypes.Count > 0) {
                for (int i = persistentTypes.Count - 1; i > -1; i--) {
                    try {
                        var sampleInstance = this.CreateInstance(persistentTypes[i]);
                        this.EnsureContainer(sampleInstance);
                        persistentTypes.RemoveAt(i);
                    } catch (DependencyException) {
                        // ignore dependency exceptions
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Ensures that a container for storing persistent objects
        /// of type T exists in persistence mechanism and that it is
        /// up to date.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// machnism!
        /// </summary>
        /// <typeparam name="T">type of persistent objects to ensure
        /// container for</typeparam>
        public void EnsureContainer<T>() where T : PersistentObject, new() {
            this.EnsureContainer(this.CreateInstance<T>());
            if (this.HasVersioningEnabled) {
                this.VersioningRepository.EnsureContainer<T>();
            }
            return;
        }

        /// <summary>
        /// Ensures that a container for storing persistent objects
        /// of type T exists in persistence mechanism and that it is
        /// up to date.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// machnism!
        /// </summary>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to ensure container for</param>
        protected void EnsureContainer(PersistentObject sampleInstance) {
            Type type = sampleInstance.Type;
            if (!this.ContainsContainer(type.AssemblyQualifiedName)) {
                this.AddContainer(sampleInstance);
            } else {
                this.UpdateContainer(sampleInstance);
            }
            return;
        }

        /// <summary>
        /// Gets the persistent container of a sample object.
        /// </summary>
        /// <param name="assemblyQualifiedTypeName">assembly
        /// qualified name of type to get persistent container base
        /// for</param>
        /// <returns>persistent container base of type</returns>
        internal PersistentContainer FindContainer(string assemblyQualifiedTypeName) {
            PersistentContainer persistentContainer;
            if (this.ContainerCache.Contains(assemblyQualifiedTypeName)) {
                persistentContainer = this.ContainerCache[assemblyQualifiedTypeName];
            } else {
                var containerType = typeof(PersistentContainer<>).MakeGenericType(Type.GetType(assemblyQualifiedTypeName));
                persistentContainer = Activator.CreateInstance(containerType, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { this, assemblyQualifiedTypeName }, null) as PersistentContainer;
                this.ContainerCache.Add(persistentContainer);
            }
            return persistentContainer;
        }

        /// <summary>
        /// Gets the persistent container of a sample object.
        /// </summary>
        /// <param name="type">type of persistent objects to get the
        /// container for</param>
        /// <returns>persistent container base of sample object</returns>
        internal PersistentContainer FindContainer(Type type) {
            return this.FindContainer(type.AssemblyQualifiedName);
        }

        /// <summary>
        /// Gets the persistent container of all objects of type T.
        /// </summary>
        /// <typeparam name="T">type of persistent objects to get the
        /// container for</typeparam>
        /// <returns>persistent container of all objects of type T</returns>
        public PersistentContainer<T> FindContainer<T>() where T : PersistentObject, new() {
            PersistentContainer<T> persistentContainer;
            string assemblyQualifiedTypeName = typeof(T).AssemblyQualifiedName;
            if (this.ContainerCache.Contains(assemblyQualifiedTypeName)) {
                persistentContainer = this.ContainerCache[assemblyQualifiedTypeName] as PersistentContainer<T>;
            } else {
                persistentContainer = new PersistentContainer<T>(this, assemblyQualifiedTypeName);
                this.ContainerCache.Add(persistentContainer);
            }
            return persistentContainer;
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
        protected internal abstract string GetAssemblyQualifiedTypeNameOfContainer(string internalContainerName);

        /// <summary>
        /// Gets info objects for all containers.
        /// </summary>
        /// <returns>info objects for all containers</returns>
        protected abstract ICollection<ContainerInfo> GetContainerInfos();

        /// <summary>
        /// Gets the internal name of a container in this
        /// persistence mechanism (e.g. the name of the related SQL
        /// database table).
        /// </summary>
        /// <param name="assemblyQualifiedTypeName">assembly qualifoed type name
        /// of persistent object to get internal name for</param>
        /// <returns>internal name of a container in this persistence
        /// mechanism or an empty string if container does not exist</returns>
        protected internal abstract string GetInternalNameOfContainer(string assemblyQualifiedTypeName);

        /// <summary>
        /// Gets the internal name of a container in this
        /// persistence mechanism (e.g. the name of the related SQL
        /// database table).
        /// </summary>
        /// <param name="type">type of persistent object to get
        /// internal name for</param>
        /// <returns>internal name of a container in this persistence
        /// mechanism or an empty string if container does not exist</returns>
        protected internal virtual string GetInternalNameOfContainer(Type type) {
            return this.GetInternalNameOfContainer(type.AssemblyQualifiedName);
        }

        /// <summary>
        /// Gets a list of all persistent types for given types
        /// cascadedly.
        /// </summary>
        /// <param name="types">types to get persistent types for</param>
        /// <returns>list of all persistent types for given types</returns>
        protected static IList<Type> GetPersistentTypesCascadedly(IEnumerable<Type> types) {
            var potentialBaseTypes = new List<Type>();
            foreach (var type in types) {
                PersistenceMechanism.GetPotentialBaseTypes(potentialBaseTypes, type);
            }
            var persistentTypes = new List<Type> {
                TypeOf.AllowedGroups,
                typeof(Group)
            };
            foreach (var type in types) {
                PersistenceMechanism.GetPersistentTypesOfType(persistentTypes, type, potentialBaseTypes);
            }
            return persistentTypes;
        }

        /// <summary>
        /// Adds all persistent types to a list of types used in
        /// compositions or associations of a type.
        /// </summary>
        /// <param name="persistentTypes">list of types to add
        /// persistent types to</param>
        /// <param name="type">type to analyze</param>
        /// <param name="potentialBaseTypes">potential base types of
        /// type</param>
        private static void GetPersistentTypesOfType(IList<Type> persistentTypes, Type type, IEnumerable<Type> potentialBaseTypes) {
            if (type.IsSubclassOf(TypeOf.PersistentObject) && !type.ContainsGenericParameters && !persistentTypes.Contains(type)) {
                if (!type.IsAbstract) {
                    if (null != type.GetConstructor(new Type[0])) {
                        persistentTypes.Add(type);
                    }
                    foreach (var potentialBaseType in potentialBaseTypes) {
                        if (potentialBaseType.IsSubclassOf(type)) {
                            PersistenceMechanism.GetPersistentTypesOfType(persistentTypes, potentialBaseType, potentialBaseTypes);
                        }
                    }
                }
                foreach (var nestedType in type.GetNestedTypes()) {
                    PersistenceMechanism.GetPersistentTypesOfType(persistentTypes, nestedType, potentialBaseTypes);
                }
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                    Type fieldType = field.FieldType;
                    if (fieldType.IsSubclassOf(typeof(PersistentField))) {
                        foreach (var genericArgument in fieldType.GetGenericArguments()) {
                            if (TypeOf.PersistentObject == genericArgument) {
                                throw new PersistenceException("Invalid generic argument: Usage of " + nameof(PersistentFieldForPersistentObject) + "<" + nameof(PersistentObject) + "> or " + nameof(PersistentFieldForPersistentObjectCollection) + "<" + nameof(PersistentObject) + "> is not allowed in type " + type.FullName + ".");
                            }
                            PersistenceMechanism.GetPersistentTypesOfType(persistentTypes, genericArgument, potentialBaseTypes);
                        }
                    }
                }
                foreach (var field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                    Type propertyType = field.PropertyType;
                    if (propertyType.IsSubclassOf(typeof(PersistentField))) {
                        foreach (var genericArgument in propertyType.GetGenericArguments()) {
                            if (TypeOf.PersistentObject == genericArgument) {
                                throw new PersistenceException("Invalid generic argument: Usage of " + nameof(PersistentFieldForPersistentObject) + "<" + nameof(PersistentObject) + "> or " + nameof(PersistentFieldForPersistentObjectCollection) + "<" + nameof(PersistentObject) + "> is not allowed in type " + type.FullName + ".");
                            }
                            PersistenceMechanism.GetPersistentTypesOfType(persistentTypes, genericArgument, potentialBaseTypes);
                        }
                    }
                }
                PersistenceMechanism.GetPersistentTypesOfType(persistentTypes, type.BaseType, potentialBaseTypes);
            }
            return;
        }

        /// <summary>
        /// Adds all potential persistent base types of a type to a
        /// list of types.
        /// </summary>
        /// <param name="potentialBaseTypes">list of types to add
        /// potential persistent base types to</param>
        /// <param name="type">type to analyze</param>
        private static void GetPotentialBaseTypes(IList<Type> potentialBaseTypes, Type type) {
            if (type.IsSubclassOf(TypeOf.PersistentObject) && !potentialBaseTypes.Contains(type)) {
                potentialBaseTypes.Add(type);
                try {
                    foreach (var assemblyType in type.Assembly.GetTypes()) {
                        PersistenceMechanism.GetPotentialBaseTypes(potentialBaseTypes, assemblyType);
                    }
                } catch (ReflectionTypeLoadException) {
                    // ignore reflection type load exceptions for missing referenced assemblies
                }
                foreach (var nestedType in type.GetNestedTypes()) {
                    PersistenceMechanism.GetPotentialBaseTypes(potentialBaseTypes, nestedType);
                }
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                    Type fieldType = field.FieldType;
                    if (fieldType.IsSubclassOf(typeof(PersistentField))) {
                        foreach (var genericArgument in fieldType.GetGenericArguments()) {
                            PersistenceMechanism.GetPotentialBaseTypes(potentialBaseTypes, genericArgument);
                        }
                    }
                }
                foreach (var field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                    Type propertyType = field.PropertyType;
                    if (propertyType.IsSubclassOf(typeof(PersistentField))) {
                        foreach (var genericArgument in propertyType.GetGenericArguments()) {
                            PersistenceMechanism.GetPotentialBaseTypes(potentialBaseTypes, genericArgument);
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Creates all required system containers in an empty
        /// persistence mechanism - this needs to be called from
        /// "AddContainer()", "MigrateAllContainers()",
        /// "RemoveAllContainers()" and "RenameAllContainers()" if
        /// necessary.
        /// </summary>
        protected abstract void InitializePersistenceMechanism();

        /// <summary>
        /// Ensures that containers for storing all types of
        /// persistent objects contained in a specific assembly exist
        /// in persistence mechanism and that they are up to date.
        /// Orphaned containers are deleted.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// machnism!
        /// </summary>
        /// <param name="assembly">assembly to migrate persistent
        /// containers for</param>
        public void MigrateAllContainers(Assembly assembly) {
            this.MigrateAllContainers(assembly.GetTypes());
            return;
        }

        /// <summary>
        /// Ensures that containers for storing all types of
        /// persistent objects contained in a given enumerable of
        /// types exist in persistence mechanism and that they are up
        /// to date. Orphaned containers are deleted.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// machnism!
        /// </summary>
        /// <param name="types">types to ensure containers for</param>
        public virtual void MigrateAllContainers(IEnumerable<Type> types) {
            this.MigrateAllContainersDirty(types);
            return;
        }

        /// <summary>
        /// Ensures that containers for storing all types of
        /// persistent objects contained in a given enumerable of
        /// types exist in persistence mechanism and that they are up
        /// to date. Orphaned containers are deleted. Does not clean
        /// up the persistence mechanism afterwards. Please use this
        /// method if you intent to start the migration from within
        /// a performance critical context.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// machanism!
        /// </summary>
        /// <param name="types">types to ensure containers for</param>
        public virtual void MigrateAllContainersDirty(IEnumerable<Type> types) {
            try {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10))) {
                    this.MigrateForeignKeys();
                    this.RenameRenamedContainers(types);
                    this.MigrateAllowedGroups(PersistenceMechanism.GetPersistentTypesCascadedly(types));
                    this.EnsureAllContainers(types);
                    this.RemoveOrphanedContainers();
                    transactionScope.Complete();
                }
                if (this.HasVersioningEnabled) {
                    var versioningTypes = new List<Type>(types);
                    var typeOfVersion = typeof(Model.Version);
                    if (!versioningTypes.Contains(typeOfVersion)) {
                        versioningTypes.Add(typeOfVersion);
                    }
                    this.Log?.WriteEntry("Starting migration of schema of versioning repository.", LogLevel.Information);
                    this.VersioningRepository.MigrateAllContainersDirty(versioningTypes);
                }
            } catch (PersistenceMechanismNotInitializedException) {
                this.InitializePersistenceMechanism();
                this.MigrateAllContainersDirty(types);
            }
            return;
        }

        /// <summary>
        /// Migrates all allowed groups by ensuring columns with type
        /// name.
        /// </summary>
        /// <param name="persistentTypes">types to migrate allowed groups for</param>
        // TODO: Remove this method after successfull migration of all framework applications.
        protected virtual void MigrateAllowedGroups(IEnumerable<Type> persistentTypes) {
            // nothing to do
            return;
        }

        /// <summary>
        /// Migrates all foreign keys.
        /// </summary>
            // TODO: Remove this method after successfull migration of all framework applications.
        protected virtual void MigrateForeignKeys() {
            // nothing to do
            return;
        }

        /// <summary>
        /// Deletes all containers for storing persistent objects
        /// and reinitializes the persistence mechanism.
        /// </summary>
        public virtual void RemoveAllContainers() {
            try {
                var containerInfos = this.GetContainerInfos();
                foreach (var containerInfo in containerInfos) {
                    this.RemoveContainer(containerInfo.InternalName);
                }
            } catch (PersistenceMechanismNotInitializedException) {
                this.InitializePersistenceMechanism();
            }
            if (this.HasVersioningEnabled) {
                this.VersioningRepository.RemoveAllContainers();
            }
            return;
        }

        /// <summary>
        /// Deletes the container for storing persistent objects of
        /// type T and all its persistent objects in persistence
        /// mechanism. Containers of subclasses are not affected.
        /// </summary>
        /// <typeparam name="T">type of persistent object to delete
        /// container for</typeparam>
        public void RemoveContainer<T>() where T : PersistentObject, new() {
            string internalContainerName = this.GetInternalNameOfContainer(typeof(T));
            this.RemoveContainer(internalContainerName);
            if (this.HasVersioningEnabled) {
                this.VersioningRepository.RemoveContainer<T>();
            }
            return;
        }

        /// <summary>
        /// Deletes the container for storing persistent objects of
        /// a type and all its persistent objects in persistence
        /// mechanism. Containers of subclasses are not affected.
        /// </summary>
        /// <param name="internalContainerName">internal name of
        /// container in this persistence mechanism to remove</param>
        protected abstract void RemoveContainer(string internalContainerName);

        /// <summary>
        /// Removes all orphaned containers.
        /// </summary>
        private void RemoveOrphanedContainers() {
            var containerInfos = this.GetContainerInfos();
            foreach (var containerInfo in containerInfos) {
                if (null == containerInfo.Type || containerInfo.Type.IsAbstract) {
                    this.RemoveContainer(containerInfo.InternalName);
                }
            }
            return;
        }

        /// <summary>
        /// Updates the names of all containers for storing
        /// persistent objects.
        /// </summary>
        public virtual void RenameAllContainers() {
            try {
                var containerInfos = this.GetContainerInfos();
                foreach (var containerInfo in containerInfos) {
                    if (null != containerInfo.Type && !containerInfo.Type.IsAbstract) {
                        var sampleInstance = this.CreateInstance(containerInfo.Type);
                        this.RenameContainer(containerInfo.AssemblyQualifiedTypeName, sampleInstance);
                    }
                }
            } catch (PersistenceMechanismNotInitializedException) {
                this.InitializePersistenceMechanism();
            }
            if (this.HasVersioningEnabled) {
                this.VersioningRepository.RenameAllContainers();
            }
            return;
        }

        /// <summary>
        /// Updates the names of all renamed containers for storing
        /// persistent objects.
        /// </summary>
        /// <param name="types">types to rename renamed containers
        /// for</param>
        private void RenameRenamedContainers(IEnumerable<Type> types) {
            var persistentTypes = PersistenceMechanism.GetPersistentTypesCascadedly(types);
            while (persistentTypes.Count > 0) {
                for (int i = persistentTypes.Count - 1; i > -1; i--) {
                    try {
                        var persistentType = persistentTypes[i];
                        var sampleInstance = this.CreateInstance(persistentType);
                        if (!this.ContainsContainer(sampleInstance)) {
                            foreach (var attribute in Attribute.GetCustomAttributes(persistentType)) {
                                if (attribute is PreviousNameAttribute previousNameAttribute && this.RenameContainer(previousNameAttribute.PreviousName, sampleInstance)) {
                                    break;
                                }
                            }
                        }
                        persistentTypes.RemoveAt(i);
                    } catch (DependencyException) {
                        // ignore dependency exceptions
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Renames a container for storing persistent objects of a
        /// type.
        /// </summary>
        /// <param name="oldName">old name of container</param>
        /// <typeparam name="T">type of persistent object to rename
        /// container for</typeparam>
        /// <returns>true if container could be renamed successfully,
        /// false otherwise or if it did not exist in persistence
        /// mechanism</returns>
        public bool RenameContainer<T>(string oldName) where T : PersistentObject, new() {
            bool success = this.RenameContainer(oldName, this.CreateInstance<T>());
            if (this.HasVersioningEnabled) {
                this.VersioningRepository.RenameContainer<T>(oldName);
            }
            return success;
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
        protected abstract bool RenameContainer(string oldName, PersistentObject sampleInstance);

        /// <summary>
        /// Updates the container for storing persistent objects of
        /// type T in persistence mechanism. This needs to be called
        /// whenever there is a change in the fields to persist of
        /// the related persistent objects.
        /// WARNING: Deleted persistent fields will cause the
        /// deletion of the corresponding column in the persistence
        /// machnism!
        /// </summary>
        /// <typeparam name="T">type of persistent object stored in
        /// the container to update</typeparam>
        public void UpdateContainer<T>() where T : PersistentObject, new() {
            this.UpdateContainer(this.CreateInstance<T>());
            if (this.HasVersioningEnabled) {
                this.VersioningRepository.UpdateContainer<T>();
            }
            return;
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
        protected abstract void UpdateContainer(PersistentObject sampleInstance);

    }

}