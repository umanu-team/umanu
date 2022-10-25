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
    using Framework.Presentation;
    using Persistence.Filters;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Data provider for a singleton persistent type.
    /// </summary>
    /// <typeparam name="T">type of objects to provide</typeparam>
    public abstract class SingletonDataProvider<T> : PersistentDataProvider<T> where T : PersistentObject, IProvidableObject, new() {

        /// <summary>
        /// Lock for singleton object.
        /// </summary>
        private static readonly object singletonLock = new object();

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        public SingletonDataProvider(PersistenceMechanism persistenceMechanism)
            : base(persistenceMechanism, FilterCriteria.Empty) {
            // nothing to do
        }

        /// <summary>
        /// Creates a new object of a specific type.
        /// </summary>
        /// <param name="type">type of new object</param>
        /// <returns>new object of specified type or null if current
        /// user is not allowed to create objects</returns>
        public sealed override T Create(Type type) {
            throw new InvalidOperationException("Creation of new singleton objects is not allowed this way.");
        }

        /// <summary>
        /// Creates a new singleton object.
        /// </summary>
        /// <returns>new singleton object</returns>
        protected abstract T Create();

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
            var singletonObjects = new List<T>(1);
            if (ulong.MinValue == startPosition && maxResults > ulong.MinValue) {
                var singletonObject = this.FindOrCreateOne();
                if (null != singletonObject && (null == filterCriteria || filterCriteria.IsMatch(singletonObject))) {
                    singletonObjects.Add(singletonObject);
                }
            }
            return singletonObjects;
        }

        /// <summary>
        /// Finds the singleton object or creates it.
        /// </summary>
        /// <returns>singleton object</returns>
        public T FindOrCreateOne() {
            var singletonContainer = this.PersistenceMechanism.FindContainer<T>();
            var singletonObjects = singletonContainer.Find(FilterCriteria.Empty, SortCriterionCollection.Empty);
            if (singletonObjects.Count < 1) {
                lock (SingletonDataProvider<T>.singletonLock) {
                    var elevatedContainer = this.PersistenceMechanism.CopyWithElevatedPrivileges().FindContainer<T>();
                    if (elevatedContainer.Count < 1) {
                        var elevatedSingleton = this.Create();
                        elevatedContainer.AddCascadedly(elevatedSingleton);
                    }
                    singletonObjects = singletonContainer.Find(FilterCriteria.Empty, SortCriterionCollection.Empty);
                }
            } else if (singletonObjects.Count > 1) {
#if DEBUG
                throw new ApplicationException("Singleton object must be unique. However, multiple singleton objects were detected.");
# else
                lock (SingletonDataProvider<T>.singletonLock) {
                    var elevatedContainer = this.PersistenceMechanism.CopyWithElevatedPrivileges().FindContainer<T>();
                    while (elevatedContainer.Count > 1) {
                        Framework.Model.JobQueue.Log?.WriteEntry("Duplicated signleton object was deleted automatically. Usually this should not be necessary - please check your application code!", Diagnostics.LogLevel.Error);
                        var elevatedDuplicatedObject = elevatedContainer.FindOne(FilterCriteria.Empty, new SortCriterionCollection() {
                            new SortCriterion(nameof(PersistentObject.CreatedAt), SortDirection.Descending)
                        });
                        elevatedDuplicatedObject.RemoveCascadedly();
                    }
                    singletonObjects = singletonContainer.Find(FilterCriteria.Empty, SortCriterionCollection.Empty);
                }
#endif
            }
            return singletonObjects[0];
        }

    }

}