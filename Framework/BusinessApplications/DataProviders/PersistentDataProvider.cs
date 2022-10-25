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

namespace Framework.BusinessApplications.DataProviders {

    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Data provider for a persistent type.
    /// </summary>
    /// <typeparam name="T">type of objects to provide</typeparam>
    public abstract class PersistentDataProvider<T> : DataProvider<T>, ISearchProvider<T> where T : PersistentObject, IProvidableObject, new() {

        /// <summary>
        /// Base filter criteria to apply.
        /// </summary>
        protected FilterCriteria BaseFilterCriteria { get; private set; }

        /// <summary>
        /// Base sort criteria to apply.
        /// </summary>
        protected SortCriterionCollection BaseSortCriteria { get; private set; }

        /// <summary>
        /// Persistent container for type T.
        /// </summary>
        protected PersistentContainer<T> PersistentContainer {
            get {
                if (null == this.persistentContainer) {
                    this.persistentContainer = this.PersistenceMechanism.FindContainer<T>();
                }
                return this.persistentContainer;
            }
        }
        private PersistentContainer<T> persistentContainer;

        /// <summary>
        /// Persistence mechanism to get data from.
        /// </summary>
        protected internal PersistenceMechanism PersistenceMechanism { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        /// <param name="baseFilterCriteria">base filter criteria to
        /// apply</param>
        public PersistentDataProvider(PersistenceMechanism persistenceMechanism, FilterCriteria baseFilterCriteria)
            : this(persistenceMechanism, baseFilterCriteria, new SortCriterion(nameof(PersistentObject.ModifiedAt), SortDirection.Descending)) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        /// <param name="baseFilterCriteria">base filter criteria to
        /// apply</param>
        /// <param name="baseSortCriterion">base sort criterion to
        /// apply</param>
        public PersistentDataProvider(PersistenceMechanism persistenceMechanism, FilterCriteria baseFilterCriteria, SortCriterion baseSortCriterion)
            : this(persistenceMechanism, baseFilterCriteria, new SortCriterionCollection(new SortCriterion[] { baseSortCriterion })) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        /// <param name="baseFilterCriteria">base filter criteria to
        /// apply</param>
        /// <param name="baseSortCriteria">base sort criteria to
        /// apply</param>
        public PersistentDataProvider(PersistenceMechanism persistenceMechanism, FilterCriteria baseFilterCriteria, SortCriterionCollection baseSortCriteria)
            : base() {
            this.PersistenceMechanism = persistenceMechanism;
            this.BaseFilterCriteria = baseFilterCriteria;
            this.BaseSortCriteria = baseSortCriteria;
        }

        /// <summary>
        /// Adds or updates an object in persistence mechanism.
        /// </summary>
        /// <param name="element">object to add or update</param>
        public override void AddOrUpdate(T element) {
            this.PersistentContainer.AddOrUpdateCascadedly(element);
            this.PersistenceMechanism.RemoveExpiredTemporaryFiles();
            return;
        }

        /// <summary>
        /// Deletes an object in persistence mechanism.
        /// </summary>
        /// <param name="element">object to delete</param>
        public override void Delete(T element) {
            bool isWriteProtected = true;
            if (null != element.AllowedGroups) {
                var currentUser = this.PersistenceMechanism.UserDirectory.CurrentUser;
                isWriteProtected = !element.AllowedGroups.ForWriting.ContainsPermissionsFor(currentUser);
            }
            if (!isWriteProtected) {
                var elevatedPersistenceMechanism = this.PersistenceMechanism.CopyWithElevatedPrivileges();
                var elevatedTypedElements = elevatedPersistenceMechanism.FindContainer<T>();
                var elevatedTypedElement = elevatedTypedElements.FindOne(element.Id);
                elevatedTypedElement.RemoveCascadedly();
                if (elevatedTypedElement.IsRemoved) {
                    element.IsRemoved = true;
                }
            }
            return;
        }

        /// <summary>
        /// Finds all matching results of this data provider.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>all matching objects from this container</returns>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <returns>matching objects from this data provider</returns>
        public override ICollection<T> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            return this.PersistentContainer.Find(FilterCriteria.Concat(filterCriteria, this.BaseFilterCriteria), SortCriterionCollection.Concat(sortCriteria, this.BaseSortCriteria), startPosition, maxResults);
        }

        /// <summary>
        /// Finds the file with a specific ID and file name.
        /// </summary>
        /// <param name="fileId">ID to get file for</param>
        /// <param name="fileName">name of requested file</param>
        /// <returns>file with specific ID and file name</returns>
        public override File FindFile(Guid fileId, string fileName) {
            var files = this.PersistenceMechanism.FindContainer<File>();
            var filterCriteria = new FilterCriteria(nameof(File.Id), RelationalOperator.IsEqualTo, fileId)
                .And(nameof(File.Name), RelationalOperator.IsEqualTo, fileName, FilterTarget.IsOtherTextValue);
            return files.FindOne(filterCriteria, SortCriterionCollection.Empty);
        }

        /// <summary>
        /// Finds all matching results for query.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to find
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <returns>search results matching the query</returns>
        public virtual ICollection<T> FindFullText(string fullTextQuery, FilterCriteria filterCriteria, ulong startPosition, ulong maxResults) {
            return this.PersistentContainer.FindFullText(fullTextQuery, FilterCriteria.Concat(filterCriteria, this.BaseFilterCriteria), startPosition, maxResults);
        }

        /// <summary>
        /// Finds the object with a specific ID.
        /// </summary>
        /// <param name="id">ID to get object for</param>
        /// <returns>object with specific ID or null if no
        /// match was found</returns>
        public override T FindOne(string id) {
            T element;
            if (Guid.TryParse(id, out Guid guid)) {
                var filterCriteria = new FilterCriteria(new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, guid));
                element = this.PersistentContainer.FindOne(FilterCriteria.Concat(filterCriteria, this.BaseFilterCriteria), SortCriterionCollection.Empty);
            } else {
                element = null;
            }
            return element;
        }

        /// <summary>
        /// Preloads the state of multiple objects.
        /// </summary>
        /// <param name="elements">objects to preload</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        public sealed override void Preload(IEnumerable<T> elements, IEnumerable<string[]> keyChains) {
            this.PersistentContainer.Preload(elements, keyChains);
            return;
        }

    }

}