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

namespace Framework.Persistence.Rdbms {

    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// View of a relational database.
    /// </summary>
    internal class RelationalViewForRelations {

        /// <summary>
        /// Name of this view.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Releated persistence mechanism.
        /// </summary>
        private readonly PersistenceMechanism persistenceMechanism;

        /// <summary>
        /// Type this view is for.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// List of tables and columns to unite in view.
        /// </summary>
        public List<RelationalViewUnion> Unions { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">related persistence
        /// mechanism</param>
        /// <param name="type">type this view is for</param>
        public RelationalViewForRelations(PersistenceMechanism persistenceMechanism, Type type) {
            this.persistenceMechanism = persistenceMechanism;
            var tableName = persistenceMechanism.GetInternalNameOfContainer(type);
            if (!string.IsNullOrEmpty(tableName)) {
                this.Name = RelationalDatabase.GetViewNameForSingleRelations(tableName);
            }
            this.Type = type;
            this.Unions = new List<RelationalViewUnion>();
        }

        /// <summary>
        /// Adds relational unions for a container.
        /// </summary>
        /// <param name="containerInfo">container to add relational
        /// unions for.</param>
        public void AddContainer(ContainerInfo containerInfo) {
            var sampleInstance = this.persistenceMechanism.CreateInstance(containerInfo.Type);
            foreach (var persistentField in sampleInstance.PersistentFieldsForPersistentObjects) {
                if (persistentField.GetContentType().IsAssignableFrom(this.Type)) {
                    var union = new RelationalViewUnion(containerInfo.InternalName);
                    union.DynamicColumns.Add(new KeyValuePair<string, string>("ParentID", nameof(PersistentObject.Id)));
                    union.DynamicColumns.Add(new KeyValuePair<string, string>("ChildID", persistentField.Key));
                    union.StaticColumns.Add(new Tuple<string, string, Type>("ParentTable", containerInfo.InternalName, TypeOf.IndexableString));
                    union.StaticColumns.Add(new Tuple<string, string, Type>("ParentField", persistentField.Key, TypeOf.IndexableString));
                    this.Unions.Add(union);
                }
            }
            return;
        }

        /// <summary>
        /// Adds an empty dummy union.
        /// </summary>
        internal void AddDummyUnion() {
            var tableName = this.persistenceMechanism.GetInternalNameOfContainer(this.Type);
            var union = new RelationalViewUnion(tableName);
            union.StaticColumns.Add(new Tuple<string, string, Type>("ParentID", null, TypeOf.Guid));
            union.StaticColumns.Add(new Tuple<string, string, Type>("ChildID", null, TypeOf.Guid));
            union.StaticColumns.Add(new Tuple<string, string, Type>("ParentTable", null, TypeOf.IndexableString));
            union.StaticColumns.Add(new Tuple<string, string, Type>("ParentField", null, TypeOf.IndexableString));
            union.FilterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsNotEqualTo, nameof(PersistentObject.Id), FilterTarget.IsOtherField);
            this.Unions.Add(union);
            return;
        }

        /// <summary>
        /// Gets the relational unions to be part of relational view.
        /// </summary>
        /// <returns>relational unions to be part of relational view</returns>
        public IEnumerable<RelationalViewUnion> GetUnions() {
            return this.Unions;
        }

    }

}