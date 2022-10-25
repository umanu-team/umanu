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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// View of a table of a relational database.
    /// </summary>
    internal class RelationalViewForTable {

        /// <summary>
        /// First pair of table name and type of this view.
        /// </summary>
        protected KeyValuePair<string, Type> FirstTableNameAndType {
            get {
                KeyValuePair<string, Type> firstTableNameAndType;
                using (var enumerator = this.TableNamesAndTypes.GetEnumerator()) {
                    enumerator.MoveNext();
                    firstTableNameAndType = enumerator.Current;
                }
                return firstTableNameAndType;
            }
        }

        /// <summary>
        /// Name of this view.
        /// </summary>
        public virtual string Name {
            get { return RelationalDatabase.GetViewNameForContainer(this.TableName); }
        }

        /// <summary>
        /// Releated persistence mechanism.
        /// </summary>
        private readonly PersistenceMechanism persistenceMechanism;

        /// <summary>
        /// Names of first table this view is for.
        /// </summary>
        public string TableName {
            get { return this.FirstTableNameAndType.Key; }
        }

        /// <summary>
        /// Dictionary of names/types of tables to unite in view.
        /// </summary>
        public Dictionary<string, Type> TableNamesAndTypes { get; private set; }

        /// <summary>
        /// First type this view is for.
        /// </summary>
        public Type Type {
            get { return this.FirstTableNameAndType.Value; }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">related persistence
        /// mechanism</param>
        protected RelationalViewForTable(PersistenceMechanism persistenceMechanism) {
            this.persistenceMechanism = persistenceMechanism;
            this.TableNamesAndTypes = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">related persistence
        /// mechanism</param>
        /// <param name="tableName">name of table this view is for</param>
        /// <param name="typeName">type this view is for</param>
        public RelationalViewForTable(PersistenceMechanism persistenceMechanism, string tableName, string typeName)
            : this(persistenceMechanism) {
            this.TableNamesAndTypes.Add(tableName, Type.GetType(typeName));
        }

        /// <summary>
        /// Gets an enumerable of fields this view is for.
        /// </summary>
        /// <returns>enumerable of fields this view is for</returns>
        protected virtual IEnumerable<string> GetFieldNames() {
            var sampleInstance = this.persistenceMechanism.CreateInstance(this.Type);
            foreach (var persistentFieldsForElement in sampleInstance.PersistentFieldsForElements) {
                yield return persistentFieldsForElement.Key;
            }
            foreach (var persistentFieldForPersistentObject in sampleInstance.PersistentFieldsForPersistentObjects) {
                yield return persistentFieldForPersistentObject.Key;
                yield return persistentFieldForPersistentObject.Key + '_';
            }
        }

        /// <summary>
        /// Gets the relational unions to be part of relational view.
        /// </summary>
        /// <returns>relational unions to be part of relational view</returns>
        public IEnumerable<RelationalViewUnion> GetUnions() {
            foreach (var tableNameAndType in this.TableNamesAndTypes) {
                var union = new RelationalViewUnion(tableNameAndType.Key);
                foreach (var fieldName in this.GetFieldNames()) {
                    union.DynamicColumns.Add(new KeyValuePair<string, string>(fieldName, fieldName));
                }
                union.StaticColumns.Add(new Tuple<string, string, Type>("TypeName", tableNameAndType.Value.AssemblyQualifiedName, TypeOf.IndexableString));
                yield return union;
            }
        }

        /// <summary>
        /// Gets views for all sub tables of type of this view.
        /// </summary>
        /// <returns>views for all sub tables of type of this view</returns>
        public IEnumerable<RelationalViewForSubTable> GetViewsForSubTables() {
            var sampleInstance = this.persistenceMechanism.CreateInstance(this.Type);
            foreach (var persistentFieldForCollectionOfElements in sampleInstance.PersistentFieldsForCollectionsOfElements) {
                var subTableView = new RelationalViewForSubTable(this.persistenceMechanism);
                foreach (var tableNameAndType in this.TableNamesAndTypes) {
                    string subTableName = RelationalDatabase.GetInternalNameOfSubTable(tableNameAndType.Key, persistentFieldForCollectionOfElements.Key);
                    subTableView.TableNamesAndTypes.Add(subTableName, tableNameAndType.Value);
                }
                yield return subTableView;
            }
        }

    }

}