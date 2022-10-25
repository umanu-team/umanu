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

namespace Framework.Presentation {

    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract base class for data providers.
    /// </summary>
    /// <typeparam name="T">type of objects to provide</typeparam>
    public abstract class DataProvider<T> where T : class, IProvidableObject {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        protected DataProvider() {
            // nothing to do
        }

        /// <summary>
        /// Adds or updates an object.
        /// </summary>
        /// <param name="element">object to add or update</param>
        public abstract void AddOrUpdate(T element);

        /// <summary>
        /// Creates a new object of a specific type.
        /// </summary>
        /// <param name="type">type of new object</param>
        /// <returns>new object of specified type or null if current
        /// user is not allowed to create objects</returns>
        public abstract T Create(Type type);

        /// <summary>
        /// Deletes an object.
        /// </summary>
        /// <param name="element">object to delete</param>
        public abstract void Delete(T element);

        /// <summary>
        /// Filters, sorts and subsets a list in memory. However, for
        /// performance reasons letting the persistence mechanism do
        /// this should be preferred whenever possible.
        /// </summary>
        /// <param name="list">list to be filtered, sorted and to get
        /// subset of</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>all matching objects from this container</returns>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        protected static void FilterSortAndSubsetList(List<T> list, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            filterCriteria?.Filter(list);
            if (null != sortCriteria) {
                list.Sort(new SortCriteriaComparer(sortCriteria));
            }
            if (startPosition > ulong.MinValue || maxResults < ulong.MaxValue) {
                new Range(startPosition, maxResults).ApplyTo(list);
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
        /// <returns>matching objects from this data provider</returns>
        public ICollection<T> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria) {
            return this.Find(filterCriteria, sortCriteria, ulong.MinValue, ulong.MaxValue);
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
        public abstract ICollection<T> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults);

        /// <summary>
        /// Finds the file with a specific ID and file name.
        /// </summary>
        /// <param name="fileId">ID to get file for</param>
        /// <param name="fileName">name of requested file</param>
        /// <returns>file with specific ID and file name</returns>
        public abstract File FindFile(Guid fileId, string fileName);

        /// <summary>
        /// Finds the object with a specific ID.
        /// </summary>
        /// <param name="id">ID to get object for</param>
        /// <returns>object with specific ID or null if no match was
        /// found</returns>
        public abstract T FindOne(string id);

        /// <summary>
        /// Gets all objects this data provider is supposed to
        /// provide.
        /// </summary>
        /// <returns>collection of objects</returns>
        public ICollection<T> GetAll() {
            return this.Find(FilterCriteria.Empty, SortCriterionCollection.Empty);
        }

        /// <summary>
        /// Indicates whether data provider is empty.
        /// </summary>
        /// <returns>returns true if data provider does not provide
        /// any data, false otherwise</returns>
        public bool IsEmpty() {
            bool isEmpty = true;
            foreach (var element in this.GetAll()) {
                isEmpty = false;
                break;
            }
            return isEmpty;
        }

        /// <summary>
        /// Preloads the state of multiple objects.
        /// </summary>
        /// <param name="elements">objects to preload</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        public abstract void Preload(IEnumerable<T> elements, IEnumerable<string[]> keyChains);

    }

}