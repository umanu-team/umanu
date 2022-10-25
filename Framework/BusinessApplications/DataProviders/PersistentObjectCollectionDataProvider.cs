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
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Data provider for a collection of persistent objects.
    /// </summary>
    /// <typeparam name="T">type of objects to provide</typeparam>
    public class PersistentObjectCollectionDataProvider<T> : PersistentDataProvider<T> where T : PersistentObject, IProvidableObject, new() {

        /// <summary>
        /// Persistent field for persistent object collection to
        /// provide data for.
        /// </summary>
        protected PersistentFieldForPersistentObjectCollection<T> PersistentFieldForPersistentObjectCollection { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistentFieldForPersistentObjectCollection">
        /// persistent field for persistent object collection to
        /// provide data for</param>
        public PersistentObjectCollectionDataProvider(PersistentFieldForPersistentObjectCollection<T> persistentFieldForPersistentObjectCollection)
            : base(persistentFieldForPersistentObjectCollection.ParentPersistentObject.ParentPersistentContainer.ParentPersistenceMechanism, FilterCriteria.Empty, SortCriterionCollection.Empty) {
            this.PersistentFieldForPersistentObjectCollection = persistentFieldForPersistentObjectCollection;
        }

        /// <summary>
        /// Adds or updates an object in persistence mechanism.
        /// </summary>
        /// <param name="element">object to add or update</param>
        public override void AddOrUpdate(T element) {
            if (element.IsNew) {
                // adding new elements is supposed to be possible even if current user has no write access to this.PersistentFieldForPersistentObjectCollection - otherwise this data provider would behave different than other data providers
                var elevatedParentPersistentObject = this.PersistentFieldForPersistentObjectCollection.ParentPersistentObject.GetWithElevatedPrivileges();
                var elevatedPersistentFieldForPersistentObjectCollection = elevatedParentPersistentObject.GetPersistentFieldForCollectionOfPersistentObjects(this.PersistentFieldForPersistentObjectCollection.Key);
                elevatedPersistentFieldForPersistentObjectCollection.AddObject(element);
                elevatedParentPersistentObject.UpdateCascadedly();
            } else {
                element.UpdateCascadedly();
            }
            this.PersistenceMechanism.RemoveExpiredTemporaryFiles();
            return;
        }

        /// <summary>
        /// Concatenates base filter criteria and provided filter
        /// criteria.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to
        /// concatenate with base filter criteria</param>
        /// <returns>voncatenation of base filter criteria and
        /// provided filter criteria</returns>
        protected FilterCriteria ConcatBaseFilterCriteriaAnd(FilterCriteria filterCriteria) {
            var idFilterCriteria = FilterCriteria.Empty;
            foreach (var element in this.PersistentFieldForPersistentObjectCollection) {
                idFilterCriteria = idFilterCriteria.Or(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, element.Id);
            }
            return FilterCriteria.Concat(filterCriteria, idFilterCriteria);
        }

        /// <summary>
        /// Creates a new object of a specific type.
        /// </summary>
        /// <param name="type">type of new object</param>
        /// <returns>new object of specified type</returns>
        public override T Create(Type type) {
            T element;
            if (typeof(T) == type) {
                element = new T();
            } else {
                element = null;
            }
            return element;
        }

        /// <summary>
        /// Deletes an object in persistence mechanism.
        /// </summary>
        /// <param name="element">object to delete</param>
        public override void Delete(T element) {
            if (this.PersistentFieldForPersistentObjectCollection.Contains(element)) {
                base.Delete(element);
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
            ICollection<T> results;
            if (null != filterCriteria && !filterCriteria.IsEmpty && this.PersistentFieldForPersistentObjectCollection.Count > 0) {
                filterCriteria = this.ConcatBaseFilterCriteriaAnd(filterCriteria);
                results = this.PersistentContainer.Find(filterCriteria, sortCriteria, startPosition, maxResults);
            } else if (null != sortCriteria && sortCriteria.Count > 0 && this.PersistentFieldForPersistentObjectCollection.Count > 1) {
                var sortedResults = new List<T>(this.PersistentFieldForPersistentObjectCollection);
                sortedResults.Sort(new SortCriteriaComparer(sortCriteria));
                results = sortedResults;
            } else {
                results = this.PersistentFieldForPersistentObjectCollection;
            }
            return results;
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
        public override ICollection<T> FindFullText(string fullTextQuery, FilterCriteria filterCriteria, ulong startPosition, ulong maxResults) {
            ICollection<T> results;
            if (this.PersistentFieldForPersistentObjectCollection.Count > 0) {
                filterCriteria = this.ConcatBaseFilterCriteriaAnd(filterCriteria);
                results = this.PersistentContainer.FindFullText(fullTextQuery, filterCriteria, startPosition, maxResults);
            } else {
                results = new T[0];
            }
            return results;
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
                element = this.PersistentFieldForPersistentObjectCollection.Find(guid);
                element?.RetrieveCascadedly();
            } else {
                element = null;
            }
            return element;
        }

    }

}
